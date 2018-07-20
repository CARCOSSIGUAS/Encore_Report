using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class PerformanceIndicatorDay_Mongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public ObjectId PerformanceIndicatorDayID { get; set; }
        public int AccountID { get; set; }
        public int DayOfMonth { get; set; }
        public int Ingresos { get; set; }
        public int PeriodID { get; set; }
    }
}
