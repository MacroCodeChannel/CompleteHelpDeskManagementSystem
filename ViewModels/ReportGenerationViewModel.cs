using DinkToPdf;

namespace HelpDeskSystem.ViewModels
{
    public class ReportGenerationViewModel<T>
    {
        public string Title { get; set; }
        public string FileName { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }
        public string User { get; set; }
        public string DateGenerated { get; set; }
        public string ExportType { get; set; }

        public T Model { get; set; }
        public Orientation PageOrientation { get; set; } = Orientation.Portrait;

        public PaperKind PaperKind { get; set; } = PaperKind.A3;

    }
}
