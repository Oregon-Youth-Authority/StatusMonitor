using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace StatusMonitor.Agent.Installer
{
   [RunInstaller(true)]
   public partial class AgentInstaller : System.Configuration.Install.Installer
   {
      private const string ServiceName = "JJISStatusMonitorAgent";
      private const string DisplayName = "JJIS Status Monitor Agent";
      private const string Description = "Periodically updates JJIS support team with connectivity status.";

      private const int ServiceType = 16; // SERVICE_WIN32_OWN_PROCESS
      private const int ServiceStartType = 2; // Automatic
      private const int FullControlAccess = 983103;
      private const int GenericAllAccess = 983551;
      private const string ServicesStartName = "NT AUTHORITY\\LocalService";

      public AgentInstaller()
      {
         InitializeComponent();
      }

      public override void Commit(IDictionary savedState)
      {
         try
         {
            using (var serviceController = new ServiceController(ServiceName))
            {
               serviceController.Start();
               serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
         }
         finally
         {
            base.Commit(savedState);
         }
      }

      public override void Install(IDictionary stateSaver)
      {
         var installDir = Context.Parameters["targetdir"].TrimEnd(new[] { '\\' });
         var binaryPath = $"\"{Path.Combine(installDir, "JJISStatusMonitorAgent.exe")}\" --service";
         var scManagerPtr = NativeMethods.OpenSCManager((string)null, (string)null, FullControlAccess);

         var servicePtr = IntPtr.Zero;
         try
         {
            servicePtr = NativeMethods.CreateService(scManagerPtr, ServiceName, DisplayName, GenericAllAccess, ServiceType, ServiceStartType, 1, binaryPath, (string)null, IntPtr.Zero, (string)null, ServicesStartName, (string)null);
            if (servicePtr == IntPtr.Zero)
               throw new Win32Exception();
            var serviceDesc = new NativeMethods.SERVICE_DESCRIPTION
            {
               description = Marshal.StringToHGlobalUni(Description)
            };
            var flag = NativeMethods.ChangeServiceConfig2(servicePtr, 1U, ref serviceDesc);
            Marshal.FreeHGlobal(serviceDesc.description);
            if (!flag)
               throw new Win32Exception();
         }
         finally
         {
            if (servicePtr != IntPtr.Zero)
               NativeMethods.CloseServiceHandle(servicePtr);
            NativeMethods.CloseServiceHandle(scManagerPtr);

            base.Install(stateSaver);
         }
      }

      public override void Rollback(IDictionary savedState)
      {
         try
         {
            RemoveService();
         }
         catch { }
         base.Rollback(savedState);
      }

      public override void Uninstall(IDictionary savedState)
      {
         try
         {
            RemoveService();
         }
         catch { }
         base.Uninstall(savedState);
      }

      private void RemoveService()
      {
         StopService();
         Thread.Sleep(5000);

         var scManagerPtr = NativeMethods.OpenSCManager((string)null, (string)null, FullControlAccess);
         if (scManagerPtr == IntPtr.Zero)
            throw new Win32Exception();
         var servicePtr = IntPtr.Zero;
         try
         {
            servicePtr = NativeMethods.OpenService(scManagerPtr, ServiceName, 65536);
            if (servicePtr == IntPtr.Zero)
               throw new Win32Exception();
            NativeMethods.DeleteService(servicePtr);
         }
         finally
         {
            if (servicePtr != IntPtr.Zero)
               NativeMethods.CloseServiceHandle(servicePtr);
            NativeMethods.CloseServiceHandle(scManagerPtr);
         }
      }

      private void StopService()
      {
         try
         {
            using (var serviceController = new ServiceController(ServiceName))
            {
               if (serviceController.Status != ServiceControllerStatus.Stopped)
               {
                  serviceController.Stop();
                  var remainingAttempts = 10;
                  serviceController.Refresh();
                  while (serviceController.Status != ServiceControllerStatus.Stopped)
                  {
                     if (remainingAttempts > 0)
                     {
                        Thread.Sleep(1000);
                        serviceController.Refresh();
                        --remainingAttempts;
                     }
                     else
                        break;
                  }
               }
            }
         }
         catch
         {
            // Eat exceptions while stopping service
         }
      }
   }
}