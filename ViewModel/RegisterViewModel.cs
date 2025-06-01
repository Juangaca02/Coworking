using System.ComponentModel;
using System.Windows.Input;
using TFG.Models;
using TFG.Services;

namespace TFG.ViewModel
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly SalaServicio _servicio;

        public RegisterViewModel()
        {
            _servicio = SalaServicio.GetInstancia();
            RegistrarCommand = new Command(async () => await Registrar());
        }

        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public string ConfirmarContraseña { get; set; }

        public ICommand RegistrarCommand { get; }

        private async Task Registrar()
        {
            if (string.IsNullOrWhiteSpace(Usuario) || Usuario.Length < 4)
            {
                await App.Current.MainPage.DisplayAlert("Error", "El usuario debe tener al menos 4 caracteres.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || !EsGmailValido(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingresa un correo válido (ej: nombre@gmail.com).", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Contraseña) || Contraseña.Length < 4)
            {
                await App.Current.MainPage.DisplayAlert("Error", "La contraseña debe tener al menos 4 caracteres.", "OK");
                return;
            }

            if (Contraseña != ConfirmarContraseña)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                return;
            }

            var nuevoUsuario = new Usuarios
            {
                Nombre = Usuario.Trim(),
                Email = Email.Trim(),
                Contrasena = Contraseña,
                Admin = 0,
                Imagen = "usuario.png"
            };

            await _servicio.RegistrarUsuario(nuevoUsuario);
            var usuarioAutenticado = await _servicio.Autenticar(Usuario, Contraseña);

            if (usuarioAutenticado != null)
            {
                await App.Current.MainPage.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                SesionActual.UsuarioLogueado = usuarioAutenticado;
                App.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync("//menuPrincipal/TabInicio");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo iniciar sesión automáticamente.", "OK");
            }
        }
        private bool EsGmailValido(string email)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}
