using Belcorp.Encore.Entities.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Belcorp.Encore.Application.Extension
{
    public static class Extensions
    {
        public static List<ReportAccount_DTO> ToReportAccount_DTO(this List<AccountsInformation_MongoWithAccountAndSponsor> list)
        {
            var result = list.Select(ai => new ReportAccount_DTO()
            {
                AccountID = ai.AccountID,
                AccountName = ai.AccountName,
                AccountNumber = ai.AccountNumber,
                Activity = ai.Activity,
                CareerTitle = ai.CareerTitle_Des,
                DQV = ai.DQV,
                DQVT = ai.DQVT,
                EmailAddress = ai.EmailAddress,
                Generation = ai.Generation,
                JoinDate = ai.JoinDate.HasValue ? ai.JoinDate.Value.ToString("dd/MM/yyyy") : "",
                LEVEL = ai.LEVEL,
                MainAddress = ai.Account != null ? ai.Account.Addresses.Where(a => a.AddressTypeID == 1).Select(a => a.Street + " - " + a.Address1 + " - " + a.County + " - " + a.City + " - " + a.State).FirstOrDefault() : "",
                PaidAsCurrentMonth = ai.Account != null ? ai.PaidAsCurrentMonth_Des : "",
                PCV = ai.PCV,
                Phones = ai.Account != null ? String.Join(" - ", ai.Account.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
                PQV = ai.PQV,
                SponsorEmailAddress = ai.Sponsor != null ? ai.Sponsor.EmailAddress : "",
                SponsorID = ai.SponsorID,
                SponsorName = ai.SponsorName,
                SponsorPhones = ai.Sponsor != null ? String.Join(" - ", ai.Sponsor.AccountPhones.Select(p => p.PhoneNumber).ToList()) : ""
            }).ToList();

            return result;
        }

        public static List<Excel_DTO> ToExcel_DTO(this List<AccountsInformation_MongoWithAccountAndSponsor> list)
        {
            var result = list.Select(ai => new Excel_DTO()
            {
                AccountID = ai.AccountID,
                AccountName = ai.AccountName,
                Activity = ai.Activity,
                CareerTitle = ai.CareerTitle_Des,
                DQV = ai.DQV,
                DQVT = ai.DQVT,
                EmailAddress = ai.EmailAddress,
                Generation = ai.Generation,
                JoinDate = ai.JoinDate.HasValue ? ai.JoinDate.Value.ToString("dd/MM/yyyy") : "",
                LEVEL = ai.LEVEL,
                MainAddress = ai.Account != null ? ai.Account.Addresses.Where(a => a.AddressTypeID == 1).Select(a => a.Street + " - " + a.Address1 + " - " + a.County + " - " + a.City + " - " + a.State).FirstOrDefault() : "",
                PaidAsCurrentMonth = ai.PaidAsCurrentMonth_Des,
                PCV = ai.PCV,
                AccountPhone_1 = ai.Sponsor != null ? ai.Account.AccountPhones.Where(p => p.AccountPhoneID == 1).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_2 = ai.Sponsor != null ? ai.Account.AccountPhones.Where(p => p.AccountPhoneID == 2).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_3 = ai.Sponsor != null ? ai.Account.AccountPhones.Where(p => p.AccountPhoneID == 3).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_4 = ai.Sponsor != null ? ai.Account.AccountPhones.Where(p => p.AccountPhoneID == 4).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_5 = ai.Sponsor != null ? ai.Account.AccountPhones.Where(p => p.AccountPhoneID == 5).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_6 = ai.Sponsor != null ? ai.Account.AccountPhones.Where(p => p.AccountPhoneID == 6).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_7 = ai.Sponsor != null ? ai.Account.AccountPhones.Where(p => p.AccountPhoneID == 7).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                PQV = ai.PQV,
                SponsorEmailAddress = ai.Sponsor != null ? ai.Sponsor.EmailAddress : "",
                SponsorID = ai.SponsorID,
                SponsorName = ai.SponsorName,
                SponsorPhone_1 = ai.Sponsor != null ? ai.Sponsor.AccountPhones.Where(p => p.PhoneTypeID == 1).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                SponsorPhone_2 = ai.Sponsor != null ? ai.Sponsor.AccountPhones.Where(p => p.PhoneTypeID == 2).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                SponsorPhone_3 = ai.Sponsor != null ? ai.Sponsor.AccountPhones.Where(p => p.PhoneTypeID == 3).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                SponsorPhone_4 = ai.Sponsor != null ? ai.Sponsor.AccountPhones.Where(p => p.PhoneTypeID == 4).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                SponsorPhone_5 = ai.Sponsor != null ? ai.Sponsor.AccountPhones.Where(p => p.PhoneTypeID == 5).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                SponsorPhone_6 = ai.Sponsor != null ? ai.Sponsor.AccountPhones.Where(p => p.PhoneTypeID == 6).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                SponsorPhone_7 = ai.Sponsor != null ? ai.Sponsor.AccountPhones.Where(p => p.PhoneTypeID == 7).Select(p => p.PhoneNumber).FirstOrDefault() : ""
            }).ToList();

            return result;
        }
    }
}
