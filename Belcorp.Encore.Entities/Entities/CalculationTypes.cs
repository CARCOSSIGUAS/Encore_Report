using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Belcorp.Encore.Entities.Entities
{
    public class CalculationTypes
    {
        [Key]
        public int CalculationTypeID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool UserOverridable { get; set; }
        public bool RealTime { get; set; }
        public string TermName { get; set; }
        public DateTime DateModified { get; set; }
        public bool ReportVisibility { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
    }
}
