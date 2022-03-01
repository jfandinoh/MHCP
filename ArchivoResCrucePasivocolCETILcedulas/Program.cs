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
                string[] Archivos = { @"D:\jfandino\Documentos\05. CETIL\Cruce2022\RES_PASIVOCOLREGISTROSPERSONA_2022_20220209111948.CSV" };
                string ArchivoR2 = @"D:\jfandino\Documentos\05. CETIL\Cruce2022\RegistrosPersona2022\RES_PASIVOCOLREGISTROSPERSONA_2022_R2.txt";
                string ArchivoR3 = @"D:\jfandino\Documentos\05. CETIL\Cruce2022\RegistrosPersona2022\RES_PASIVOCOLREGISTROSPERSONA_2022_R3.txt";
                string ArchivoR4 = @"D:\jfandino\Documentos\05. CETIL\Cruce2022\RegistrosPersona2022\RES_PASIVOCOLREGISTROSPERSONA_2022_R4.txt";
                string ArchivoR5 = @"D:\jfandino\Documentos\05. CETIL\Cruce2022\RegistrosPersona2022\RES_PASIVOCOLREGISTROSPERSONA_2022_R5.txt";
                string ArchivoR6 = @"D:\jfandino\Documentos\05. CETIL\Cruce2022\RegistrosPersona2022\RES_PASIVOCOLREGISTROSPERSONA_2022_R6.txt";
                string ArchivoErrores = @"D:\jfandino\Documentos\05. CETIL\Cruce2022\RegistrosPersona2022\Errores";

                List<string> LsRegistros = new List<string>();
                int NumArchivo = 0;
                int NumeroLinea = 0;

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
                            Console.WriteLine(string.Format("Linea numero {0}", NumeroLinea+=1));
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
                                                        DateTime fecha = StringToDatetime(Arreglo[3]);
                                                        if(fecha != new DateTime())
                                                        {
                                                            Arreglo[3] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[3]));
                                                        }
                                                        
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[4]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[4]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[4] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[4]));
                                                        }
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
                                                        DateTime fecha = StringToDatetime(Arreglo[3]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[3] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[3]));
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[4]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[4]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[4] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[4]));
                                                        }
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
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[5]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[5]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[5] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[5]));
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[6]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[6]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[6] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[6]));
                                                        }
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
                                                encabezado3 = false;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[5]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[5]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[5] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[5]));
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[6]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[6]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[6] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[6]));
                                                        }
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
                                case "4":
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
                                                        DateTime fecha = StringToDatetime(Arreglo[14]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[14] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[14]));
                                                        }
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
                                                        DateTime fecha = StringToDatetime(Arreglo[14]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[14] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[14]));
                                                        }
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
                                case "5":
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
                                                    ls.Add("NIT ESTABLECIMIENTO");
                                                    ls.Add("NIVEL ESTABLECIMENTO");
                                                    ls.Add("MUNICIPIO");
                                                    ls.Add("ARCHIVO");
                                                    sw.WriteLine(string.Join(";", ls));
                                                }
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(Arreglo[4]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[4]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[4] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[4]));
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[5]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[5]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[5] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[5]));
                                                        }
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
                                                        DateTime fecha = StringToDatetime(Arreglo[4]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[4] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[4]));
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[5]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[5]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[5] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[5]));
                                                        }
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
                                case "6":
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
                                                        DateTime fecha = StringToDatetime(Arreglo[9]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[9] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[9]));
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[10]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[10]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[10] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[10]));
                                                        }
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
                                                        DateTime fecha = StringToDatetime(Arreglo[9]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[9] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[9]));
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(Arreglo[10]))
                                                    {
                                                        DateTime fecha = StringToDatetime(Arreglo[10]);
                                                        if (fecha != new DateTime())
                                                        {
                                                            Arreglo[10] = fecha.ToString("yyyy-MM-dd");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("La cadena {0} no tiene un formato fecha valido", Arreglo[10]));
                                                        }
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

                return "Proceso finalizado";
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("En la linea {0} se genera error: {1}", linea, ex.Message));
            }
        }

        private static DateTime StringToDatetime(string strFecha)
        {
            string[] formatos = new[] { "yyyyMMdd", "yyyy-MM-dd", "yyyy/MM/dd" };
            DateTime fechaValida;
            DateTime.TryParseExact(strFecha, formatos, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fechaValida);
            return fechaValida;
        }
    }
}
