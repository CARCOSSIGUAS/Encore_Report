using Belcorp.Encore.Application.Extension;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;

namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/reportaccount")]
    public class ReportAccountController : Controller
    {
        private readonly IReportAccountService reportAccountService;
        private IUrlHelper urlHelper;

        public ReportAccountController
        (
            IReportAccountService _reportAccountService,
            IUrlHelper _urlHelper
        )
        {
            reportAccountService = _reportAccountService;
            urlHelper = _urlHelper;
        }

        [HttpGet("sponsoreds", Name = "GetReportAccountsSponsoreds")]
        public IActionResult GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter, string country = null)
        {
            if (filter == null)
            {
                return BadRequest();
            }

            var result = reportAccountService.GetReportAccountsSponsoreds(filter, country);

            if (result == null)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", result.GetHeader().ToJson());

            var outputModel = new ReportAccountsSponsoredsPaging
            {
                Paging = result.GetHeader(),
                Items = result.List.ToReportAccount_DTO()
            };

            return Ok(outputModel);
        }

        [HttpGet("periods", Name = "GetReportAccountsPeriods")]
        public IActionResult GetReportAccountsPeriods(string country)
        {
            var result = reportAccountService.GetReportAccountsPeriods(country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("exportexcel")]
        public IActionResult ExportExcelAccounts(ReportAccountsSponsoredsSearch filter, string country)
        {
            try
            {
                const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                filter.PageNumber = 1;
                filter.PageSize = int.MaxValue;

                var result = reportAccountService.GetReportAccountsSponsoreds(filter, country);
                if (result == null)
                {
                    return NotFound();
                }

                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Accounts");
                    int totalRows = result.TotalItems;

                    var excel = result.List.ToReportAccount_DTO();

                    worksheet.Cells["A1"].LoadFromCollection(excel, PrintHeaders: true);

                    int noOfProperties = excel.GetType().GetGenericArguments()[0].GetProperties().Length;

                    using (ExcelRange r = worksheet.Cells[1, 1, 1, noOfProperties])
                    {
                        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    }

                    //Install libgdiplus in server
                    //for (var col = 1; col < noOfProperties + 1; col++)
                    //{
                    //    worksheet.Column(col).AutoFit();
                    //}

                    var dateReport = DateTime.Now;
                    return File(package.GetAsByteArray(), XlsxContentType, string.Format("Accounts_{0}.{1}", dateReport.ToString("dd-MM-yyyy"), ".xlsx"));
                }
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException.ToString());
            }
        }
    }
}

