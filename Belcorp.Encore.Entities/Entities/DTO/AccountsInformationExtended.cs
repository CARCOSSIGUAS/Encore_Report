﻿using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class AccountsInformationExtended
    {
		public Accounts_Mongo accounts_Mongo { get; set; }

		public int? RigthBower { get; set; }

		public int? LeftBower { get; set; }

		public DateTime? periodStartDateUTC { get; set; }

		public DateTime? periodEndDateUTC { get; set; }

		public string periodDescription { get; set; }

        public string cantFinalPeriodo { get; set; }

        public string CareerTitle { get; set; }
        public string CareerTitle_Des { get; set; }
    }
}
