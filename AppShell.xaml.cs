
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
            Routing.RegisterRoute("calendar", typeof(CalendarPage));
            Routing.RegisterRoute("salaDetail", typeof(SalaDetailPage));
            Routing.RegisterRoute("editarSala", typeof(EditarSalaPage));
            Routing.RegisterRoute("ajustesPage", typeof(AjustesPage));
            Routing.RegisterRoute(nameof(SalaDetailPage), typeof(SalaDetailPage));

            CargarInfoUsuario();
            ActualizarEstadoSesion();
        }
        public void ActualizarEstadoSesion()
        {
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
                SalasAjustesUsuario.IsVisible = false;
                btnCerrarSesion.IsVisible = false;
            }
            else
            {
                if (SesionActual.UsuarioLogueado.Admin == 1)
                {
                    TabHistorial.IsVisible = false;
                }
                else
                {
                    TabHistorial.IsVisible = true;
                }
                // Si el usuario está logueado, ocultar las opciones de login y registro
                LoginMenuItem.IsVisible = false;
                RegisterMenuItem.IsVisible = false;
                DefaultContent.IsVisible = false;  // Ocultar el contenido por defecto

                // Mostrar las pestañas correspondientes
                TabHistorial.IsVisible = true;
                TabInicio.IsVisible = true;
                TabSalas.IsVisible = true;
                SalasAjustesUsuario.IsVisible = true;
                btnCerrarSesion.IsVisible = true;

                // Este código no es necesario si navegamos explícitamente desde la página de login
                // pero es bueno tenerlo como respaldo
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync("//menuPrincipal/TabInicio");
                });
            }
        }

        private void CargarInfoUsuario()
        {
            var usuario = SesionActual.UsuarioLogueado;
            if (usuario != null)
            {
                UsuarioLabel.Text = usuario.Nombre;

                if (!string.IsNullOrEmpty(usuario.Imagen))
                {
                    try
                    {
                        var base64 = usuario.Imagen;
                        imgUsuarioFlyout.Source = ImageSource.FromStream(() =>
                        {
                            byte[] bytes = Convert.FromBase64String(base64);
                            return new MemoryStream(bytes);
                        });
                    }
                    catch
                    {
                        imgUsuarioFlyout.Source = "default_user.png";
                    }
                }
                else
                {
                    imgUsuarioFlyout.Source = "default_user.png";
                }
            }
            else
            {
                UsuarioLabel.Text = "Invitado";
                imgUsuarioFlyout.Source = "default_user.png";
            }
        }

        private async void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Cerrar sesión", "¿Estás seguro que quieres cerrar sesión?", "Sí", "No");
            if (confirm)
            {
                SesionActual.UsuarioLogueado = null;
                ActualizarEstadoSesion();
            }
        }
    }
}

