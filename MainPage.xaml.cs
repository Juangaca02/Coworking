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

            // Mostrar los botones de login y registro si no está logueado
            if (SesionActual.UsuarioLogueado == null)
            {
                btnLogin.IsVisible = true;
                btnRegister.IsVisible = true;
            }
            else
            {
                lblInfo.IsVisible = true;
            }
        }
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}