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
                Console.WriteLine(ValidarTiposDocumento());
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0}",ex.Message));
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

        private static string ValidarTiposDocumento()
        {
            try
            {
                string Archivo = @"D:\jfandino\Documentos\04. SUPPT\Bogota\20220509110523_899999061_AjustadoPruebas.csv";
                string ArchivoNuevo = @"D:\jfandino\Documentos\04. SUPPT\Bogota\20220509110523_899999061_AjustadoPruebas_TD.csv";

                List<string> LsRegistrosR2 = new List<string>();
                List<string> LsRegistrosR4NoEncontrados = new List<string>();

                using (StreamReader sr = new StreamReader(Archivo))
                {
                    //using (StreamWriter sw = new StreamWriter(ArchivoNuevo))
                    //{
                    int AuxLinea = 0;
                    while (sr.Peek() >= 0)
                    {
                        AuxLinea += 1;
                        string linea = sr.ReadLine();
                        if (linea.Substring(0, 1) == "2")
                        {
                            string[] Arreglo = linea.Split(';');

                            if (!LsRegistrosR2.Contains(Arreglo[1] + ";" + Arreglo[2]))
                            {
                                LsRegistrosR2.Add(Arreglo[1] + ";" + Arreglo[2]);
                                Console.WriteLine(string.Format("Registro agregado en la linea {1}", Arreglo[1] + ";" + Arreglo[2], AuxLinea.ToString()));
                            }
                            else
                            {
                                Console.WriteLine(string.Format("Ya existe un registro 2 con la cédula {0} en la linea {1}", Arreglo[2], AuxLinea.ToString()));
                            }
                        }
                        else if (linea.Substring(0, 1) == "4")
                        {
                            string[] Arreglo = linea.Split(';');

                            if (!LsRegistrosR2.Contains(Arreglo[13] + ";" + Arreglo[14]))
                            {
                                Console.WriteLine(string.Format("Registro {0} NO encontrado en lista de documentos R2 en la linea {1}", Arreglo[13] + ";" + Arreglo[14], AuxLinea.ToString()));
                                LsRegistrosR4NoEncontrados.Add(Arreglo[13] + ";" + Arreglo[14]);
                            }
                            else
                            {
                                Console.WriteLine(string.Format("Registro {0} de la linea {1} fue encontrado con exito en lista de documentos R2", Arreglo[13] + ";" + Arreglo[14], AuxLinea.ToString()));
                            }
                        }
                    }

                    //}
                }

                Console.WriteLine("Registros no encontrados");
                foreach(string registro in LsRegistrosR4NoEncontrados)
                {
                    Console.WriteLine(registro);
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
