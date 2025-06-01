using TFG.Models;
using TFG.Services;

namespace TFG.Pages;

public partial class AjustesPage : ContentPage
{

    private SalaServicio _salaServicio;
    private Usuarios _usuarioActual;
    private byte[] imagenTemporalBytes = null;


    public AjustesPage()
    {
        InitializeComponent();
        _salaServicio = SalaServicio.GetInstancia();

        CargarDatosUsuario();
    }

    /**
    * Metodo para recargar la imagen del usuario al volver a la pantalla
    * 
    */
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Limpiar imagen temporal porque ya no debe usarse
        imagenTemporalBytes = null;

        // Recargar la imagen real
        if (!string.IsNullOrEmpty(_usuarioActual.Imagen))
        {
            try
            {
                var base64 = _usuarioActual.Imagen;
                imgUsuario.Source = ImageSource.FromStream(() =>
                {
                    byte[] bytes = Convert.FromBase64String(base64);
                    return new MemoryStream(bytes);
                });
            }
            catch
            {
                imgUsuario.Source = "usuario.png";
            }
        }
        else
        {
            imgUsuario.Source = "usuario.png";
        }
    }

    /*
    * Metodo para cargar los datos del usuario logueado
    * 
    */
    private void CargarDatosUsuario()
    {
        _usuarioActual = SesionActual.UsuarioLogueado;
        if (_usuarioActual != null)
        {
            entryNombre.Text = _usuarioActual.Nombre;
            entryEmail.Text = _usuarioActual.Email;
            if (!string.IsNullOrEmpty(_usuarioActual.Imagen))
            {
                try
                {
                    var base64 = _usuarioActual.Imagen;
                    imgUsuario.Source = ImageSource.FromStream(() =>
                    {
                        byte[] bytes = Convert.FromBase64String(base64);
                        return new MemoryStream(bytes);
                    });
                }
                catch
                {
                    imgUsuario.Source = "usuario.png";
                }
            }
            else
            {
                imgUsuario.Source = "usuario.png";
            }

        }
    }

    /**
    * Metodo para seleccionar una imagen de la galer�a y mostrarla en la pantalla
    * 
    */
    private async void CambiarImagen_Clicked(object sender, EventArgs e)
    {
        try
        {
            var resultado = await MediaPicker.PickPhotoAsync();
            if (resultado != null)
            {
                using var stream = await resultado.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                imagenTemporalBytes = ms.ToArray();

                // Mostrar previsualizaci�n
                imgUsuario.Source = ImageSource.FromStream(() => new MemoryStream(imagenTemporalBytes));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cargar la imagen: " + ex.Message, "OK");
        }
    }

    /**
    * M�todo para guardar los cambios realizados en la pantalla
    * 
    */
    private async void GuardarCambios_Clicked(object sender, EventArgs e)
    {
        string nombre = entryNombre.Text?.Trim();
        string email = entryEmail.Text?.Trim();
        string contrasena = entryContrasena.Text;
        string confirmarContrasena = entryConfirmarContrasena.Text;

        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Error", "Nombre y Email son obligatorios.", "OK");
            return;
        }

        if (!string.IsNullOrEmpty(contrasena) || !string.IsNullOrEmpty(confirmarContrasena))
        {
            if (contrasena != confirmarContrasena)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                return;
            }
            _usuarioActual.Contrasena = contrasena;
        }
        if (imagenTemporalBytes != null)
        {
            _usuarioActual.Imagen = Convert.ToBase64String(imagenTemporalBytes);
        }

        _usuarioActual.Nombre = nombre;
        _usuarioActual.Email = email;

        try
        {
            bool exito = await _salaServicio.ActualizarUsuario(_usuarioActual);
            if (exito)
            {
                await DisplayAlert("Exito", "Datos actualizados correctamente.", "OK");
                SesionActual.UsuarioLogueado = _usuarioActual;
                App.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync("//menuPrincipal");
                CargarDatosUsuario();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al actualizar: " + ex.Message, "OK");
        }
    }

    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Navigation.PopAsync();
        });

        return true;
    }

}