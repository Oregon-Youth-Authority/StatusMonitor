using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationStatusMonitor.Controllers
{
  [Route("StatusMonitor")]
  [ApiController]
  [Authorize]
  public class StatusMonitorController : ControllerBase
  {
    private const string forwardedHeader = "X-Forwarded-For";
    private IStatusRepository<StatusMonitorReply> _repliesRepo;
    private IMonitorConfigurationRepository<MonitorConfiguration> _configRepo;

    public StatusMonitorController(IStatusRepository<StatusMonitorReply> repliesRepo, IMonitorConfigurationRepository<MonitorConfiguration> configRepo)
    {
      _repliesRepo = repliesRepo;
      _configRepo = configRepo;
    }

    // spit out headers for testing
    public string Get()
    {
      try
      {
        var requestHeaders = String.Join("\n\t", Request.Headers.Select(h => $"{h.Key}: '{h.Value}'"));
        return $"\tRemoteIp: '{Request.HttpContext.Connection.RemoteIpAddress}'\n\t{requestHeaders}";
      }
      catch (Exception e)
      {
        return e.ToString();
      }
    }

    [HttpPost]
    [Route("/updateStatus")]
    public StatusMonitorReply UpdateStatus([FromBody]StatusMonitorRequest statusMonitorRequest)
    {
      // origin location of request, currently public IP
      var locationId = Request.Headers.Keys.Contains(forwardedHeader) ? Request.Headers[forwardedHeader].ToString() : $"No LocationId provided in {forwardedHeader} header";
      var lastUpdateTime = DateTime.Now;

      // get last status
      var recentStatus = _repliesRepo.GetLatestStatusRecord(locationId, statusMonitorRequest.MonitorName);

      // if status changed from previous status or there are no status records
      if (recentStatus == null || recentStatus.Status != statusMonitorRequest.Status)
      {
        // add new record for new status with start time of new status, application name, display name, and origin's public IP Address
        var reply = new StatusMonitorReply()
        {
          LocationId = locationId,
          Status = statusMonitorRequest.Status,
          MonitorName = statusMonitorRequest.MonitorName,
          DisplayName = statusMonitorRequest.DisplayName,
          StatusStartTime = lastUpdateTime,
          LastStatusUpdateTime = lastUpdateTime
        };

        _repliesRepo.AddStatusRecord(reply);

        return reply;
      }

      return _repliesRepo.Update(recentStatus.LocationId, recentStatus.MonitorName, DateTime.Now);
    }

    [HttpGet]
    [Route("/getConfiguration")]
    public string GetConfiguration(string monitorName)
    {
      var config = _configRepo.GetMonitorConfiguration(monitorName);
      return config.Value;
    }

    [HttpGet]
    [Route("/allConfigurations")]
    public IEnumerable<MonitorConfiguration> GetConfigurations()
    {
      return _configRepo.GetAll();
    }

    [HttpGet]
    [Route("/allConfigurationsByType")]
    public IEnumerable<MonitorConfiguration> GetAllConfigurationsByType(string type)
    {
      return _configRepo.GetByType(type);
    }
  }
}