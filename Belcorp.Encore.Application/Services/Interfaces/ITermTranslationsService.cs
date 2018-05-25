using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface ITermTranslationsService
    {
        List<TermTranslations_Mongo> GetLanguageTerm(int LanguageID, string TermName);
       IDictionary<string, string> GetLanguage(int LanguageID);
    }
}
