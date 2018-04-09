﻿using Belcorp.Encore.Application.ViewModel;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.MetaData_Mongo;
using Belcorp.Encore.Services.Report.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services
{
    public interface IAccountInformationService
    {

        void Migrate_AccountInformationByPeriod(int periodId);

        Task<IEnumerable<ReportPerformance_HeaderModel>> GetPerformance_Header(int accountId, int periodId);

        Task<IEnumerable<ReportPerformance_DetailModel>> GetPerformance_Detail(int accountId, int periodId);

        IEnumerable<Accounts_DTO> GetPerformance_HeaderFront(int accountId);

        Task<List<AccountsInformation_DTO>> GetPerformance_AccountInformation(int accountId, int periodId);

		AccountsExtended GetAccounts(Filtros_DTO filtrosDTO);


	}

}
