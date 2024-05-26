namespace RestProject.pdfGenerator;

using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

public class HeaderFooterEventHandler : IEventHandler
{
    private readonly string headerText;
    private readonly string footerText;

    public HeaderFooterEventHandler(string headerText, string footerText)
    {
        this.headerText = headerText;
        this.footerText = footerText;
    }

    public void HandleEvent(Event @event)
    {
        PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
        PdfDocument pdfDocument = docEvent.GetDocument();
        PdfPage page = docEvent.GetPage();

        PageSize pageSize = pdfDocument.GetDefaultPageSize();
        float pageWidth = pageSize.GetWidth();
        float pageHeight = pageSize.GetHeight();

        PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDocument);

        pdfCanvas.BeginText()
            .SetFontAndSize(PdfFontFactory.CreateFont(), 10)
            .MoveText(pageWidth / 2, pageHeight - 20)
            .ShowText(headerText)
            .EndText()
            .BeginText()
            .SetFontAndSize(PdfFontFactory.CreateFont(), 10)
            .MoveText(pageWidth / 2, 20)
            .ShowText(footerText)
            .EndText();

        pdfCanvas.Release();
    }
}
