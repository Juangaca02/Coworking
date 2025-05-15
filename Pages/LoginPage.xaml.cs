using System.ComponentModel;
using System.Runtime.CompilerServices;
using TFG.Models;
using TFG.Services;

namespace TFG.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void IniciarSesion_Clicked(object sender, EventArgs e)
    {
        string usuario = usuarioEntry.Text?.Trim();
        string contraseña = contraseñaEntry.Text;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contraseña))
        {
            await DisplayAlert("Error", "Completa todos los campos", "OK");
            return;
        }

        var servicio = SalaServicio.GetInstancia();
        var usuarioAutenticado = await servicio.Autenticar(usuario, contraseña);

        if (usuarioAutenticado != null)
        {
            SesionActual.UsuarioLogueado = usuarioAutenticado;

            // Crear nueva instancia de AppShell
            Application.Current.MainPage = new AppShell();

            // Navegar explícitamente a la página de inicio
            await Shell.Current.GoToAsync("//menuPrincipal/TabInicio");
        }
        else
        {
            await DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
        }
    }



    private async void IrARegistro_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}