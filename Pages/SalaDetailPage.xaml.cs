using TFG.Models;
using TFG.Services;

namespace TFG.Pages;

public partial class SalaDetailPage : ContentPage
{
    public SalaDetailPage()
    {
        InitializeComponent();
        if (SesionActual.UsuarioLogueado.Admin == 0)
        {
            btnEliminar.IsVisible = false;
            btnEditar.IsVisible = false;
        }
        else
        {
            btnEliminar.IsVisible = false;
            btnEditar.IsVisible = true;
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is Salas sala)
        {
            if (sala.Estado == "En Mantenimiento")
            {
                btnReservar.IsEnabled = false;
                btnReservar.Text = "No disponible";
            }
        }
    }



    private async void Eliminar_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Eliminar Sala", "¿Estás seguro de que quieres eliminar esta sala?", "Sí", "No");
        if (!confirm) return;

        if (BindingContext is Salas sala)
        {
            var servicio = SalaServicio.GetInstancia();
            await servicio.EliminarSala(sala);

            await DisplayAlert("Eliminado", "La sala ha sido eliminada correctamente", "OK");
            await Navigation.PopAsync(); // Vuelve a la página anterior
        }
    }

    private async void Reservar_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is Salas sala)
        {
            // Navegar a la página de Calendar pasando la sala como parámetro
            //await Shell.Current.GoToAsync($"{nameof(CalendarPage)}?SalaId={sala.Id}");
            await Shell.Current.GoToAsync($"calendar?SalaId={sala.Id}");
        }
    }

    private async void EditarSala_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is Salas sala)
        {
            await Navigation.PushAsync(new EditarSalaPage(sala));
        }
    }
}