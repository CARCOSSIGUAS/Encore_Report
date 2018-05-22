using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class PerformanceIndicator_DTO
    {
        public decimal? PQV { get; set; }

        public decimal? DQV { get; set; }
  
        public decimal? DQVT { get; set; }

        public string CareerTitle { get; set; }
        public string CareerTitle_Desc { get; set; }

        public string PaidTitle { get; set; }
        public string PaidTitle_Desc { get; set; }

    }
}
