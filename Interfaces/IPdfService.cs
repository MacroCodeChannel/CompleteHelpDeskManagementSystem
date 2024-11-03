using DinkToPdf;

namespace HelpDeskSystem.Interfaces
{
    public interface IPdfService
    {
        byte[] ConvertPdf(string htmlcontent, Orientation? orientation = Orientation.Landscape, PaperKind? pk = PaperKind.A3);
    }
}
