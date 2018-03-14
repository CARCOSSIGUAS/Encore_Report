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


        public ProcessOnlineMlmService(IUnitOfWork<EncoreCore_Context> unitOfWork_Core, IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm)
        {
            _unitOfWork_Core = unitOfWork_Core;
            _unitOfWork_Comm = unitOfWork_Comm;
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
