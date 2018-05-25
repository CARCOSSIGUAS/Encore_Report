using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Belcorp.Encore.Application.Services
{
    public class TermTranslationsService : ITermTranslationsService
    {
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAuthenticationService authenticationService;

        public TermTranslationsService
        (
            IAuthenticationService _authenticationService,
            IOptions<Settings> settings
        )
        {
            encoreMongo_Context = new EncoreMongo_Context(settings);
            authenticationService = _authenticationService;
        }

        public List<TermTranslations_Mongo> GetLanguageTerm(int LanguageID, string TermName)
        {
            var result = encoreMongo_Context.TermTranslationsProvider.Find(a => a.LanguageID == LanguageID && a.TermName == TermName).ToList();
            return result;
        }

        public IDictionary<string, string> GetLanguage(int LanguageID)
        {
            var projection = Builders<term_DTo>.Projection.Include("TermName").Include("Term");

            var result = encoreMongo_Context.TermTranslationsProvider.Find(a => a.LanguageID == LanguageID)
                .ToList()
                .Select(t => new term_DTo()
                {
                    term = t.Term,
                    termName = t.TermName
                }
                    )
                .ToDictionary(a => a.termName, a => a.term);

            return result;
        }
    }


    public class term_DTo
    {
        public string termName { get; set; }
        public string term { get; set; }
    }
}
