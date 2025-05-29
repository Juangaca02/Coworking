using TFG.Models;
using TFG.Services;

namespace TFG.Pages;

public partial class EditarSalaPage : ContentPage
{
    private Salas _sala;
    private SalaServicio _servicio;
    public EditarSalaPage(Salas sala)
    {
        InitializeComponent();

        _servicio = SalaServicio.GetInstancia();
        _sala = sala;

        CargarDatosSala();
    }

    private void CargarDatosSala()
    {
        entryNombre.Text = _sala.Nombre;
        entryCapacidad.Text = _sala.Capacidad.ToString();
        entryEquipamiento.Text = string.Join(", ", _sala.Equipamiento);
        pickerEstado.SelectedItem = _sala.Estado;
    }

    private async void GuardarCambios_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryNombre.Text) ||
            !int.TryParse(entryCapacidad.Text, out int capacidad) ||
            pickerEstado.SelectedItem == null)
        {
            await DisplayAlert("Error", "Por favor, rellena todos los campos correctamente.", "OK");
            return;
        }

        _sala.Nombre = entryNombre.Text.Trim();
        _sala.Capacidad = capacidad;
        _sala.Equipamiento = entryEquipamiento.Text?.Split(',').Select(s => s.Trim()).ToList() ?? new List<string>();
        _sala.Estado = pickerEstado.SelectedItem.ToString();

        try
        {
            await _servicio.ActualizarSala(_sala);
            await DisplayAlert("Éxito", "Sala actualizada correctamente.", "OK");
            await Navigation.PopToRootAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar la sala: {ex.Message}", "OK");
        }
    }
}