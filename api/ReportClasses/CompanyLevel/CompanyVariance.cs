using System.Collections.Generic;
using api.EventHandlers;
using api.Objects.AnyLevel;
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
    public class CompanyVariance : PortraitTemplate
    {
        public Summary getPdf(List<Company> companyList, List<Variance> varianceList)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFs/Test/variance-report-test.pdf");
            reader = new PdfReader("./Files/PDFs/Templates/base-three-slot-table.pdf");
            pdfdoc = new PdfDocument(reader, writer);

            document = new Document(pdfdoc, pageSize, false);
            document.SetMargins(80, 50, 50, 50);

            Image backgroundImage = new Image(ImageDataFactory.Create("./Files/Images/portrait-background.png"));
            backgroundImage.SetMaxWidth(792).SetMaxHeight(1150);

            BackgroundImageHandler handler = new BackgroundImageHandler(backgroundImage, writer);
            pdfdoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

            double totalEpumpVolumeSold = 0;
            double totalManualVolumeSold = 0;
            double totalVariance = 0;

            #region add AcroForm Fields
            form = PdfAcroForm.GetAcroForm(pdfdoc, true);

            PdfTextFormField nameField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 1005, 200, 15), "name", "---").SetColor(ColorConstants.WHITE).SetFontAndSize(nunito, (float)14);
            nameField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(nameField);

            PdfTextFormField dateField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 968, 100, 15), "date", $"{dt:d}").SetFont(nunitoBold).SetColor(purpleFontColor);
            dateField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(dateField);

            PdfTextFormField addressField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 931, 260, 15), "address", "---").SetFont(nunitoBold).SetColor(purpleFontColor);
            addressField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(addressField);

            PdfTextFormField accountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 894, 100, 15), "account", "Company").SetFont(nunitoBold).SetColor(purpleFontColor);
            accountField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(accountField);

            PdfTextFormField currencyField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 854, 50, 15), "currency", "NGN").SetFont(nunitoBold).SetColor(purpleFontColor);
            currencyField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(currencyField);

            PdfTextFormField epumpSoldField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(241, 790, 157, 15), "epumpVol", "Total Epump Volume Sold").SetFontAndSize(nunitoBold, 10f).SetColor(ColorConstants.BLACK);
            epumpSoldField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(epumpSoldField);

            PdfTextFormField epumpSoldValue = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(241, 730, 157, 15), "variance", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            epumpSoldValue.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(epumpSoldValue);

            PdfTextFormField manualSoldField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(400, 790, 165, 15), "ManualVol", "Total Manual Volume Sold").SetFontAndSize(nunitoBold, 10f).SetColor(ColorConstants.BLACK);
            manualSoldField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(manualSoldField);

            PdfTextFormField manualSoldValue = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(400, 730, 165, 15), "variance", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            manualSoldValue.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(manualSoldValue);

            PdfTextFormField varianceField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(565, 790, 170, 15), "variance", "Variance").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            varianceField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(varianceField);

            PdfTextFormField varianceValue = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(565, 730, 170, 15), "variance", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            varianceValue.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(varianceValue);

            #endregion

            Table dataTable = new Table(new float[] { 1, 2, 2, 1, 3, 3, 3 }, true);
            // this keeps the table aligned with the rest of the document
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Date", "Branch", "Product", "Epump Volume Sold(litres)", "Manual Volume Sold(litres)", "Variance(litres)" };

            foreach (var item in itemHeaders)
            {
                dataTable.AddCell(new Cell()
                            .SetBackgroundColor(headerBackground)
                            .SetBorder(Border.NO_BORDER)
                            .Add(new Paragraph(item))
                            .SetFontSize(10)
                            .SetFontColor(purpleFontColor)
                            .SetFont(nunitoBold))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }

            for (int i = 0; i < 13; i++)
            {
                // prevents the table overlapping the other elements
                document.Add(new Paragraph("\n"));
            }

            #region //* Insert into Table
            int sn = 1;
            double variance = 0;
            foreach (var item in varianceList)
            // add items to table.
            {
                if (sn % 2 != 0)
                {
                    variance = item.volumeSold - item.mannualVolumeSold;
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.branchName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.productName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));

                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"₦{item.volumeSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.mannualVolumeSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{variance.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    totalEpumpVolumeSold += item.volumeSold;
                    totalManualVolumeSold += item.mannualVolumeSold;
                }
                else if (sn % 2 == 0)
                {
                    variance = item.volumeSold - item.mannualVolumeSold;
                    dataTable.AddCell(
                    new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.branchName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.productName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"₦{item.volumeSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.mannualVolumeSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{variance.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    totalEpumpVolumeSold += item.volumeSold;
                    totalManualVolumeSold += item.mannualVolumeSold;
                }
                sn++;
            }
            #endregion

            epumpSoldValue.SetValue($"{totalEpumpVolumeSold.ToString("0.00")} L");
            manualSoldValue.SetValue($"{totalManualVolumeSold.ToString("0.00")} L");
            totalVariance = totalEpumpVolumeSold - totalManualVolumeSold;
            varianceValue.SetValue($"{totalVariance.ToString("0.00")}");

            foreach (var item in companyList)
            {
                addressField.SetValue($"{item.street}, {item.state}.");
                nameField.SetValue($"{item.name}");

                addLogo(item, 900);
            }

            document.Add(dataTable);
            addFooter();
            form.FlattenFields();
            document.Flush();
            document.Close();

            Summary summary = new();
            summary.TotalEpumpVolumeSold = $"{totalEpumpVolumeSold:0.00}L";
            summary.TotalManualVolumeSold = $"{totalManualVolumeSold:0.00}L";
            summary.TotalVariance = $"{totalVariance:0.00}L";
            summary.PdfReport = pdfstream.ToArray();
            return summary;
        }
    }
}