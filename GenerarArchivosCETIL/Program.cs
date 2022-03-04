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
                                mySQL.AbrirConexion(string.Empty);
                                string Query = string.Empty;
                                DataTable dataTable = new DataTable();
                                try
                                {
                                    Query = "DROP TABLE IF EXISTS EntidadesEntregadasCETIL;" +
                                        
                                    CREATE TABLE Usuarios20211105
                                        "SELECT a.cod_et,a.cod_ua,a.nombre_ua,a.tipo_organizacion, b.* " +
                                    "FROM bd_webpasivocol.tbl_directorio_unidades_adtvas a  " +
                                    "LEFT JOIN( " +
                                        "Select a.ET_CO_DANE, a.CodUA, a.ET_NOMBRE, a.ET_CO_DEPT, a.DP_NOMBRE, a.NomUnidadAdtva, max(a.NroInforme) NroInforme, max(a.FechaCorte) FechaCorte " +
                                        "from pasivocol.ae_tblconsolidadocontrolua_internet a " +
                                        "group by a.ET_CO_DANE, a.CodUA " +
                                    ") as b on concat(a.cod_et, a.cod_ua) = concat(b.ET_CO_DANE, b.CodUa) " +
                                    "WHERE a.estado_registro <> 'A' and b.ET_CO_DANE is not null; ";

                                    dataTable = mySQL.ConsultarDatos(Query);

                                    Console.WriteLine("consulta ok");
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
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
