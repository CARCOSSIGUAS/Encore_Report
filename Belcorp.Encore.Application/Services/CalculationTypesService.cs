using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class CalculationTypesService : ICalculationTypesService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        public CalculationTypesService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm)
        {
            unitOfWork_Comm = _unitOfWork_Comm;
        }

        public List<CalculationTypes> GetCalculationTypesByCodes(List<string> codes)
        {
            IRepository<CalculationTypes> calculationTypesRepository = unitOfWork_Comm.GetRepository<CalculationTypes>();
            var result = calculationTypesRepository.GetPagedList(c => codes.Contains(c.Code), null, null, 0, codes.Count, true);
            return result == null ? null : result.Items.ToList();
        }

        public int GetCalculationTypeIdByCode(string code)
        {
            IRepository<CalculationTypes> calculationTypesRepository = unitOfWork_Comm.GetRepository<CalculationTypes>();
            var result = calculationTypesRepository.GetFirstOrDefault(c => c.Code == code, null, null, true);
            if (result != null)
            {
                return result.CalculationTypeID;
            }

            return 0;
        }
    }
}
