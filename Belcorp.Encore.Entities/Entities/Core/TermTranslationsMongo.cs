using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class TermTranslationsMongo
    {
        [Key]
        public int TermTranslationID { get; set; }

        public int LanguageID { get; set; }
        public string TermName { get; set; }
        public string Term { get; set; }
        public DateTime ? LastUpdatedUTC { get; set; }
        public bool Active { get; set; }

        public Languages Languages { get; set; }
    }
}
