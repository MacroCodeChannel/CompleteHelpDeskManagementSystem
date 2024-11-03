using ClosedXML.Excel;
using ClosedXML.Extensions;
using DinkToPdf;
using ElmahCore;
using HelpDeskSystem.Interfaces;
using HelpDeskSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace HelpDeskSystem.Services
{
    public class ExportService : IExportService
    {

        private readonly IPdfService _pdfService;
        public ExportService(IPdfService pdfService) 
        {
            _pdfService = pdfService;
        }

        public async Task<byte[]> ExportPageToPdf<T>(ReportGenerationViewModel<T> model)
        {
            byte[] pdf = null;
            try
            {
                if (model.Method == "POST" || string.IsNullOrEmpty(model.Method))
                {
                    var client = new RestClient(model.Url);
                    var request = new RestRequest
                    {
                        Method = Method.Post
                    };
                    request.AddBody(model);

                    var response =await client.PostAsync(request);
                    pdf = _pdfService.ConvertPdf(response.Content, pk:model.PaperKind, orientation:model.PageOrientation);

                }
                else
                {
                    var client = new RestClient(model.Url);
                    var request = new RestRequest
                    {
                        Method = Method.Get
                    };
                    request.AddBody(model);

                    var response = await client.GetAsync(request);
                    pdf = _pdfService.ConvertPdf(response.Content, pk: model.PaperKind, orientation: model.PageOrientation);

                }
            }
            catch(Exception ex)
            {
                ElmahExtensions.RaiseError(ex);
                return null;
            }

            return pdf;
        }

        public FileStreamResult ExportToExcel(IEnumerable<object> data, string filename)
        {
            var wb = new XLWorkbook();
            var worksheetName = filename;
            if (worksheetName.Length > 23)
            {
                worksheetName = worksheetName.Substring(0, 25) + "...";
            }
            var ws = wb.Worksheets.Add(worksheetName);
            filename = filename + ".xlsx";
            ws.Cell(1, 1).InsertData(data);
            ws.Columns().AdjustToContents();
            var xltable = ws.Tables.FirstOrDefault();

            if (xltable != null)
            {
                xltable.ShowAutoFilter = true;
            }

            return wb.Deliver(filename);
        }

        public FileStreamResult ExportToExcel<T>(List<T> listdata, string filename)
        {
             var wb = new XLWorkbook();
            var worksheetName = filename;
            if (worksheetName.Length > 23)
            {
                worksheetName = worksheetName.Substring(0, 25) + "...";
            }
            var ws = wb.Worksheets.Add(worksheetName);
            filename = filename + ".xlsx";
            ws.Cell(1, 1).InsertData(listdata);
            ws.Columns().AdjustToContents();
            var xltable = ws.Tables.FirstOrDefault();

            if(xltable != null) 
            {
                xltable.ShowAutoFilter = true;
            }

            return wb.Deliver(filename);

        }

        public async Task<byte[]> ExportToPdf(string url, string reportTitle, string paperKind = "A3", Orientation orientation = Orientation.Portrait)
        {
            try
            {

                PaperKind pk = PaperKind.A3;

                var httpclient = new HttpClient();
                httpclient.BaseAddress = new Uri(url);
                var client = new RestClient(httpclient);
                var request = new RestRequest
                {
                    Method = Method.Get
                };

                var response = await client.ExecuteAsync(request);
                var htmlinput = response.Content!.Replace(@"\n","").Replace(@"\r","");
                byte[] pdf = _pdfService.ConvertPdf(htmlcontent: htmlinput, orientation, pk: pk);

                return pdf;


            } catch (Exception ex)
            {
                ElmahExtensions.RaiseError(ex);
                return null;
            }

        }
    }
}
