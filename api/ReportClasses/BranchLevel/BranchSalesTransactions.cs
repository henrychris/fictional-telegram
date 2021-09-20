using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class BranchSalesTransactions : PortraitTemplate
    {
        // /Branch/Sales/branchId?startDate&endDate
        public Summary getPdf(string endDate, string branchId, List<BranchDetails> branchDetails, List<Sales> SalesJsonList)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFs/Test/branch-sales-transaction-report.pdf");
            reader = new PdfReader("./Files/PDFs/Templates/empty-sales-transaction-report.pdf");
            pdfdoc = new PdfDocument(reader, writer);

            document = new Document(pdfdoc, pageSize, false);
            document.SetMargins(80, 50, 50, 50);

            Image backgroundImage = new Image(ImageDataFactory.Create("./Files/Images/portrait-background.png"));
            backgroundImage.SetMaxHeight(1150).SetMaxWidth(792);

            // adds bacground image to every page
            BackgroundImageHandler handler = new BackgroundImageHandler(backgroundImage, writer);
            pdfdoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

            // calculate these values in table
            double pmsTotalVolume = 0.0;
            double pmsTotalCost = 0.0;
            double agoTotalCost = 0.0;
            double agoTotalVolume = 0.0;
            double dpkTotalCost = 0.0;
            double dpkTotalVolume = 0.0;
            double totalCost = 0.0;
            double totalVolume = 0.0;

            double pmsPrice = 0.0;
            double agoPrice = 0.0;
            double dpkPrice = 0.0;

            foreach (var item in SalesJsonList)
            {
                if (item.productName == "PMS")
                {
                    pmsPrice = item.price;
                    pmsTotalVolume += item.volumeSold;
                    totalVolume += item.volumeSold;
                }
                if (item.productName == "AGO")
                {
                    agoPrice = item.price;
                    agoTotalVolume += item.volumeSold;
                    totalVolume += item.volumeSold;
                }
                if (item.productName == "DPK")
                {
                    dpkPrice = item.price;
                    dpkTotalVolume += item.volumeSold;
                    totalVolume += item.volumeSold;
                }
            }

            #region
            form = PdfAcroForm.GetAcroForm(pdfdoc, true);

            PdfTextFormField nameField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 1005, 200, 15), "name", "---").SetColor(ColorConstants.WHITE).SetFontAndSize(nunito, (float)14);
            nameField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(nameField);

            PdfTextFormField dateField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 968, 100, 15), "date", $"{dt:d}").SetFont(nunitoBold).SetColor(purpleFontColor);
            dateField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(dateField);

            PdfTextFormField addressField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 931, 260, 15), "address", "---").SetFont(nunitoBold).SetFontSize(10f).SetColor(purpleFontColor);
            addressField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(addressField);

            PdfTextFormField accountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 894, 100, 15), "account", "Station").SetFont(nunitoBold).SetColor(purpleFontColor);
            accountField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(accountField);

            PdfTextFormField currencyField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 854, 50, 15), "currency", "NGN").SetFont(nunitoBold).SetColor(purpleFontColor);
            currencyField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(currencyField);

            PdfTextFormField volumeField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 816, 200, 15), "volume", "---").SetFont(nunitoBold).SetColor(purpleFontColor);
            volumeField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(volumeField);

            PdfTextFormField growthField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(83, 753, 170, 15), "volume", "---").SetFontAndSize(nunitoBold, 14f).SetColor(ColorConstants.WHITE);
            growthField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(growthField);

            PdfTextFormField transactionField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(321, 753, 170, 15), "volume", $"---").SetFontAndSize(nunitoBold, 14f).SetColor(ColorConstants.WHITE);
            transactionField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(transactionField);

            PdfTextFormField litresPerTransactionField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(573, 753, 170, 15), "volume", "---").SetFontAndSize(nunitoBold, 14f).SetColor(ColorConstants.WHITE);
            litresPerTransactionField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(litresPerTransactionField);

            PdfTextFormField totalPmsCashField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(38, 645, 157, 15), "volume", "---").SetFontAndSize(nunitoBold, 15f).SetColor(ColorConstants.BLACK);
            totalPmsCashField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalPmsCashField);

            PdfTextFormField totalPmsVolField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(38, 594, 157, 15), "volume", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunito, (float)8);
            totalPmsVolField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalPmsVolField);

            PdfTextFormField totalAgoCashField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(195, 645, 166, 15), "volume", "---").SetFontAndSize(nunitoBold, 15f).SetColor(ColorConstants.BLACK);
            totalAgoCashField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalAgoCashField);

            PdfTextFormField totalAgoVolField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(195, 594, 166, 15), "volume", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunito, (float)8);
            totalAgoVolField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalAgoVolField);

            PdfTextFormField totalDpkCashField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(362, 645, 174, 15), "volume", "---").SetFontAndSize(nunitoBold, 15f).SetColor(ColorConstants.BLACK);
            totalDpkCashField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalDpkCashField);

            PdfTextFormField totalDpkVolField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(362, 594, 174, 15), "volume", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunito, (float)8);
            totalDpkVolField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalDpkVolField);

            PdfTextFormField totalVolCashField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(536, 645, 208, 15), "volume", "---").SetFontAndSize(nunitoBold, 15f).SetColor(ColorConstants.BLACK);
            totalVolCashField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalVolCashField);

            PdfTextFormField totalVolField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(536, 594, 208, 15), "volume", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunito, (float)8);
            totalVolField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalVolField);

            #endregion

            // set values for total "product" sold
            pmsTotalCost = pmsTotalVolume * pmsPrice;
            agoTotalCost = agoTotalVolume * agoPrice;
            dpkTotalCost = dpkTotalVolume * dpkPrice;
            totalCost = pmsTotalCost + agoTotalCost + dpkTotalCost;

            totalPmsCashField.SetValue($"{nairaSign}" + $"{pmsTotalCost.ToString("0.00")}");
            totalPmsVolField.SetValue($"{pmsTotalVolume.ToString("0.00")}" + " litres");
            totalAgoCashField.SetValue($"{nairaSign}" + $"{agoTotalCost.ToString("0.00")}");
            totalAgoVolField.SetValue($"{agoTotalVolume.ToString("0.00")}" + " litres");
            totalDpkCashField.SetValue($"{nairaSign}" + $"{dpkTotalCost.ToString("0.00")}");
            totalDpkVolField.SetValue($"{dpkTotalVolume.ToString("0.00")}" + " litres");
            totalVolCashField.SetValue($"{nairaSign}" + $"{totalCost.ToString("0.00")}");
            totalVolField.SetValue($"{totalVolume.ToString("0.00")}" + " litres");

            Table dataTable = new Table(new float[] { 1, 2, 2, 2, 3, 3, 3, 2 }, true);
            // this keeps the table aligned with the rest of the document
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Date", "Pump Name", "Product", "Opening Reading", "Closing Reading", "Volume Dispensed", "Amount" };

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

            for (int i = 0; i < 18; i++)
            {
                // prevents the table overlapping the other elements
                document.Add(new Paragraph("\n"));
            }

            #region //* Insert into Table
            int sn = 1;
            foreach (var item in SalesJsonList)
            // add items to table.
            {
                if (sn % 2 != 0)
                {
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.pumpName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.productName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.openingReading.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.lastReading.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.volumeSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.amountSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                }
                else if (sn % 2 == 0)
                {
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.pumpName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.productName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.openingReading.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.lastReading.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.volumeSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    // dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"0")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.amountSold.ToString("0.00")}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                }
                sn++;
            }
            #endregion

            transactionField.SetValue((sn - 1).ToString());
            litresPerTransactionField.SetValue((totalVolume / (sn - 1)).ToString("0.00"));
            volumeField.SetValue(totalVolume.ToString() + " litres");

            // set BRANCH details
            foreach (var item in branchDetails)
            {
                if (branchId == item.id)
                {
                    addressField.SetValue($"{item.street}, {item.state}.");
                    nameField.SetValue($"{item.name}");

                    addBranchLogo(item, 870);
                }
            }

            document.Add(dataTable);
            calculateDailyGrowth(endDate, growthField, SalesJsonList);
            addFooter();
            form.FlattenFields();
            document.Flush();
            document.Close();

            Summary summary = new();
            summary.PdfReport = pdfstream.ToArray();
            summary.PmsAmountSold = $"{pmsTotalCost.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.PmsVolumeSold = $"{pmsTotalVolume}L";
            summary.AgoAmountSold = $"{agoTotalCost.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.AgoVolumeSold = $"{agoTotalVolume}L";
            summary.DpkAmountSold = $"{dpkTotalCost.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.DpkVolumeSold = $"{dpkTotalVolume}L";
            summary.TotalAmountSold = $"{totalCost.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.TotalVolumeSold = $"{totalVolume}L";

            return summary;
        }

        ///<summary>
        /// calculates daily growth relative to the previous date's transactions
        ///</summary>
        public void calculateDailyGrowth(string endDate, PdfTextFormField growthField, List<Sales> transactionsList)
        {
            double endDateTransactions = 0.0;
            double previousDateTransactions = 0.0;
            double percentageChange = 0.0;

            foreach (var item in transactionsList)
            {
                // date of specific transaction
                var x = item.date.ToShortDateString();
                if (x == (endDate))
                {
                    endDateTransactions += item.amountSold;
                }
            }

            foreach (var item in transactionsList)
            {
                var previousDate = DateTime.Parse(endDate);
                previousDate = previousDate.AddDays(-1);
                previousDate.ToShortDateString();

                if (item.date.ToShortDateString() == previousDate.ToShortDateString())
                {
                    previousDateTransactions += item.amountSold;
                }
            }

            percentageChange = ((endDateTransactions - previousDateTransactions) / endDateTransactions) * 100;

            if (Double.IsNaN(percentageChange))
            {
                growthField.SetValue($"0%");
            }
            else
            {
                percentageChange = Math.Round(percentageChange, 2);
                growthField.SetValue($"{percentageChange}%");
            }
        }
    }
}