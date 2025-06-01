using System.ComponentModel;
using System.Windows.Input;
using TFG.Models;
using TFG.Services;

namespace TFG.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly SalaServicio _servicio;

        public string Usuario { get; set; }
        public string Contraseña { get; set; }

        public ICommand IniciarSesionCommand { get; }
        public ICommand IrARegistroCommand { get; }

        public LoginViewModel()
        {
            _servicio = SalaServicio.GetInstancia();
            IniciarSesionCommand = new Command(async () => await IniciarSesion());
            IrARegistroCommand = new Command(async () => await IrARegistro());
        }

        private async Task IniciarSesion()
        {
            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Contraseña))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            var usuarioAutenticado = await _servicio.Autenticar(Usuario.Trim(), Contraseña);
            if (usuarioAutenticado != null)
            {
                SesionActual.UsuarioLogueado = usuarioAutenticado;
                App.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync("//menuPrincipal/TabInicio");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
            }
        }

        private async Task IrARegistro()
        {
            await Shell.Current.GoToAsync("register");
        }
    }
}
