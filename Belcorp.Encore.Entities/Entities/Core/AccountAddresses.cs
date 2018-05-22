using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class AccountAddresses
    {
        [Key, Column(Order = 0)]
        public int AccountID { get; set; }

        [Key, Column(Order = 1)]
        public int AddressID { get; set; }

        public string ETLNaturalKey { get; set; }
        public string ETLHash { get; set; }
        public string ETLPhase { get; set; }
        public DateTime? ETLDate { get; set; }

        public Addresses Addresses { get; set; }
    }
}
