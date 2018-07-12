using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class PersonalIndicatorDetailLogService : IPersonalIndicatorDetailLogService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        public PersonalIndicatorDetailLogService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm)
        {
            unitOfWork_Comm = _unitOfWork_Comm;
        }

        public PersonalIndicatorDetailLog Get(PersonalIndicatorDetailLog personalIndicatorDetailLog)
        {
            IRepository<PersonalIndicatorDetailLog> personalIndicatorDetailLogRepository = unitOfWork_Comm.GetRepository<PersonalIndicatorDetailLog>();
            return personalIndicatorDetailLogRepository.GetFirstOrDefault(ld => ld.PersonalIndicatorLogID == personalIndicatorDetailLog.PersonalIndicatorLogID && ld.CodeSubProcess == personalIndicatorDetailLog.CodeSubProcess, null, null, false);
        }

        public PersonalIndicatorDetailLog Insert(PersonalIndicatorDetailLog personalIndicatorDetailLog)
        {
            IRepository<PersonalIndicatorDetailLog> personalIndicatorDetailLogRepository = unitOfWork_Comm.GetRepository<PersonalIndicatorDetailLog>();

            var result = Get(personalIndicatorDetailLog);
            if (result != null)
            {
                return result;
            }
            else
            {
                personalIndicatorDetailLog.TermName = personalIndicatorDetailLog.CodeSubProcess;
                personalIndicatorDetailLog.StarTime = DateTime.Now;

                personalIndicatorDetailLogRepository.Insert(personalIndicatorDetailLog);
                unitOfWork_Comm.SaveChanges();
                return Get(personalIndicatorDetailLog);
            }
        }

        public PersonalIndicatorDetailLog Update(PersonalIndicatorDetailLog personalIndicatorDetailLog)
        {
            IRepository<PersonalIndicatorDetailLog> personalIndicatorDetailLogRepository = unitOfWork_Comm.GetRepository<PersonalIndicatorDetailLog>();
            if (personalIndicatorDetailLog != null)
            {
                personalIndicatorDetailLog.EndTime = DateTime.Now;

                personalIndicatorDetailLogRepository.Update(personalIndicatorDetailLog);
                unitOfWork_Comm.SaveChanges();
            }

            return Get(personalIndicatorDetailLog);
        }

        public PersonalIndicatorDetailLog Create(PersonalIndicatorLog personalIndicatorLog, string codeSubProcess)
        {
            var childLog = new PersonalIndicatorDetailLog()
            {
                PersonalIndicatorLogID = personalIndicatorLog.PersonalIndicatorLogID,
                CodeSubProcess = codeSubProcess,
                TermName = codeSubProcess
            };

            return childLog;
        }
    }
}
