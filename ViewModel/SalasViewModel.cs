using System.Collections.ObjectModel;
using System.ComponentModel;
using TFG.Models;
using TFG.Services;

namespace TFG.ViewModel
{
    public class SalasViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Salas> _salasDisponibles;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            // Se dispara el evento PropertyChanged para que la vista se entere del cambio
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Salas> SalasDisponibles
        {
            get => _salasDisponibles;
            set
            {
                _salasDisponibles = value;
                OnPropertyChanged(nameof(SalasDisponibles));
            }
        }

        public SalasViewModel()
        {
            SalasDisponibles = new ObservableCollection<Salas>();
            Task.Run(async () => await CargarSalas());
        }

        public async Task RecargarSalas()
        {
            var servicio = SalaServicio.GetInstancia();
            var salas = await servicio.ObtenerTodasLasSalas();
            SalasDisponibles = new ObservableCollection<Salas>(salas);
        }

        private async Task CargarSalas()
        {
            var servicio = SalaServicio.GetInstancia();
            await servicio.InicializarDatos();
            var salas = await servicio.ObtenerTodasLasSalas();
            SalasDisponibles = new ObservableCollection<Salas>(salas);
        }

    }
}
