
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Search.Paging;
using System.Collections.Generic;

namespace Belcorp.Encore.Entities.Entities.Search.Paging
{ 
    public class ReportAccountsSponsoredsPaging
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<AccountsInformation_Mongo> Items { get; set; }
    }
}
