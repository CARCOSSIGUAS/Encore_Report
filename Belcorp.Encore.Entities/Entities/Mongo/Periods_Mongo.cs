using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class Periods_Mongo
    {
        [BsonId]
        public int PeriodID { get; set; }

        public int CountryID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? PlanID { get; set; }
        public bool? EarningsViewable { get; set; }
        public DateTime? BackOfficeDisplayStartDate { get; set; }
        public bool? DisbursementsProcessed { get; set; }
        public string Description { get; set; }
        public DateTime? StartDateUTC { get; set; }
        public DateTime? EndDateUTC { get; set; }
        public DateTime? LockDate { get; set; }
    }
}
