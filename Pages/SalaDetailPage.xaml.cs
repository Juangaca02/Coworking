using TFG.Models;
using TFG.Services;

namespace TFG.Pages;

public partial class SalaDetailPage : ContentPage
{
    public SalaDetailPage()
    {
        InitializeComponent();
    }

    private async void Eliminar_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Eliminar Sala", "�Est�s seguro de que quieres eliminar esta sala?", "S�", "No");
        if (!confirm) return;

        if (BindingContext is Salas sala)
        {
            var servicio = SalaServicio.GetInstancia();
            await servicio.EliminarSala(sala);

            await DisplayAlert("Eliminado", "La sala ha sido eliminada correctamente", "OK");
            await Navigation.PopAsync(); // Vuelve a la p�gina anterior
        }
    }

    private async void Reservar_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is Salas sala)
        {
            // Navegar a la p�gina de Calendar pasando la sala como par�metro
            await Navigation.PushAsync(new CalendarPage(sala));
        }
    }
}