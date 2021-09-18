using System;
using System.IO;
using api.Objects.CompanyLevel;
using api.Objects.RetailLevel;
using iText.Forms;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace api.ReportClasses.Templates
{
    ///<summary>
    /// A template class for creating portrait oriented reports.
    /// <remarks> Inherit at the start of every report.</remarks>
    ///</summary>
    public class PortraitTemplate
    {
        #region
        public const string BOLD = "./Files/Font/Nunito/Nunito-Bold.ttf";
        public const string LIGHT = "./Files/Font/Nunito/Nunito-Light.ttf";
        public const string REGULAR = "./Files/Font/Nunito/Nunito-Regular.ttf";
        protected Document document;
        protected DateTime dt;
        protected PdfAcroForm form;
        protected Color headerBackground;
        protected string nairaSign;
        protected PdfFont nunito;
        protected PdfFont nunitoBold;
        protected PdfFont nunitoLight;
        protected PageSize pageSize;
        protected PdfDocument pdfdoc;
        protected MemoryStream pdfstream = new MemoryStream();
        protected Color purpleFontColor;
        protected PdfReader reader;
        protected Color tableBackgroundColor;
        protected PdfWriter writer;
        #endregion

        /// <summary>
        /// This constructor initializes important variables.
        /// </summary>
        public PortraitTemplate()
        {
            MemoryStream pdfstream = new MemoryStream();
            dt = new DateTime();
            dt = DateTime.Today;
            dt.ToShortDateString();

            pageSize = new PageSize(792, 1080);

            purpleFontColor = new DeviceRgb(75, 45, 115);
            headerBackground = new DeviceRgb(236, 235, 235);
            tableBackgroundColor = new DeviceRgb(218, 218, 218);

            // create new fonts
            FontProgram fontprogram = FontProgramFactory.CreateFont(REGULAR);
            FontProgram Boldfontprogram = FontProgramFactory.CreateFont(BOLD);
            FontProgram LightFontProgram = FontProgramFactory.CreateFont(LIGHT);

            nunito = PdfFontFactory.CreateFont(fontprogram, PdfEncodings.IDENTITY_H);
            nunitoBold = PdfFontFactory.CreateFont(Boldfontprogram, PdfEncodings.IDENTITY_H);
            nunitoLight = PdfFontFactory.CreateFont(LightFontProgram, PdfEncodings.IDENTITY_H);
        }

        ///<summary>
        /// Adds a footer to the bottom of the page. Footer contains: date, page no. and company website.
        ///</summary>
        public void addFooter()
        {
            int numberOfPages = pdfdoc.GetNumberOfPages();
            for (int i = 1; i <= numberOfPages; i++)
            {
                document.ShowTextAligned(new Paragraph($"Invoice Created: {dt:d}")
                .SetFont(nunito)
                .SetFontSize(10), 62, 40, i, TextAlignment.LEFT, VerticalAlignment.TOP, 0);
                document.ShowTextAligned(new Paragraph($"Page {i} of {numberOfPages}")
                .SetFont(nunito)
                .SetFontSize(10), 400, 40, i, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
                document.ShowTextAligned(new Paragraph($"www.epump.com.ng")
                .SetFont(nunito)
                .SetFontSize(10), 710, 40, i, TextAlignment.RIGHT, VerticalAlignment.TOP, 0);

            }
        }

        ///<summary>
        /// adds a Logo to company level reports
        ///</summary>
        /// <param name="item">
        /// an instance of the <c>Company</c> class
        /// </param>
        /// <param name="imageHeightOnPage">Where the image will display on the page</param>
        public void addLogo(Company item, float imageHeightOnPage)
        {
            try
            {
                Image companyLogo = new Image(ImageDataFactory.Create($"{item.url}"));
                companyLogo.SetMaxHeight(200).SetMaxWidth(200);
                companyLogo.SetFixedPosition(1, 50, imageHeightOnPage);
                document.Add(companyLogo);
            }
            catch (Exception e)
            {
                Image baseLogo = new Image(ImageDataFactory.Create($"./Files/Images/emptylogo.png"));
                baseLogo.SetMaxHeight(200).SetMaxWidth(200);
                baseLogo.SetFixedPosition(1, 50, imageHeightOnPage);
                document.Add(baseLogo);

                e.ToString();
            }

        }

        ///<summary>
        /// adds a Logo to company level reports
        ///</summary>
        /// <param name="item">
        /// an instance of the <c>BranchDetails</c> class
        /// </param>
        /// <param name="imageHeightOnPage">Where the image will display on the page</param>
        public void addBranchLogo(BranchDetails item, float imageHeightOnPage)
        {
            try
            {
                Image companyLogo = new Image(ImageDataFactory.Create($"{item.companyLogoUrl}"));
                companyLogo.SetMaxHeight(200).SetMaxWidth(200);
                companyLogo.SetFixedPosition(1, 50, imageHeightOnPage);
                document.Add(companyLogo);
            }
            catch (Exception e)
            {
                Image baseLogo = new Image(ImageDataFactory.Create($"./Files/Images/emptylogo.png"));
                baseLogo.SetMaxHeight(200).SetMaxWidth(200);
                baseLogo.SetFixedPosition(1, 50, imageHeightOnPage);
                document.Add(baseLogo);

                e.ToString();
            }
        }
    }
}