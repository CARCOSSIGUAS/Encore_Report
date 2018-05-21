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
                CareerTitle = ai.CareerTitle,
                CareerTitle_Des = ai.CareerTitle_Des,
                DQV = ai.DQV,
                DQVT = ai.DQVT,
                EmailAddress = ai.EmailAddress,
                Generation = ai.Generation,
                JoinDateToString = ai.JoinDate.HasValue ? ai.JoinDate.Value.ToString("dd/MM/yyyy") : "",
                LEVEL = ai.LEVEL,
                MainAddress = ai.Account.Addresses.Where(a => a.AddressTypeID == 1).Select(a => a.Street + " - " + a.Address1 + " - " + a.County + " - " + a.City + " - " + a.State).FirstOrDefault(),
                PaidAsCurrentMonth = ai.PaidAsCurrentMonth,
                PaidAsCurrentMonth_Des = ai.PaidAsCurrentMonth_Des,
                PCV = ai.PCV,
                Phones = String.Join(" - ", ai.Account.AccountPhones.Select(p => p.PhoneNumber).ToList()),
                PQV = ai.PQV,
                SponsorEmailAddress = ai.Sponsor.EmailAddress,
                SponsorID = ai.SponsorID,
                SponsorName = ai.SponsorName,
                SponsorPhones = String.Join(" - ", ai.Sponsor.AccountPhones.Select(p => p.PhoneNumber).ToList())
            }).ToList();

            return result;
        }
    }
}
