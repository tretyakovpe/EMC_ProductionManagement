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
        public LabelService(LoggerService logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {

        }

        public async Task GenerateAndPrintLabelAsync(Prod box, string Description, Material material, string PrinterName)
        {
            try
            {
                // Генерируем имя файла для PDF
                string fileName = System.IO.Path.Combine(@"./pdf/", $"{box.Label}.pdf");

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
            _logger.SendLog($"Создаём бирку для {box.Line}-{box.Label}-{description} - {material.CustomerCode}", "info");
            try
            {
                Label label = new();
                string DT = DateTime.Now.ToString("dd.MM.yyyy HH:mm: ss");
                label.LabelFields[0].Value = "АвтоВАЗ";
                label.LabelFields[1].Value = material.Destination;
                label.LabelFields[2].Value = "992410";
                label.LabelFields[4].Value = (material.Netto * box.Amount).ToString();
                label.LabelFields[5].Value = (material.Netto * box.Amount + material.Brutto).ToString();
                label.LabelFields[6].Value = "1";
                label.LabelFields[7].Value = material.CustomerCode;
                label.LabelFields[8].Value = box.Amount.ToString();
                label.LabelFields[9].Value = box.Material;
                label.LabelFields[10].Value = box.Label;
                label.LabelFields[11].Value = "M0FAP";
                label.LabelFields[12].Value = DT;
                label.LabelFields[13].Value = material.HU;
                label.LabelFields[14].Value = description;

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

                foreach (int[] b in Label.borders)
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
                    doc.ShowTextAligned(new Paragraph(field.Value).SetFontSize(18).SetFont(font),
                        (float)(field.X * mm + 20), (float)(field.Y * mm - 9), TextAlignment.LEFT, VerticalAlignment.TOP);
                    //  ШТРИХКОД
                    if (field.Barcode == true)
                    {
                        barcode.SetCode(field.Code + field.Value);
                        barcode.SetSize(1);
                        barcode.SetBarHeight(32);
                        PdfFormXObject img = barcode.CreateFormXObject(ColorConstants.BLACK, ColorConstants.WHITE, pdfDoc);
                        canvas.AddXObjectAt(img, (float)(field.X * mm + 60), (float)(field.Y * mm - 70));
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
