using System.Collections.Generic;
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
    public class CompanyTankStock : PortraitTemplate
    {
        // Dashboard/TankStock/{companyId}
        public Summary getPdf(List<Company> companyList, List<TankStock> tankStockList)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFS/Test/company-tank-stock-test.pdf");
            reader = new PdfReader("./Files/PDFs/Templates/dateTemplate.pdf");
            pdfdoc = new PdfDocument(reader, writer);

            document = new Document(pdfdoc, pageSize, false);
            document.SetMargins(80, 50, 50, 50);

            Image backgroundImage = new Image(ImageDataFactory.Create("./Files/Images/portrait-background.png"));
            backgroundImage.SetMaxWidth(792).SetMaxHeight(1150);

            BackgroundImageHandler handler = new BackgroundImageHandler(backgroundImage, writer);
            pdfdoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

            #region add AcroForm Fields
            form = PdfAcroForm.GetAcroForm(pdfdoc, true);

            PdfTextFormField nameField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 1003, 200, 15), "name", "---").SetColor(ColorConstants.WHITE).SetFontAndSize(nunito, (float)14);
            nameField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(nameField);

            PdfTextFormField dateField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 966, 100, 15), "date", $"{dt:d}").SetFont(nunitoBold).SetColor(purpleFontColor);
            dateField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(dateField);

            PdfTextFormField addressField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 929, 260, 15), "address", "---").SetFontAndSize(nunitoBold, 10f).SetColor(purpleFontColor);
            addressField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(addressField);

            PdfTextFormField accountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 892, 100, 15), "account", "Company").SetFont(nunitoBold).SetColor(purpleFontColor);
            accountField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(accountField);

            PdfTextFormField currencyField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(455, 852, 50, 15), "currency", "NGN").SetFont(nunitoBold).SetColor(purpleFontColor);
            currencyField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(currencyField);

            #endregion

            Table dataTable = new Table(new float[] { 1, 2, 2, 2, 2, 2 }, true);
            // this keeps the table aligned with the rest of the document
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Branch Name", "PMS Volume", "AGO Volume", "DPK Volume", "LPG Volume" };

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

            for (int i = 0; i < 8; i++)
            {
                // prevents the table overlapping the other elements
                document.Add(new Paragraph("\n"));
            }

            List<string> branchList = new List<string>();
            branchList = tankStockList.Select(x => x.branchName).Distinct().ToList();

            int serialNumber = 1;
            double PMSVol = 0;
            double AGOVol = 0;
            double DPKVol = 0;
            double LPGVol = 0;
            string tempBranchName = "hi";

            foreach (var branchName in branchList)
            {
                foreach (var tank in tankStockList)
                {
                    if (tank.branchName == branchName)
                    {
                        tempBranchName = tank.branchName;

                        if (tank.productName == "PMS")
                        {
                            PMSVol += tank.currentVolume;
                        }
                        if (tank.productName == "AGO")
                        {
                            AGOVol += tank.currentVolume;
                        }
                        if (tank.productName == "DPK")
                        {
                            DPKVol += tank.currentVolume;
                        }
                        if (tank.productName == "LPG")
                        {
                            LPGVol += tank.currentVolume;
                        }
                    }
                }
                if (tempBranchName == null)
                {
                    serialNumber--;
                }
                else
                {
                    addToTable(dataTable, serialNumber, branchName, PMSVol, AGOVol, DPKVol, LPGVol);
                }

                PMSVol = 0;
                AGOVol = 0;
                DPKVol = 0;
                LPGVol = 0;
                serialNumber++;
            }

            foreach (var item in companyList)
            {
                addressField.SetValue($"{item.street}, {item.state}.");
                nameField.SetValue($"{item.name}");

                addLogo(item, 890);
            }

            form.FlattenFields();
            document.Add(dataTable);
            addFooter();
            document.Flush();
            document.Close();

            Summary summary = new();
            summary.pmsVolume = $"{PMSVol:0.00}L";
            summary.agoVolume = $"{AGOVol:0.00}L";
            summary.dpkVolume = $"{DPKVol:0.00}L";
            summary.lpgVolume = $"{LPGVol:0.00}L";
            summary.PdfReport = pdfstream.ToArray();
            
            return summary;
        }

        private void addToTable(Table dataTable, int serialNumber, string branchName, double pMSVol, double aGOVol, double dPKVol, double lPGVol)
        {
            if (serialNumber % 2 != 0)
            {
                dataTable.AddCell(
            new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{serialNumber.ToString()}")).SetFontSize(9));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{branchName}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{pMSVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{aGOVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{dPKVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{lPGVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
            }
            else if (serialNumber % 2 == 0)
            {
                dataTable.AddCell(
            new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{serialNumber.ToString()}")).SetFontSize(9));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{branchName}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{pMSVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{aGOVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{dPKVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
                dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{lPGVol.ToString("0.00")}")).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
            }
        }
    }
}