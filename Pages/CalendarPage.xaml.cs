using Syncfusion.Maui.Calendar;
using System.Globalization;
using TFG.Models;
using TFG.Services;

namespace TFG.Pages;

public partial class CalendarPage : ContentPage, IQueryAttributable
{
    private int _salaId;
    private int _usuarioId;
    private Salas _sala;
    private Usuarios _usuario;

    private bool hayUsuario = false;

    public CalendarPage()
    {
        InitializeComponent();

        if (SesionActual.UsuarioLogueado == null)
        {
            _usuario = null;
        }
        else
        {
            _usuario = SesionActual.UsuarioLogueado;
            _usuarioId = _usuario.Id;
            hayUsuario = true;
        }
    }
    public CalendarPage(Salas sala)
    {
        InitializeComponent();
        _sala = sala;
        Title = $"Reservar {sala.Nombre}";

        calendar_reservas.SelectedDate = DateTime.Now.Date;
        calendar_reservas.SelectionChanged += OnCalendarSelectionChanged;
        _ = CargarHorasOcupadas();
    }
    private void OnCalendarSelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
    {
        _ = CargarHorasOcupadas();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SalaId", out var salaIdObj) && int.TryParse(salaIdObj.ToString(), out int salaId))
        {
            _salaId = salaId;
            LoadSala();
        }
    }

    private async void LoadSala()
    {
        var servicio = SalaServicio.GetInstancia();
        _sala = (await servicio.ObtenerTodasLasSalas()).FirstOrDefault(s => s.Id == _salaId);

        if (_sala != null)
        {
            Title = $"Reservar {_sala.Nombre}";
            lblSalaCapacidad.Text = $"Capacidad: {_sala.Capacidad}";
            lblSalaEquipamiento.Text = $"Equipamiento: {_sala.Equipamiento}";
        }
    }


    private string timeSlot = string.Empty;
    private void SlotBooking_Change(object sender, EventArgs e)
    {
        var selectedButton = (Button)sender;

        if (!selectedButton.IsEnabled) return; // No hacer nada si está deshabilitado

        foreach (Button child in flexLayout.Children)
        {
            if (child.IsEnabled)
            {
                child.TextColor = Colors.Black;
                child.Background = Colors.White;
                child.BorderColor = Colors.Black;
            }
        }

        selectedButton.TextColor = Colors.White;
        selectedButton.Background = Color.FromArgb("#6200EE");
        selectedButton.BorderColor = Color.FromArgb("#6200EE");
        timeSlot = selectedButton.Text;
    }

    private async Task CargarHorasOcupadas()
    {
        if (!calendar_reservas.SelectedDate.HasValue) return;

        var servicio = SalaServicio.GetInstancia();
        var reservas = await servicio.ObtenerReservasPorSala(_sala.Id);
        var fechaSeleccionada = calendar_reservas.SelectedDate.Value.Date;

        foreach (Button child in flexLayout.Children)
        {
            string hora = child.Text;
            bool estaReservada = reservas.Any(r => r.Hora == hora && r.Fecha.Date == fechaSeleccionada);

            if (estaReservada)
            {
                child.IsEnabled = false;
                child.Background = Colors.Gray;
                child.BorderColor = Colors.Gray;
                child.TextColor = Colors.White;
            }
            else
            {
                child.IsEnabled = true;
                child.Background = Colors.White;
                child.BorderColor = Colors.Black;
                child.TextColor = Colors.Black;
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (_usuario == null)
        {
            {
                string mensaje = $"Debes estar logueado para reserva.";
                await DisplayAlert("Error", mensaje, "OK");
                return;
            }
        }
        else
        {
            if (!calendar_reservas.SelectedDate.HasValue)
            {
                await DisplayAlert("Error", "Debes seleccionar una fecha para reservar.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(timeSlot))
            {
                await DisplayAlert("Error", "Debes seleccionar una hora para reservar.", "OK");
                return;
            }

            DateTime dateTime = calendar_reservas.SelectedDate.Value;
            string horaSeleccionada = timeSlot; // Hora seleccionada

            if (string.IsNullOrEmpty(horaSeleccionada)) return;

            if (dateTime < DateTime.Now.Date) return; // Verificar si la fecha seleccionada es menor que la fecha actual )


            // Crear la reserva
            var reserva = new Reservas
            {
                SalaId = _sala.Id,  // ID de la sala
                UsuarioId = _usuario.Id,
                Fecha = dateTime,  // Fecha seleccionada
                Hora = horaSeleccionada  // Hora seleccionada
            };

            // Guardar la reserva en la base de datos
            var servicio = SalaServicio.GetInstancia();
            await servicio.InsertarReserva(reserva);

            await CargarHorasOcupadas();

            // Mostrar mensaje de confirmación
            string mensaje = $"Reserva realizada para {_sala.Nombre} a las {horaSeleccionada}.";
            await DisplayAlert("Reserva Exitosa", mensaje, "OK");

            // Limpiar la selección
            calendar_reservas.SelectedDate = DateTime.Now.Date;
            this.timeSlot = string.Empty;
        }
    }
    private async void Volver_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//menuPrincipal/TabSalas");
    }

}
