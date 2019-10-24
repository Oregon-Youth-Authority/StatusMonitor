using ApplicationStatusMonitor.Controllers;
using MongoDB.Driver;

namespace ApplicationStatusMonitor
{
   public class MonitorConfigurationMongoRepository<T> : IMonitorConfigurationRepository<T>
   {
      private IMongoCollection<T> _configs;

      public MonitorConfigurationMongoRepository(IStatusMonitorDatabaseSettings settings)
      {
         var client = new MongoClient(settings.ConnectionString);
         var database = client.GetDatabase(settings.DatabaseName);
         _configs = database.GetCollection<T>(settings.MonitorConfigsCollection);
      }

      #region Implementation of IMonitorConfigurationRepository<T>

      public T GetMonitorConfiguration(string monitorName)
      {
         var filter = Builders<T>.Filter.Eq("MonitorName", monitorName);
         var record = _configs.Find(filter).Limit(1).FirstOrDefault();
         return record;
      }

      #endregion
   }
}