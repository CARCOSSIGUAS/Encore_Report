using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class Report_Downline
    {
        public int AccountsInformationID { get; set; }

        public int PeriodID { get; set; }
        public int AccountID { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string STATE { get; set; }


        public Accounts accounts { get; set; }
    }
}
