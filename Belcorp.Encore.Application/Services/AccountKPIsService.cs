using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Belcorp.Encore.Application.Services
{
    public class AccountKPIsService : IAccountKPIsService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        public AccountKPIsService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm)
        {
            unitOfWork_Comm = _unitOfWork_Comm;
        }

        public AccountKPIs GetAmountAccountKPI(int accountID, int periodID, int calculationType)
        { 
            IRepository<AccountKPIs> accountKPIsRepository = unitOfWork_Comm.GetRepository<AccountKPIs>();
            var result = accountKPIsRepository.GetFirstOrDefault(c => c.AccountID == accountID && c.PeriodID == periodID && c.CalculationTypeID == calculationType, null, null, true);
            if (result != null)
            {
                return result;
            }

            return result;
        }
    }
}
