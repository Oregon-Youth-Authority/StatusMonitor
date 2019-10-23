using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Controllers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MongoDB.Driver;

namespace ApplicationStatusMonitor
{
   public class StatusMonitorMongoRepository<T> : IStatusRepository<T>
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
         var filter = Builders<T>.Filter.Eq("LocationId", location) & Builders<T>.Filter.Eq("MonitorName", monitorName);
         var sort = Builders<T>.Sort.Descending("StatusStartTime");
         var record = _replies.Find(filter).Sort(sort).Limit(1).FirstOrDefault();
         return record;
      }

      #endregion
   }
}
