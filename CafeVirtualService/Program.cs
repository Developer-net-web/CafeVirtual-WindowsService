using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CafeVirtualService
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        //static void Main()
        //{
        //    ServiceBase[] ServicesToRun;
        //    ServicesToRun = new ServiceBase[]
        //    {
        //        new Service1()
        //    };

        //    ServiceBase.Run(ServicesToRun);
        //}

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var servicio = new Service1();

            servicio.IniciarComoConsola();

            Console.WriteLine("==================================");
            Console.WriteLine(" IMPORTADOR DE REGISTROS SQL");
            Console.WriteLine("==================================");
            Console.WriteLine("Servicio iniciado correctamente.");
            Console.WriteLine("Presiona ENTER para detener.");

            Console.ReadLine();

            servicio.DetenerComoConsola();

#else
                    ServiceBase.Run(new Service1());
#endif
        }
    }
}
