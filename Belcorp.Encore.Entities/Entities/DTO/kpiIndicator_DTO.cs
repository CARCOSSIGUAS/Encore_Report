﻿using Belcorp.Encore.Entities.Entities.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class KpiIndicator_DTO
    {
        public int AccountKPIDetailID { get; set; }
        public int PeriodID { get; set; }
        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public int DownlineID { get; set; }
        public string DownlineName { get; set; }
        public string KPICode { get; set; }
        public decimal? Value { get; set; }
        public decimal? Percentage { get; set; }
        public string DownlinePaidAsTitle { get; set; }
        public int? CurrencyTypeID { get; set; }
        public int? AccountSponsorTypeID { get; set; }
        public int? TreeLevel { get; set; }
        public DateTime DateModified { get; set; }
    }
}
