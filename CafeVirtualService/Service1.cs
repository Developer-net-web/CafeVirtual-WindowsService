using CafeVirtualService.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CafeVirtualService
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer _timer;
        private readonly ArchivoService _archivoService;

        private string _entrada;
        private string _procesados;
        private string _errores;

        private int _procesando;

        public Service1()
        {
            InitializeComponent();

            ServiceName = "ImportadorRegistros";

            _archivoService = new ArchivoService();
        }

        public void IniciarComoConsola()
        {
            OnStart(null);

            ProcesarArchivos();
        }

        public void DetenerComoConsola()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _entrada = ConfigurationManager.AppSettings["CarpetaEntrada"];
                _procesados = ConfigurationManager.AppSettings["CarpetaProcesados"];
                _errores = ConfigurationManager.AppSettings["CarpetaErrores"];

                int intervalo = int.Parse(ConfigurationManager.AppSettings["Intervalo"]);

                Directory.CreateDirectory(_entrada);
                Directory.CreateDirectory(_procesados);
                Directory.CreateDirectory(_errores);

                _timer = new System.Timers.Timer(intervalo);

                _timer.AutoReset = false;
                _timer.Elapsed += Timer_Elapsed;

                _timer.Start();

                EventLog.WriteEntry(
                    "Servicio de importación iniciado.",
                    EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(
                    $"Error al iniciar el servicio: {ex}",
                    EventLogEntryType.Error);

                throw;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref _procesando, 1) == 1)
                return;

            try
            {
                ProcesarArchivos();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(
                    $"Error general: {ex}",
                    EventLogEntryType.Error);
            }
            finally
            {
                Interlocked.Exchange(ref _procesando, 0);

                if (_timer != null)
                    _timer.Start();
            }
        }

        private void ProcesarArchivos()
        {
            var archivos = Directory.GetFiles(_entrada, "*.txt");

            foreach (var archivo in archivos)
            {
                string nombre = Path.GetFileName(archivo);

                string temporal = Path.Combine(
                    _entrada, nombre + ".processing");

                try
                {
                    // Reservar el archivo.
                    File.Move(archivo, temporal);

                    EventLog.WriteEntry(
                        $"Procesando archivo: {nombre}",
                        EventLogEntryType.Information);

                    _archivoService.ProcesarArchivo(temporal);

                    string destino = ObtenerRutaDestino(
                        _procesados, nombre);

                    File.Move(temporal, destino);

                    EventLog.WriteEntry(
                        $"Archivo procesado correctamente: {nombre}",
                        EventLogEntryType.Information);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry(
                        $"Error procesando {nombre}: {ex}",
                        EventLogEntryType.Error);

                    if (File.Exists(temporal))
                    {
                        string destino = ObtenerRutaDestino(
                            _errores, nombre);

                        File.Move(temporal, destino);
                    }
                }
            }
        }

        private string ObtenerRutaDestino(
            string carpeta, string nombre)
        {
            string destino = Path.Combine(carpeta, nombre);

            if (File.Exists(destino))
            {
                string nombreNuevo =
                    $"{Path.GetFileNameWithoutExtension(nombre)}_" +
                    $"{DateTime.Now:yyyyMMddHHmmssfff}.txt";

                destino = Path.Combine(carpeta, nombreNuevo);
            }

            return destino;
        }

        protected override void OnStop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            EventLog.WriteEntry("Servicio de importación detenido.", EventLogEntryType.Information);
        }
    }
}
