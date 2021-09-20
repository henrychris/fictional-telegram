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
    public class CompanyTanksFilled : PortraitTemplate
    {
        public Summary getPdf(List<TanksFilled> tanksFilledList, List<Company> companyList, string date)
        {
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFs/Test/tanks-filled-test.pdf");
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

            PdfTextFormField addressField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 787, 260, 15), "address", "---").SetFont(nunitoBold).SetFontSize(9).SetColor(purpleFontColor);
            addressField.SetJustification(PdfFormField.ALIGN_LEFT).SetVisibility(0).SetReadOnly(true);
            form.AddField(addressField);

            PdfTextFormField accountField = (PdfTextFormField)PdfTextFormField.CreateText(pdfdoc, new Rectangle(425, 750, 100, 15), "account", "Company").SetFont(nunitoBold).SetColor(purpleFontColor);
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

            double epumpDischarge = 0;
            double manualDischarge = 0;

            foreach (var item in tanksFilledList)
            {
                epumpDischarge += item.endVolume - item.startVolume;
                double manualVariance = 0;
                if (item.manualStartVolume == null && item.manualEndVolume == null)
                {
                    manualVariance = 0;
                }
                else
                {
                    manualVariance = int.Parse(item.manualEndVolume.ToString()) - int.Parse(item.manualStartVolume.ToString());
                }
                manualDischarge += manualVariance;
            }

            Table dataTable = new Table(new float[] { 1, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 1, 1 }, true);
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Branch Name", "Tank Name", "Epump Volume Discharged(ltr)", "Manual Volume Discharged(ltr)", "Variance", "Epump Start Volume", "Epump End Volume", "Manual Start Volume", "Manual End Volume", "Plate Number", "Start Time", "End Time" };

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

            #region add table
            int sn = 1;
            foreach (var item in tanksFilledList)
            {
                if (sn % 2 != 0)
                {
                    var epumpVariance = item.endVolume - item.startVolume;
                    double manualVariance = 0;
                    if (item.manualStartVolume == null && item.manualEndVolume == null)
                    {
                        manualVariance = 0;
                    }
                    else
                    {
                        manualVariance = int.Parse(item.manualEndVolume.ToString()) - int.Parse(item.manualStartVolume.ToString());
                    }
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{sn.ToString()}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.branchName}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.tankName}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER)
                                                .Add(new Paragraph($"{epumpVariance}")))
                                                .SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{manualVariance}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{epumpVariance - manualVariance}")))
                                                                            .SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.startVolume}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.endVolume}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.manualStartVolume}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.manualEndVolume}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.plateNumber}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.startTime.ToShortTimeString()}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.endTime.ToShortTimeString()}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    //
                }
                else if (sn % 2 == 0)
                {
                    var epumpVariance = item.endVolume - item.startVolume;
                    double manualVariance = 0;
                    if (item.manualStartVolume == null && item.manualEndVolume == null)
                    {
                        manualVariance = 0;
                    }
                    else
                    {
                        manualVariance = int.Parse(item.manualEndVolume.ToString()) - int.Parse(item.manualStartVolume.ToString());
                    }
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{sn.ToString()}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.branchName}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.tankName}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor)
                                                .Add(new Paragraph($"{epumpVariance}")))
                                                .SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{manualVariance}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{epumpVariance - manualVariance}")))
                                                                            .SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.startVolume}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.endVolume}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.manualStartVolume}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.manualEndVolume}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.plateNumber}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.startTime.ToShortTimeString()}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.endTime.ToShortTimeString()}"))).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER);
                }
                sn++;
            }
            #endregion

            foreach (var item in companyList)
            {
                addressField.SetValue($"{item.street}, {item.state}.");
                nameField.SetValue($"{item.name}");

                addLogo(item, 750);
            }

            dateField.SetValue($"{date}");

            form.FlattenFields();
            document.Add(dataTable);
            addFooter();
            document.Flush();
            document.Close();

            Summary summary = new();
            summary.EpumpDischarge = $"{epumpDischarge:0.00}L";
            summary.ManualDischarge = $"{manualDischarge:0.00}L";
            summary.PdfReport = pdfstream.ToArray();

            return summary;
        }
    }
}