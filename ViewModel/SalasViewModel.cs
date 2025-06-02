using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TFG.Models;
using TFG.Services;

namespace TFG.ViewModel
{
    public class SalasViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Salas> _salasDisponibles;
        private bool _isLoading;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
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
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !IsLoading;

        public SalasViewModel()
        {
            SalasDisponibles = new ObservableCollection<Salas>();
            IsLoading = true;
            Task.Run(async () => await CargarSalas());
        }

        public async Task RecargarSalas()
        {
            IsLoading = true;
            var servicio = SalaServicio.GetInstancia();
            var salas = await servicio.ObtenerTodasLasSalas();
            SalasDisponibles = new ObservableCollection<Salas>(salas);
            IsLoading = false;
        }

        private async Task CargarSalas()
        {
            IsLoading = true;
            var servicio = SalaServicio.GetInstancia();
            await servicio.InicializarDatos();
            var salas = await servicio.ObtenerTodasLasSalas();
            SalasDisponibles = new ObservableCollection<Salas>(salas);
            IsLoading = false;
        }

    }
}
