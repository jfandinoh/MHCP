using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using System.Configuration;

namespace BibliotecaClases
{
    public class PostgreSql
    {
        NpgsqlConnection npgsqlConnection = new NpgsqlConnection();
        NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter();

        public PostgreSql()
        {
            System.Console.WriteLine("Clase PostgreSql");
        }

        public void AbrirConexion(String database)
        {
            try
            {
                //npgsqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["Conexión"].ConnectionString;
                npgsqlConnection.ConnectionString = "host=192.168.249.8; Username=postgres; Password=PriaS2584SofT; Database="+database;
                npgsqlConnection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error abriendo conexion a base de datos. {0}", ex.Message));
            }
        }

        public NpgsqlConnection ObtenerConexion()
        {
            try
            {
                return npgsqlConnection;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error obteniendo conexion existente a base de datos. {0}", ex.Message));
            }
        }

        public void CerrarConexion()
        {
            try
            {
                if (npgsqlConnection.State == ConnectionState.Open)
                {
                    npgsqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cerrando conexion a base de datos. {0}", ex.Message));
            }
        }

        public DataTable ConsultarDatos(String query)
        {
            try
            {
                DataTable dataTable = new DataTable();

                npgsqlCommand.Connection = npgsqlConnection;
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandType = CommandType.Text;
                npgsqlCommand.CommandTimeout = 0;
                npgsqlDataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                npgsqlDataAdapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error consultando datos de base de datos. {0}", ex.Message));
            }
        }

        public DataTable ConsultarDatos(String query, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                DataTable dataTable = new DataTable();

                npgsqlCommand.Connection = npgsqlConnection;
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandType = CommandType.Text;
                npgsqlCommand.Transaction = npgsqlTransaction;
                npgsqlCommand.CommandTimeout = 0;
                npgsqlDataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                npgsqlDataAdapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error consultando datos de base de datos. {0}", ex.Message));
            }
        }

        public string ConsultarDato(String query)
        {
            try
            {
                npgsqlCommand.Connection = npgsqlConnection;
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandType = CommandType.Text;
                npgsqlCommand.CommandTimeout = 0;

                var ExecuteScalar = npgsqlCommand.ExecuteScalar();

                if(ExecuteScalar != null)
                {
                    return ExecuteScalar.ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error consultando datos de base de datos. {0}", ex.Message));
            }
        }

        public string ConsultarDato(String query, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                npgsqlCommand.Connection = npgsqlConnection;
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandType = CommandType.Text;
                npgsqlCommand.Transaction = npgsqlTransaction;
                npgsqlCommand.CommandTimeout = 0;

                var ExecuteScalar = npgsqlCommand.ExecuteScalar();

                if (ExecuteScalar != null)
                {
                    return ExecuteScalar.ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error consultando datos de base de datos. {0}", ex.Message));
            }
        }

        public void EjecutarQuery(String query)
        {
            try
            {
                DataTable dataTable = new DataTable();

                npgsqlCommand.Connection = npgsqlConnection;
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandType = CommandType.Text;
                npgsqlCommand.CommandTimeout = 0;
                npgsqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error ejecutando query en base de datos. {0}", ex.Message));
            }
        }

        public void EjecutarQuery(String query, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                DataTable dataTable = new DataTable();

                npgsqlCommand.Connection = npgsqlConnection;
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandType = CommandType.Text;
                npgsqlCommand.Transaction = npgsqlTransaction;                
                npgsqlCommand.CommandTimeout = 0;
                npgsqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error ejecutando query en base de datos. {0}", ex.Message));
            }
        }
    }
}
