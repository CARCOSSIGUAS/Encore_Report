﻿using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Mongo.Extension;
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
                AccountName = ai.AccountName.ToLower(),
                AccountNumber = ai.AccountNumber,
                Activity = ai.Activity,
                CareerTitleDes = ai.CareerTitle_Des,
                CareerTitle = ai.CareerTitle,
                DQV = ai.DQV,
                DQVT = ai.DQVT,
                EmailAddress = ai.EmailAddress,
                Generation = ai.Generation,
                JoinDate = ai.JoinDate.HasValue ? ai.JoinDate.Value.ToString("dd/MM/yyyy") : "",
                LEVEL = ai.LEVEL,
                MainAddress = ai.Account != null ? ai.Account.Addresses.Where(a => a.AddressTypeID == 1).Select(a => a.Street + " - " + a.Address1 + " - " + a.County + " - " + a.City + " - " + a.State).FirstOrDefault() : "",
                PaidAsCurrentMonthDesc = ai.Account != null ? ai.PaidAsCurrentMonth_Des : "",
                PaidAsCurrentMonth = ai.Account != null ? ai.PaidAsCurrentMonth : "",
                PCV = ai.PCV,
                Phones = ai.Account != null ? String.Join(" - ", ai.Account.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
                PQV = ai.PQV,
                SponsorEmailAddress = ai.Sponsor != null ? ai.Sponsor.EmailAddress : "",
                SponsorID = ai.SponsorID,
                SponsorName = ai.SponsorName.ToLower(),
                SponsorPhones = ai.Sponsor != null ? String.Join(" - ", ai.Sponsor.AccountPhones.Select(p => p.PhoneNumber).ToList()) : ""
            }).ToList();

            return result;
        }

        public static List<ReportAccountExcel_DTO> ToReportAccountExcel_DTO(this List<AccountsInformation_MongoWithAccountAndSponsor> list)
        {
            var result = list.Select(ai => new ReportAccountExcel_DTO()
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
                MainAddress = ai.Account != null ? ai.Account.Addresses
                                                         .Where(a => a.AddressTypeID == 1)
                                                         .Select(a => a.Street + " - " + a.Address1 + " - " + a.County + " - " + a.City + " - " + a.State).FirstOrDefault() 
                                                 : "",
                PostalCode = ai.PostalCode != null && ai.PostalCode.Length == 5  ? ai.PostalCode.Substring(0, 5) + "-" + ai.PostalCode.Substring(5) : "",
                PaidAsCurrentMonth = ai.PaidAsCurrentMonth_Des,
                PCV = ai.PCV,
                AccountPhone_1 = ai.Account != null ? ai.Account.AccountPhones.Where(p => p.PhoneTypeID == 1).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_2 = ai.Account != null ? ai.Account.AccountPhones.Where(p => p.PhoneTypeID == 2).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_3 = ai.Account != null ? ai.Account.AccountPhones.Where(p => p.PhoneTypeID == 3).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_4 = ai.Account != null ? ai.Account.AccountPhones.Where(p => p.PhoneTypeID == 4).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_5 = ai.Account != null ? ai.Account.AccountPhones.Where(p => p.PhoneTypeID == 5).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_6 = ai.Account != null ? ai.Account.AccountPhones.Where(p => p.PhoneTypeID == 6).Select(p => p.PhoneNumber).FirstOrDefault() : "",
                AccountPhone_7 = ai.Account != null ? ai.Account.AccountPhones.Where(p => p.PhoneTypeID == 7).Select(p => p.PhoneNumber).FirstOrDefault() : "",
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

        public static ReportAccount_DTO ToReportAccount_DTO(this AccountsInformation_MongoWithAccountAndSponsor item)
        {
            var result = new ReportAccount_DTO()
            {
                AccountID = item.AccountID,
                AccountName = item.AccountName.ToLower(),
                AccountNumber = item.AccountNumber,
                Activity = item.Activity,
                CareerTitleDes= item.CareerTitle_Des,
                CareerTitle =item.CareerTitle, 
                DQV = item.DQV,
                DQVT = item.DQVT,
                CQL = item.CQL,
                EmailAddress = item.EmailAddress,
                Generation = item.Generation,
                JoinDate = item.JoinDate.HasValue ? item.JoinDate.Value.ToString("dd/MM/yyyy") : "",
                LEVEL = item.LEVEL,
                MainAddress = item.Account != null ? item.Account.Addresses
                                                         .Where(a => a.AddressTypeID == 1)
                                                         .Select(a => a.Street + " - " + a.Address1 + " - " + a.County + " - " + a.City + " - " + a.State).FirstOrDefault() : "",
                PostalCode = item.PostalCode != null ? item.PostalCode.Substring(0, 5) + "-" + item.PostalCode.Substring(5) : "",
                PaidAsCurrentMonth = item.PaidAsCurrentMonth,
                PaidAsCurrentMonthDesc = item.PaidAsCurrentMonth_Des,
                PCV = item.PCV,
                Phones = item.Account != null ? String.Join(" - ", item.Account.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
                PQV = item.PQV,
                SponsorEmailAddress = item.Sponsor != null ? item.Sponsor.EmailAddress : "",
                SponsorID = item.SponsorID,
                SponsorName = item.SponsorName.ToLower(),
                SponsorPhones = item.Sponsor != null ? String.Join(" - ", item.Sponsor.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
                ActiveDownline = item.ActiveDownline != null ? item.ActiveDownline : 0,
                ConsultActive = item.ConsultActive != null ? item.ConsultActive : 0,
                Birthday = item.Account.BirthdayUTC.HasValue ? item.Account.BirthdayUTC.Value.ToString("dd/MM/yyyy") : "",

                NCWP = item.NCWP,
                AdditionalTitularName = item.Account.AccountAdditionalTitulars.Count > 0 ? item.Account.AccountAdditionalTitulars[0].FirstName + " " + item.Account.AccountAdditionalTitulars[0].LastName : "",
                AdditionalTitularBirthday = item.Account.AccountAdditionalTitulars.Count > 0 ? item.Account.AccountAdditionalTitulars[0].Brithday.Value.ToString("dd/MM/yyyy") : "",

                UplineLeader0ID = item.Leader0 != null ? item.Leader0.AccountID : 0,
                UplineLeader0Name = item.Leader0 != null ? item.Leader0.FirstName + " " + item.Leader0.LastName : "",
                UplineLeader0EmailAddress = item.Leader0 != null ? item.Leader0.EmailAddress : "",
                UplineLeader0Phones = item.Leader0 != null ? String.Join(" - ", item.Leader0.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
            };

            return result;
        }

        public static List<AccountsAutoComplete_DTO> toAccountsAutocomplete_DTO(this List<AccountsInformation_Mongo> list)
        {
            var result = list.Select(ai => new AccountsAutoComplete_DTO()
            {
                PhotoURL = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT38n3CrP_ImhTcMxK2N8gwLnfHHVQaunUSS235Pk-JbFP6xkjw8w",
                AccountID = ai.AccountID,
                AccountName = ai.AccountName.ToLower(),
                CareerTitle = ai.CareerTitle_Des ?? "",
            }).ToList();

            return result;
        }

        public static List<ReportAccount_DTO> ToTopologyList(this IEnumerable<AccountsInformation_Mongo> list)
        {
            var result = list.Select(ai => new ReportAccount_DTO()
            {
                AccountID = ai.AccountID,
                AccountName = ai.AccountName.ToLower(),
                AccountNumber = ai.AccountNumber,
                Activity = ai.Activity,
                CareerTitle = ai.CareerTitle_Des,
                DQV = ai.DQV,
                DQVT = ai.DQVT,
                EmailAddress = ai.EmailAddress,
                Generation = ai.Generation,
                JoinDate = ai.JoinDate.HasValue ? ai.JoinDate.Value.ToString("dd/MM/yyyy") : "",
                LEVEL = ai.LEVEL,
                PCV = ai.PCV,
                PQV = ai.PQV,
                SponsorID = ai.SponsorID,
                SponsorName = ai.SponsorName.ToLower(),
            }).ToList();

            return result;
        }

    }
}
