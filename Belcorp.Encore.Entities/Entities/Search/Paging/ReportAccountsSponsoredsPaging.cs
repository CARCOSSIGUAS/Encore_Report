
using Belcorp.Encore.Entities.Entities.DTO;
using System.Collections.Generic;

namespace Belcorp.Encore.Entities.Entities.Search.Paging
{
    public class ReportAccountsSponsoredsPaging
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<ReportAccount_DTO> Items { get; set; }
    }
}
