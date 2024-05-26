namespace RestProject.pdfGenerator;

using System;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;

public class PdfGenerator
{
    private readonly PdfWriter writer;
    private readonly FlightReservationPDFData reservation;
    private bool isHeaderFooter;
    private bool isImage;
    private string headerText = null!;
    private string footerText = null!;
    private string imagePath = null!;

    public PdfGenerator(Stream outputStream, FlightReservationPDFData reservation)
    {
        this.writer = new PdfWriter(outputStream);
        this.reservation = reservation;
        this.isHeaderFooter = false;
        this.isImage = false;
    }

    public void SetHeaderFooter(string headerText, string footerText)
    {
        this.headerText = headerText;
        this.footerText = footerText;
        this.isHeaderFooter = true;
    }

    public void SetImage(string imgPath)
    {
        this.imagePath = imgPath;
        this.isImage = true;
    }

    public void Generate()
    {
        PdfDocument pdfDocument = new PdfDocument(writer);

        if (isHeaderFooter)
        {
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new HeaderFooterEventHandler(headerText, footerText));
        }

        pdfDocument.SetDefaultPageSize(PageSize.A4);
        Document document = new Document(pdfDocument);

        float col = 280f;
        float[] columnWidth = { col, col };

        Table table = new Table(columnWidth)
            .SetBackgroundColor(new DeviceRgb(63, 169, 219))
            .SetFontColor(ColorConstants.WHITE);

        table.AddCell(new Cell()
            .Add(new Paragraph("Reservation"))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
            .SetMarginTop(30f)
            .SetMarginBottom(30f)
            .SetFontSize(30f)
            .SetBorder(Border.NO_BORDER));

        table.AddCell(new Cell()
            .Add(new Paragraph("Reservation number: " + reservation.ReservationId + "\n" +
                               "Current reservations count: " + reservation.NumberOfReservedSeats))
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginTop(30f)
            .SetMarginBottom(30f)
            .SetMarginRight(10f)
            .SetBorder(Border.NO_BORDER));

        Text text = new Text("Your data\n").SetFontSize(17).SetBold();
        Paragraph userDataHeader = new Paragraph(text).SetMarginTop(25f);
        Paragraph userData = new Paragraph("Login: " + reservation.Login + "\n" +
                                           "E-mail: " + reservation.Email + "\n");

        Text text2 = new Text("Flight " + reservation.FlightCode).SetFontSize(17).SetBold();
        Paragraph flightDataHeader = new Paragraph(text2);
        Paragraph flightData = new Paragraph("Departure: " + reservation.DepartureAirport + " at " + reservation.DepartureTime + "  ----->  " +
                                             "Destination: " + reservation.DestinationAirport + " at " + reservation.ArrivalTime);

        document.Add(table);
        document.Add(userDataHeader);
        document.Add(userData);
        document.Add(flightDataHeader);
        document.Add(flightData);

        if (isImage)
        {
            ImageData imageData = ImageDataFactory.Create(imagePath);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
            image.SetFixedPosition(pdfDocument.GetDefaultPageSize().GetWidth() / 2 - 320, pdfDocument.GetDefaultPageSize().GetHeight() / 2 - 160);
            image.SetOpacity(0.3f);
            document.Add(image);
        }

        document.Close();
    }
}
