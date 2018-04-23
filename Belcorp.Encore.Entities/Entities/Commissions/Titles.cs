using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class Titles
    {
        [Key]
        public int TitleID { get; set; }
        public string TitleCode { get; set; }
        public string Name { get; set; }
        public string TermName { get; set; }
        public int SortOrder { get; set; }
        public bool Active { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public bool ReportVisibility { get; set; }
        public int? TitlePhaseID { get; set; }
    }
}
