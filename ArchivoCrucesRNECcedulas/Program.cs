using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ArchivoCrucesRNECcedulas
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
                Console.WriteLine(string.Format("Error procesando archivo CRUCE_RNEC_CEDULAS.txt: {0}", ex.Message));
                Console.ReadLine();
            }
        }

        private static string Proceso()
        {
            string linea = string.Empty;
            try
            {
                string[] Archivos = { @"D:\jfandino\Documentos\05. CETIL\CRUCE_RNEC_CEDULAS_HOJA1_20210708\CRUCE_RNEC_CEDULAS_HOJA1_20210708.txt", @"D:\jfandino\Documentos\05. CETIL\CRUCE_RNEC_CEDULAS_HOJA2_20210708\CRUCE_RNEC_CEDULAS_HOJA2_20210708.txt" };
                string ArchivoNuevo = @"D:\jfandino\Documentos\05. CETIL\CRUCE_RNEC_CEDULAS_HOJA1_2_20210708.txt";
                string ArchivoErrores = @"D:\jfandino\Documentos\05. CETIL\Errores.txt";

                List<string> LsRegistros = new List<string>();
                int NumArchivo = 0;

                foreach (string archivo in Archivos.ToList())
                {
                    NumArchivo = NumArchivo + 1;
                    bool encabezado = true;
                    string NombreArchivo = Path.GetFileName(archivo);
                    using (StreamReader sr = new StreamReader(archivo, Encoding.GetEncoding(1252)))
                    {
                        using (FileStream fs = new FileStream(ArchivoNuevo, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                            {
                                while (sr.Peek() >= 0)
                                {
                                    linea = sr.ReadLine();
                                    List<string> Arreglo = linea.Split('\t').ToList();
                                    Arreglo.Add(NombreArchivo);
                                    
                                    if (encabezado)
                                    {
                                        if(NumArchivo < 2)
                                        {
                                            sw.WriteLine(string.Join(";", Arreglo));
                                        }
                                        encabezado = false;
                                    }
                                    else
                                    {
                                        //try
                                        //{
                                        //    if (!string.IsNullOrEmpty(Arreglo[6]))
                                        //    {
                                        //        Arreglo[6] = DateTime.ParseExact(Arreglo[6], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                        //    }
                                        //    if (!string.IsNullOrEmpty(Arreglo[7]))
                                        //    {
                                        //        Arreglo[7] = DateTime.ParseExact(Arreglo[7], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                        //    }
                                        //}
                                        //catch (Exception ex)
                                        //{
                                        //    using (FileStream fs1 = new FileStream(ArchivoErrores, FileMode.Append, FileAccess.Write))
                                        //    {
                                        //        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                        //        {
                                        //            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                        //        }
                                        //    }
                                        //}
                                        sw.WriteLine(string.Join(";", Arreglo));
                                    }
                                }
                            }
                        }
                    }
                }
                
                return "Proceso exitoso";
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("En la linea {0} se genera error: {1}",linea, ex.Message));
            }
        }
    }
}
