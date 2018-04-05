using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class AccountKPIs
    {
		[Key, Column(Order = 0)]
		public int AccountID { get; set; }

		[Key, Column(Order = 1)]
		public int PeriodID { get; set; }

		[Key, Column(Order = 2)]
		public int CalculationTypeID { get; set; }

		public decimal Value { get; set; }

		public DateTime DateModified { get; set; }

	}
}
