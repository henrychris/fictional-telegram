using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class CompanyPosTransactions : PortraitTemplate
    {
        public Summary getPdf(List<PosTransactions> posTransactionsList, List<Company> companyList)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFs/Test/pos-transactions-test.pdf");
            reader = new PdfReader("./Files/PDFs/Templates/pos-transactions-templates.pdf");
            pdfdoc = new PdfDocument(reader, writer);

            document = new Document(pdfdoc, pageSize, false);
            document.SetMargins(80, 50, 50, 50);

            Image backgroundImage = new Image(ImageDataFactory.Create("./Files/Images/portrait-background.png"));
            backgroundImage.SetMaxWidth(792).SetMaxHeight(1150);

            BackgroundImageHandler handler = new BackgroundImageHandler(backgroundImage, writer);
            pdfdoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

            int succesfulTransactions = 0;
            int failedTransactions = 0;
            int noOfCashbackCharges = 0;
            double cashbackAmount = 0;
            double succesfulAmount = 0;
            double failedAmount = 0;

            form = PdfAcroForm.GetAcroForm(pdfdoc, true);

            #region

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

            PdfTextFormField totalTransactionsAmountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(38, 780, 157, 15), "totalTransactionsAmt", "---").SetFontAndSize(nunitoBold, 15f).SetColor(ColorConstants.BLACK);
            totalTransactionsAmountField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(totalTransactionsAmountField);

            PdfTextFormField numOfTotalTransactions = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(38, 729, 157, 15), "numOfTotalTransactions", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunito, 10f);
            numOfTotalTransactions.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(numOfTotalTransactions);

            PdfTextFormField succesfulTransactionsAmountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(195, 780, 166, 15), "successfulTransactionsAmt", "---").SetFontAndSize(nunitoBold, 15f).SetColor(ColorConstants.BLACK);
            succesfulTransactionsAmountField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(succesfulTransactionsAmountField);

            PdfTextFormField noOfSuccesfulTransactions = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(195, 729, 166, 15), "noOfSuccessfulTransactions", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunito, 10f);
            noOfSuccesfulTransactions.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(noOfSuccesfulTransactions);

            PdfTextFormField failedTransactionsAmountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(362, 780, 174, 15), "failedTransactionsAmount", "---").SetFontAndSize(nunitoBold, 15f).SetColor(ColorConstants.BLACK);
            failedTransactionsAmountField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(failedTransactionsAmountField);

            PdfTextFormField noOfFailedTransactions = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(362, 729, 174, 15), "noOfFailedTransactions", "---").SetColor(ColorConstants.BLACK).SetFontAndSize(nunito, 10f);
            noOfFailedTransactions.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(noOfFailedTransactions);

            PdfTextFormField cashbackChargesAmountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(536, 780, 208, 15), "cashbackChargesAmount", "---").SetFontAndSize(nunitoBold, 15f).SetColor(ColorConstants.BLACK);
            cashbackChargesAmountField.SetJustification(PdfFormField.ALIGN_CENTER).SetVisibility(0).SetReadOnly(true);
            form.AddField(cashbackChargesAmountField);

            #endregion

            Table dataTable = new Table(new float[] { 1, 2, 2, 1, 2, 3, 3, 2 }, true);
            // this keeps the table aligned with the rest of the document
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Transaction Date", "Branch", "Pump", "Amount (NGN)", "Transaction Reference", "Masked Pan", "Status" };

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

            for (int i = 0; i < 14; i++)
            {
                // prevents the table overlapping the other elements
                document.Add(new Paragraph("\n"));
            }

            #region //* Insert into Table
            int sn = 1;
            foreach (var item in posTransactionsList)
            // add items to table.
            {
                if (sn % 2 != 0)
                {
                    
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.branchName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.pumpName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));

                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"₦{item.amount}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.transactionReference}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.maskedPan}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.statusDescription}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    if (item.cashBack == "true")
                    {
                        cashbackAmount += Convert.ToDouble(item.charge);
                        noOfCashbackCharges++;
                    }
                }
                else if (sn % 2 == 0)
                {
                    dataTable.AddCell(
                    new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.date.ToShortDateString()}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.branchName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.pumpName}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"₦{item.amount}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.transactionReference}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.maskedPan}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.statusDescription}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                    if (item.cashBack == "true")
                    {
                        cashbackAmount += Convert.ToDouble(item.charge);
                        noOfCashbackCharges++;
                    }
                }
                sn++;
                if (item.statusCode.Equals("00"))
                {
                    succesfulTransactions++;
                    succesfulAmount += Convert.ToDouble(item.amount);
                }
                else
                {
                    failedTransactions++;
                    failedAmount += Convert.ToDouble(item.amount);
                }
            }
            totalTransactionsAmountField.SetValue($"₦{(succesfulAmount + failedAmount).ToString("0.00")}");
            noOfSuccesfulTransactions.SetValue($"No: {succesfulTransactions}");
            noOfFailedTransactions.SetValue($"No: {failedTransactions}");
            succesfulTransactionsAmountField.SetValue($"₦{succesfulAmount.ToString("0.00")}");
            failedTransactionsAmountField.SetValue($"₦{failedAmount.ToString("0.00")}");
            cashbackChargesAmountField.SetValue($"₦{cashbackAmount.ToString("0.00")}");

            numOfTotalTransactions.SetValue($"Transactions: {sn - 1}");
            foreach (var item in companyList)
            {
                addressField.SetValue($"{item.street}, {item.state}.");
                nameField.SetValue($"{item.name}");

                addLogo(item, 900);
            }

            #endregion

            document.Add(dataTable);
            addFooter();
            form.FlattenFields();
            document.Flush();
            document.Close();

            Summary summary = new();
            summary.cashbackAmount = $"{cashbackAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.failedAmount = $"{failedAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.succesfulAmount = $"{succesfulAmount.ToString("C2", CultureInfo.CreateSpecificCulture(("HA-LATN-NG")))}";
            summary.PdfReport = pdfstream.ToArray();

            return summary;
        }
    }
}