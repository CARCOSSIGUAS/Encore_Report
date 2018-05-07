using Belcorp.Encore.Entities.Entities.Search.Paging;
using System;

namespace Belcorp.Encore.Entities.Entities.Search
{
    public class ReportAccountsSponsoredsSearch : PagingSearch
    {
        public ReportAccountsSponsoredsSearch()
        {
            AccountNumberSearch = 0;
            AccountNameSearch = "";
            SponsorNumberSearch = 0;
            SponsorNameSearch = "";

            LevelIds = "";
            GenerationIds = "";
            CareerTitleIds = "";
            AccountStatusIds = "";

            PQVFrom = 0;
            PQVTo = decimal.MaxValue;

            DQVFrom = 0;
            DQVTo = decimal.MaxValue;

            joinDateFrom = DateTime.MinValue;
            joinDateTo = DateTime.MaxValue;
        }

        public int AccountId { get; set; }
        public int PeriodId { get; set; }

        public int? AccountNumberSearch { get; set; }
        public string AccountNameSearch { get; set; } 

        public int? SponsorNumberSearch { get; set; }
        public string SponsorNameSearch { get; set; }

        public string LevelIds { get; set; }
        public string GenerationIds { get; set; }
        public string CareerTitleIds { get; set; }
        public string AccountStatusIds { get; set; }

        public DateTime joinDateFrom { get; set; }
        public DateTime joinDateTo { get; set; }

        public decimal PQVFrom { get; set; }
        public decimal PQVTo { get; set; }

        public decimal DQVFrom { get; set; }
        public decimal DQVTo { get; set; }

        public string OrderBy { get; set; }
    }
}
