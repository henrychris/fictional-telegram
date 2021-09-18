using System.Collections.Generic;
using api.EventHandlers;
using api.Objects.AnyLevel;
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
    public class BranchCashFlow : LandscapeTemplate
    {
        public byte[] GetPdf(string branchId, List<BranchDetails> branchDetails, List<Cashflow> cashflowList)
        {
            writer = new PdfWriter(pdfstream);
            reader = new PdfReader("./Files/PDFs/landscape-A4.pdf");
            pdfdoc = new PdfDocument(reader, writer);

            document = new Document(pdfdoc, PageSize.A4.Rotate(), false);
            document.SetMargins(60, 50, 49, 50);

            Image backgroundImage = new (ImageDataFactory.Create("./Files/Images/landscape-background.png"));
            BackgroundImageHandler handler = new(backgroundImage, writer);

            // This event handler adds a background image every time the end of a page is reached
            pdfdoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

            form = PdfAcroForm.GetAcroForm(pdfdoc, true);

            #region   
            // These fields hold company/branch details or other important info

            PdfTextFormField nameField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(480, 573, 175, 15), "name", "jfbwjrfiw").SetColor(ColorConstants.WHITE).SetFontAndSize(nunitoLight, (float)14);
            nameField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(nameField);

            PdfTextFormField addressField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(480, 549, 195, 15), "address", "---").SetFontAndSize(nunitoBold, 8f).SetColor(purpleFontColor);
            addressField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(addressField);


            PdfTextFormField accountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(480, 527, 175, 15), "account", "Station").SetColor(purpleFontColor).SetFont(nunitoBold);
            accountField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(accountField);

            PdfTextFormField currencyField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(480, 500, 100, 15), "currency", $"NGN").SetFont(nunitoBold).SetColor(purpleFontColor);
            currencyField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(currencyField);

            // set values

            #endregion

            Table dataTable = new Table(new float[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, true);
            dataTable.SetMarginLeft(20).SetMarginRight(50).SetMarginBottom(45);
            dataTable.SetKeepTogether(true);

            string[] itemHeaders = { "S/N", "Date", "Branch", "Total Sales", "POS Sales", "Retainer Sales", "Epump Sales", "Banked Cash", "Expense", "Previous Balance", "Outstanding Cash", "Cash Insurance Ratio", "Compliance (%)" };

            foreach (var item in itemHeaders)
            {
                // adds headers to table
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

            for (int i = 0; i < 2; i++)
            {
                // prevents the table overlapping the other elements
                document.Add(new Paragraph("\n"));
            }

            #region //* Insert into Table
            int sn = 1;
            foreach (var item in cashflowList)
            // add items to table.
            // the condition sn % 2 is what causes the table colours to alternate
            {
                if (sn % 2 != 0)
                {
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.branchName}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.totalSales.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.posSales.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.retainerSales.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.epumpSales.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.bankedCash.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.expense.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.previousBalance.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.outstandingCash.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.cashInsuranceRatio.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.compliance.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                }
                else if (sn % 2 == 0)
                {
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.branchName}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.totalSales.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.posSales.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.retainerSales.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.epumpSales.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.bankedCash.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.expense.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.previousBalance.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.outstandingCash.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.cashInsuranceRatio.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.compliance.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                }
                sn++;
            }

            #endregion

            foreach (var item in branchDetails)
            {
                // Here, branch details and the company logo are added
                if (item.id == branchId)
                {
                    addressField.SetValue($"{item.street}, {item.state}.");
                    nameField.SetValue($"{item.name}");

                    addLandscapeBranchLogo(item);
                }
            }

            document.Add(dataTable);
            addFooter();
            form.FlattenFields();
            document.Flush();
            document.Close();
            
            // return an array of bytes
            return pdfstream.ToArray();
        }

    }
}