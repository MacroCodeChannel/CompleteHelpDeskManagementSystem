using DinkToPdf;
using HelpDeskSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace HelpDeskSystem.Interfaces
{
    public interface IExportService
    {
        Task<byte[]> ExportToPdf(string url,string reportTitle,string paperKind="A3",Orientation or= Orientation.Portrait);

        Task<byte[]> ExportPageToPdf<T>(ReportGenerationViewModel<T> model);

        FileStreamResult ExportToExcel(IEnumerable<object> data,string filename);

        FileStreamResult ExportToExcel<T>(List<T> listdata, string filename);

    }
}
