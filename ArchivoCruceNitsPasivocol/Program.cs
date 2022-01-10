using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ArchivoCruceNitsPasivocol
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
                Console.WriteLine(string.Format("Error procesando archivo CRUCE_NITS_PASIVOCOL.txt: {0}", ex.Message));
            }
        }

        private static string Proceso()
        {
            try
            {
                string Archivo = @"D:\jfandino\Documentos\05. CETIL\CRUCE_NITS_PASIVOCOL\CRUCE_NITS_PASIVOCOL.txt";
                string ArchivoNuevo = @"D:\jfandino\Documentos\05. CETIL\CRUCE_NITS_PASIVOCOL\CRUCE_NITS_PASIVOCOL_modificado.txt";

                bool encabezado = true;
                List<string> LsRegistros = new List<string>();

                using (StreamReader sr = new StreamReader(Archivo,Encoding.GetEncoding(1252)))
                {
                    using (FileStream fs = new FileStream(ArchivoNuevo,FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        while (sr.Peek() >= 0)
                        {
                            string linea = sr.ReadLine();
                            string[] Arreglo = linea.Split('\t');

                            if (encabezado)
                            {
                                sw.WriteLine(string.Join(";", Arreglo));
                                encabezado = false;
                            }
                            else
                            {
                                if(!string.IsNullOrEmpty(Arreglo[2]))
                                {
                                    Arreglo[2] = DateTime.ParseExact(Arreglo[2], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                }
                                if (!string.IsNullOrEmpty(Arreglo[3]))
                                {
                                    Arreglo[3] = DateTime.ParseExact(Arreglo[3], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                }
                                if (!string.IsNullOrEmpty(Arreglo[4]))
                                {
                                    Arreglo[4] = DateTime.ParseExact(Arreglo[4], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                }
                                if (!string.IsNullOrEmpty(Arreglo[5]))
                                {
                                    Arreglo[5] = DateTime.ParseExact(Arreglo[5], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                }
                                sw.WriteLine(string.Join(";", Arreglo));
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
