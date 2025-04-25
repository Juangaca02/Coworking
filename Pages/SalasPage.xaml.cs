using TFG.ViewModel;
using TFG.Pages;
using TFG.Models;
using System.Collections.ObjectModel;
using TFG.Services;
namespace TFG.Pages;

public partial class SalasPage : ContentPage
{
    public SalasPage()
    {
        InitializeComponent();
        BindingContext = new SalasViewModel();
    }
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue.ToLower();
        var filteredSalas = ((SalasViewModel)BindingContext).SalasDisponibles
            .Where(s => s.Nombre.ToLower().Contains(searchText) || s.Equipamiento.ToLower().Contains(searchText) ||
            s.Capacidad.ToString().Contains(searchText))
            .ToList();
        salasCollectionView.ItemsSource = filteredSalas;
    }

    // Manejar el clic en el bot�n "Ver Reservas"
    private async void OnVerReservasClicked(object sender, EventArgs e)
    {
        // Obtener el bot�n que fue presionado
        var button = sender as Button;

        // Obtener el objeto de la sala asociado con este bot�n
        var sala = button?.BindingContext as Salas;

        if (sala != null)
        {
            // Navegar a la p�gina de detalle de la sala
            await Navigation.PushAsync(new SalaDetailPage { BindingContext = sala });
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is SalasViewModel viewModel)
        {
            await viewModel.RecargarSalas();
        }
    }
}