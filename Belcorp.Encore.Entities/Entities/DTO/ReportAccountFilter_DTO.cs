using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class ReportAccountFilter_DTO
    {
        public string photoURL { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string CareerTitle_Des { get; set; }
    }
}
