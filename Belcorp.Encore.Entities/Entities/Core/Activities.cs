using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class Activities
    {
        [Key]
        public long ActivityID { get; set; }

        public int AccountID { get; set; }
        public short ActivityStatusID { get; set; }
        public int PeriodID { get; set; }
        public bool IsQualified { get; set; }
        public short ? AccountConsistencyStatusID { get; set; }
        public bool ? HasContinuity { get; set; }

        public ActivityStatuses ActivityStatuses { get; set; }
        public AccountConsistencyStatuses AccountConsistencyStatuses { get; set; }
    }
}
