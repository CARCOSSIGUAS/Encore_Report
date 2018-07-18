using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class AccountAdditionalTitulars
    {
        [Key]
        public int AccountAdditionalTitularID { get; set; }

        public int AccountID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Int16? GerderID { get; set; }
        public DateTime? Brithday { get; set; }
        public int? SortIndex { get; set; }
        public int RelationshipID { get; set; }
    }
}
