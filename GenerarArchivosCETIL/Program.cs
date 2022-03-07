using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using BibliotecaClases;
using System.IO;


namespace GenerarArchivosCETIL
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                try
                {

                    String TipoEntidad = string.Empty;
                    Int32 CantidadArchivos = 0;                     

                    Console.WriteLine("**************************************************************************************");
                    Console.WriteLine("***************************GENERAR ARCHIVOS PARA CETIL********************************");
                    Console.WriteLine("**************************************************************************************");

                    Console.WriteLine("¿Para que tipo de entidades quiere generar archivos?");
                    Console.WriteLine("\t1 - Centrales");
                    Console.WriteLine("\t2 - Descentralizadas");
                    Console.WriteLine("\t3 - Hospitales");

                    Console.WriteLine("Por favor digite su opcion.");
                    TipoEntidad = Console.ReadLine();

                    Console.WriteLine("¿Cuantos archivos desea generar?");

                    int.TryParse(Console.ReadLine(), out CantidadArchivos);

                    if (CantidadArchivos == 0)
                    {
                        throw new Exception("Cantidad de archivos a generar no valida");
                    }

                    switch (TipoEntidad)
                    {
                        case "1": //Centrales
                            Console.WriteLine("**************************************************************************************");
                            Console.WriteLine("************************GENERAR ARCHIVOS ENTIDADES CENTRALES**************************");
                            Console.WriteLine("**************************************************************************************");
                            //Console.WriteLine(clsCentrales.Proceso());
                            Console.WriteLine("**************************************************************************************");

                            try
                            {
                                MySQL mySQL = new MySQL();
                                mySQL.AbrirConexion();
                                string Query = string.Empty;
                                DataTable dataTable = new DataTable();
                                try
                                {
                                    Query = "DROP TABLE IF EXISTS test.EntidadesEntregadasCETIL; "+
                                    "CREATE TABLE test.EntidadesEntregadasCETIL( " +
                                      "id int AUTO_INCREMENT, " +
                                      "cod_Et varchar(5) NOT NULL, " +
                                      "cod_Ua varchar(2) NOT NULL, " +
                                      "nit_Ua varchar(25) NOT NULL, "+
                                      "nombre_Ua varchar(500) DEFAULT NULL, " +
                                      "tipo_Organizacion varchar(50) NOT NULL DEFAULT '', " +
                                      "fecha_Corte char(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '', " +
                                      "nro_Informe char(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '', " +
                                      "anio_Informe char(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '', "+
                                      "fecha_GeneradoCETIL char(25) CHARACTER SET utf8mb4 NOT NULL DEFAULT '', "+
                                      "PRIMARY KEY(id) "+
                                    "); "+
                                    "INSERT INTO test.EntidadesEntregadasCETIL(cod_Et, cod_Ua,nit_Ua, nombre_Ua, tipo_Organizacion) " +
                                    "SELECT cod_et AS cod_Et, cod_ua AS cod_Ua,IFNULL(nit_ua,'') AS nit_Ua, nombre_ua AS nombre_Ua, " +
                                    "IFNULL(b.descrip_organizacion_ua, '') AS tipo_Organizacion " +
                                    "FROM bd_webpasivocol.tbl_directorio_unidades_adtvas a "+
                                    "LEFT OUTER JOIN bd_webpasivocol.tbl_tipo_organizacion_ua b "+
                                    "ON a.tipo_organizacion = b.tipo_organizacion_ua "+
                                    "WHERE a.estado_registro <> 'A' "+
                                    "ORDER BY cod_et,cod_ua; ";

                                    mySQL.EjecutarQuery(Query);

                                    Console.WriteLine("Crear tabla ok");

                                    dataTable = mySQL.ConsultarDatos("SELECT * FROM test.EntidadesEntregadasCETIL");

                                    Console.WriteLine("consulta ok");

                                    DataRow[] dataRowsCentrales = dataTable.Select("Cod_Ua = '01' AND fecha_GeneradoCETIL = ''");

                                    Console.WriteLine("Select");
                                    for (int i = 0; i < dataRowsCentrales.Length; i++)
                                    {
                                        Query = "UPDATE test.EntidadesEntregadasCETIL " +
                                        "SET fecha_Corte = '" + "fecha_corte" + "', nro_Informe = '" + "nro_infomre" + "', anio_Informe = '" + "anio" + "', fecha_GeneradoCETIL = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "' " +
                                        "WHERE id = '" + dataRowsCentrales[i][0] + "' AND cod_Et = '" + dataRowsCentrales[i][1] + "' AND cod_Ua = '" + dataRowsCentrales[i][2] + "'";

                                        mySQL.EjecutarQuery(Query);
                                    }


                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                                finally
                                {
                                    mySQL.CerrarConexion();
                                }

                                Comprimir comprimir = new Comprimir();
                                comprimir.Carpeta(@"D:\jfandino\Documentos\12. Reuniones");


                                String Query2 = string.Empty;
                                DataTable dataTable2 = new DataTable();
                                PostgreSql postgreSql = new PostgreSql();

                                try
                                {
                                    postgreSql.AbrirConexion("pc_datos");

                                    try
                                    {
                                        Query = "SELECT \"CodET\", \"CodUA\", MAX(\"NroInforme\") AS NroInforme, MAX(\"AñoInforme\") AS AnoInforme, " +
                                        "MAX(\"FechaCorte\") AS FechaCorte " +
                                        "FROM pc_consegui.\"AE_SeguimientoUnidadesAdtvas\" " +
                                        "GROUP BY \"CodET\", \"CodUA\" "+
                                        "ORDER BY \"CodET\", \"CodUA\"";

                                        dataTable2 = postgreSql.ConsultarDatos(Query);

                                        string A = string.Empty;
                                        
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
                            catch (Exception ex) { }

                            break;
                        case "2": //Descentralizadas
                            Console.WriteLine("**************************************************************************************");
                            Console.WriteLine("********************GENERAR ARCHIVOS ENTIDADES DESCENTRALIZADAS***********************");
                            Console.WriteLine("**************************************************************************************");
                            //Console.WriteLine(clsDescentralizadas.Proceso());
                            Console.WriteLine("**************************************************************************************");
                            break;
                        case "3": //Hospitales
                            Console.WriteLine("**************************************************************************************");
                            Console.WriteLine("**********************GENERAR ARCHIVOS ENTIDADES HOSPITALES***************************");
                            Console.WriteLine("**************************************************************************************");
                            //Console.WriteLine(clsHospitales.Proceso());
                            Console.WriteLine("**************************************************************************************");
                            break;
                        default:
                            throw new Exception("Tipo de entidad no valido");
                    }

                    Console.WriteLine("\r\n");
                    Console.WriteLine("**************************************************************************************");
                    Console.WriteLine("***********************GENERAR ARCHIVOS CETIL FINALIZADO *****************************");
                    Console.WriteLine("**************************************************************************************");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }
        }
    }
}
