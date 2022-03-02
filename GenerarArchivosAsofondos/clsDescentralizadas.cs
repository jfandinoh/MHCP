using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using BibliotecaClases;
using System.IO;
using System.Data;

namespace GenerarArchivosAsofondos
{
    public class clsDescentralizadas
    {
        public clsDescentralizadas(){}

        public string Proceso()
        {
            try
            {
                String database = "pc_datos_ultenvio_dc";

                DataTable dataTable = new DataTable();
                DataTable dtEsquemas = new DataTable();
                NpgsqlTransaction npgsqlTransaction = null;
                NpgsqlConnection npgsqlConnection = null;
                PostgreSql postgreSql = new PostgreSql();
                postgreSql.AbrirConexion("pc_pruebas");
                npgsqlConnection = postgreSql.ObtenerConexion();
                npgsqlTransaction = npgsqlConnection.BeginTransaction();

                try
                {
                    dtEsquemas = ObtenerEsquemas(database);
                    CrearConexiondb(database, postgreSql, npgsqlTransaction);

                    foreach (DataRow row in dtEsquemas.Rows)
                    {
                        Activos(database, row["schemaname"].ToString(), postgreSql, npgsqlTransaction);
                        Pensionados(database, row["schemaname"].ToString(), postgreSql, npgsqlTransaction);
                        Sustitutos(database, row["schemaname"].ToString(), postgreSql, npgsqlTransaction);
                        Retirados(database, row["schemaname"].ToString(), postgreSql, npgsqlTransaction);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    postgreSql.CerrarConexion();
                }

                return "Se generan los archivos con la informacion de entidades descentralizadas satisfactoriamente";
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error en proceso. {0}", ex.Message));
            }
        }

        private void CrearConexiondb(String database, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
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
                    dataTable = postgreSql.ConsultarDatos(Query, npgsqlTransaction);
                    if (dataTable.Rows.Count == 0)
                    {
                        Console.WriteLine("NO existe dblink");
                        postgreSql.EjecutarQuery("CREATE EXTENSION dblink;");
                        Console.WriteLine("Se instala la extensión DbLink");

                        Query = "SELECT dblink_connect('host=192.168.249.8 user=postgres password=PriaS2584SofT dbname=" + database + "')";
                        Console.WriteLine("Prueba de la conexión de la base de datos " + database + ": " + postgreSql.ConsultarDato(Query));

                        //Cree un contenedor de datos externo para la autenticación global. Una vez creado y configurado este contenedor de conexión, puede utilizar este nombre para la consulta de base de datos cruzada.
                        Query = "DROP SERVER IF EXISTS demodbrnd CASCADE;";
                        postgreSql.EjecutarQuery(Query);

                        Query = "DROP FOREIGN DATA WRAPPER IF EXISTS dbrnd CASCADE;";
                        postgreSql.EjecutarQuery(Query);

                        Query = "CREATE FOREIGN DATA WRAPPER dbrnd VALIDATOR postgresql_fdw_validator;";
                        postgreSql.EjecutarQuery(Query);

                        Query = "CREATE SERVER demodbrnd FOREIGN DATA WRAPPER dbrnd OPTIONS(hostaddr '192.168.249.8', dbname '" + database + "');";
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
                throw new Exception(string.Format("Error creando conexion a base de datos {0}. {1}", database, ex.Message));
            }
        }

        private DataTable ObtenerEsquemas(String database)
        {
            String Query = string.Empty;
            DataTable dataTable = new DataTable();
            PostgreSql postgreSql = new PostgreSql();

            try
            {
                postgreSql.AbrirConexion(database);

                try
                {
                    Query = "SELECT  DISTINCT(schemaname) AS schemaname FROM pg_tables " +
                    "WHERE tablename IN('tbl_f3_empleados_activos', 'tbl_f5_pensionados', 'tbl_f6_beneficiarios', 'tbl_f9_retirados') " +
                    "AND schemaname NOT IN('pc_datos_00', 'pc_datos_indice','public') " +
                    "ORDER BY 1";
                    dataTable = postgreSql.ConsultarDatos(Query);

                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error consultando esquemas de base de datos {0}.", ex.Message));
            }
            finally
            {
                postgreSql.CerrarConexion();
            }
        }        

        private void Activos(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try 
            {
                clsActivos clsActivos = new clsActivos();
                String Query = string.Empty;

                clsActivos.CargarInformacion(database, Esquema, postgreSql, npgsqlTransaction);
                clsActivos.CargarInformacionHl(database, Esquema, postgreSql, npgsqlTransaction);
                clsActivos.ConstruirInformacion(postgreSql, npgsqlTransaction,"ED",string.Empty);
                clsActivos.ConstruirInformacionHl(postgreSql, npgsqlTransaction, "ED", string.Empty);

                Query = "SELECT REPLACE(concat(tipo_registro,'_;_',regexp_replace(nombre_entidad, '\\r|\\n', '', 'g'),'_;_',nit_entidad,'_;_',digito_verificacion,'_;_', " +
                "tipo_documento,'_;_',numero_documento,'_;_',regexp_replace(nombre_persona, '\\r|\\n', '', 'g'),'_;_', genero,'_;_', " +
                "estado_civil,'_;_',fecha_inicial,'_;_',concurrencia,'_;_',to_char(NOW(), 'YYYYMMDD')),'\"','') " +
                "FROM temp_reg1; ";
                EscribirArchivo("centrales_R1", Query, postgreSql, npgsqlTransaction);

                Query = "SELECT REPLACE(concat(tipo_registro,'_;_',regexp_replace(nombre_entidad, '\\r|\\n', '', 'g'),'_;_',nit_entidad,'_;_',digito_verificacion,'_;_', " +
                "tipo_documento,'_;_',numero_documento,'_;_',regexp_replace(nombre_persona, '\\r|\\n', '', 'g'),'_;_', genero,'_;_', " +
                "estado_civil,'_;_',fecha_inicial,'_;_',concurrencia,'_;_',to_char(NOW(), 'YYYYMMDD')),'\"','') " +
                "FROM temp_reg1; ";
                EscribirArchivo("centrales_R2", Query, postgreSql, npgsqlTransaction);

                Console.WriteLine("Informacion activos generada correctamente");
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error en proceso de informacion Activos. {0}", ex.Message));
            }
        }

        private void Pensionados(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                cslPensionados cslPensionados = new cslPensionados();
                String Query = string.Empty;

                cslPensionados.CargarInformacion(database, Esquema, postgreSql, npgsqlTransaction);
                cslPensionados.CargarInformacionFallecidos(database, Esquema, postgreSql, npgsqlTransaction);
                cslPensionados.CargarInformacionHl(database, Esquema, postgreSql, npgsqlTransaction);
                cslPensionados.CargarInformacionHlFallecidos(database, Esquema, postgreSql, npgsqlTransaction);
                cslPensionados.ConstruirInformacion(postgreSql, npgsqlTransaction, "ED", string.Empty);
                cslPensionados.ConstruirInformacionHl(postgreSql, npgsqlTransaction, "ED", string.Empty);

                Query = "SELECT REPLACE(concat(tipo_registro,'_;_',regexp_replace(nombre_entidad,'\\r|\\n','','g'),'_;_',nit_entidad,'_;_',digito_verificacion,'_;_', "+
                "tipo_documento,'_;_',numero_documento,'_;_',regexp_replace(nombre_persona, '\\r|\\n', '', 'g'),'_;_', genero,'_;_', " +
                "estado_civil,'_;_',modalidad_pension,'_;_',concurrencia,'_;_',tipo_pensionado,'_;_', to_char(CAST(fecha_corte AS date), 'YYYYMMDD')),'\"','') "+
                "FROM temp_reg3; ";
                EscribirArchivo("centrales_R3", Query, postgreSql, npgsqlTransaction);

                Query = "SELECT REPLACE(concat(tipo_registro,'_;_',tipo_documento,'_;_',numero_documento,'_;_',nit_empresa,'_;_', "+
                "regexp_replace(nombre_empresa, '\\r|\\n', '', 'g'), '_;_', fecha_inicial,'_;_', fecha_final,'_;_',ciuidad,'_;_',departamento,'_;_', " +
                "tipo_pensionado, '_;_',to_char(CAST(fecha_corte AS date), 'YYYYMMDD')),'\"','') "+
                "FROM temp_reg4; ";
                EscribirArchivo("centrales_R4", Query, postgreSql, npgsqlTransaction);

                Console.WriteLine("Informacion pensionados generada correctamente");
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error en proceso de informacion pensionados. {0}", ex.Message));
            }
        }

        private void Sustitutos(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                clsSustitutos clsSustitutos = new clsSustitutos();
                String Query = string.Empty;

                clsSustitutos.CargarInformacion(database, Esquema, postgreSql, npgsqlTransaction);
                clsSustitutos.ConstruirInformacion(postgreSql, npgsqlTransaction, "ED", string.Empty);

                Query = "SELECT REPLACE(concat(tipo_registro,'_;_',regexp_replace(nombre_entidad, '\\r|\\n', '', 'g'),'_;_',nit_entidad,'_;_',digito_verificacion,'_;_', "+
                        "tipo_documento,'_;_',numero_documento,'_;_',regexp_replace(nombre_persona, '\\r|\\n', '', 'g'),'_;_', genero,'_;_', " +
                        "tipo_documento_causante,'_;_',numero_documento_causante,'_;_',regexp_replace(nombre_causante, '\\r|\\n', '', 'g'),'_;_', " +
                        "genero_causante,'_;_',estado_civil_causante,'_;_',modalidad_pension,'_;_',tipo_pension,'_;_',concurrencia,'_;_', " +
                        "to_char(CAST(fecha_corte AS date), 'YYYYMMDD')),'\"','') "+
                "FROM temp_reg5; ";
                EscribirArchivo("centrales_R5", Query, postgreSql, npgsqlTransaction);

                Console.WriteLine("Informacion sustitutos generada correctamente");
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error en proceso de informacion sustitutos. {0}", ex.Message));
            }
        }

        private void Retirados(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                cslRetirados cslRetirados = new cslRetirados();
                String Query = string.Empty;

                cslRetirados.CargarInformacion(database, Esquema, postgreSql, npgsqlTransaction);
                cslRetirados.CargarInformacionHl(database, Esquema, postgreSql, npgsqlTransaction);
                cslRetirados.CargarInformacionAFP(database, Esquema, postgreSql, npgsqlTransaction);
                cslRetirados.ConstruirInformacion(postgreSql, npgsqlTransaction, "ED", string.Empty);
                cslRetirados.ConstruirInformacionHl(postgreSql, npgsqlTransaction, "ED", string.Empty);
                cslRetirados.ConstruirInformacionHlAFP(postgreSql, npgsqlTransaction, "ED", string.Empty);

                Query = "SELECT REPLACE(concat(tipo_registro,'_;_',regexp_replace(nombre_entidad, '\\r|\\n', '', 'g'),'_;_',nit_entidad,'_;_',digito_verificacion,'_;_', "+
                "tipo_documento,'_;_',numero_documento,'_;_',regexp_replace(nombre_persona, '\\r|\\n', '', 'g'),'_;_', genero,'_;_', " +
                "estado_civil,'_;_',fecha_inicial,'_;_',fecha_final,'_;_',concurrencia,'_;_', " +
                "to_char(CAST(fecha_corte AS date), 'YYYYMMDD')),'\"','') "+
                "FROM temp_reg6; ";
                EscribirArchivo("centrales_R6", Query, postgreSql, npgsqlTransaction);

                Query = "SELECT REPLACE(concat(tipo_registro,'_;_',tipo_documento,'_;_',numero_documento,'_;_',nit_empresa,'_;_', "+
                "regexp_replace(nombre_empresa, '\\r|\\n', '', 'g'), '_;_', fecha_inicial,'_;_', fecha_final,'_;_',ciuidad,'_;_',departamento,'_;_', "+
                "to_char(CAST(fecha_corte AS date), 'YYYYMMDD')),'\"','') "+
                "FROM temp_reg7; ";
                EscribirArchivo("centrales_R7", Query, postgreSql, npgsqlTransaction);

                Query = "SELECT REPLACE(concat(tipo_registro,'_;_',tipo_documento,'_;_',numero_documento,'_;_',regexp_replace(nombre_fondo_aportes,'\\r|\\n','','g'),'_;_', "+
                "fecha_inicial,'_;_', fecha_final,'_;_',	to_char(CAST(fecha_corte AS date), 'YYYYMMDD')),'\"','') "+
                "FROM temp_reg8; ";
                EscribirArchivo("centrales_R7", Query, postgreSql, npgsqlTransaction);

                Console.WriteLine("Informacion retirados generada correctamente");
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error en proceso de informacion retirados. {0}", ex.Message));
            }
        }

        private void EscribirArchivo(String nombreArchivo, String query, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + @"\asofondos\"+ DateTime.Now.Year.ToString()+ @"\descentralizadas";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string pathArchivo = System.IO.Path.Combine(path, nombreArchivo + ".csv");

                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    dataTable = postgreSql.ConsultarDatos(query);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                if (dataTable.Rows.Count > 0)
                {
                    using (FileStream fs = new FileStream(pathArchivo, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            foreach (DataRow row in dataTable.Rows)
                            {
                                sw.WriteLine(row[0]);
                            }
                        }
                    }
                    Console.WriteLine("Archivo generado (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + pathArchivo);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error almacenando informacion en archivo. {0}", ex.Message));
            }
        }

    }
}
