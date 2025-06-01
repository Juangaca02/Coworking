using System.ComponentModel;
using System.Windows.Input;
using TFG.Models;
using TFG.Services;
using Microsoft.Maui.Controls;

namespace TFG.ViewModel
{
    public class EditarSalaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string nombre) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));

        private readonly SalaServicio _servicio;
        private readonly Salas _salaOriginal;

        public EditarSalaViewModel(Salas sala)
        {
            _servicio = SalaServicio.GetInstancia();
            _salaOriginal = sala;

            // Inicializar propiedades editables
            Nombre = sala.Nombre;
            Capacidad = sala.Capacidad.ToString();
            Equipamiento = string.Join(", ", sala.Equipamiento);
            Estado = sala.Estado;

            EstadosDisponibles = new List<string> { "Disponible", "En Mantenimiento" };

            GuardarCambiosCommand = new Command(async () => await GuardarCambios());
        }

        public string Nombre { get; set; }
        public string Capacidad { get; set; }
        public string Equipamiento { get; set; }
        public string Estado { get; set; }

        public List<string> EstadosDisponibles { get; }

        public ICommand GuardarCambiosCommand { get; }

        private async Task GuardarCambios()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || !int.TryParse(Capacidad, out int capacidad) || string.IsNullOrEmpty(Estado))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor, rellena todos los campos correctamente.", "OK");
                return;
            }

            _salaOriginal.Nombre = Nombre.Trim();
            _salaOriginal.Capacidad = capacidad;
            _salaOriginal.Equipamiento = Equipamiento.Split(',').Select(e => e.Trim()).ToList();
            _salaOriginal.Estado = Estado;

            try
            {
                await _servicio.ActualizarSala(_salaOriginal);
                await Application.Current.MainPage.DisplayAlert("Éxito", "Sala actualizada correctamente.", "OK");
                await Shell.Current.GoToAsync("//menuPrincipal/TabSalas");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar la sala: {ex.Message}", "OK");
            }
        }
    }
}
