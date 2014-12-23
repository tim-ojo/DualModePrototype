using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DualModePrototype
{
    [RunInstaller(true)]
    public class DualModePrototypeInstaller : Installer
    {
        public DualModePrototypeInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            //serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Account = ServiceAccount.NetworkService;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information
            serviceInstaller.DisplayName = "DualModePrototype Windows Service";
            serviceInstaller.Description = @"A service to illustrate the ability to make an app run as 
                either a console application or a Windows service";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = "MyWidgetService";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
