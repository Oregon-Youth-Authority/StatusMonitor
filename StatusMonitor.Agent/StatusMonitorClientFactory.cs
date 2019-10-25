using Microsoft.Extensions.DependencyInjection;
using State.Or.Oya.StatusMonitor.Client.Generated;
using System;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   public class StatusMonitorClientFactory
   {
      private readonly IServiceProvider _serviceProvider;

      public StatusMonitorClientFactory(IServiceProvider serviceProvider)
      {
         _serviceProvider = serviceProvider;
      }

      public StatusMonitorClient Create()
      {
         return _serviceProvider.GetService<StatusMonitorClient>();
      }
   }
}