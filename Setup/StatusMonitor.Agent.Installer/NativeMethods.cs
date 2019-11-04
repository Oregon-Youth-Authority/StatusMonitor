using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace StatusMonitor.Agent.Installer
{
   internal static class NativeMethods
   {

      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern bool ChangeServiceConfig2(
         IntPtr serviceHandle,
         uint infoLevel,
         ref NativeMethods.SERVICE_DESCRIPTION serviceDesc);

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern bool CloseServiceHandle(IntPtr handle);
      
      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern IntPtr CreateService(
         IntPtr databaseHandle,
         string serviceName,
         string displayName,
         int access,
         int serviceType,
         int startType,
         int errorControl,
         string binaryPath,
         string loadOrderGroup,
         IntPtr pTagId,
         string dependencies,
         string servicesStartName,
         string password);


      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern bool DeleteService(IntPtr serviceHandle);

      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern IntPtr OpenSCManager(
         string machineName,
         string databaseName,
         int access);
      
      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern IntPtr OpenService(
        IntPtr databaseHandle,
        string serviceName,
        int access);


      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct SERVICE_DESCRIPTION
      {
         public IntPtr description;
      }
   }
}
