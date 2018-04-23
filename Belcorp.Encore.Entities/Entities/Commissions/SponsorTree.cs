using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class SponsorTree
    {
		[Key]
		public int AccountID { get; set; }
		public int? SponsorID { get; set; }
		public int? HLevel { get; set; }
		public int? HGen { get; set; }
		public int? LeftBower { get; set; }
		public int? RightBower { get; set; }
		public int? NodeNumber { get; set; }
		public int? NodeCount { get; set; }
		public byte[] SortPath { get; set; }
		public int? CurrentPAT { get; set; }
		public int? PreviousCT { get; set; }
		public int? CurrentCT { get; set; }
		public int? CQ { get; set; }
		public int? CQL { get; set; }
		public decimal? PQV { get; set; }
		public decimal? PCV { get; set; }
		public decimal? GQV { get; set; }
		public decimal? DQV { get; set; }
		public decimal? DCV { get; set; }
		public int? EnrollerID { get; set; }
		public DateTime? EnrollmentDateUTC { get; set; }

	}
}
