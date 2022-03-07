using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;
using System.Configuration;

namespace BibliotecaClases
{
    public class MySQL
    {
        MySqlConnection MySqlConnection = new MySqlConnection();
        MySqlCommand MySqlCommand = new MySqlCommand();
        MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter();

        public MySQL() 
        {
            System.Console.WriteLine("Clase MySQL");
        }

        public void AbrirConexion()
        {
            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
                builder.Server = "192.168.249.8";
                builder.UserID = "root";
                builder.Password = "P@siv0C0l2021%";
                builder.Port = 3306;
                //builder.Database = database;
                MySqlConnection.ConnectionString = builder.ConnectionString;
                MySqlConnection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error abriendo conexion a base de datos. {0}", ex.Message));
            }
        }

        public MySqlConnection ObtenerConexion()
        {
            try
            {
                return MySqlConnection;
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
                if (MySqlConnection.State == ConnectionState.Open)
                {
                    MySqlConnection.Close();
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

                MySqlCommand.Connection = MySqlConnection;
                MySqlCommand.CommandText = query;
                MySqlCommand.CommandType = CommandType.Text;
                MySqlCommand.CommandTimeout = 0;
                MySqlDataAdapter = new MySqlDataAdapter(MySqlCommand);
                MySqlDataAdapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error consultando datos de base de datos. {0}", ex.Message));
            }
        }

        public DataTable ConsultarDatos(String query, MySqlTransaction mySqlTransaction)
        {
            try
            {
                DataTable dataTable = new DataTable();

                MySqlCommand.Connection = MySqlConnection;
                MySqlCommand.CommandText = query;
                MySqlCommand.CommandType = CommandType.Text;
                MySqlCommand.Transaction = mySqlTransaction;
                MySqlCommand.CommandTimeout = 0;
                MySqlDataAdapter = new MySqlDataAdapter(MySqlCommand);
                MySqlDataAdapter.Fill(dataTable);

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
                MySqlCommand.Connection = MySqlConnection;
                MySqlCommand.CommandText = query;
                MySqlCommand.CommandType = CommandType.Text;
                MySqlCommand.CommandTimeout = 0;

                return MySqlCommand.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error consultando datos de base de datos. {0}", ex.Message));
            }
        }

        public string ConsultarDato(String query, MySqlTransaction mySqlTransaction)
        {
            try
            {
                MySqlCommand.Connection = MySqlConnection;
                MySqlCommand.CommandText = query;
                MySqlCommand.CommandType = CommandType.Text;
                MySqlCommand.Transaction = mySqlTransaction;
                MySqlCommand.CommandTimeout = 0;

                return MySqlCommand.ExecuteScalar().ToString();
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

                MySqlCommand.Connection = MySqlConnection;
                MySqlCommand.CommandText = query;
                MySqlCommand.CommandType = CommandType.Text;
                MySqlCommand.CommandTimeout = 0;
                MySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error ejecutando query en base de datos. {0}", ex.Message));
            }
        }

        public void EjecutarQuery(String query, MySqlTransaction mySqlTransaction)
        {
            try
            {
                DataTable dataTable = new DataTable();

                MySqlCommand.Connection = MySqlConnection;
                MySqlCommand.CommandText = query;
                MySqlCommand.CommandType = CommandType.Text;
                MySqlCommand.Transaction = mySqlTransaction;
                MySqlCommand.CommandTimeout = 0;
                MySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error ejecutando query en base de datos. {0}", ex.Message));
            }
        }

    }
}
