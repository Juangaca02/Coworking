
using TFG.Models;
using TFG.Pages;

namespace TFG
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rutas para la navegación
            Routing.RegisterRoute("historial", typeof(HistorialPage));
            Routing.RegisterRoute("inicio", typeof(MainPage));
            Routing.RegisterRoute("salas", typeof(SalasPage));
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));

            // Verificar si el usuario está logueado
            if (SesionActual.UsuarioLogueado == null)
            {
                // Si no está logueado, mostrar el menú de login y registro
                LoginMenuItem.IsVisible = true;
                RegisterMenuItem.IsVisible = true;
                menuPrincipal.IsVisible = true;
                DefaultContent.IsVisible = true;  // Mostrar el contenido por defecto

                // Ocultar las pestañas y contenido
                TabHistorial.IsVisible = false;
                TabInicio.IsVisible = false;
                TabSalas.IsVisible = false;

                // No mostrar el nombre de usuario en la cabecera
                UsuarioLabel.IsVisible = false;
            }
            else
            {
                // Si el usuario está logueado, ocultar las opciones de login y registro
                LoginMenuItem.IsVisible = false;
                RegisterMenuItem.IsVisible = false;
                DefaultContent.IsVisible = false;  // Ocultar el contenido por defecto

                // Mostrar las pestañas correspondientes
                TabHistorial.IsVisible = true;
                TabInicio.IsVisible = true;
                TabSalas.IsVisible = true;

                // Mostrar el nombre de usuario en la cabecera
                UsuarioLabel.IsVisible = true;
                UsuarioLabel.Text = SesionActual.UsuarioLogueado.Nombre;

                // Este código no es necesario si navegamos explícitamente desde la página de login
                // pero es bueno tenerlo como respaldo
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync("//menuPrincipal/TabInicio");
                });
            }
        }
    }
}
