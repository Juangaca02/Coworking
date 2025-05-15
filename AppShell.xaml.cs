
using TFG.Models;

namespace TFG
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Mostrar las páginas de contenido solo si el usuario está logueado
            if (SesionActual.UsuarioLogueado == null)
            {
                // Deshabilitar el menú hamburguesa si no está logueado
                FlyoutIsPresented = false;
            }
            else
            {
                // Habilitar el menú hamburguesa si está logueado
                FlyoutIsPresented = true;
            }
        }
    }
}
