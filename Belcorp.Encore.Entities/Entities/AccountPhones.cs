using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class AccountPhones
    {
        [Key]
        public int AccountPhoneID { get; set; }

        public int AccountID { get; set; }
        public int PhoneTypeID { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsDefault { get; set; }
        public byte[] DataVersion { get; set; }
        public int? ModifiedByUserID { get; set; }
        public string ETLNaturalKey { get; set; }
        public string ETLHash { get; set; }
        public string ETLPhase { get; set; }
        public DateTime? ETLDate { get; set; }
    }
}
