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
    public class OrderCalculationTypesService : IOrderCalculationTypesService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        public OrderCalculationTypesService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm)
        {
            unitOfWork_Comm = _unitOfWork_Comm;
        }

        public List<OrderCalculationTypes> GetOrderCalculationTypesByCodes(List<string> codes)
        {
            IRepository<OrderCalculationTypes> orderCalculationTypesRepository = unitOfWork_Comm.GetRepository<OrderCalculationTypes>();
            var result = orderCalculationTypesRepository.GetPagedList(c => codes.Contains(c.Code), null, null, 0, codes.Count, true);
            return result == null ? null : result.Items.ToList();
        }

        public int GetOrderCalculationTypeIdByCode(string code)
        {
            IRepository<OrderCalculationTypes> orderCalculationTypesRepository = unitOfWork_Comm.GetRepository<OrderCalculationTypes>();
            var result = orderCalculationTypesRepository.GetFirstOrDefault(o => o.Code == code, null, null, true);
            if (result != null)
            {
                return result.OrderCalculationTypeID;
            }

            return 0;
        }
    }
}
