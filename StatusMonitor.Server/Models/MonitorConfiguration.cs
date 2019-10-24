using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApplicationStatusMonitor.Controllers
{
  public class MonitorConfiguration
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string MonitorName { get; set; }

    public string Value { get; set; }

    public string Type { get; set; }

    public bool Active { get; set; }
  }
}