using Belcorp.Encore.Application.Extension;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;
using Belcorp.Encore.Services.Report.Filters;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;

namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/reportaccount")]
    [ServiceFilter(typeof(FilterActionProxy))]
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

        [HttpGet("sponsoredsthree", Name = "GetReportAccountsSponsoredsThree")]
        public IActionResult GetReportAccountsSponsoredsThree(ReportAccountsSponsoredsSearch filter, string country = null)
        {
            if (filter == null)
            {
                return BadRequest();
            }

            var result = reportAccountService.GetReportAccountsSponsoredsThree(filter, country);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.ToReportAccount_DTO());
        }

        [HttpGet("exportexcel")]
        public IActionResult ExportExcelAccounts(ReportAccountsSponsoredsSearch filter, string language, string country = null)
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

                    var excel = result.List.ToReportAccountExcel_DTO();

                    worksheet.Cells["A1"].Value = termTranslationsService.GetLanguageTerm(language, "ConsultantCode", country);
                    worksheet.Cells["B1"].Value = termTranslationsService.GetLanguageTerm(language, "ConsultantName", country);
                    worksheet.Cells["C1"].Value = termTranslationsService.GetLanguageTerm(language, "DateEnrolled", country);
                    worksheet.Cells["D1"].Value = termTranslationsService.GetLanguageTerm(language, "Email", country);
                    worksheet.Cells["E1"].Value = termTranslationsService.GetLanguageTerm(language, "Generation", country);
                    worksheet.Cells["F1"].Value = termTranslationsService.GetLanguageTerm(language, "Level", country);
                    worksheet.Cells["G1"].Value = termTranslationsService.GetLanguageTerm(language, "Status", country);
                    worksheet.Cells["H1"].Value = termTranslationsService.GetLanguageTerm(language, "PQV", country);
                    worksheet.Cells["I1"].Value = termTranslationsService.GetLanguageTerm(language, "PCV", country);
                    worksheet.Cells["J1"].Value = termTranslationsService.GetLanguageTerm(language, "DQV", country);
                    worksheet.Cells["K1"].Value = termTranslationsService.GetLanguageTerm(language, "DQVT", country);
                    worksheet.Cells["L1"].Value = termTranslationsService.GetLanguageTerm(language, "CareerTitle", country);
                    worksheet.Cells["M1"].Value = termTranslationsService.GetLanguageTerm(language, "PaidAsTitle", country);
                    worksheet.Cells["N1"].Value = termTranslationsService.GetLanguageTerm(language, "Address", country);

                    worksheet.Cells["O1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 1";
                    worksheet.Cells["P1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 2";
                    worksheet.Cells["Q1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 3";
                    worksheet.Cells["R1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 4";
                    worksheet.Cells["S1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 5";
                    worksheet.Cells["T1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 6";
                    worksheet.Cells["U1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 7";

                    worksheet.Cells["V1"].Value = termTranslationsService.GetLanguageTerm(language, "SponsorCode", country);
                    worksheet.Cells["W1"].Value = termTranslationsService.GetLanguageTerm(language, "SponsorName", country);
                    worksheet.Cells["X1"].Value = termTranslationsService.GetLanguageTerm(language, "Email", country);
                    worksheet.Cells["Y1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 1";
                    worksheet.Cells["Z1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 2";
                    worksheet.Cells["AA1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 3";
                    worksheet.Cells["AB1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 4";
                    worksheet.Cells["AC1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 5";
                    worksheet.Cells["AD1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 6";
                    worksheet.Cells["AE1"].Value = $"{termTranslationsService.GetLanguageTerm(language, "Phone", country)} 7";

                    worksheet.Cells["A2"].LoadFromCollection(excel);
                    int noOfProperties = excel.GetType().GetGenericArguments()[0].GetProperties().Length;

                    using (ExcelRange r = worksheet.Cells[1, 1, 1, noOfProperties])
                    {
                        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    }

                    try
                    {
                        //Install libgdiplus in server
                        for (var col = 1; col < noOfProperties + 1; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    var dateReport = DateTime.Now;
                    return File(package.GetAsByteArray(), XlsxContentType, string.Format("Accounts_{0}.{1}", dateReport.ToString("dd-MM-yyyy"), ".xlsx"));
                }
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException.ToString());
            }
        }

        [HttpGet("consultantdetails", Name = "GetConsultantDetails")]
        public IActionResult GetConsultantDetails(int? periodId, int accountId, string country = null)
        {
            if (accountId == 0 && periodId == 0)
            {
                return BadRequest();
            }

            var result = reportAccountService.GetConsultantDetails(periodId, accountId, country);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.ToReportAccount_DTO());
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
    }
}

