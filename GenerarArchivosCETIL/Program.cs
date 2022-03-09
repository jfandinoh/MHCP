using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


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
                    DataTable dataTableEntidadesGenerarInformacion = new DataTable();

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

                    try
                    {
                        clsInformacionEntidades clsInformacionEntidades = new clsInformacionEntidades();
                        dataTableEntidadesGenerarInformacion = clsInformacionEntidades.Consultar();
                    }
                    catch (Exception ex) 
                    {
                        throw new Exception(ex.Message);
                    }

                    switch (TipoEntidad)
                    {
                        case "1": //Centrales
                            Console.WriteLine("**************************************************************************************");
                            Console.WriteLine("************************GENERAR ARCHIVOS ENTIDADES CENTRALES**************************");
                            Console.WriteLine("**************************************************************************************");
                            clsCentrales clsCentrales = new clsCentrales();
                            Console.WriteLine(clsCentrales.Proceso(dataTableEntidadesGenerarInformacion, CantidadArchivos));
                            Console.WriteLine("**************************************************************************************");
                            break;
                        case "2": //Descentralizadas
                            Console.WriteLine("**************************************************************************************");
                            Console.WriteLine("********************GENERAR ARCHIVOS ENTIDADES DESCENTRALIZADAS***********************");
                            Console.WriteLine("**************************************************************************************");
                            clsDescentralizadas clsDescentralizadas = new clsDescentralizadas();
                            Console.WriteLine(clsDescentralizadas.Proceso(dataTableEntidadesGenerarInformacion, CantidadArchivos));
                            Console.WriteLine("**************************************************************************************");
                            break;
                        case "3": //Hospitales
                            Console.WriteLine("**************************************************************************************");
                            Console.WriteLine("**********************GENERAR ARCHIVOS ENTIDADES HOSPITALES***************************");
                            Console.WriteLine("**************************************************************************************");
                            clsHospitales clsHospitales = new clsHospitales();
                            Console.WriteLine(clsHospitales.Proceso(dataTableEntidadesGenerarInformacion, CantidadArchivos));
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
