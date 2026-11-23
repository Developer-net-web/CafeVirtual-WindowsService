using CafeVirtualService.Entidades;
using CafeVirtualService.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeVirtualService.Repositorios
{
    public class RegistroRepository : IRegistroRepository
    {
        private readonly string _connectionString;

        public RegistroRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public void InsertarLote(List<Registro> registros)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var registro in registros)
                        {
                            const string sql = @"

                            INSERT INTO dbo.Usuarios
                            (
                                Nombre,
                                Email,
                                FechaRegistro,
                                Activo
                            )
                            VALUES
                            (
                                @Nombre,
                                @Email,
                                @FechaRegistro,
                                @Activo
                            )";

                            using (var command = new SqlCommand(sql, connection, transaction))
                            {
                                command.Parameters.Add("@Nombre", SqlDbType.VarChar, 150).Value = registro.Nombre;

                                command.Parameters.Add("@Email", SqlDbType.VarChar, 200).Value = registro.Email;

                                command.Parameters.Add("@FechaRegistro", SqlDbType.DateTime2).Value = registro.FechaRegistro;

                                command.Parameters.Add("@Activo", SqlDbType.Bit).Value = registro.Activo;

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
