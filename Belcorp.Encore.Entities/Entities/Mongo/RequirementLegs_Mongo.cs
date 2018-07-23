using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class RequirementLegs_Mongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public ObjectId RequirementLegs { get; set; }

        public int TitleID { get; set; }
        public int PlanID { get; set; }
        public int TitleRequired { get; set; }
        public int Generation { get; set; }
        public int Level { get; set; }
        public decimal TitleQty { get; set; }
    }
}
