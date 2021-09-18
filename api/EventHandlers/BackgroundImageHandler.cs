using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;

namespace api.EventHandlers
{
    ///<summary>
    /// This doesn't need to be touched. It's perfect
    ///</summary>
    public class BackgroundImageHandler : IEventHandler
    {
        private readonly Image _img;

        public BackgroundImageHandler(Image img, PdfWriter writer)
        {
            _img = img;
        }
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent documentEvent = (PdfDocumentEvent)@event;
            PdfDocument doc = documentEvent.GetDocument();
            PdfPage page = documentEvent.GetPage();

            PdfCanvas canvas = new(page.NewContentStreamAfter(), page.GetResources(), doc);
            Rectangle area = page.GetPageSize();

            // .setFillOpacity determines how opaque or transparent the background image is
            PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(1);
            canvas.SetExtGState(gs1);

            // adds image to canvas, then to document
            new Canvas(canvas, area).Add(_img);
            // canvas.RestoreState();
        }
    }
}