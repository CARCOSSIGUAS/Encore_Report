using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class OrderCalculationTypes
    {
        [Key]
        public int OrderCalculationTypeID { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string TermName { get; set; }
        public DateTime DateModified { get; set; }

    }
}
