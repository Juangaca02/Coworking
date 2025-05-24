using TFG.Models;
using TFG.Services;

namespace TFG.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void Registrar_Clicked(object sender, EventArgs e)
    {
        string usuario = usuarioEntry.Text?.Trim();
        string gmail = emailEntry.Text?.Trim();
        string contraseña = contraseñaEntry.Text;
        string confirmar = confirmarContraseñaEntry.Text;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(gmail) || string.IsNullOrWhiteSpace(contraseña) || string.IsNullOrWhiteSpace(confirmar))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
            return;
        }

        if (contraseña != confirmar)
        {
            await DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
            return;
        }

        var nuevoUsuario = new Usuarios { Nombre = usuario, Email = gmail, Contrasena = contraseña };
        var servicio = SalaServicio.GetInstancia();
        await servicio.RegistrarUsuario(nuevoUsuario);
        var usuarioAutenticado = await servicio.Autenticar(usuario, contraseña);

        if (usuarioAutenticado != null)
        {
            await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
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

}