using TFG.Models;
using TFG.ViewModel;

namespace TFG.Pages;

public partial class EditarSalaPage : ContentPage
{
    public EditarSalaPage(Salas sala)
    {
        InitializeComponent();
        BindingContext = new EditarSalaViewModel(sala);
    }
    private async void Volver_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//menuPrincipal/TabSalas");
    }
}
