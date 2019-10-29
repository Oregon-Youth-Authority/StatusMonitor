using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using State.Or.Oya.Jjis.StatusMonitor.Monitors;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace StatusMonitorAgent.Test
{
   [TestClass]
   public class HttpStatusMonitorTest
   {
      [TestMethod]
      public async Task HasStatusChanged_Status_Is_Up()
      {
         var statusMonitor = new HttpStatusMonitor(new MonitorConfiguration
         {
            Value = "{\"url\":\"https://www.google.com\"}"
         });

         await statusMonitor.HasStatusChanged();

         statusMonitor.Status.Should().Be(MonitorStatus.Up);
      }

      [TestMethod]
      public async Task HasStatusChanged_Status_Is_Down()
      {
         var statusMonitor = new HttpStatusMonitor(new MonitorConfiguration
         {
            Value = "{\"url\":\"http://google.com/fake/this_should_not_be_valid.php\"}"
         });

         await statusMonitor.HasStatusChanged();

         statusMonitor.Status.Should().Be(MonitorStatus.Down);
      }

      [TestMethod]
      public async Task HasStatusChanged_Status_Is_Timeout()
      {
         var statusMonitor = new HttpStatusMonitor(new MonitorConfiguration
         {
            Value = "{\"url\":\"https://169.254.0.1\",\"timeout\":1}"
         });

         await statusMonitor.HasStatusChanged();

         statusMonitor.Status.Should().Be(MonitorStatus.Timeout);
      }

      [TestMethod]
      public async Task HasStatusChanged_Status_Is_NameResolutionFailure()
      {
         var statusMonitor = new HttpStatusMonitor(new MonitorConfiguration
         {
            Value = "{\"url\":\"https://random_fake_unknown_bogus_893jd_jjaswe_w83wsd_qnv93.xom\"}"
         });

         await statusMonitor.HasStatusChanged();

         statusMonitor.Status.Should().Be(MonitorStatus.NameResolutionFailure);
      }
   }
}
