using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace EliminarRegistrosReptidosArchivoSUPPT
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
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("Error eliminando archivos repetidos: {0}",ex.Message));
            }
        }

        private static string Proceso()
        {
            try
            {
                string Archivo = @"D:\jfandino\Descargas\ArchivoBogota 20220509\20220509110523_899999061.csv";
                string ArchivoNuevo = @"D:\jfandino\Descargas\ArchivoBogota 20220509\20220509110523_899999061_AjustadoPruebas.csv";

                List<string> LsRegistros = new List<string>();

                using (StreamReader sr = new StreamReader(Archivo))
                {
                    using(StreamWriter sw = new StreamWriter(ArchivoNuevo))
                    {
                        while (sr.Peek() >= 0)
                        {
                            string linea = sr.ReadLine();
                            if(linea.Substring(0,1)=="2")
                            {
                                string[] Arreglo = linea.Split(';');

                                if (!LsRegistros.Contains(Arreglo[2]))
                                {
                                    LsRegistros.Add(Arreglo[2]);
                                    if (linea.Contains("_11931022_"))
                                    {
                                        linea = linea.Replace("_11931022_", "_19931022_");
                                    }
                                    sw.WriteLine(linea);
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("Ya existe un registro 2 con la cédula {0}", Arreglo[2]));
                                }
                            }
                            else
                            {
                                sw.WriteLine(linea);
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
