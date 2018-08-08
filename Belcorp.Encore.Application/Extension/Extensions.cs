using Belcorp.Encore.Entities.Entities.DTO;
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
            var result = new List<ReportAccount_DTO>();
            if(list != null)
            {
                result = list.Select(ai => new ReportAccount_DTO()
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
                    SponsorPhones = ai.Sponsor != null ? String.Join(" - ", ai.Sponsor.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
                    LeftBower = ai.LeftBower
                }).ToList();
            }
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
                //MainAddress = ai.Account != null ? ai.Account.Addresses
                //                                         .Where(a => a.AddressTypeID == 1)
                //                                         .Select(a => a.Street + " - " + a.Address1 + " - " + a.County + " - " + a.City + " - " + a.State).FirstOrDefault()
                //                                 : "",


                Street = ai.Account != null ? ai.Account.Addresses
                                                         .Where(a => a.AddressTypeID == 1)
                                                         .Select(a => a.Street).FirstOrDefault()
                                                 : "",

                Address1 = ai.Account != null ? ai.Account.Addresses
                                                         .Where(a => a.AddressTypeID == 1)
                                                         .Select(a => a.Address1).FirstOrDefault()
                                                 : "",


                County = ai.Account != null ? ai.Account.Addresses
                                                         .Where(a => a.AddressTypeID == 1)
                                                         .Select(a =>  a.County ).FirstOrDefault()
                                                 : "",


                City = ai.Account != null ? ai.Account.Addresses
                                                         .Where(a => a.AddressTypeID == 1)
                                                         .Select(a =>  a.City).FirstOrDefault()
                                                 : "",

                State = ai.Account != null ? ai.Account.Addresses
                                                         .Where(a => a.AddressTypeID == 1)
                                                         .Select(a => a.State).FirstOrDefault()
                                                 : "",

                PostalCode = ai.PostalCode != null && ai.PostalCode.Length == 5 ? ai.PostalCode.Substring(0, 5) + "-" + ai.PostalCode.Substring(5) : "",
                PaidAsCurrentMonth = ai.PaidAsCurrentMonth_Des,
                //PCV = ai.PCV,
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

        public static ReportAccount_DTO ToReportAccount_DTO(this AccountsInformation_MongoWithAccountAndSponsor item, string country)
        {
            DateTime dateTime = DateTime.UtcNow;
            var hrBrasilia = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            var date = TimeZoneInfo.ConvertTimeFromUtc(dateTime, hrBrasilia);

            var nombreCompleto = string.Empty;
            var nombreCompletoLider = string.Empty;
            var nombreCompletoLiderM3 = string.Empty;
            var nombreCompletoSponsor = string.Empty;


            if (item.Account != null)
            {
                var apellidos = (item.Account.LastName.ToLower().Trim()).Split(" ");
                nombreCompleto = item.Account.FirstName.ToLower().Trim().Split(" ")[0]; 

                if (country == "BRA")
                    nombreCompleto += " " + (apellidos.Length > 0 ? apellidos[apellidos.Length - 1] : " ").ToLower();
                else if (country == "USA")
                    nombreCompleto += " " + (apellidos.Length != 0 ? apellidos[0] : " ").ToLower();
            }

            if (item.Leader0 != null)
            {
                var apellidosLeader = (item.Leader0.LastName.ToLower().Trim()).Split(" ");
                nombreCompletoLider = item.Leader0.FirstName.ToLower().Trim().Split(" ")[0];

                if (country == "BRA")
                    nombreCompletoLider += " " + (apellidosLeader.Length > 0 ? apellidosLeader[apellidosLeader.Length - 1] : " ").ToLower();
                else if (country == "USA")
                    nombreCompletoLider += " " + (apellidosLeader.Length != 0 ? apellidosLeader[0] : " ").ToLower();
            }

            if (item.LeaderM3 != null)
            {
                var apellidosLeaderM3 = (item.LeaderM3.LastName.ToLower().Trim()).Split(" ");
                nombreCompletoLiderM3 = item.LeaderM3.FirstName.ToLower().Trim().Split(" ")[0];

                if (country == "BRA")
                    nombreCompletoLiderM3 += " " + (apellidosLeaderM3.Length > 0 ? apellidosLeaderM3[apellidosLeaderM3.Length - 1] : " ").ToLower();
                else if (country == "USA")
                    nombreCompletoLiderM3 += " " + (apellidosLeaderM3.Length != 0 ? apellidosLeaderM3[0] : " ").ToLower();
            }

            if (item.Sponsor != null)
            {
                var apellidosSponsor = (item.Sponsor.LastName.ToLower().Trim()).Split(" ");
                nombreCompletoSponsor = item.Sponsor.FirstName.ToLower().Trim().Split(" ")[0];

                if (country == "BRA")
                    nombreCompletoSponsor += " " + (apellidosSponsor.Length > 0 ? apellidosSponsor[apellidosSponsor.Length - 1] : " ").ToLower();
                else if (country == "USA")
                    nombreCompletoSponsor += " " + (apellidosSponsor.Length != 0 ? apellidosSponsor[0] : " ").ToLower();
            }

            var result = new ReportAccount_DTO()
            {
                AccountID = item.AccountID,
                AccountName = nombreCompleto,
                AccountNumber = item.AccountNumber,
                Activity = item.Activity,
                CareerTitleDes = item.CareerTitle_Des,
                CareerTitle = item.CareerTitle,
                DQV = item.DQV,
                DQVT = item.DQVT,
                CQL = item.CQL,
                EmailAddress = item.EmailAddress,
                Generation = Math.Abs((decimal)item.Generation),
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
                SponsorName = nombreCompletoSponsor,
                SponsorPhones = item.Sponsor != null ? String.Join(" - ", item.Sponsor.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
                ActiveDownline = item.ActiveDownline != null ? item.ActiveDownline : 0,
                ConsultActive = item.ConsultActive != null ? item.ConsultActive : 0,
                Birthday = item.Account.BirthdayUTC.HasValue ? item.Account.BirthdayUTC.Value.ToString("dd/MM/yyyy") : "",
                IsBirthday = item.Account.BirthdayUTC.HasValue && item.Account.BirthdayUTC.Value.Month == date.Month && item.Account.BirthdayUTC.Value.Day == date.Day ? true : false,
                NCWP = item.NCWP,
                AdditionalTitularName = item.Account.AccountAdditionalTitulars.Count > 0 ? item.Account.AccountAdditionalTitulars[0].FirstName.ToLower() + " " + item.Account.AccountAdditionalTitulars[0].LastName.ToLower() : "",
                AdditionalTitularBirthday = item.Account.AccountAdditionalTitulars.Count > 0 ? item.Account.AccountAdditionalTitulars[0].Brithday.Value.ToString("dd/MM/yyyy") : "",

                UplineLeader0ID = item.Leader0 != null ? item.Leader0.AccountID : 0,
                UplineLeader0Name = nombreCompletoLider,
                UplineLeader0EmailAddress = item.Leader0 != null ? item.Leader0.EmailAddress : "",
                UplineLeader0Phones = item.Leader0 != null ? String.Join(" - ", item.Leader0.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",

                UplineLeaderM3ID = item.LeaderM3 != null ? item.LeaderM3.AccountID : 0,
                UplineLeaderM3Name = nombreCompletoLiderM3,
                UplineLeaderM3EmailAddress = item.LeaderM3 != null ? item.LeaderM3.EmailAddress : "",
                UplineLeaderM3Phones = item.LeaderM3 != null ? String.Join(" - ", item.LeaderM3.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",

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

        public static List<ReportAccount_DTO> ToTopologyList(this IEnumerable<AccountsInformation_MongoWithAccountAndSponsor> list, string country)
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
                FirstName = ai.Account.FirstName != null ? ai.Account.FirstName : "",
                LastName1 = ai.Account.LastName != null ? ai.Account.LastName : "",
                country = country,
            }).ToList();

            result.ForEach(a =>
            {
                if (a.FirstName != null && a.LastName1 != null)
                {
                    string[] nombres = (a.FirstName.ToLower().Trim()).Split(" ");
                    string[] apellidos = (a.LastName1.ToLower().Trim()).Split(" ");

                    a.FirstName = (nombres.Length != 0 ? nombres[0] : " ").ToLower();
                    a.FirstName2 = (nombres.Length > 1 ? nombres[1] : " ").ToLower();

                    if (a.country == "BRA")
                    {
                        a.LastName1 = (apellidos.Length > 0 ? apellidos[apellidos.Length - 1] : " ").ToLower();
                    }
                    else if (a.country == "USA")
                    {
                        a.LastName1 = (apellidos.Length != 0 ? apellidos[0] : " ").ToLower();
                    }
                }
            });
            return result;
        }

        public static IEnumerable<ReportAccountPerformance_DTO> ToReportAccountPerformance_DTO(this IEnumerable<AccountsInformation_Mongo> list)
        {
            var result = list.Select(item => new ReportAccountPerformance_DTO()
            {
                AccountID = item.AccountID,
                AccountNumber = item.AccountNumber,
                AccountName = item.AccountName,
                CareerTitle = item.CareerTitle,
                CareerTitleDes = item.CareerTitle_Des,
                PaidAsCurrentMonth = item.PaidAsCurrentMonth,
                PaidAsCurrentMonthDesc = item.PaidAsCurrentMonth_Des,
                PQV = item.PQV,
                PCV = item.PCV,
                DQVT = item.DQVT,
                DQV = item.DQV,
                CQL = item.CQL,
                LEVEL = item.LEVEL,
                Title1Legs = item.Title1Legs,
                Title2Legs = item.Title2Legs,
                Title3Legs = item.Title3Legs,
                Title4Legs = item.Title4Legs,
                Title5Legs = item.Title5Legs,
                Title6Legs = item.Title6Legs,
                Title7Legs = item.Title7Legs,
                Title8Legs = item.Title8Legs,
                Title9Legs = item.Title9Legs,
                Title10Legs = item.Title10Legs,
                Title11Legs = item.Title11Legs,
                Title12Legs = item.Title12Legs,
                Title13Legs = item.Title13Legs,
                Title14Legs = item.Title14Legs,
            }).OrderByDescending(x => x.DQVT);

            return result;
        }

        public static ReportAccountPerformance_DTO ToReportAccountPerformance_DTO(this AccountsInformationPerformance_Mongo item, string country)
        {
            var nombreCompleto = string.Empty;
         


            if (item.Account != null)
            {
                var apellidos = (item.Account.LastName.ToLower().Trim()).Split(" ");
                nombreCompleto = item.Account.FirstName.ToLower().Trim().Split(" ")[0];

                if (country == "BRA")
                    nombreCompleto += " " + (apellidos.Length > 0 ? apellidos[apellidos.Length - 1] : " ").ToLower();
                else if (country == "USA")
                    nombreCompleto += " " + (apellidos.Length != 0 ? apellidos[0] : " ").ToLower();
            }

            var result = new ReportAccountPerformance_DTO()
            {
                AccountID = item.AccountID,
                AccountNumber = item.AccountNumber,
                AccountName = nombreCompleto,
                CareerTitle = item.CareerTitle,
                CareerTitleDes = item.CareerTitle_Des,
                PaidAsCurrentMonth = item.PaidAsCurrentMonth,
                PaidAsCurrentMonthDesc = item.PaidAsCurrentMonth_Des,
                PQV = item.PQV,
                PCV = item.PCV,
                DQVT = item.DQVT,
                DQV = item.DQV,
                CQL = item.CQL,

                Title1Legs = item.Title1Legs,
                Title2Legs = item.Title2Legs,
                Title3Legs = item.Title3Legs,
                Title4Legs = item.Title4Legs,
                Title5Legs = item.Title5Legs,
                Title6Legs = item.Title6Legs,
                Title7Legs = item.Title7Legs,
                Title8Legs = item.Title8Legs,
                Title9Legs = item.Title9Legs,
                Title10Legs = item.Title10Legs,
                Title11Legs = item.Title11Legs,
                Title12Legs = item.Title12Legs,
                Title13Legs = item.Title13Legs,
                Title14Legs = item.Title14Legs,
            };

            return result;
        }

        public static List<ExportBirthDayAccount_DTO> ToExportBirthday(this List<BirthDayAccount_DTO> list)
        {
            var result = list.Select(ai => new ExportBirthDayAccount_DTO()
            {
                AccountID = ai.AccountID,
                AccountName = ai.AccountName.ToLower(),
                CareerTitleDes = ai.CareerTitle_Des,
                EmailAddress = ai.EmailAddress,
                Birthday = ai.HB,
            }).ToList();

            return result;
        }

        public static List<BirthDayAccount_DTO> Birthday(this List<BirthDayAccount_DTO> list)
        {
            DateTime dateTime = DateTime.Now;

            var result = list.Select(item => new BirthDayAccount_DTO()
            {
                isBirthDay = item.BirthdayUTC.HasValue && item.BirthdayUTC.Value.Month == dateTime.Month && item.BirthdayUTC.Value.Day == dateTime.Day ? true : false,
                BirthdayUTC = item.BirthdayUTC,
                AccountID = item.AccountID,
                AccountName = item.AccountName,
                HB = item.HB,
                AccountNumber = item.AccountNumber,
                AccountsInformationID = item.AccountsInformationID,
                ActiveDownline = item.ActiveDownline,
                Activity = item.Activity,
                Address = item.Address,
                CareerTitle = item.CareerTitle,
                CareerTitle_Des = item.CareerTitle_Des,
                City = item.City,
                ConsultActive = item.ConsultActive,
                CQL = item.CQL,
                CreditAvailable = item.CreditAvailable,
                DCV = item.DCV,
                DebtsToExpire = item.DebtsToExpire,
                DQV = item.DQV,
                DQVT = item.DQVT,
                EmailAddress = item.EmailAddress,
                ExpiredDebts = item.ExpiredDebts,
                GCV = item.GCV,
                Generation = item.Generation,
                GenerationM3 = item.GenerationM3,
                GQV = item.GQV,
                IsCommissionQualified = item.IsCommissionQualified,
                JoinDate = item.JoinDate,
                LastOrderDate = item.LastOrderDate,
                LeftBower = item.LeftBower,
                LEVEL = item.LEVEL,
                PaidAsCurrentMonth = item.PaidAsCurrentMonth,
                PaidAsCurrentMonth_Des = item.PaidAsCurrentMonth_Des,
                NewStatus = item.NewStatus,
                PaidAsLastMonth = item.PaidAsLastMonth,
                PCV = item.PCV,
                PeriodID = item.PeriodID,
                PQV = item.PQV,
                PostalCode = item.PostalCode,
                Region = item.Region,
                RightBower = item.RightBower,
                SortPath = item.SortPath,
                SponsorID = item.SponsorID,
                SponsorName = item.SponsorName,
                STATE = item.STATE,
                TotalDownline = item.TotalDownline,
                VolumeForCareerTitle = item.VolumeForCareerTitle,
                Anios = item.Anios,
                Phones = item.Phones
            }).ToList();

            return result;
        }

    }
}
