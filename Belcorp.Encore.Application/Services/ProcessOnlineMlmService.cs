using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories;
using Belcorp.Encore.Repositories.Interfaces;
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
        private readonly IProcessOnlineRepository _processOnlineRepository;
        private readonly EncoreMongo_Context _encoreMongo_Context;

        private List<CalculationTypes> calculationTypes;

        public ProcessOnlineMlmService(IUnitOfWork<EncoreCore_Context> unitOfWork_Core, IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm, IProcessOnlineRepository processOnlineRepository)
        {
            _unitOfWork_Core = unitOfWork_Core;
            _unitOfWork_Comm = unitOfWork_Comm;
            _processOnlineRepository = processOnlineRepository;
            calculationTypes = GetCalculationTypesByCode();
        }

        public void ProcessMLM(int orderId, int accountId, int periodId)
        {
            IRepository<Orders> ordersRepository = _unitOfWork_Core.GetRepository<Orders>();
            var order = ordersRepository.GetFirstOrDefault(o => o.OrderID == orderId, null, null, true);

            var accounts = GetSortPathByAccount(accountId);
            GetListAccounts_InitializesKPIsInGroup(accounts.Select(a => a.AccountID).ToList(), calculationTypes.Select(c => c.CalculationTypeID).ToList(), periodId);
        }

        public List<CalculationTypes> GetCalculationTypesByCode()
        {
            List<string> codigos = new List<string>{"PQV", "PRV", "PCV", "GQV", "GCV", "DQV", "DCV", "CQL" };

            IRepository<CalculationTypes> calculationTypesRepository = _unitOfWork_Comm.GetRepository<CalculationTypes>();
            var result = calculationTypesRepository.GetPagedList(c => codigos.Contains(c.Code), null, null, 0, codigos.Count, true);
            return result == null ? null : result.Items.ToList();
        }

        public List<Accounts> GetSortPathByAccount(int accountId)
        {
            IRepository<Accounts> accountsRepository = _unitOfWork_Comm.GetRepository<Accounts>();
            var accounts = accountsRepository.FromSql($"SELECT * FROM fnGetAccountsInGroup_Aux ({accountId})").ToList();

            return accounts;
        }

        #region Calculos por Grupo

        public void InitializesKPIsInGroup(int accountId, int periodId)
        {
            var result = GetSortPathByAccount(accountId);
            IRepository<AccountKPIs> accountsKpisRepository = _unitOfWork_Comm.GetRepository<AccountKPIs>();
        }

        public List<AccountKPIs> GetListAccounts_InitializesKPIsInGroup(List<int> accounts, List<int> calculationTypes, int periodId)
        {
            return _processOnlineRepository.GetListAccounts_InitializesKPIsInGroup(accounts, calculationTypes, periodId);
        }

        #endregion
    }
}
