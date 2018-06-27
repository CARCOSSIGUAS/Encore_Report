﻿using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Belcorp.Encore.Application.Services
{
    public class TermTranslationsService : ITermTranslationsService
    {
        private readonly EncoreMongo_Context encoreMongo_Context;

        public TermTranslationsService
        (
            IConfiguration configuration
        )
        {
            encoreMongo_Context = new EncoreMongo_Context(configuration);
        }

        public string GetLanguageTerm(string languageCode, string termName, string country)
        {
            IMongoCollection<TermTranslations_Mongo> termTranslationsCollection = encoreMongo_Context.TermTranslationsProvider(country);
            var result = termTranslationsCollection.Find(a => a.LanguageCode == languageCode && a.TermName == termName).FirstOrDefault();

            if (result == null || result.Term == null)
                return termName;

            return result.Term;
        }

        public Dictionary<string, IDictionary<string, IDictionary<string, string>>> GetLanguage(int languageID, string country)
        {
            IMongoCollection<TermTranslations_Mongo> termTranslationsCollection = encoreMongo_Context.TermTranslationsProvider(country);

            Dictionary<string, IDictionary<string, IDictionary<string, string>>> termTranslations_DTO = new Dictionary<string, IDictionary<string, IDictionary<string, string>>>();
            Dictionary<string, string> result = new Dictionary<string, string>();

            var filter = Builders<TermTranslations_Mongo>.Filter.Empty;

            var lng = termTranslationsCollection.Find(filter).ToList();
            var languageType = lng.GroupBy(x => x.LanguageCode).Select(x => x.Key).ToList();

            foreach (var item in languageType)
            {
                result = lng.FindAll(a => a.LanguageCode == item).GroupBy(a => a.TermName).Select(a => a.First()).ToDictionary(a => a.TermName, a => a.Term);
                termTranslations_DTO.Add(item, new Dictionary<string, IDictionary<string, string>> { { "translations", result } });
            }  
            
            return termTranslations_DTO;
        }
    }
}
