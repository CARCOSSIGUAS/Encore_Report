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
        public IActionResult ExportExcelAccounts(ReportAccountsSponsoredsSearch filter, int languageID, string country)
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
                    
                    var excel = result.List.ToExcel_DTO();
                   
                    worksheet.Cells["A1"].Value = termTranslationsService.GetLanguageTerm("en", "AccountID", country);
                    worksheet.Cells["B1"].Value = termTranslationsService.GetLanguageTerm("en", "AccountNumber", country);
                    worksheet.Cells["C1"].Value = termTranslationsService.GetLanguageTerm("en", "AccountName", country);
                    worksheet.Cells["D1"].Value = termTranslationsService.GetLanguageTerm("en", "JoinDateToString", country);
                    worksheet.Cells["E1"].Value = termTranslationsService.GetLanguageTerm("en", "EmailAddress", country);
                    worksheet.Cells["F1"].Value = termTranslationsService.GetLanguageTerm("en", "Generation", country);
                    worksheet.Cells["G1"].Value = termTranslationsService.GetLanguageTerm("en", "LEVEL", country);
                    worksheet.Cells["H1"].Value = termTranslationsService.GetLanguageTerm("en", "Activity", country);
                    worksheet.Cells["I1"].Value = termTranslationsService.GetLanguageTerm("en", "PQV", country);
                    worksheet.Cells["J1"].Value = termTranslationsService.GetLanguageTerm("en", "PCV", country);
                    worksheet.Cells["K1"].Value = termTranslationsService.GetLanguageTerm("en", "DQV", country);
                    worksheet.Cells["L1"].Value = termTranslationsService.GetLanguageTerm("en", "DQVT", country);
                    worksheet.Cells["M1"].Value = termTranslationsService.GetLanguageTerm("en", "CareerTitle", country);
                    worksheet.Cells["N1"].Value = termTranslationsService.GetLanguageTerm("en", "PaidAsCurrentMonth", country);
                    worksheet.Cells["O1"].Value = termTranslationsService.GetLanguageTerm("en", "MainAddress", country);
                    worksheet.Cells["P1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 1), country);
                    worksheet.Cells["Q1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 2), country);
                    worksheet.Cells["R1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 3), country);
                    worksheet.Cells["S1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 4), country);
                    worksheet.Cells["T1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 5), country);
                    worksheet.Cells["U1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 6), country);
                    worksheet.Cells["V1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("AccountPhone_{0}", 7), country);
                    worksheet.Cells["W1"].Value = termTranslationsService.GetLanguageTerm("en", "SponsorID", country);
                    worksheet.Cells["X1"].Value = termTranslationsService.GetLanguageTerm("en", "SponsorName", country); 
                    worksheet.Cells["Y1"].Value = termTranslationsService.GetLanguageTerm("en", "SponsorEmailAddress", country);
                    worksheet.Cells["Z1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 1), country);
                    worksheet.Cells["AA1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 2), country);
                    worksheet.Cells["AB1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 3), country);
                    worksheet.Cells["AC1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 4), country);
                    worksheet.Cells["AD1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 5), country);
                    worksheet.Cells["AE1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 6), country);
                    worksheet.Cells["AF1"].Value = termTranslationsService.GetLanguageTerm("en", string.Format("SponsorPhone_{0}", 7), country);
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

