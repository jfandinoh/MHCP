using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ArchivoCedulasExtranjeria
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(Proceso());
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error obteniendo cedulas de extranjeria: {0}", ex.Message));
            }
        }

        private static string Proceso()
        {
            try
            {
                string Archivo = @"D:\jfandino\Documentos\05. CETIL\Consultas Cruces 2022\Pasivocol_RegistrosPersona original.txt";
                string ArchivoNuevo = @"D:\jfandino\Documentos\05. CETIL\Consultas Cruces 2022\Pasivocol_RegistrosPersonaCE.txt";

                bool encabezado = true;
                List<string> LsRegistros = new List<string>();

                using (StreamReader sr = new StreamReader(Archivo))
                {
                    using (StreamWriter sw = new StreamWriter(ArchivoNuevo))
                    {
                        while (sr.Peek() >= 0)
                        {
                            string linea = sr.ReadLine();
                            string[] Arreglo = linea.Split(';');

                            if (encabezado)
                            {
                                encabezado = false;
                            }
                            else
                            {
                                if(Arreglo[0] == "E")
                                {
                                    sw.WriteLine(linea);
                                }
                            }
                        }

                    }
                }
                return "Proceso exitoso";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
