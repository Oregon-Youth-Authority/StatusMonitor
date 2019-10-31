using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Controllers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApplicationStatusMonitor
{
   public class StatusMonitorMongoRepository<T> : IStatusRepository<T> where T : StatusMonitorReply
   {
      private readonly IMongoCollection<T> _replies;
      public StatusMonitorMongoRepository(IStatusMonitorDatabaseSettings settings)
      {
         var client = new MongoClient(settings.ConnectionString);
         var database = client.GetDatabase(settings.DatabaseName);
         _replies = database.GetCollection<T>(settings.StatusMonitorRepliesCollection);
      }

      #region Implementation of IStatusRepository<T>

      public T AddStatusRecord(T record)
      {
         _replies.InsertOne(record);

         return record;
      }

      public T GetLatestStatusRecord(string location, string monitorName)
      {
         var filter = Builders<T>.Filter.Eq(e => e.LocationId, location) & Builders<T>.Filter.Eq(e=> e.MonitorName, monitorName);
         var sort = Builders<T>.Sort.Descending(e => e.StatusStartTime);
         var record = _replies.Find(filter).Sort(sort).Limit(1).FirstOrDefault();
         return record;
      }

      public T Update(string location, string monitorName, DateTime lastStatusUpdateTime)
      {
         var filter = Builders<T>.Filter.Eq(e => e.LocationId, location) & Builders<T>.Filter.Eq(e => e.MonitorName, monitorName);
         var sort = Builders<T>.Sort.Descending(e => e.StatusStartTime);

         var setLastUpdate = Builders<T>.Update.Set(e => e.LastStatusUpdateTime, lastStatusUpdateTime);
         return _replies.FindOneAndUpdate(filter, setLastUpdate, new FindOneAndUpdateOptions<T> {Sort = sort});
      }

      public IEnumerable<T> GetLatestStatusRecordForEachLocation()
      {
         // get a list of the latest status replies for monitor, location, and name
         var pipeline = PipelineDefinition<T,T>.Create(
            "{ $sort: { LastStatusUpdateTime: -1 } }",
            "{ $addFields: { LocationAndMonitor: {$concat: [\"$MonitorName\",\"-\",\"$LocationId\",\"-\", \"$DisplayName\"]} } }",
            "{ $group: { \"_id\": \"$LocationAndMonitor\", \"latest\": { $first: \"$$ROOT\" } } }",
            "{ $replaceRoot: { newRoot: \"$latest\" } }",
            "{ $project: {\"LocationAndMonitor\": 0} }"
            );

         var results = _replies.Aggregate(pipeline);
         return results.ToList();
      }

      #endregion
   }

   public class MonitorReplySummary
   {
      public string _id {get;set;}
      public string latest {get;set;}
   }
}
