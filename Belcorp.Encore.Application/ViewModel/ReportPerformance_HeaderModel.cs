using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Belcorp.Encore.Services.Report.ViewModel
{
    public class ReportPerformance_HeaderModel
    {
		public decimal? VP { get; set; }

		public decimal? VOT { get; set; }

		public decimal? VOQ { get; set; }

		public string IdTituloCarrera { get; set; }

		public string TituloCarrera { get; set; }

		public string IdTituloPago { get; set; }

		public string TituloPago { get; set; }

		public string BrazosActivos { get; set; }

	}
}
