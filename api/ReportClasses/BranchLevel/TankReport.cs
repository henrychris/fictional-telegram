using System.Collections.Generic;
using api.EventHandlers;
using api.Objects.RetailLevel;
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

namespace api.ReportClasses.BranchLevel
{
    ///<summary>
    /// Returns a byte array representing the named PDF Report
    ///</summary>
    public class TankReport : PortraitTemplate
    {
        public byte[] getPdf(string branchId, List<BranchDetails> branchDetails, List<TankReportObj> tankReportList, string startDate, string endDate)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFs/Test/branch-tank-report-test.pdf");
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

            PdfTextFormField dateField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 824, 150, 15), "date", $"---").SetFont(nunitoBold).SetColor(purpleFontColor);
            dateField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(dateField);

            PdfTextFormField addressField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 787, 260, 15), "address", "---").SetFont(nunitoBold).SetFontSize(9).SetColor(purpleFontColor);
            addressField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(addressField);

            PdfTextFormField accountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 750, 100, 15), "account", "Station").SetFont(nunitoBold).SetColor(purpleFontColor);
            accountField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(accountField);

            PdfTextFormField currencyField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 710, 50, 15), "currency", "NGN").SetFont(nunitoBold).SetColor(purpleFontColor);
            currencyField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(currencyField);

            #endregion

            for (int i = 0; i < 9; i++)
            {
                // prevents the table overlapping the other elements
                document.Add(new Paragraph("\n"));
            }

            Table dataTable = new Table(new float[] { 1, 2, 3, 3, 3, 3 }, true);
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Tank Name", "Sale Volume (ltr)", "Volume Refilled (ltr)", "Opening Volume", "Closing Volume" };

            foreach (var item in itemHeaders)
            {
                dataTable.AddCell(new Cell()
                            .SetBackgroundColor(headerBackground)
                            .SetBorder(Border.NO_BORDER)
                            .Add(new Paragraph(item))
                            .SetFontSize(8)
                            .SetFontColor(purpleFontColor)
                            .SetFont(nunitoBold))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }

            #region //* Insert into Table
            int sn = 1;
            foreach (var item in tankReportList)
            // add items to table.
            {
                if (sn % 2 != 0)
                {
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{sn.ToString()}")).SetFontSize(10));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.tankName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.volumeSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.volumeFilled.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.openingDip.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.closingDip.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                }
                else if (sn % 2 == 0)
                {
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{sn.ToString()}")).SetFontSize(10));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.tankName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.volumeSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.volumeFilled.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.openingDip.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.closingDip.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                }
                sn++;
            }
            #endregion

            dateField.SetValue($"{startDate} - {endDate}");

            foreach (var item in branchDetails)
            {
                if (item.id == branchId)
                {
                    addressField.SetValue($"{item.street}, {item.state}.");
                    nameField.SetValue($"{item.name}");

                    addBranchLogo(item, 740);
                }
            }

            form.FlattenFields();
            document.Add(dataTable);
            addFooter();
            document.Flush();
            document.Close();
            return pdfstream.ToArray();
        }
    }
}