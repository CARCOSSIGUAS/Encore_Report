using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class Languages
    {
        [Key]
        public int LanguageID { get; set; }

        public string Name { get; set; }
        public string TermName { get; set; }
        public string Description { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageCode3 { get; set; }
        public string CultureInfo { get; set; }
        public bool Active { get; set; }
        public bool ? IsBase { get; set; }
    }
}
