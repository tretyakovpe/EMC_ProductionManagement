using iText.Barcodes;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ProductionManagement.Models;

namespace ProductionManagement.Services
{
    public class LabelService : IDisposable
    {
        private readonly LoggerService _logger;
        private string _baseDir = AppDomain.CurrentDomain.BaseDirectory;

        public LabelService(LoggerService logger)
        {
            _logger = logger;
        }

        public void Dispose() { }

        public async Task GenerateAndPrintLabelAsync(Prod box, string Description, Material material, string PrinterName)
        {
            try
            {
                // Генерируем имя файла для PDF
                string fileName = System.IO.Path.Combine($"{_baseDir}/pdf/", $"{box.Label}.pdf");

                // Генерация PDF-файла
                await GeneratePdfAsync(box, Description, material, fileName);

                // Отправляем PDF на печать
                await PrintPdfAsync(fileName, PrinterName);
            }
            catch (Exception ex)
            {
                _logger.SendLog($"Ошибка создания файла в службе Label Service {ex.Message}", "error");
            }

        }

        private async Task GeneratePdfAsync(Prod box, string description, Material material, string filename)
        {
            //_logger.SendLog($"Создаём бирку для {box.Line}-{box.Label}-{description} - {material.CustomerCode}", "info");
            try
            {
                Label label = new();
                string DT = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                // Теперь устанавливаем значения полей
                label.SetField("Destination", "АвтоВАЗ");
                label.SetField("Delivery place", material.Destination);
                label.SetField("Document #", "992410");
                label.SetField("Netto", (material.Netto * box.Amount).ToString());
                label.SetField("Brutto", (material.Netto * box.Amount + material.Brutto).ToString());
                label.SetField("Boxes", "1");
                label.SetField("Product", material.CustomerCode);
                label.SetField("Quantity", box.Amount.ToString());
                label.SetField("Part name", box.Material);
                label.SetField("Label number", box.Label);
                label.SetField("Supplier", "");
                label.SetField("Date", DT);
                label.SetField("Packing type", material.HU);
                label.SetField("Description", description); // Обратите внимание, тут имя поля пустое (" ")

                PdfFontFactory.RegisterSystemDirectories();
                //var fontLibrary = PdfFontFactory.GetRegisteredFonts();
                PdfFont font = PdfFontFactory.CreateRegisteredFont("arial");
                PdfDocument pdfDoc = new(new PdfWriter(filename));
                pdfDoc.SetDefaultPageSize(PageSize.A5);
                Document doc = new(pdfDoc);
                PdfPage page = pdfDoc.AddNewPage()
                    .SetIgnorePageRotationForContent(true)
                    .SetRotation(90);
                PdfCanvas canvas = new(page);
                Barcode128 barcode = new(pdfDoc);
                double mm = Label.MM;

                foreach (int[] b in Label.Borders)
                {
                    canvas
                        .MoveTo(b[2] * mm, b[3] * mm)
                        .LineTo(b[0] * mm, b[1] * mm)
                        .ClosePathStroke();
                }

                foreach (Field field in label.LabelFields)
                {
                    //  НАЗВАНИЕ ПОЛЯ
                    doc.ShowTextAligned(new Paragraph(field.Name).SetFontSize(8).SetFont(font),
                        (float)(field.X * mm), (float)(field.Y * mm), TextAlignment.LEFT, VerticalAlignment.TOP);
                    //  ЗНАЧЕНИЕ
                    doc.ShowTextAligned(new Paragraph(field.Text).SetFontSize(18).SetFont(font),
                        (float)(field.X * mm + 20), (float)(field.Y * mm - 9), TextAlignment.LEFT, VerticalAlignment.TOP);
                    //  ШТРИХКОД
                    if (field.Barcode == true)
                    {
                        barcode.SetCode(field.Code + field.Text);
                        barcode.SetSize(1);
                        barcode.SetBarHeight(field.bcSize);
                        PdfFormXObject img = barcode.CreateFormXObject(ColorConstants.BLACK, ColorConstants.WHITE, pdfDoc);
                        //canvas.AddXObjectAt(img, (float)(field.X * mm + 60), (float)(field.Y * mm - 70));
                        canvas.AddXObjectAt(img, (float)(field.bcX * mm), (float)(field.bcY * mm));
                    }
                }
                canvas.Release();
                doc.Close();
            }
            catch (Exception ex)
            {
                _logger.SendLog($"Ошибка при генерации PDF файла {ex.Message}", "error");
            }
        }

        private async Task PrintPdfAsync(string fileName, string PrinterName)
        {
            try
            {
                // Код для печати PDF-файла
                File.Copy(fileName, @"\\NAS\" + PrinterName, true);
            }
            catch (Exception ex)
            {
                _logger.SendLog($"Ошибка печати {fileName} на {PrinterName}: {ex.Message}", "error");
            }
        }
    }
}
