using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;


namespace BibliotecaClases
{
    public class ConectarDB
    {
        SqlConnection connection; //Establecer conexión a la DB
        SqlCommand command;//Realizar consultas en DB
        SqlDataReader datareader;//Guardar respuesta de consulta
        SqlDataAdapter dataadapter;//Se usa para llenar DataTable
        DataTable datatable;
        
        public ConectarDB()
        {
            System.Console.WriteLine("Clase ConectarDB");
            connection = new SqlConnection();

        }

        public void AbrirConexion()
        {
            try
            {
                //connection = new SqlConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["Conexión"].ConnectionString;
                //connection.ConnectionString = "Data Source=JAFH-PC\\SQLEXPRESS;Initial Catalog=Ejemplo1;Integrated Security=True";
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error abriendo conexion a base de datos. {0}", ex.Message));
            }
        }

        public void  CerrarConexion()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cerrando conexion a base de datos. {0}", ex.Message));
            }
        }

        public void AgregarDatos()
        {
            try
            {
                command = new SqlCommand();
                command.Connection = connection;   
                command.CommandText = "insert into datosPrueba(transaccion,valor,limite,estado) values (1015,10000,110,4)";
                command.ExecuteNonQuery(); 
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error insertando datos a base de datos. {0}", ex.Message));
            }
        }

        public void SeleccionarDatos()
        {
            try
            {
                int a = 0;
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "select * from datosPrueba";
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    a++;
                }
                datareader.Close();

                datatable = new DataTable();
                dataadapter = new SqlDataAdapter("select * from datosPrueba", connection);
                dataadapter.Fill(datatable);

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error seleccionando datos a base de datos. {0}", ex.Message));
            }
        }
    }
}
