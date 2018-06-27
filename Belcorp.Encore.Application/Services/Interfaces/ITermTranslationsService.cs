using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface ITermTranslationsService
    {
        string GetLanguageTerm(string languageCode, string termName, string country);
        Dictionary<string, IDictionary<string, IDictionary<string, string>>> GetLanguage(int languageID, string country);
    }
}
