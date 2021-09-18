using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public class CompanyBranchSales : PortraitTemplate
    {
        public Summary getPdf(List<Company> companyList, List<SalesPerBranch> salesPerBranchList)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFS/Test/company-branch-sales-test.pdf");
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

            PdfTextFormField dateField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 824, 100, 15), "date", $"{dt:d}").SetFont(nunitoBold).SetColor(purpleFontColor);
            dateField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(dateField);

            PdfTextFormField addressField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 787, 260, 15), "address", "---").SetFont(nunitoBold).SetFontSize(10f).SetColor(purpleFontColor);
            addressField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(addressField);

            PdfTextFormField accountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 750, 100, 15), "account", "Company").SetFont(nunitoBold).SetColor(purpleFontColor);
            accountField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(accountField);

            PdfTextFormField currencyField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 710, 50, 15), "currency", "NGN").SetFont(nunitoBold).SetColor(purpleFontColor);
            currencyField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(currencyField);

            #endregion

            Table dataTable = new Table(new float[] { 1, 2, 2, 2, 2, 2, 2, 2, 2 }, true);
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Branch", "PMS Vol(ltr)", "PMS Amt(NGN)", "AGO Vol(ltr)", "AGO Amt(NGN)", "DPK Vol(ltr)", "DPK Amt(NGN)", "Total(NGN)" };

            foreach (var item in itemHeaders)
            {
                dataTable.AddCell(new Cell()
                            .SetBackgroundColor(headerBackground)
                            .SetBorder(Border.NO_BORDER)
                            .Add(new Paragraph(item))
                            .SetFontSize(9)
                            .SetFontColor(purpleFontColor)
                            .SetFont(nunitoBold))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }

            for (int i = 0; i < 10; i++)
            {
                // prevents the table overlapping the other elements
                document.Add(new Paragraph("\n"));
            }

            List<string> branchList = new List<string>();
            branchList = salesPerBranchList.Select(x => x.branchName).Distinct().ToList();
            // unique list of branch names

            int serialNumber = 1;
            double PMSVol = 0;
            double PMSAmount = 0;
            double AGOVol = 0;
            double AGOAmount = 0;
            double DPKVol = 0;
            double DPKAmount = 0;
            double totalAmount = 0;
            string tempBranchName = "hi";

            /*
                Here the table values are calculated and arranged by branch
            */
            foreach (var branchName in branchList)
            {
                foreach (var saleRecord in salesPerBranchList)
                {
                    if (saleRecord.branchName == branchName)
                    {
                        // this is used to prevent null entries in the table
                        tempBranchName = saleRecord.branchName;

                        if (saleRecord.productName == "PMS")
                        {
                            PMSVol += saleRecord.volumeSold;
                            PMSAmount += saleRecord.amountSold;
                            totalAmount += saleRecord.amountSold;
                        }
                        if (saleRecord.productName == "AGO")
                        {
                            AGOVol += saleRecord.volumeSold;
                            AGOAmount += saleRecord.amountSold;
                            totalAmount += saleRecord.amountSold;
                        }
                        if (saleRecord.productName == "DPK")
                        {
                            DPKVol += saleRecord.volumeSold;
                            DPKAmount += saleRecord.amountSold;
                            totalAmount += saleRecord.amountSold;
                        }
                    }
                }
                if (tempBranchName == null)
                {
                    serialNumber--;
                }
                else
                {
                    addToTable(dataTable, serialNumber, branchName, PMSVol, PMSAmount, AGOVol, AGOAmount, DPKVol, DPKAmount, totalAmount);
                }

                PMSVol = 0;
                PMSAmount = 0;
                AGOVol = 0;
                AGOAmount = 0;
                DPKVol = 0;
                DPKAmount = 0;
                totalAmount = 0;
                serialNumber++;
            }

            foreach (var item in companyList)
            {
                addressField.SetValue($"{item.street}, {item.state}.");
                nameField.SetValue($"{item.name}");
                addLogo(item, 750);
            }

            form.FlattenFields();
            document.Add(dataTable);
            addFooter();
            document.Flush();
            document.Close();

            Summary Summary = new();
            Summary.PdfReport = pdfstream.ToArray();
            Summary.PmsAmount = $"{PMSAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            Summary.AgoAmount = $"{AGOAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            Summary.DpkAmount = $"{DPKAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            Summary.TotalAmount = $"{totalAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";

            return Summary;
        }

        // This function exists solely to reduce clutter
        public void addToTable(Table dataTable, int serialNumber, string branchName, double PMSVol, double PMSAmount, double AGOVol, double AGOAmount, double DPKVol, double DPKAmount, double totalAmount)
        {
            if (serialNumber % 2 != 0)
            {
                dataTable.AddCell(
            new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{serialNumber.ToString()}")).SetFontSize(9));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{branchName}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{PMSVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{PMSAmount.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{AGOVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{AGOAmount.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{DPKVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{DPKAmount.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{totalAmount.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
            }
            else if (serialNumber % 2 == 0)
            {
                dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{serialNumber.ToString()}")).SetFontSize(9));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{branchName}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{PMSVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{PMSAmount.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{AGOVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{AGOAmount.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{DPKVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{DPKAmount.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{totalAmount.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
            }
        }
    }
}
