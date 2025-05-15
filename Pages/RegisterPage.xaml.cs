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

        var nuevoUsuario = new Usuario { Nombre = usuario, Email = gmail, Contrasena = contraseña };
        var servicio = SalaServicio.GetInstancia();
        await servicio.RegistrarUsuario(nuevoUsuario);

        await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
        await Navigation.PopAsync(); // Vuelve al login
    }
}