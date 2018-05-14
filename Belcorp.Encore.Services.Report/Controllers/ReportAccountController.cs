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
    [Route("api/reportaccount")]
    public class ReportAccountController : Controller
    {
        private readonly IReportAccountService accountInformationService;
        private IUrlHelper urlHelper;

        public ReportAccountController
        (
            IReportAccountService _accountInformationService,
            IUrlHelper _urlHelper
        )
        {
            accountInformationService = _accountInformationService;
            urlHelper = _urlHelper;
        }

        [HttpGet("sponsoreds", Name = "GetReportAccountsSponsoreds")]
        public IActionResult GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter)
        {
            if (filter == null)
            {
                return BadRequest();
            }

            var result = accountInformationService.GetReportAccountsSponsoreds(filter);
            if (result == null)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", result.GetHeader().ToJson());

            var outputModel = new ReportAccountsSponsoredsPaging
            {
                Paging = result.GetHeader(),
                Links = GetLinks(filter, result),
                Items = result.List.ToList(),
            };

            return Ok(outputModel);
        }

        [HttpGet("periods", Name = "GetReportAccountsPeriods")]
        public IActionResult GetReportAccountsPeriods()
        {
            var result = accountInformationService.GetReportAccountsPeriods();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("exportexcel")]
        public IActionResult ExportExcelAccounts(ReportAccountsSponsoredsSearch filter)
        {
            try
            {
                const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                filter.PageNumber = 1;
                filter.PageSize = int.MaxValue;

                var result = accountInformationService.GetReportAccountsSponsoreds(filter);
                if (result == null)
                {
                    return NotFound();
                }

                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Accounts");
                    int totalRows = result.TotalItems;

                    worksheet.Cells["A1"].LoadFromCollection(result.List, PrintHeaders: true);

                    int noOfProperties = result.GetType().GetGenericArguments()[0].GetProperties().Length;

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

        private LinkInfo CreateLink(ReportAccountsSponsoredsSearch filter, string routeName, int pageNumber, int pageSize, string rel)
        {
            return new LinkInfo
            {
                Href = urlHelper.Link(routeName,
                        new
                        {
                            periodId = filter.PeriodId,
                            accountId = filter.AccountId,
                            accountNumberSearch = filter.AccountNumberSearch,
                            sponsorNumberSearch = filter.SponsorNumberSearch,
                            titleTypes = filter.TitleType,
                            titleIds = filter.TitleIds,
                            accountStatusIds = filter.AccountStatusIds,
                            pageNumber = pageNumber,
                            pageSize = pageSize
                        }),
                Rel = rel
            };
        }
        #endregion
    }
}

