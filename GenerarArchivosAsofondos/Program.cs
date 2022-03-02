using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using BibliotecaClases;
using System.IO;

namespace GenerarArchivosAsofondos
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                clsCentrales clsCentrales = new clsCentrales();
                clsDescentralizadas clsDescentralizadas = new clsDescentralizadas();
                clsHospitales clsHospitales = new clsHospitales();

                Console.WriteLine("**************************************************************************************");
                Console.WriteLine("************************GENERAR ARCHIVOS ENTIDADES CENTRALES**************************");
                Console.WriteLine("**************************************************************************************");
                Console.WriteLine(clsCentrales.Proceso());
                Console.WriteLine("**************************************************************************************\r\n\r\n\r\n\r\n");
                Console.WriteLine("**************************************************************************************");
                Console.WriteLine("********************GENERAR ARCHIVOS ENTIDADES DESCENTRALIZADAS***********************");
                Console.WriteLine("**************************************************************************************");
                Console.WriteLine(clsDescentralizadas.Proceso());
                Console.WriteLine("**************************************************************************************\r\n\r\n");
                Console.WriteLine("**************************************************************************************");
                Console.WriteLine("**********************GENERAR ARCHIVOS ENTIDADES HOSPITALES***************************");
                Console.WriteLine("**************************************************************************************");
                Console.WriteLine(clsHospitales.Proceso());
                Console.WriteLine("**************************************************************************************\r\n\r\n");
                Console.WriteLine("**************************************************************************************");
                Console.WriteLine("**********************GENERAR ARCHIVOS ASOFONDO FINALIZADO ***************************");
                Console.WriteLine("**************************************************************************************");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error generando archivos Asofondos. {0}", ex.Message));
                Console.ReadLine();
            }

        }
    }
}
