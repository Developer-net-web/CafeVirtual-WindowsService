using CafeVirtualService.Entidades;
using CafeVirtualService.Interfaces;
using CafeVirtualService.Repositorios;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeVirtualService.Services
{
    public class ArchivoService
    {
        private readonly IRegistroRepository _repository;

        public ArchivoService()
        {
            _repository = new RegistroRepository();
        }

        public void ProcesarArchivo(string rutaArchivo)
        {
            // Hilos

            //  --> Automatizar el proceso con hilos
            

            var registros = new List<Registro>();

            var lineas = File.ReadAllLines(rutaArchivo);

            // Omitir encabezados (primera línea).
            for (int i = 1; i < lineas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lineas[i]))
                    continue;

                var campos = lineas[i].Split('|');

                if (campos.Length != 4)
                {
                    throw new FormatException(
                        $"Formato incorrecto en la línea {i + 1}.");
                }

                if (string.IsNullOrWhiteSpace(campos[0]) ||
                    string.IsNullOrWhiteSpace(campos[1]) ||
                    string.IsNullOrWhiteSpace(campos[2]) ||
                    string.IsNullOrWhiteSpace(campos[3]))
                {
                    throw new FormatException(
                        $"Campos obligatorios vacíos en la línea {i + 1}.");
                }

                DateTime fecha;

                if (!DateTime.TryParseExact(
                    campos[2].Trim(),
                    "yyyy-MM-dd HH:mm:ss.fffffff",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fecha))
                {
                    throw new FormatException(
                        $"Fecha incorrecta en la línea {i + 1}.");
                }

                if (!bool.TryParse(campos[3].Trim(), out bool activo) &&
                    campos[3].Trim() != "0" &&
                    campos[3].Trim() != "1")
                {
                    throw new FormatException(
                        $"Valor Activo incorrecto en la línea {i + 1}.");
                }

                activo = campos[3].Trim() == "1" ||
                         campos[3].Trim().Equals(
                             "true", StringComparison.OrdinalIgnoreCase);

                registros.Add(new Registro
                {
                    Nombre = campos[0].Trim(),
                    Email = campos[1].Trim(),
                    FechaRegistro = fecha,
                    Activo = activo
                });
            }

            if (registros.Count > 0)
                _repository.InsertarLote(registros);
        }
    }
}
