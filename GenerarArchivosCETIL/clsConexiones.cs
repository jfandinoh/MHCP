using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using BibliotecaClases;
using System.IO;
using System.Data;

namespace GenerarArchivosCETIL
{
    public class clsConexiones
    {
        public clsConexiones(){}

        private void CrearConexion(String databaseOrigen, PostgreSql postgreSql)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Console.WriteLine("Version: " + postgreSql.ConsultarDato("SELECT version();"));

                    Query = "DROP EXTENSION IF EXISTS dblink;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT pg_namespace.nspname, pg_proc.proname FROM pg_proc, pg_namespace WHERE pg_proc.pronamespace = pg_namespace.oid AND pg_proc.proname LIKE '%dblink%';";
                    dataTable = postgreSql.ConsultarDatos(Query);
                    if (dataTable.Rows.Count == 0)
                    {
                        Console.WriteLine("NO existe dblink");
                        postgreSql.EjecutarQuery("CREATE EXTENSION dblink;");
                        Console.WriteLine("Se instala la extensión DbLink");

                        Query = "SELECT dblink_connect('host=192.168.249.8 user=postgres password=PriaS2584SofT dbname=" + databaseOrigen + "')";
                        Console.WriteLine("Prueba de la conexión de la base de datos " + databaseOrigen + ": " + postgreSql.ConsultarDato(Query));

                        //Cree un contenedor de datos externo para la autenticación global. Una vez creado y configurado este contenedor de conexión, puede utilizar este nombre para la consulta de base de datos cruzada.
                        Query = "DROP SERVER IF EXISTS demodbrnd CASCADE;";
                        postgreSql.EjecutarQuery(Query);

                        Query = "DROP FOREIGN DATA WRAPPER IF EXISTS dbrnd CASCADE;";
                        postgreSql.EjecutarQuery(Query);

                        Query = "CREATE FOREIGN DATA WRAPPER dbrnd VALIDATOR postgresql_fdw_validator;";
                        postgreSql.EjecutarQuery(Query);

                        Query = "CREATE SERVER demodbrnd FOREIGN DATA WRAPPER dbrnd OPTIONS(hostaddr '192.168.249.8', dbname '" + databaseOrigen + "');";
                        postgreSql.EjecutarQuery(Query);

                        Query = "CREATE USER MAPPING FOR postgres SERVER demodbrnd OPTIONS(user 'postgres', password 'PriaS2584SofT');";
                        postgreSql.EjecutarQuery(Query);
                        Console.WriteLine("Se crea un contenedor de datos externo para la autenticación global. Una vez creado y configurado este contenedor de conexión, puede utilizar este nombre para la consulta de base de datos cruzada");

                        Query = "SELECT dblink_connect('demodbrnd')";
                        Console.WriteLine("Prueba del servidor creado: " + postgreSql.ConsultarDato(Query));

                        //Se requiere dar permiso para asignar usuario
                        Query = "GRANT USAGE ON FOREIGN SERVER demodbrnd TO postgres;";
                        postgreSql.EjecutarQuery(Query);
                    }
                    else
                    {
                        Console.WriteLine("Ya existe la extensión DbLink");
                        /*
                        //Eliminar la extensión DbLink.
                        Query = "DROP EXTENSION IF EXISTS dblink;";
                        postgreSql.EjecutarQuery(Query, npgsqlTransaction);
                        
                        //Eliminar un contenedor de datos externo para la autenticación global.
                        Query = "DROP USER MAPPING IF EXISTS FOR postgres SERVER demodbrnd;";
                        postgreSql.EjecutarQuery(Query, npgsqlTransaction);

                        Query = "DROP SERVER IF EXISTS demodbrnd;";
                        postgreSql.EjecutarQuery(Query, npgsqlTransaction);

                        Query = "DROP FOREIGN DATA WRAPPER IF EXISTS dbrnd;";
                        postgreSql.EjecutarQuery(Query, npgsqlTransaction);
                        */
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error creando conexion a base de datos {0}. {1}", databaseOrigen, ex.Message));
            }
        }

    }
}
