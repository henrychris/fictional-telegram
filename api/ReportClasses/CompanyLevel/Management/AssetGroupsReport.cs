using System;
using System.Collections.Generic;
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

namespace api.ReportClasses.CompanyLevel.Management
{
    public class AssetGroupsReport : PortraitTemplate
    {
        public Summary getPdf(List<Company> companyList, List<AssetGroups> assetGroupList)
        {
            // TODO Requires some changes before it can be used
            /*
            by that I mean the asset groups. They are received as numbers.
            They need to be switched out for text.
            */
            writer = new PdfWriter(pdfstream);
            // writer = new PdfWriter("./Files/PDFS/Test/assetGroupReport-test.pdf");
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

            Table dataTable = new Table(5, true);
            dataTable.SetMaxWidth(717);

            string[] itemHeaders = { "S/N", "Group Name", "Description", "Number of Assets", "Assets Requiring Attention" };

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

            double numberOfAssets = 0;
            double numberOfAssetsRequiringAttention = 0;

            foreach (var item in assetGroupList)
            {
                numberOfAssets += Convert.ToDouble(item.assetsCount);
                numberOfAssetsRequiringAttention += Convert.ToDouble(item.assetsRequiringAttentionCount);
            }

            #region //* Insert into Table
            int sn = 1;

            foreach (var item in assetGroupList)
            // add items to table.
            /*
                group type is an integer. this report needs to be changed to fit the possible tex values.
                Maybe a dictionary.
            */
            {
                if (sn % 2 != 0)
                {
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.name}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.description}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    // dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.groupType}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.assetsCount}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph($"{item.assetsRequiringAttentionCount}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                }
                else if (sn % 2 == 0)
                {
                    dataTable.AddCell(
                new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{sn.ToString()}")));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.name}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.description}"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);
                    // dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.groupType}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.assetsCount}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                    dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).SetBackgroundColor(tableBackgroundColor).Add(new Paragraph($"{item.assetsRequiringAttentionCount}")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
                }
                sn++;
            }

            foreach (var item in companyList)
            {
                addressField.SetValue($"{item.street}, {item.state}.");
                nameField.SetValue($"{item.name}");

                addLogo(item, 900);
            }

            #endregion

            form.FlattenFields();
            document.Add(dataTable);
            addFooter();
            document.Flush();
            document.Close();

            Summary summary = new();
            summary.numberOfAssets = $"{numberOfAssets}";
            summary.numberOfAssetsRequiringAttention = $"{numberOfAssetsRequiringAttention}";
            summary.PdfReport = pdfstream.ToArray();

            return summary;
        }
    }
}