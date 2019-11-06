using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Abstractions;
using ApplicationStatusMonitor.Controllers;
using MongoDB.Driver;

namespace ApplicationStatusMonitor.Implementations
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

      public T GetLatestStatusRecord(string location, string monitorName, string displayName)
      {
         var filter = Builders<T>.Filter.Eq(e => e.LocationId, location) &
                      Builders<T>.Filter.Eq(e => e.MonitorName, monitorName) &
                      Builders<T>.Filter.Eq(e => e.DisplayName, displayName);
         var sort = Builders<T>.Sort.Descending(e => e.StatusStartTime);
         var record = _replies.Find(filter).Sort(sort).Limit(1).FirstOrDefault();
         return record;
      }

      public T Update(string location, string monitorName, string displayName, DateTime lastStatusUpdateTime)
      {
         var filter = Builders<T>.Filter.Eq(e => e.LocationId, location) &
                      Builders<T>.Filter.Eq(e => e.MonitorName, monitorName) &
                      Builders<T>.Filter.Eq(e => e.DisplayName, displayName);

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
            "{ $sort: { LocationId: 1, MonitorName: 1, DisplayName: 1 } }",
            "{ $project: {\"LocationAndMonitor\": 0} }"
            );

         var results = _replies.Aggregate(pipeline);
         return results.ToList();
      }

      public IEnumerable<T> GetCurrentlyDown()
      {
         // get a list of the latest status replies for monitor, location, and name
         var pipeline = PipelineDefinition<T, T>.Create(
            "{ $sort: { LastStatusUpdateTime: -1 } }",
            "{ $addFields: { LocationAndMonitor: {$concat: [\"$MonitorName\",\"-\",\"$LocationId\",\"-\", \"$DisplayName\"]} } }",
            "{ $group: { \"_id\": \"$LocationAndMonitor\", \"latest\": { $first: \"$$ROOT\" } } }",
            "{ $replaceRoot: { newRoot: \"$latest\" } }",
            "{ $sort: { LocationId: 1, MonitorName: 1, DisplayName: 1 } }",
            "{ $project: {\"LocationAndMonitor\": 0} }",
            "{ $match : { Status: {$ne: \"Up\"} } }"
         );

         var results = _replies.Aggregate(pipeline);
         return results.ToList();
      }

      public async Task<IEnumerable<T>> GetStatusMonitorRepliesOlderThan(DateTime dateTime)
      {
         var filter = Builders<T>.Filter.Lt(e => e.LastStatusUpdateTime, dateTime);
         return (await _replies.FindAsync(filter)).ToList();
      }

      public async Task DeleteStatusRecords(IEnumerable<T> statusRecordsToDelete)
      {
         foreach (var statusMonitorReply in statusRecordsToDelete)
         {
            var filter = Builders<T>.Filter.Eq(e => e.Id, statusMonitorReply.Id);
            await _replies.FindOneAndDeleteAsync(filter);
         }
      }
      

      #endregion
   }
}
