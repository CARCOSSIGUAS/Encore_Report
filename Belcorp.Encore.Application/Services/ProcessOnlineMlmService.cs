using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class ProcessOnlineMlmService : IProcessOnlineMlmService
    {
        private readonly IUnitOfWork<EncoreCore_Context> _unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm;
        private readonly EncoreMongo_Context encoreMongo_Context;
		private readonly IRepository<AccountKPIs> _accountKPIsRepository;


		public ProcessOnlineMlmService(IUnitOfWork<EncoreCore_Context> unitOfWork_Core, IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm,IRepository<AccountKPIs> accountKPIsRepository)
        {
            _unitOfWork_Core = unitOfWork_Core;
            _unitOfWork_Comm = unitOfWork_Comm;
			_accountKPIsRepository = accountKPIsRepository;
		}


		public void ProcessMLM(int orderId)
        {
            int codePQV = GetCalculationTypesByCode("PQV").CalculationTypeID;
            int codePRV = GetCalculationTypesByCode("PRV").CalculationTypeID;
            int codePCV = GetCalculationTypesByCode("PCV").CalculationTypeID;
            int codeGQV = GetCalculationTypesByCode("GQV").CalculationTypeID;
            int codeGCV = GetCalculationTypesByCode("GCV").CalculationTypeID;
            int codeDQV = GetCalculationTypesByCode("DQV").CalculationTypeID;
            int codeDCV = GetCalculationTypesByCode("DCV").CalculationTypeID;
            int codeCQL = GetCalculationTypesByCode("CQL").CalculationTypeID;

            IRepository<Orders> ordersRepository = _unitOfWork_Core.GetRepository<Orders>();
            var order = ordersRepository.GetFirstOrDefault();

        }


		public void UpdateOnlineGroupKPIsForReports(int accountID,int periodID,decimal QV,decimal CV)
		{
			IRepository<AccountsInformation> accountsInformationRepository = _unitOfWork_Comm.GetRepository<AccountsInformation>();

			var listAccounts = GetSortPathByAccount(accountID);

			var idAccounts = listAccounts.Select(x => x.AccountID).ToList();

			var accountsFiltered= accountsInformationRepository.GetPagedList(x => idAccounts.Contains(x.AccountID) && x.PeriodID==periodID,null,null,0,idAccounts.Count,true).Items.ToList();

			using (_unitOfWork_Comm)
			{
				foreach (var item in accountsFiltered)
				{
					item.GQV = item.GQV + QV;
					item.GCV = item.GCV + CV;
					accountsInformationRepository.Update(item);
				}
				
			}
			_unitOfWork_Comm.SaveChanges();
		}


		public void PersonalKPIs(int orderID, int statusOrderID, int accountID, int periodID, decimal CV, decimal QV, decimal RP)
		{
			DateTime startTime;
			string Result;
			DateTime dateModified;

			startTime = DateTime.Now;
			dateModified = DateTime.Now;

			int codePQV = GetCalculationTypesByCode("PQV").CalculationTypeID;
			int codePRV = GetCalculationTypesByCode("PRV").CalculationTypeID;
			int codePCV = GetCalculationTypesByCode("PCV").CalculationTypeID;

			int nPQV = CountAccountsKPIs(accountID, periodID, codePQV);
			int nPRV = CountAccountsKPIs(accountID, periodID, codePRV);
			int nPCV = CountAccountsKPIs(accountID, periodID, codePCV);

			using (_unitOfWork_Comm)
			{
				if (nPQV > 0)
				{
					UpdateAccountsKPIs(QV, dateModified, accountID, periodID, codePQV);
				}
				else
				{
					_accountKPIsRepository.Insert(new AccountKPIs { AccountID = accountID, PeriodID = periodID, CalculationTypeID = codePCV, Value = CV, DateModified = dateModified });
				}

				if (nPRV > 0)
				{
					UpdateAccountsKPIs(QV, dateModified, accountID, periodID, codePRV);

				}
				else
				{
					_accountKPIsRepository.Insert(new AccountKPIs { AccountID = accountID, PeriodID = periodID, CalculationTypeID = codePRV, Value = RP, DateModified = dateModified });
				}

				if (nPCV > 0)
				{
					UpdateAccountsKPIs(QV, dateModified, accountID, periodID, codePCV);

				}
				else
				{
					_accountKPIsRepository.Insert(new AccountKPIs { AccountID = accountID, PeriodID = periodID, CalculationTypeID = codePCV, Value = CV, DateModified = dateModified });

				}
			}		
			_unitOfWork_Comm.SaveChanges();

		}

		public void UpdateAccountsKPIs(decimal paramSUM, DateTime dateModified , int accountId, int periodId, int codeGeneric) {

			var account = _accountKPIsRepository.GetFirstOrDefault(x => x.AccountID == accountId && x.PeriodID == periodId && x.CalculationTypeID == codeGeneric , null, null, true);
			
			account.Value = account.Value + paramSUM;
			account.DateModified = dateModified;

			_accountKPIsRepository.Update(account);

		}

		public int CountAccountsKPIs(int accountID, int periodID,int codeGeneric) {

			int n = _accountKPIsRepository.Count(x => x.AccountID == accountID && x.PeriodID == periodID && x.CalculationTypeID == codeGeneric);

			return n;
		}

		public CalculationTypes GetCalculationTypesByCode(string code)
        {
            IRepository<CalculationTypes> calculationTypesRepository = _unitOfWork_Comm.GetRepository<CalculationTypes>();
            return calculationTypesRepository.GetFirstOrDefault(c => c.Code == code, null, null, true);
        }

        public List<Accounts> GetSortPathByAccount(int accountId)
        {
            IRepository<Accounts> accountsRepository = _unitOfWork_Comm.GetRepository<Accounts>();
            var accounts = accountsRepository.FromSql($"SELECT * FROM fnGetAccountsInGroup_Aux ({accountId})").ToList();

            return accounts;
        }

    }
}
