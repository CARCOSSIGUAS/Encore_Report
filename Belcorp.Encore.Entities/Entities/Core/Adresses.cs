using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class Addresses
    {
        [Key]
        public int AddressID { get; set; }

        public short AddressTypeID { get; set; }
        public string ProfileName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Attention { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public int ? StateProvinceID { get; set; }
        public string PostalCode { get; set; }
        public int CountryID { get; set; }
        public string PhoneNumber { get; set; }
        public bool ? IsOutsideCityLimits { get; set; }
        public bool IsDefault { get; set; }
        public bool IsGeoCodeCurrent { get; set; }
        public double ? Latitude { get; set; }
        public double ? Longitude { get; set; }
        public byte[] DataVersion { get; set; }
        public int ? ModifiedByUserID { get; set; }
        public string AddressNumber { get; set; }
        public int ? PhoneTypeID { get; set; }
        public string ETLNaturalKey { get; set; }
        public string ETLHash { get; set; }
        public string ETLPhase { get; set; }
        public DateTime ? ETLDate { get; set; }
        public string Street { get; set; }
    }
}
