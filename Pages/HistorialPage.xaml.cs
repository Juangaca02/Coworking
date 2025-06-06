using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using TFG.Models;
using TFG.Services;
namespace TFG.Pages
{
    public partial class HistorialPage : ContentPage
    {
        private readonly SalaServicio _SalaServicio;
        private List<object> reservasOriginales;

        public HistorialPage()
        {
            //try{
            InitializeComponent();
            _SalaServicio = SalaServicio.GetInstancia();
            CargarHistorialVisual();
            //}
            //catch (Exception ex)
            //{
            ////ventana en la que mostrar los fallos
            //Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            //}
        }

        private async void CargarHistorialVisual()
        {
            var reservas = await _SalaServicio.ObtenerTodasLasReservas();
            if (SesionActual.UsuarioLogueado.Admin != 1)
            {
                reservas = await _SalaServicio.ObtenerReservasPorUsuario(SesionActual.UsuarioLogueado.Id);
            }

            reservasOriginales = new List<object>();

            foreach (var reserva in reservas)
            {
                string usuarioNombre = await _SalaServicio.ObtenerNombreUsuarioPorId(reserva.UsuarioId);
                string salaNombre = await _SalaServicio.ObtenerNombreSalaPorId(reserva.SalaId);
                string fechaHora = $"{reserva.Fecha:dd/MM/yy} - {reserva.Hora}";

                reservasOriginales.Add(new
                {
                    UsuarioNombre = usuarioNombre,
                    SalaNombre = salaNombre,
                    Fecha = reserva.Fecha,
                    Hora = reserva.Hora,
                    FechaHora = fechaHora
                });
            }

            AplicarFiltros();
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void OnDateRangeChanged(object sender, DateChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            var filtroTexto = searchBar.Text?.ToLower() ?? string.Empty;
            var desde = datePickerDesde.Date;
            var hasta = datePickerHasta.Date;

            var filtradas = reservasOriginales
                .Where(r =>
                {
                    var reserva = (dynamic)r;
                    bool coincideTexto = reserva.SalaNombre.ToLower().Contains(filtroTexto) || reserva.UsuarioNombre.ToLower().Contains(filtroTexto);
                    bool enRango = reserva.Fecha >= desde && reserva.Fecha <= hasta;
                    return coincideTexto && enRango;
                })
                .ToList();

            tablaReservas.ItemsSource = filtradas;
            mensajeVacio.IsVisible = filtradas.Count == 0;
            btnPdf.IsEnabled = filtradas.Count > 0;
        }

        private void OnPdfClicked(object sender, EventArgs e)
        {
            var datosFiltrados = ((List<object>)tablaReservas.ItemsSource).Cast<dynamic>().ToList();
            string desde = datePickerDesde.Date.ToString("yyyy-MM-dd");
            string hasta = datePickerHasta.Date.ToString("yyyy-MM-dd");
            GeneratePdfReport(datosFiltrados, desde, hasta);
        }

        private async void GeneratePdfReport(List<dynamic> reservas, string desde, string hasta)
        {
            try
            {
                if (reservas.Count == 0)
                {
                    DisplayAlert("Sin Reservas", "No hay reservas para este rango de fechas.", "OK");
                    return;
                }

                string pdfPath = Path.Combine(FileSystem.CacheDirectory, "HistorialReserva" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                using (PdfWriter writer = new PdfWriter(pdfPath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    document.Add(new Paragraph("Historial de Reservas"));
                    document.Add(new Paragraph($"Desde: {desde} Hasta: {hasta}"));
                    document.Add(new Paragraph("\n"));

                    Table table = new Table(4);
                    table.AddHeaderCell("Sala");
                    table.AddHeaderCell("Usuario");
                    table.AddHeaderCell("Fecha");
                    table.AddHeaderCell("Hora");

                    foreach (var reserva in reservas)
                    {
                        table.AddCell(reserva.SalaNombre);
                        table.AddCell(reserva.UsuarioNombre);
                        table.AddCell(reserva.Fecha.ToString("yyyy-MM-dd"));
                        table.AddCell(reserva.Hora);
                    }

                    document.Add(table);

                    await Shell.Current.DisplayAlert("Exito", "Informe PDF generado", "OK");
                    await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(pdfPath) });
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Hubo un problema al generar el archivo PDF: {ex.Message}", "OK");
            }
        }

        public async void OpenPdfFile(string filePath)
        {
            try
            {
                await Launcher.OpenAsync(new Uri(filePath));
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "No se pudo abrir el archivo: " + ex.Message, "OK");
            }
        }
        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopAsync();
            });
            return true;
        }
    }
}
