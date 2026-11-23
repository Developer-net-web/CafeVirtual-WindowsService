using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CafeVirtualService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private BackgroundWorker backgroundWorker1;

        public ProjectInstaller()
        {
            var processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
                //Account = ServiceAccount.User
            };

            //var processInstaller = new ServiceProcessInstaller
            //{
            //    Account = ServiceAccount.User,
            //    Username = @"NOMBRE-EQUIPO\TuUsuario",
            //    Password = "TuContraseña"
            //};

            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = "CafeVirtualService",
                DisplayName = "Café Virtual Service",
                Description = "Servicio para importar registros TXT a SQL Server.",
                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }

        private void InitializeComponent()
        {
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();

        }
    }
}
