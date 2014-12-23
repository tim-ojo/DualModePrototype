using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace DualModePrototype
{
    public abstract class APService : ServiceBase
    {
        /// <summary>
        /// Note: Static initializer doesn't get called unless you initialize the class or any of its sub classes
        /// </summary>
        static APService()
        {
            //Console.WriteLine("Static initializer executed");
        }

        public APService()
        {
            // In here we initialize things needed for a service
            this.ServiceName = "My Parent Widget Service";
            this.AutoLog = true;

            //Console.WriteLine("Parent class initialized");
        }

        public static String getArg(int i, string[] args)
        {
            String rval = "";
            if (i < args.Length)
                rval = args[i];

            return rval;
        }

        /// <summary>
        /// All subclasses must implement OnStart. This method becomes the entry point into the application in 
        /// place of the main method in order to allow the application run as a service. This is where we kick 
        /// off the execution of the work to be done. 
        /// </summary>
        /// <param name="args"></param>
        protected override abstract void OnStart(string[] args);

        /// <summary>
        /// All subclasses must implement OnStop. This method is called by the Service Control Manager (SCM) when
        /// the Service is stopped through the management console. Cleanup of resources can be done here. However, 
        /// it must be noted that this method is only called from SCM. Therefore if you are running the application
        /// in console mode, this method may not be called and resource management must be done manually.
        /// </summary>
        protected override abstract void OnStop();

        /// <summary>
        /// Use this method to change the status of the service to Running
        /// </summary>
        public void MarkServiceAsStarted()
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        /// <summary>
        /// Use this method to change the status of the service to Stopped
        /// </summary>
        public void MarkServiceAsStopped()
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        #region Stuff

        public void DoParentStuff()
        {
            Console.WriteLine(this.ServiceName + " did stuff");
        }

        #endregion
    }

    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public long dwServiceType;
        public ServiceState dwCurrentState;
        public long dwControlsAccepted;
        public long dwWin32ExitCode;
        public long dwServiceSpecificExitCode;
        public long dwCheckPoint;
        public long dwWaitHint;
    };
}
