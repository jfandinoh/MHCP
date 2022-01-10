using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ArchivoResCrucePasivocolCETILcedulas
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
                Console.WriteLine(string.Format("Error procesando archivo RES_CRUCE_PASIVOCOL_CEDULAS.csv: {0}", ex.Message));
                Console.ReadLine();
            }
        }

        private static string Proceso()
        {
            string linea = string.Empty;
            try
            {
                string[] Archivos = { @"D:\jfandino\Documentos\05. CETIL\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA1_20210531100840\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA1_20210531100840.CSV", @"D:\jfandino\Documentos\05. CETIL\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA2_20210531112255\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA2_20210531112255.CSV"};
                string ArchivoR2 = @"D:\jfandino\Documentos\05. CETIL\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA2_20210531112255\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA_R2.txt";
                string ArchivoR3 = @"D:\jfandino\Documentos\05. CETIL\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA2_20210531112255\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA_R3.txt";
                string ArchivoR4 = @"D:\jfandino\Documentos\05. CETIL\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA2_20210531112255\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA_R4.txt";
                string ArchivoR5 = @"D:\jfandino\Documentos\05. CETIL\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA2_20210531112255\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA_R5.txt";
                string ArchivoR6 = @"D:\jfandino\Documentos\05. CETIL\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA2_20210531112255\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA_R6.txt";
                string ArchivoErrores = @"D:\jfandino\Documentos\05. CETIL\RES_CRUCE_PASIVOCOL_CEDULAS_HOJA2_20210531112255\Errores";

                List<string> LsRegistros = new List<string>();
                int NumArchivo = 0;

                foreach (string archivo in Archivos.ToList())
                {
                    NumArchivo = NumArchivo + 1;
                    bool encabezado2 = true;
                    bool encabezado3 = true;
                    bool encabezado4 = true;
                    bool encabezado5 = true;
                    bool encabezado6 = true;
                    string NombreArchivo = Path.GetFileName(archivo);
                    using (StreamReader sr = new StreamReader(archivo, Encoding.GetEncoding(1252)))
                    {
                        while (sr.Peek() >= 0)
                        {
                            linea = sr.ReadLine();
                            linea = linea.Replace("_;_", "¬");
                            linea = linea.Replace(";", "/");
                            List<string> Arreglo = linea.Split('¬').ToList();
                            Arreglo.Add(NombreArchivo);

                            switch (linea.Substring(0, 1))
                            {
                                case "2":
                                    using (FileStream fs = new FileStream(ArchivoR2, FileMode.Append, FileAccess.Write))
                                    {
                                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                                        {
                                            if (encabezado2)
                                            {
                                                if (NumArchivo < 2)
                                                {
                                                    List<string> ls = new List<string>();
                                                    ls.Add("TIPO DE REGISTRO");
                                                    ls.Add("TIPO DE DOCUMENTO BENEFICIARIO");
                                                    ls.Add("DOCUMENTO BENEFICIARIO");
                                                    ls.Add("FECHA FALLECIMIENTO MINSALUD");
                                                    ls.Add("FECHA SALARIO BASE BONO PENSIONAL");
                                                    ls.Add("SALARIO BASE BONO PENSIONAL CALCULADO");
                                                    ls.Add("TASA BONO");
                                                    ls.Add("ARCHIVO");
                                                    sw.WriteLine(string.Join(";", ls));
                                                }
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[3]))
                                                    {
                                                        Arreglo[3] = DateTime.ParseExact(Arreglo[3], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[4]))
                                                    {
                                                        Arreglo[4] = DateTime.ParseExact(Arreglo[4], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores + "R2.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                                encabezado2 = false;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[3]))
                                                    {
                                                        Arreglo[3] = DateTime.ParseExact(Arreglo[3], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[4]))
                                                    {
                                                        Arreglo[4] = DateTime.ParseExact(Arreglo[4], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores+"R2.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                            }
                                        }
                                    }
                                    break;
                                case "3":
                                    using (FileStream fs = new FileStream(ArchivoR3, FileMode.Append, FileAccess.Write))
                                    {
                                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                                        {
                                            if (encabezado3)
                                            {
                                                if (NumArchivo < 2)
                                                {
                                                    List<string> ls = new List<string>();
                                                    ls.Add("TIPO DE REGISTRO");
                                                    ls.Add("TIPO DE DOCUMENTO BENEFICIARIO");
                                                    ls.Add("DOCUMENTO BENEFICIARIO");
                                                    ls.Add("NIT ENTIDAD REPORTA AFILIACION");
                                                    ls.Add("NOMBRE ENTIDAD REPORTA AFILIACION");
                                                    ls.Add("FECHA AFILIACION");
                                                    ls.Add("FECHA ULTIMO REPORTE OBP");
                                                    ls.Add("ARCHIVO");
                                                    sw.WriteLine(string.Join(";", ls));
                                                }
                                                if (!string.IsNullOrEmpty(Arreglo[5]))
                                                {
                                                    Arreglo[5] = DateTime.ParseExact(Arreglo[5], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                }
                                                if (!string.IsNullOrEmpty(Arreglo[6]))
                                                {
                                                    Arreglo[6] = DateTime.ParseExact(Arreglo[6], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                                encabezado3 = false;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[5]))
                                                    {
                                                        Arreglo[5] = DateTime.ParseExact(Arreglo[5], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[6]))
                                                    {
                                                        Arreglo[6] = DateTime.ParseExact(Arreglo[6], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores + "R3.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                            }
                                        }
                                    }
                                    break;
                                case "444":
                                    using (FileStream fs = new FileStream(ArchivoR4, FileMode.Append, FileAccess.Write))
                                    {
                                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                                        {
                                            if (encabezado4)
                                            {
                                                if (NumArchivo < 2)
                                                {
                                                    List<string> ls = new List<string>();
                                                    ls.Add("TIPO DE REGISTRO");
                                                    ls.Add("NIT ENTIDAD CERTIFICADORA");
                                                    ls.Add("SECCIONAL ENTIDAD CERTIFICADORA");
                                                    ls.Add("NOMBRE ENTIDAD CERTIFICADORA");
                                                    ls.Add("NUMERO DE CERTIFICACIÓN O INDICIO");
                                                    ls.Add("TIPO DE DOCUMENTO BENEFICIARIO");
                                                    ls.Add("NÚMERO DE DOCUMENTO BENEFICIARIO");
                                                    ls.Add("TIPO DE INFORMACION");
                                                    ls.Add("NIT SECCIONAL EMPLEADOR");
                                                    ls.Add("CODIGO SECCIONAL EMPLEADOR");
                                                    ls.Add("NOMBRE SECCIONAL EMPLEADOR");
                                                    ls.Add("SECTOR SECCIONAL EMPLEADOR");
                                                    ls.Add("NATURALEZA SECCIONAL EMPLEADOR");
                                                    ls.Add("SUB SECTOR ENTIDAD TERRITORIAL");
                                                    ls.Add("FECHA DE LA CERTIFICACIÓN");
                                                    ls.Add("FUENTE DE LA CERTIFICACIÓN");
                                                    ls.Add("ARCHIVO");
                                                    sw.WriteLine(string.Join(";", ls));
                                                }
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[14]))
                                                    {
                                                        Arreglo[14] = DateTime.ParseExact(Arreglo[14], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores + "R4.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }

                                                sw.WriteLine(string.Join(";", Arreglo));
                                                encabezado4 = false;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[14]))
                                                    {
                                                        Arreglo[14] = DateTime.ParseExact(Arreglo[14], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores + "R4.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                            }
                                        }
                                    }
                                    break;
                                case "555":
                                    using (FileStream fs = new FileStream(ArchivoR5, FileMode.Append, FileAccess.Write))
                                    {
                                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                                        {
                                            if (encabezado5)
                                            {
                                                if (NumArchivo < 2)
                                                {
                                                    List<string> ls = new List<string>();
                                                    ls.Add("TIPO DE REGISTRO");
                                                    ls.Add("NUMERO DE CERTIFICACIÓN O INDICIO");
                                                    ls.Add("TIPO DE INFORMACION");
                                                    ls.Add("TIPO DE NOVEDAD");
                                                    ls.Add("FECHA INICIAL");
                                                    ls.Add("FECHA FINAL");
                                                    ls.Add("CARGO");
                                                    ls.Add("REALIZO APORTES");
                                                    ls.Add("NIT FONDO APORTES");
                                                    ls.Add("NOMBRE FONDO APORTES");
                                                    ls.Add("HORAS LABORADAS");
                                                    ls.Add("DIAS DE INTERRUPCION");
                                                    ls.Add("FUENTE DE RECURSOS");
                                                    ls.Add("NIT ESTABLECIMIENTO");
                                                    ls.Add("NIVEL ESTABLECIMENTO");
                                                    ls.Add("MUNICIPIO");
                                                    ls.Add("FACTORES DE APORTE");
                                                    ls.Add("SESIONES ASISTIDAS");
                                                    ls.Add("SESIONES NO ASISTIDAS CON EXCUSA");
                                                    ls.Add("TOTAL DE SESIONES ASISTIDAS Y NO ASISTIDAS CON EXCUSA");
                                                    ls.Add("ARCHIVO");
                                                    sw.WriteLine(string.Join(";", ls));
                                                }
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[4]))
                                                    {
                                                        Arreglo[4] = DateTime.ParseExact(Arreglo[4], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[5]))
                                                    {
                                                        Arreglo[5] = DateTime.ParseExact(Arreglo[5], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores + "R5.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                                encabezado5 = false;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[4]))
                                                    {
                                                        Arreglo[4] = DateTime.ParseExact(Arreglo[4], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[5]))
                                                    {
                                                        Arreglo[5] = DateTime.ParseExact(Arreglo[5], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores + "R5.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                            }
                                        }
                                    }
                                    break;
                                case "666":
                                    using (FileStream fs = new FileStream(ArchivoR6, FileMode.Append, FileAccess.Write))
                                    {
                                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                                        {
                                            if (encabezado6)
                                            {
                                                if (NumArchivo < 2)
                                                {
                                                    List<string> ls = new List<string>();
                                                    ls.Add("TIPO DE REGISTRO");
                                                    ls.Add("NUMERO DE CERTIFICACIÓN O INDICIO");
                                                    ls.Add("TIPO DE INFORMACION");
                                                    ls.Add("PERIODO");
                                                    ls.Add("SALARIO INTEGRAL");
                                                    ls.Add("FACTOR SALARIAL");
                                                    ls.Add("VALOR DEVENGADO");
                                                    ls.Add("REALIZO APORTES");
                                                    ls.Add("PERIODICIDAD DEL FACTOR");
                                                    ls.Add("FECHA INICIAL DE CAUSACION");
                                                    ls.Add("FECHA FINAL DE CAUSACION");
                                                    ls.Add("ARCHIVO");
                                                    sw.WriteLine(string.Join(";", ls));
                                                }

                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[9]))
                                                    {
                                                        Arreglo[9] = DateTime.ParseExact(Arreglo[9], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[10]))
                                                    {
                                                        Arreglo[10] = DateTime.ParseExact(Arreglo[10], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores + "R6.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                                encabezado6 = false;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[9]))
                                                    {
                                                        Arreglo[9] = DateTime.ParseExact(Arreglo[9], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[10]))
                                                    {
                                                        Arreglo[10] = DateTime.ParseExact(Arreglo[10], "yyyyMMdd", null).ToString("yyyy/MM/dd");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    using (FileStream fs1 = new FileStream(ArchivoErrores + "R6.txt", FileMode.Append, FileAccess.Write))
                                                    {
                                                        using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.UTF8))
                                                        {
                                                            sw1.WriteLine(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
                                                        }
                                                    }
                                                }
                                                sw.WriteLine(string.Join(";", Arreglo));
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }

                return "Proceso exitoso";
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
            }
        }
    }
}
