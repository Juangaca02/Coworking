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
            InitializeComponent();
            _SalaServicio = SalaServicio.GetInstancia();
            CargarHistorialVisual();
        }

        private async void CargarHistorialVisual()
        {
            var reservas = await _SalaServicio.ObtenerTodasLasReservas();
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
                    bool coincideTexto = reserva.SalaNombre.ToLower().Contains(filtroTexto);
                    bool enRango = reserva.Fecha >= desde && reserva.Fecha <= hasta;
                    return coincideTexto && enRango;
                })
                .ToList();

            tablaReservas.ItemsSource = filtradas;
            mensajeVacio.IsVisible = filtradas.Count == 0;
            btnPdf.IsEnabled = filtradas.Count > 0;
        }

        private void OnExcelClicked(object sender, EventArgs e)
        {
            string desde = datePickerDesde.Date.ToString("yyyy-MM-dd");
            string hasta = datePickerHasta.Date.ToString("yyyy-MM-dd");
            GenerateExcelReport(desde, hasta);
        }

        private async void GenerateExcelReport(string desde, string hasta)
        {
            try
            {
                var servicio = SalaServicio.GetInstancia();
                var reservas = await servicio.ObtenerReservasEntreFechas(datePickerDesde.Date, datePickerHasta.Date);

                if (reservas.Count == 0)
                {
                    await DisplayAlert("Sin Reservas", "No hay reservas para este rango de fechas.", "OK");
                    return;
                }

                var filePath = Path.Combine(FileSystem.CacheDirectory, "HistorialReserva.xlsx");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Historial");
                    sheet.Cells[1, 1].Value = "Sala";
                    sheet.Cells[1, 2].Value = "Fecha";
                    sheet.Cells[1, 3].Value = "Hora";

                    using (var range = sheet.Cells[1, 1, 1, 3])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    }

                    int row = 2;
                    foreach (var reserva in reservas)
                    {
                        string salaNombre = await _SalaServicio.ObtenerNombreSalaPorId(reserva.SalaId);
                        sheet.Cells[row, 1].Value = salaNombre;
                        sheet.Cells[row, 2].Value = reserva.Fecha.ToString("yyyy-MM-dd");
                        sheet.Cells[row, 3].Value = reserva.Hora;
                        row++;
                    }

                    sheet.Cells.AutoFitColumns();
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                }

                await Shell.Current.DisplayAlert("Éxito", "Informe Excel generado", "OK");
                await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo generar el archivo Excel: " + ex.Message, "OK");
            }
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

                string pdfPath = Path.Combine(FileSystem.CacheDirectory, "HistorialReserva.pdf");
                using (PdfWriter writer = new PdfWriter(pdfPath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    document.Add(new Paragraph("Historial de Reservas"));
                    document.Add(new Paragraph($"Desde: {desde} Hasta: {hasta}"));
                    document.Add(new Paragraph("\n"));

                    Table table = new Table(3);
                    table.AddHeaderCell("Sala");
                    table.AddHeaderCell("Fecha");
                    table.AddHeaderCell("Hora");

                    foreach (var reserva in reservas)
                    {
                        table.AddCell(reserva.SalaNombre);
                        table.AddCell(reserva.Fecha.ToString("yyyy-MM-dd"));
                        table.AddCell(reserva.Hora);
                    }

                    document.Add(table);

                    await Shell.Current.DisplayAlert("Éxito", "Informe PDF generado", "OK");
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
    }
}
