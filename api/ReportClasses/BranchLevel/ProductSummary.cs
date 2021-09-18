using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using api.EventHandlers;
using api.Objects.RetailLevel;
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

namespace api.ReportClasses.BranchLevel
{
    ///<summary>
    /// Returns a byte array representing the named PDF Report
    ///</summary>
    public class ProductSummary : PortraitTemplate
    {
        public Summary getPdf(string branchId, List<BranchDetails> branchDetails, List<Sales> salesList)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFs/Test/product-summary-test.pdf");
            reader = new PdfReader("./Files/PDFs/Templates/base-three-slot-table.pdf");
            pdfdoc = new PdfDocument(reader, writer);

            document = new Document(pdfdoc, pageSize, false);
            document.SetMargins(80, 50, 50, 50);

            Image backgroundImage = new Image(ImageDataFactory.Create("./Files/Images/portrait-background.png"));
            backgroundImage.SetMaxWidth(792).SetMaxHeight(1150);

            BackgroundImageHandler handler = new BackgroundImageHandler(backgroundImage, writer);
            pdfdoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

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

            PdfTextFormField PmsField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(241, 790, 157, 15), "pms", "PMS").SetFontAndSize(nunitoBold, 10f).SetColor(ColorConstants.BLACK);
            PmsField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(PmsField);

            PdfTextFormField PmsAmountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(241, 760, 157, 15), "pmssold", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            PmsAmountField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(PmsAmountField);

            PdfTextFormField PmsSoldField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(241, 730, 157, 15), "pmssold", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            PmsSoldField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(PmsSoldField);

            PdfTextFormField AgoField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(400, 790, 165, 15), "ago", "AGO").SetFontAndSize(nunitoBold, 10f).SetColor(ColorConstants.BLACK);
            AgoField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(AgoField);

            PdfTextFormField AgoAmountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(400, 760, 165, 15), "agoamount", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            AgoAmountField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(AgoAmountField);

            PdfTextFormField AgoSoldField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(400, 730, 165, 15), "agosold", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            AgoSoldField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(AgoSoldField);

            PdfTextFormField DpkField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(565, 790, 170, 15), "dpk", "DPK").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            DpkField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(DpkField);

            PdfTextFormField DpkAmountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(565, 760, 170, 15), "dpkamount", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            DpkAmountField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(DpkAmountField);

            PdfTextFormField DpkSoldField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(565, 730, 170, 15), "dpksold", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunitoBold, 10f);
            DpkSoldField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(DpkSoldField);

            #endregion

            Table dataTable = new Table(new float[] { 1, 1, 2, 2 }, true);
            // this keeps the table aligned with the rest of the document
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Date", "Total Sold", "Total Amount (NGN)" };

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

            List<string> dateList = new List<string>();
            dateList = salesList.Select(x => x.date.ToShortDateString()).Distinct().ToList();

            #region 
            int serialNumber = 1;
            double totalVolume = 0;
            double totalAmount = 0;
            string tempdate = "hi";

            foreach (var date in dateList)
            {
                foreach (var sale in salesList)
                {
                    if (sale.date.ToShortDateString() == date)
                    {
                        tempdate = sale.date.ToShortDateString();
                        totalVolume += sale.volumeSold;
                        totalAmount += sale.amountSold;
                    }
                }
                if (tempdate == null)
                {
                    serialNumber--;
                }
                else
                {
                    addToTable(dataTable, serialNumber, tempdate, totalVolume, totalAmount);
                }
                totalVolume = 0;
                totalAmount = 0;
                serialNumber++;
            }
            #endregion 

            // calculate and set total volume of products sold

            double totalPmsVolume = 0;
            double totalPmsAmount = 0;
            double totalAgoVolume = 0;
            double totalAgoAmount = 0;
            double totalDpkVolume = 0;
            double totalDpkAmount = 0;

            foreach (var sale in salesList)
            {
                if (sale.productName == "PMS")
                {
                    totalPmsVolume += sale.volumeSold;
                    totalPmsAmount += sale.amountSold;
                }
                if (sale.productName == "AGO")
                {
                    totalAgoVolume += sale.volumeSold;
                    totalAgoAmount += sale.amountSold;
                }
                if (sale.productName == "DPK")
                {
                    totalDpkVolume += sale.volumeSold;
                    totalDpkAmount += sale.amountSold;
                }
            }

            PmsSoldField.SetValue($"{totalPmsVolume.ToString("0.00")} litres");
            PmsAmountField.SetValue($"₦{totalPmsAmount.ToString("0.00")}");
            AgoSoldField.SetValue($"{totalAgoVolume.ToString("0.00")} litres");
            AgoAmountField.SetValue($"₦{totalAgoAmount.ToString("0.00")}");
            DpkSoldField.SetValue($"{totalDpkVolume.ToString("0.00")} litres");
            DpkAmountField.SetValue($"₦{totalDpkAmount.ToString("0.00")}");

            foreach (var item in branchDetails)
            {
                if (item.id == branchId)
                {
                    addressField.SetValue($"{item.street}, {item.state}.");
                    nameField.SetValue($"{item.name}");

                    addBranchLogo(item, 890);
                }
            }

            document.Add(dataTable);
            addFooter();
            form.FlattenFields();
            document.Flush();
            document.Close();

            Summary summary = new();
            summary.pmsAmountSold = $"{totalPmsAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.agoAmountSold = $"{totalAgoAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.dpkAmountSold = $"{totalDpkAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.pmsVolumeSold = $"{totalPmsVolume:0.00}L";
            summary.agoVolumeSold = $"{totalAgoVolume:0.00}L";
            summary.dpkVolumeSold = $"{totalDpkVolume:0.00}L";
            
            var tempAmtSold = totalPmsAmount + totalAgoAmount + totalDpkAmount;
            var tempVolSold = totalPmsVolume + totalAgoVolume + totalDpkVolume;
            summary.totalAmountSold = $"{tempAmtSold.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.totalVolumeSold = $"{tempVolSold:0.00}L";

            summary.PdfReport = pdfstream.ToArray();
            return summary;
        }

        private void addToTable(Table dataTable, int serialNumber, string tempdate, double totalVolume, double totalAmount)
        {

            if (serialNumber % 2 != 0)
            {
                dataTable.AddCell(
            new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{serialNumber.ToString()}")).SetFontSize(9));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{tempdate}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{totalVolume.ToString("0.00")} litres")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{totalAmount.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
            }
            else if (serialNumber % 2 == 0)
            {
                dataTable.AddCell(
            new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{serialNumber.ToString()}")).SetFontSize(9));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{tempdate}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{totalVolume.ToString("0.00")} litres")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{totalAmount.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
            }
        }
    }
}