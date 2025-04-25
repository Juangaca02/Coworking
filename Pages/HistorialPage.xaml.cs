using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using TFG.Models;
using TFG.Services;
namespace TFG.Pages
{
    public partial class HistorialPage : ContentPage
    {
        private readonly SalaServicio _SalaServicio;
        public HistorialPage()
        {
            InitializeComponent();
            _SalaServicio = SalaServicio.GetInstancia();
        }

        // Manejar la acción de generar historial en Excel
        private void OnExcelClicked(object sender, EventArgs e)
        {
            // Aquí llamarás al servicio para generar el historial en Excel
            string desde = datePickerDesde.Date.ToString("yyyy-MM-dd");
            string hasta = datePickerHasta.Date.ToString("yyyy-MM-dd");

            // Lógica para generar el archivo Excel con el rango seleccionado
            GenerateExcelReport(desde, hasta);
        }

        // Manejar la acción de generar historial en PDF

        private async void OnPdfClicked(object sender, EventArgs e)
        {
            string desde = datePickerDesde.Date.ToString("yyyy-MM-dd");
            string hasta = datePickerHasta.Date.ToString("yyyy-MM-dd");

            // Obtener las reservas entre las fechas seleccionadas
            var servicio = SalaServicio.GetInstancia();
            List<Reservas> listReservas = await servicio.ObtenerReservasEntreFechas(datePickerDesde.Date, datePickerHasta.Date);

            // Lógica para generar el archivo PDF con el rango seleccionado
            GeneratePdfReport(listReservas, desde, hasta);
        }

        // Lógica para generar reporte Excel (puedes usar librerías como NPOI o EPPlus)
        private void GenerateExcelReport(string desde, string hasta)
        {

        }

        // Lógica para generar reporte PDF con las reservas
        // Lógica para generar reporte PDF con las reservas
        private async void GeneratePdfReport(List<Reservas> reservas, string desde, string hasta)
        {
            try
            {
                if (reservas.Count == 0)
                {
                    DisplayAlert("Sin Reservas", "No hay reservas para este rango de fechas.", "OK");
                    return;
                }

                // Crear el documento PDF
                string pdfPath = Path.Combine(FileSystem.CacheDirectory, "HistorialReserva.pdf");
                //Para crear un PDF necesitamos 3 clases diferentesPDFWriter, PDFDocument y el Document
                using (PdfWriter writer = new PdfWriter(pdfPath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        using (Document document = new Document(pdf))
                        {
                            // Título del documento
                            document.Add(new Paragraph("Historial de Reservas"));

                            // Agregar el rango de fechas seleccionado
                            document.Add(new Paragraph($"Desde: {desde} Hasta: {hasta}"));
                            document.Add(new Paragraph("\n"));

                            // Crear la tabla con los datos de las reservas
                            Table table = new Table(3); // Tres columnas: Sala, Fecha, Hora

                            // Cabecera de la tabla
                            table.AddHeaderCell("Sala");
                            table.AddHeaderCell("Fecha");
                            table.AddHeaderCell("Hora");

                            foreach (var reserva in reservas)
                            {
                                // Obtener el nombre de la sala desde la base de datos
                                string nombreSala = await _SalaServicio.ObtenerNombreSalaPorId(reserva.SalaId);

                                // Añadir los datos a la tabla
                                table.AddCell(nombreSala);  // Nombre de la sala
                                table.AddCell(reserva.Fecha.ToString("yyyy-MM-dd"));
                                table.AddCell(reserva.Hora);
                            }

                            await Shell.Current.DisplayAlert("Exito", "Informe PDF generado", "ok");

                            //Vamos a abrir el pdf con un visualizador predeterminado
                            await Launcher.OpenAsync(new OpenFileRequest
                            {
                                File = new ReadOnlyFile(pdfPath)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                DisplayAlert("Error", $"Hubo un problema al generar el archivo PDF: {ex.Message}", "OK");
            }
        }

        public async void OpenPdfFile(string filePath)
        {
            try
            {
                // Intentar abrir el archivo PDF con la aplicación predeterminada
                await Launcher.OpenAsync(new Uri(filePath));
            }
            catch (Exception ex)
            {
                // Manejar el error si no se puede abrir el archivo
                DisplayAlert("Error", "No se pudo abrir el archivo: " + ex.Message, "OK");
            }
        }
    }
}