using System.Collections.Generic;
using System.Globalization;
using api.EventHandlers;
using api.Objects.CompanyLevel;
using api.ReportClasses.Summaries;
using api.ReportClasses.Templates;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace api.ReportClasses.CompanyLevel
{
    ///<summary>
    /// Returns a byte array representing the named PDF Report
    ///</summary>
    public class CompanySalesSummary : PortraitTemplate
    {
        public Summary getPDF(List<Company> companyList, List<saleSummary> salesSummaryList, string startDate, string endDate)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFs/Test/company-sales-summary-test.pdf");
            reader = new PdfReader("./Files/PDFs/Templates/empty-branch-sales.pdf");
            pdfdoc = new PdfDocument(reader, writer);

            document = new Document(pdfdoc, pageSize, false);
            document.SetMargins(80, 50, 50, 50);

            Image backgroundImage = new Image(ImageDataFactory.Create("./Files/Images/portrait-background.png"));
            backgroundImage.SetMaxWidth(792).SetMaxHeight(1150);

            BackgroundImageHandler handler = new BackgroundImageHandler(backgroundImage, writer);
            pdfdoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

            #region add AcroForm Fields
            form = PdfAcroForm.GetAcroForm(pdfdoc, true);

            PdfTextFormField nameField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 862, 200, 15), "name", "-AAA-").SetColor(ColorConstants.WHITE).SetFontAndSize(nunito, (float)14);
            nameField.SetJustification(PdfFormField.ALIGN_LEFT);
            form.AddField(nameField);

            PdfTextFormField dateField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 824, 100, 15), "date", $"---").SetFont(nunitoBold).SetColor(purpleFontColor);
            dateField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(dateField);

            PdfTextFormField addressField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 787, 200, 15), "address", "---").SetFont(nunitoBold).SetColor(purpleFontColor);
            addressField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(addressField);

            PdfTextFormField accountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 750, 100, 15), "account", "---").SetFont(nunitoBold).SetColor(purpleFontColor);
            accountField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(accountField);

            PdfTextFormField currencyField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 710, 50, 15), "currency", "NGN").SetFont(nunitoBold).SetColor(purpleFontColor);
            currencyField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(currencyField);
            #endregion

            foreach (var item in companyList)
            {
                nameField.SetValue($"{item.name}");
                addressField.SetValue($"{item.street}, {item.city}, {item.state}.");
                accountField.SetValue("Station");

                addLogo(item, 750);
            }

            double totalSales = 0.0;
            double pmsPumpSale = 0.0;
            double agoPumpSale = 0.0;
            double dpkPumpSale = 0.0;
            double pmsTankSale = 0.0;
            double agoTankSale = 0.0;
            double dpkTankSale = 0.0;

            foreach (var item in salesSummaryList)
            {
                totalSales += item.pmsPumpSale;
                totalSales += item.pmsTankSale;
                totalSales += item.agoPumpSale;
                totalSales += item.agoTankSale;
                totalSales += item.dpkPumpSale;
                totalSales += item.dpkTankSale;

                pmsPumpSale += item.pmsPumpSale;
                agoPumpSale += item.agoPumpSale;
                dpkPumpSale += item.dpkPumpSale;
                pmsTankSale += item.pmsTankSale;
                agoTankSale += item.agoTankSale;
                dpkTankSale += item.dpkTankSale;
            }

            // dateField.SetValue($"{startDate} - {endDate}"); 

            #region Create Table
            Table dataTable = new Table(new float[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, true);
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Date", "Branch", "PMS Pump Sale", "PMS Tank Sale", "PMS Surplus", "AGO Pump Sale", "AGO Tank Sale", "AGO Surplus", "DPK Pump Sale", "DPK Tank Sale", "DPK Surplus" };

            foreach (var item in itemHeaders)
            {
                dataTable.AddCell(new Cell()
                            .SetBackgroundColor(headerBackground)
                            .SetBorder(Border.NO_BORDER)
                            .Add(new Paragraph(item))
                            .SetFontSize(7)
                            .SetFontColor(purpleFontColor)
                            .SetFont(nunitoBold))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }

            for (int i = 0; i < 9; i++)
            {
                // prevents the table overlapping the other elements
                document.Add(new Paragraph("\n"));
            }
            #endregion

            #region add items to table
            int sn = 1;
            foreach (var item in salesSummaryList)
            {
                if (sn % 2 != 0)
                {
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.branchName}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.pmsPumpSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.pmsTankSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.pmsSurplus}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.agoPumpSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.agoTankSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.agoSurplus}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.dpkPumpSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.dpkTankSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.dpkSurplus}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                }

                else if (sn % 2 == 0)
                {
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.branchName}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.pmsPumpSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.pmsTankSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.pmsSurplus}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.agoPumpSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.agoTankSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.agoSurplus}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.dpkPumpSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.dpkTankSale}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.dpkSurplus}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                }
                sn++;
            }
            #endregion
            
            document.Add(dataTable);
            addFooter();
            form.FlattenFields();
            document.Flush();
            document.Close();

            Summary summary = new();
            summary.pmsTankSale = $"{pmsTankSale.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.pmsPumpSale = $"{pmsPumpSale.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.agoPumpSale = $"{agoPumpSale.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.dpkPumpSale = $"{dpkPumpSale.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.pmsTankSale = $"{pmsTankSale.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.agoTankSale = $"{agoTankSale.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.dpkTankSale = $"{dpkTankSale.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.TotalAmount = $"{totalSales.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";

            summary.PdfReport = pdfstream.ToArray();
            return summary;
        }
    }
}