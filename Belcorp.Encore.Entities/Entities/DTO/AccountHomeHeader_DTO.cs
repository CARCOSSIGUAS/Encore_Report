using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class AccountHomeHeader_DTO
    {
        public Accounts_Mongo account { get; set; }

        public int? RightBower { get; set; }
        public int? LeftBower { get; set; }

        public int? periodId { get; set; }
        public DateTime? periodStartDateUTC { get; set; }
        public DateTime? periodEndDateUTC { get; set; }
        public string periodDescription { get; set; }
        public string cantFinalPeriodo { get; set; }

        public string CareerTitle { get; set; }
        public string CareerTitle_Des { get; set; }
    }
}
