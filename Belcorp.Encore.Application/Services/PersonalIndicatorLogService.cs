using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Belcorp.Encore.Application.Services
{
    public class PersonalIndicatorLogService : IPersonalIndicatorLogService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        public PersonalIndicatorLogService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm)
        {
            unitOfWork_Comm = _unitOfWork_Comm;
        }

        public PersonalIndicatorLog GetByOrderId(int orderID)
        {
            IRepository<PersonalIndicatorLog> personalIndicatorLogRepository = unitOfWork_Comm.GetRepository<PersonalIndicatorLog>();
            return personalIndicatorLogRepository.GetFirstOrDefault(l => l.OrderID == orderID, null, null, false);
        }

        public PersonalIndicatorLog Insert(PersonalIndicatorLog personalIndicatorLog)
        {
            IRepository<PersonalIndicatorLog> personalIndicatorLogRepository = unitOfWork_Comm.GetRepository<PersonalIndicatorLog>();
            var result = GetByOrderId((int)personalIndicatorLog.OrderID);
            if (result != null)
            {
                return result;
            }
            else
            {
                var log = new PersonalIndicatorLog()
                {
                    OrderID = personalIndicatorLog.OrderID,
                    OrderStatusID = personalIndicatorLog.OrderStatusID,
                    TermName = "MainProcessPersonalIndicator",
                    StarTime = DateTime.Now
                };

                personalIndicatorLogRepository.Insert(log);
                unitOfWork_Comm.SaveChanges();
                return GetByOrderId((int)personalIndicatorLog.OrderID);
            }
        }

        public PersonalIndicatorLog Update(PersonalIndicatorLog personalIndicatorLog)
        {
            IRepository<PersonalIndicatorLog> personalIndicatorLogRepository = unitOfWork_Comm.GetRepository<PersonalIndicatorLog>();
            if (personalIndicatorLog != null)
            {
                personalIndicatorLog.EndTime = DateTime.Now;
                personalIndicatorLogRepository.Update(personalIndicatorLog);
                unitOfWork_Comm.SaveChanges();
            }

            return GetByOrderId((int)personalIndicatorLog.OrderID);
        }
    }
}
