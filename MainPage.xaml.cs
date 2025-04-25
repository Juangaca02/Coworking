using TFG.ViewModel;
using TFG.Pages;
using TFG.Models;
using System.Collections.ObjectModel;
using TFG.Services;
namespace TFG
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new SalasViewModel();
        }
        // Acción del botón para ir a la página de salas
        private async void OnIrASalasClicked(object sender, EventArgs e)
        {
            // Navegar a la página de Salas (MainPage)
            await Navigation.PushAsync(new Pages.SalasPage());
        }
    }
}