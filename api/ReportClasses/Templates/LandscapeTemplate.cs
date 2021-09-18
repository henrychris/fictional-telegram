using System;
using System.IO;
using api.Objects.CompanyLevel;
using api.Objects.RetailLevel;
using iText.Forms;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace api.ReportClasses.Templates
{
    ///<summary>
    /// A template class for creating landscape oriented reports.
    /// <remarks> Inherit at the start of every report.</remarks>
    ///</summary>
    public class LandscapeTemplate
    {
        public const string BOLD = "./Files/Font/Nunito/Nunito-Bold.ttf";
        public const string LIGHT = "./Files/Font/Nunito/Nunito-Light.ttf";
        public const string REGULAR = "./Files/Font/Nunito/Nunito-Regular.ttf";

        #region // Declarations
        protected Document document;
        protected DateTime dt;
        protected PdfAcroForm form;
        protected Color headerBackground;
        protected string nairaSign;
        protected PdfFont nunito;
        protected PdfFont nunitoBold;
        protected PdfFont nunitoLight;
        protected PdfDocument pdfdoc;
        protected MemoryStream pdfstream = new MemoryStream();
        protected Color purpleFontColor;
        protected PdfReader reader;
        protected Color tableBackgroundColor;
        protected PdfWriter writer;
        #endregion

        /*  Important info
            Table max width must be set in report class
            page width 1711
        */

        /// <summary>
        /// This constructor initializes important variables.
        /// </summary>
        public LandscapeTemplate()
        {
            MemoryStream pdfstream = new MemoryStream();

            dt = new DateTime();
            dt = DateTime.Today;
            dt.ToShortDateString();

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

            //add naira sign
            var formatter = new System.Globalization.CultureInfo("HA-LATN-NG");
            formatter.NumberFormat.CurrencySymbol = "₦";
            nairaSign = formatter.NumberFormat.CurrencySymbol;
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
                .SetFontSize(10), 85, 40, i, TextAlignment.LEFT, VerticalAlignment.TOP, 0);
                document.ShowTextAligned(new Paragraph($"Page {i} of {numberOfPages}")
                .SetFont(nunito)
                .SetFontSize(10), 430, 40, i, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
                document.ShowTextAligned(new Paragraph($"www.epump.com.ng")
                .SetFont(nunito)
                .SetFontSize(10), 755, 40, i, TextAlignment.RIGHT, VerticalAlignment.TOP, 0);
            }
        }

        ///<summary>
        /// adds a Logo to company level reports
        ///</summary>
        /// <param name="item">
        /// an instance of the <c>Company</c> class
        /// </param>
        public void addLandscapeLogo(Company item)
        {
            try
            {
                Image companyLogo = new Image(ImageDataFactory.Create($"{item.url}"));
                companyLogo.SetMaxHeight(150).SetMaxWidth(150);
                companyLogo.SetFixedPosition(1, 220, 507);
                document.Add(companyLogo);
            }
            catch (Exception e)
            {
                Image baseLogo = new Image(ImageDataFactory.Create($"./Files/Images/emptylandscapelogo.png"));
                baseLogo.SetMaxHeight(200).SetMaxWidth(200);
                baseLogo.SetFixedPosition(1, 50, 750);
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
        public void addLandscapeBranchLogo(BranchDetails item)
        {
            try
            {
                Image companyLogo = new Image(ImageDataFactory.Create($"{item.companyLogoUrl}"));
                companyLogo.SetMaxHeight(150).SetMaxWidth(150);
                companyLogo.SetFixedPosition(1, 220, 507);
                document.Add(companyLogo);
            }
            catch (Exception e)
            {
                Image baseLogo = new Image(ImageDataFactory.Create($"./Files/Images/emptylandscapelogo.png"));
                baseLogo.SetMaxHeight(200).SetMaxWidth(200);
                baseLogo.SetFixedPosition(1, 50, 750);
                document.Add(baseLogo);
                e.ToString();
            }
        }
    }
}