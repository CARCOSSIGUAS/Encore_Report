using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class ProcessOnlineMlmService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EncoreMongo_Context encoreMongo_Context;


        public ProcessOnlineMlmService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public void ProcessMLM()
        {
            int codePQV = GetCalculationTypesByCode("PQV").CalculationTypeID;
            int codePRV = GetCalculationTypesByCode("PRV").CalculationTypeID;
            int codePCV = GetCalculationTypesByCode("PCV").CalculationTypeID;
            int codeGQV = GetCalculationTypesByCode("GQV").CalculationTypeID;
            int codeGCV = GetCalculationTypesByCode("GCV").CalculationTypeID;
            int codeDQV = GetCalculationTypesByCode("DQV").CalculationTypeID;
            int codeDCV = GetCalculationTypesByCode("DCV").CalculationTypeID;
            int codeCQL = GetCalculationTypesByCode("CQL").CalculationTypeID;



        }

        public CalculationTypes GetCalculationTypesByCode(string code)
        {
            IRepository<CalculationTypes> calculationTypesRepository = _unitOfWork.GetRepository<CalculationTypes>();
            return calculationTypesRepository.GetFirstOrDefault(c => c.Code == code, null, null, true);
        }
    }
}
