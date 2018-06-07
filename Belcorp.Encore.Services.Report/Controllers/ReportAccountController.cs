using Belcorp.Encore.Application.Extension;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
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
        private readonly ITermTranslationsService termTranslationsService;
        private IUrlHelper urlHelper;

        public ReportAccountController
        (
            IReportAccountService _reportAccountService,
            IUrlHelper _urlHelper,
            ITermTranslationsService _termTranslationsService
        )
        {
            reportAccountService = _reportAccountService;
            urlHelper = _urlHelper;
            termTranslationsService = _termTranslationsService;
        }

        [HttpGet("sponsoreds", Name = "GetReportAccountsSponsoreds")]
        public IActionResult GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter)
        {
            if (filter == null)
            {
                return BadRequest();
            }

            var result = reportAccountService.GetReportAccountsSponsoreds(filter);

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
        public IActionResult GetReportAccountsPeriods()
        {
            var result = reportAccountService.GetReportAccountsPeriods();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("exportexcel")]
        public IActionResult ExportExcelAccounts(ReportAccountsSponsoredsSearch filter, int LanguageID)
        {
            try
            {
                const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                filter.PageNumber = 1;
                filter.PageSize = int.MaxValue;

                var result = reportAccountService.GetReportAccountsSponsoreds(filter);
                if (result == null)
                {
                    return NotFound();
                }

                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Accounts");
                    int totalRows = result.TotalItems;
                    
                    var excel = result.List.ToExcel_DTO();
                   
                    worksheet.Cells["A1"].Value = termTranslationsService.GetLanguageTerm("en", "AccountID");
                    worksheet.Cells["B1"].Value = termTranslationsService.GetLanguageTerm("en", "AccountNumber");
                    worksheet.Cells["C1"].Value = termTranslationsService.GetLanguageTerm("en", "AccountName");
                    worksheet.Cells["D1"].Value = termTranslationsService.GetLanguageTerm("en", "JoinDateToString");
                    worksheet.Cells["E1"].Value = termTranslationsService.GetLanguageTerm("en", "EmailAddress");
                    worksheet.Cells["F1"].Value = termTranslationsService.GetLanguageTerm("en", "Generation");
                    worksheet.Cells["G1"].Value = termTranslationsService.GetLanguageTerm("en", "LEVEL");
                    worksheet.Cells["H1"].Value = termTranslationsService.GetLanguageTerm("en", "Activity");
                    worksheet.Cells["I1"].Value = termTranslationsService.GetLanguageTerm("en", "PQV");
                    worksheet.Cells["J1"].Value = termTranslationsService.GetLanguageTerm("en", "PCV");
                    worksheet.Cells["K1"].Value = termTranslationsService.GetLanguageTerm("en", "DQV");
                    worksheet.Cells["L1"].Value = termTranslationsService.GetLanguageTerm("en", "DQVT");
                    worksheet.Cells["M1"].Value = termTranslationsService.GetLanguageTerm("en", "CareerTitle");
                    worksheet.Cells["N1"].Value = termTranslationsService.GetLanguageTerm("en", "PaidAsCurrentMonth");
                    worksheet.Cells["O1"].Value = termTranslationsService.GetLanguageTerm("en", "MainAddress");
                    worksheet.Cells["P1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 1));
                    worksheet.Cells["Q1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 2));
                    worksheet.Cells["R1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 3));
                    worksheet.Cells["S1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 4));
                    worksheet.Cells["T1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 5));
                    worksheet.Cells["U1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 6));
                    worksheet.Cells["V1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 7));
                    worksheet.Cells["W1"].Value = termTranslationsService.GetLanguageTerm("en", "SponsorID");
                    worksheet.Cells["X1"].Value = termTranslationsService.GetLanguageTerm("en", "SponsorName"); 
                    worksheet.Cells["Y1"].Value = termTranslationsService.GetLanguageTerm("en", "SponsorEmailAddress");
                    worksheet.Cells["Z1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 1));
                    worksheet.Cells["AA1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 2));
                    worksheet.Cells["AB1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 3));
                    worksheet.Cells["AC1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 4));
                    worksheet.Cells["AD1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 5));
                    worksheet.Cells["AE1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 6));
                    worksheet.Cells["AF1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 7));
                    worksheet.Cells["A2"].LoadFromCollection(excel, PrintHeaders: true);
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

