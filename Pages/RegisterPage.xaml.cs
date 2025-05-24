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
        string contrase�a = contrase�aEntry.Text;
        string confirmar = confirmarContrase�aEntry.Text;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(gmail) || string.IsNullOrWhiteSpace(contrase�a) || string.IsNullOrWhiteSpace(confirmar))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
            return;
        }

        if (contrase�a != confirmar)
        {
            await DisplayAlert("Error", "Las contrase�as no coinciden", "OK");
            return;
        }

        var nuevoUsuario = new Usuarios { Nombre = usuario, Email = gmail, Contrasena = contrase�a };
        var servicio = SalaServicio.GetInstancia();
        await servicio.RegistrarUsuario(nuevoUsuario);
        var usuarioAutenticado = await servicio.Autenticar(usuario, contrase�a);

        if (usuarioAutenticado != null)
        {
            await DisplayAlert("�xito", "Usuario registrado correctamente", "OK");
            SesionActual.UsuarioLogueado = usuarioAutenticado;

            // Crear nueva instancia de AppShell
            Application.Current.MainPage = new AppShell();

            // Navegar expl�citamente a la p�gina de inicio
            await Shell.Current.GoToAsync("//menuPrincipal/TabInicio");
        }
        else
        {
            await DisplayAlert("Error", "Usuario o contrase�a incorrectos", "OK");
        }
    }

}