using Syncfusion.Maui.Calendar;
using System.Globalization;
using TFG.Models;
using TFG.Services;

namespace TFG.Pages;

public partial class CalendarPage : ContentPage
{
    private Salas _sala;
    public CalendarPage(Salas sala)
    {
        InitializeComponent();
        _sala = sala;
        Title = $"Reservar {sala.Nombre}"; // Mostrar el nombre de la sala en el título
        // Aquí puedes usar _sala para mostrar la información de la sala si lo deseas
        // Como la capacidad, equipamiento, etc.

        // Por ejemplo, puedes mostrar la capacidad de la sala:
        lblSalaCapacidad.Text = $"Capacidad: {sala.Capacidad}";
        lblSalaEquipamiento.Text = $"Equipamiento: {sala.Equipamiento}";

        // Aquí llamaremos a la lógica para cargar el calendario (puedes ajustarlo según tus necesidades)

        calendar_reservas.SelectedDate = DateTime.Now.Date;
        this.calendar_reservas.SelectionChanged += OnCalendarSelectionChanged;
        CargarHorasOcupadas();
    }
    private void OnCalendarSelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
    {
        CargarHorasOcupadas();
    }

    private string timeSlot = string.Empty;
    private void SlotBooking_Change(object sender, EventArgs e)
    {
        var selectedButton = (Button)sender;
        string selectedHour = selectedButton.Text;

        foreach (Button child in flexLayout.Children)
        {
            if (child == selectedButton)
            {
                // Si el botón está habilitado, cambia su color de fondo
                if (child.IsEnabled)
                {
                    child.TextColor = Colors.White;
                    child.BackgroundColor = Color.FromArgb("#6200EE");  // Color de selección
                }
            }
            else
            {
                if (child.IsEnabled)
                {
                    child.TextColor = Colors.Black;
                    child.BackgroundColor = Colors.White;  // Resetear color de fondo de los otros botones
                }
            }
        }

        // Marcar la hora seleccionada para la reserva
        timeSlot = selectedHour;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        DateTime dateTime = calendar_reservas.SelectedDate.Value;
        string horaSeleccionada = timeSlot; // Hora seleccionada

        if (string.IsNullOrEmpty(horaSeleccionada)) return;

        if (dateTime < DateTime.Now.Date) return; // Verificar si la fecha seleccionada es menor que la fecha actual )


        // Crear la reserva
        var reserva = new Reservas
        {
            SalaId = _sala.Id,  // ID de la sala
            Fecha = dateTime,  // Fecha seleccionada
            Hora = horaSeleccionada  // Hora seleccionada
        };

        // Guardar la reserva en la base de datos
        var servicio = SalaServicio.GetInstancia();
        await servicio.InsertarReserva(reserva);

        // Deshabilitar el botón de la hora seleccionada
        foreach (Button child in flexLayout.Children)
        {
            if (child.Text == horaSeleccionada)
            {
                child.IsEnabled = false;
                child.BackgroundColor = Colors.Gray;  // Marcar como deshabilitado
            }
        }

        // Mostrar mensaje de confirmación
        string mensaje = $"Reserva realizada para {_sala.Nombre} a las {horaSeleccionada}.";
        await DisplayAlert("Reserva Exitosa", mensaje, "OK");

        // Limpiar la selección
        calendar_reservas.SelectedDate = DateTime.Now.Date;
        this.timeSlot = string.Empty;
    }

    private async Task CargarHorasOcupadas()
    {
        var servicio = SalaServicio.GetInstancia();
        var reservas = await servicio.ObtenerReservasPorSala(_sala.Id);

        // Recorrer los botones de las horas y marcar los ocupados para la fecha seleccionada
        foreach (Button child in flexLayout.Children)
        {
            string hora = child.Text;
            // Verificar si la reserva ya existe para esa hora y fecha seleccionada
            if (reservas.Any(r => r.Hora == hora && r.Fecha.Date == calendar_reservas.SelectedDate.Value.Date))
            {
                child.IsEnabled = false;  // Deshabilitar el botón
                child.BackgroundColor = Colors.Gray;  // Cambiar color de fondo para mostrar que está ocupado
                child.TextColor = Colors.Black;  // Cambiar el texto a color negro
            }
            else
            {
                child.IsEnabled = true;  // Habilitar el botón si no está ocupado
                child.BackgroundColor = Colors.White;  // Volver al color de fondo original
                child.TextColor = Colors.Black;  // Asegurar que el texto esté en negro
            }
        }
    }

}

//private void Button_Clicked(object sender, EventArgs e)
//    {
//        DateTime dateTime = calendar_reservas.SelectedDate.Value;
//        string dayText = dateTime.ToString(dateTime.Day.ToString(), CultureInfo.CurrentUICulture);
//        string text = "Reseva realizada para el dia " + dayText + " a las " + timeSlot;
//        appointmentPopup.HeaderTitle = "Reserva realizada";
//        appointmentPopup.Message = text;
//        //notificationLabel.Text = text;
//        appointmentPopup.Show();
//        //notificationPopup.Show();
//        calendar_reservas.SelectedDate = DateTime.Now.Date;
//        calendar_reservas.DisplayDate = DateTime.Now.Date;
//        this.timeSlot = string.Empty;
//        foreach (Button child in flexLayout.Children)
//        {
//            Button button = (Button)child;
//            button.TextColor = Colors.Black;
//            button.Background = Colors.White;
//        }
//    }