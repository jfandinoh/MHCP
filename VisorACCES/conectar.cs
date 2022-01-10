using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb; 
namespace VisorACCES
{
    public class conectar
    {
        OleDbConnection conexion = new OleDbConnection(); //Establecer conexión a la DB
        OleDbCommand comando; //Realizar consultas en DB
        OleDbDataAdapter adaptador;
        DataTable datatable = new DataTable();

        public conectar()
        {
            System.Console.WriteLine("Clase ClsConectarDB");
        }

        public string AbrirConexion()
        {
            string estado;
            try
            {
                string db = @"C:\Users\jfandino\Desktop\pc_datos.mdb";
                string Jet_db = @"C:\Users\jfandino\Desktop\System.mdw";
                conexion.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Jet OLEDB:System database=" + Jet_db +"; Data Source=" + db + ";";
                if (conexion.State == ConnectionState.Closed)
                {
                    conexion.Open();

                    List<string> listaTablas = new List<string>(); 
                    DataTable dt = conexion.GetSchema("Tables");
                    foreach (DataRow row in dt.Rows)
                    {
                        listaTablas.Add(row["TABLE_NAME"].ToString());
                    }

                    estado = "conectado";
                }
                else 
                {
                    estado = "La conexión ya se encuentra abierta";
                }      
            }
            catch (Exception ex)
            {
                estado = ex.Message.ToString(); 
            }
            return estado;
        }

        public string CerrarConexion()
        {
            string estado;
            try
            {
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                    estado = "Cerrado";
                }
                else
                {
                    estado="La conexión ya se encuentra cerrada";
                }
            }
            catch (Exception ex)
            {
                estado = ex.Message.ToString();
            }
            return estado;           
            
        }

        public DataTable consultar()
        {
            DataTable respuesta;
            try
            {
                int a = 0;
                comando = new OleDbCommand();
                comando.Connection = conexion;
                comando.CommandText = "SELECT * from Departamentos";
                OleDbDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    a++;
                    Console.WriteLine("Cantidad de filas en la tabla:" + a);
                    
                }
                reader.Close();

                adaptador = new OleDbDataAdapter("SELECT * from Departamentos", conexion);
                adaptador.Fill(datatable);

                adaptador.Dispose();
                respuesta = datatable;
            }
            catch (Exception ex)
            {
                datatable.Clear();
                respuesta = datatable;
            }
            return respuesta;
        }       

    }
}
