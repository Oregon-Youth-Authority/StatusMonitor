using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApplicationStatusMonitor.Controllers
{
   public class StatusMonitorReply
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; }

      public string Status { get; set; }
      public DateTime StatusStartTime { get; set; }
      public string MonitorName { get; set; }
      public string LocationId { get; set; }
      public string DisplayName { get; set; }
   }
}