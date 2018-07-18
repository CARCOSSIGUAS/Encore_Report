using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class TransactionMonitor_Mongo
    {
        [BsonId]
        public int TransactionMonitorID { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TransactionDate { get; set; }
    }
}
