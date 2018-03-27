using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities;

namespace Belcorp.Encore.Repositories
{
    public interface IAccountInformationRepository : IRepository<AccountsInformation>
    {
        IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId);
        IEnumerable<AccountsInformation_DTO> GetListAccountInformationByPeriodIdAndAccountId(int periodId, List<int> accountIds);
    }
}
