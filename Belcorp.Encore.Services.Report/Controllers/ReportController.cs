using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/report")]
    public class ReportController : Controller
    {
        private readonly IAccountInformationService accountInformationService;
        private readonly IAccountsService accountsService;
        private readonly IMonitorMongoService monitorMongoService;
        private IUrlHelper urlHelper;

        public ReportController
        (
            IAccountInformationService _accountInformationService,
            IAccountsService _accountsService,
            IMonitorMongoService _monitorMongoService,
            IUrlHelper _urlHelper
        )
        {
            accountInformationService = _accountInformationService;
            accountsService = _accountsService;
            monitorMongoService = _monitorMongoService;
            urlHelper = _urlHelper;
        }

        // GET: api/Report
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPerformance_Header(int accountId, int periodId)
        {
            var header = await accountInformationService.GetPerformance_Header(accountId, periodId);

            return Json(header);
        }

        // GET: api/Report
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPerformance_Detail(int accountId, int periodId)
        {
            var header = await accountInformationService.GetPerformance_Detail(accountId, periodId);

            return Json(header);
        }

        [HttpGet("[action]")]
        public IActionResult GetAccountsFilterPaginated(Filtros_DTO filtrosDTO)
        {
            if (filtrosDTO == null)
            {
                return BadRequest();
            }

            var header = accountInformationService.GetAccounts(filtrosDTO);

            return Json(header);
        }

        [HttpGet("sponsoreds", Name = "GetReportAccountsSponsoreds")]
        public IActionResult GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch reportAccountsSearch)
        {
            if (reportAccountsSearch == null)
            {
                return BadRequest();
            }

            var result = accountInformationService.GetReportAccountsSponsoreds(reportAccountsSearch);
            Response.Headers.Add("X-Pagination", result.GetHeader().ToJson());

            var outputModel = new ReportAccountsSponsoredsPaging
            {
                Paging = result.GetHeader(),
                Links = GetLinks(reportAccountsSearch, result),
                Items = result.List.ToList(),
            };

            return Ok(outputModel);
        }

        // GET: api/Report
        [HttpGet("[action]")]
        public JsonResult GetPerformance_HeaderFront(int accountId)
        {
            var header = accountInformationService.GetPerformance_HeaderFront(accountId);
            return Json(header);
        }
        // GET: api/Report
        [HttpGet("[action]")]
        public JsonResult GetPerformance_AccountInformation(int accountId, int periodId)
        {
            var header = accountInformationService.GetPerformance_AccountInformation(accountId, periodId);
            return Json(header);
        }

        [HttpGet("exportexcel")]
        public IActionResult ExportExcelAccounts(Filtros_DTO filtrosDTO)
        {
            try
            {
                const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                filtrosDTO.NumeroRegistros = 999999999;
                var result = accountInformationService.GetAccounts(filtrosDTO);
                var resultData = result != null ? result.accountsInformationDTO : new List<Entities.Entities.Mongo.AccountsInformation_Mongo>();

                using (var package = new ExcelPackage())
                {

                    //var worksheet = package.Workbook.Worksheets.Add("Excel");
                    //worksheet.Cells["A1"].LoadFromCollection(result, PrintHeaders: true);

                    //int noOfProperties = result.GetType().GetGenericArguments()[0].GetProperties().Length;

                    //using (ExcelRange r = worksheet.Cells[1, 1, 1, noOfProperties])
                    //{
                    //    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    //    r.Style.Font.Bold = true;
                    //    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    //}

                    //for (var col = 1; col < noOfProperties + 1; col++)
                    //{
                    //    worksheet.Column(col).AutoFit();
                    //}

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Accounts");
                    int totalRows = resultData.Count();

                    worksheet.Cells[1, 1].Value = "Account Number";
                    worksheet.Cells[1, 2].Value = "Account Name";
                    worksheet.Cells[1, 3].Value = "Career Title";
                    worksheet.Cells[1, 4].Value = "DQVT";

                    int i = 0;
                    for (int row = 2; row <= totalRows + 1; row++)
                    {
                        worksheet.Cells[row, 1].Value = resultData[i].AccountNumber;
                        worksheet.Cells[row, 2].Value = resultData[i].AccountName;
                        worksheet.Cells[row, 3].Value = resultData[i].CareerTitle_Des;
                        worksheet.Cells[row, 4].Value = resultData[i].DQVT;
                        i++;
                    }
                    var dateReport = DateTime.Now;
                    return File(package.GetAsByteArray(), XlsxContentType, string.Format("Accounts_{0}.{1}", dateReport.ToString("dd-MM-yyyy"), ".xlsx"));
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }


        #region Metodos
        private List<LinkInfo> GetLinks(ReportAccountsSponsoredsSearch reportAccountsSponsoredsSearch, PagedList<AccountsInformation_Mongo> list)
        {
            var links = new List<LinkInfo>();

            if (list.HasPreviousPage)
                links.Add(CreateLink(reportAccountsSponsoredsSearch, "GetReportAccountsSponsoreds", list.PreviousPageNumber, list.PageSize, "previousPage"));

            if (list.HasNextPage)
                links.Add(CreateLink(reportAccountsSponsoredsSearch, "GetReportAccountsSponsoreds", list.NextPageNumber, list.PageSize, "nextPage"));

            links.Add(CreateLink(reportAccountsSponsoredsSearch, "GetReportAccountsSponsoreds", list.PageNumber, list.PageSize, "self"));

            return links;
        }

        private LinkInfo CreateLink(ReportAccountsSponsoredsSearch reportAccountsSponsoredsSearch, string routeName, int pageNumber, int pageSize, string rel)
        {
            return new LinkInfo
            {
                Href = urlHelper.Link(routeName,
                        new
                        {
                            periodId = reportAccountsSponsoredsSearch.PeriodId,
                            accountId = reportAccountsSponsoredsSearch.AccountId,
                            accountNumberSearch = reportAccountsSponsoredsSearch.AccountNumberSearch,
                            sponsorNumberSearch = reportAccountsSponsoredsSearch.SponsorNumberSearch,
                            careerTitleIds = reportAccountsSponsoredsSearch.CareerTitleIds,
                            accountStatusIds = reportAccountsSponsoredsSearch.AccountStatusIds,
                            pageNumber = pageNumber,
                            pageSize = pageSize
                        }),
                Rel = rel
            };
        }
        #endregion
    }
}

