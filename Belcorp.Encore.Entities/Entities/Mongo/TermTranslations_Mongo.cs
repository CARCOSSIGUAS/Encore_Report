using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class TermTranslations_Mongo
    {
        //[BsonId]
        //public object id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        public int TermTranslationID { get; set; }
        public int LanguageID { get; set; }
        public string TermName { get; set; }
        public string Term { get; set; }
        public DateTime LastUpdatedUTC { get; set; }
        public bool? Active { get; set; }
    }
}
