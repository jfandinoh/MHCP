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

                Console.WriteLine(clsCentrales.Proceso());
                Console.WriteLine(clsDescentralizadas.Proceso());
                Console.WriteLine(clsDescentralizadas.Proceso());
                Console.WriteLine(clsHospitales.Proceso());
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
