
using System.Collections.Generic;
using System.Threading;
using ApplicationStatusMonitor.Controllers;

using MongoDB.Driver;

namespace ApplicationStatusMonitor
{
  public class MonitorConfigurationMongoRepository<T> : IMonitorConfigurationRepository<T> where T : MonitorConfiguration
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
      var filter = Builders<T>.Filter.Eq(e=>e.MonitorName, monitorName);
      var record = _configs.Find(filter).Limit(1).FirstOrDefault();
      return record;
    }

    public IEnumerable<T> GetAll()
    {
      return _configs.Find(Builders<T>.Filter.Empty).ToList();
    }

    public IEnumerable<T> GetByType(string type)
    {
      return _configs.Find(Builders<T>.Filter.Eq(e=>e.Type, type)).ToList();
    }

    #endregion
  }
}