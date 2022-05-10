using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace GenerarArchivosSUPPT_CursosVirtuales
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
                Console.WriteLine(string.Format("Error generando archivos: {0}", ex.Message));
            }
        }

        private static string Proceso()
        {
            try
            {
                string ArchivoEntidades = @"D:\jfandino\Documentos\04. SUPPT\Archivos Curso virtual SUPPT\ENtidadesPruebasSUPPT.csv";
                string ArchivoOriginal = @"D:\jfandino\Documentos\04. SUPPT\Archivos Curso virtual SUPPT\20220403000437_890983718 - modificada validada.csv";

                DataTable dataTable = TablaEntidades(ArchivoEntidades);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string ArchivoNuevo =string.Format(@"D:\jfandino\Documentos\04. SUPPT\Archivos Curso virtual SUPPT\20220403000437_{0}.csv", dataTable.Rows[i][1].ToString());

                    using (StreamReader sr = new StreamReader(ArchivoOriginal))
                    {
                        using (StreamWriter sw = new StreamWriter(ArchivoNuevo))
                        {
                            while (sr.Peek() >= 0)
                            {
                                string linea = sr.ReadLine();

                                if (linea.Contains("_891855029_"))
                                {
                                    linea = linea.Replace("_891855029_", string.Format("_{0}_", dataTable.Rows[i][1].ToString()));
                                }
                                if (linea.Contains("_HOSPITAL YOPAL E.S.E._"))
                                {
                                    linea = linea.Replace("_HOSPITAL YOPAL E.S.E._", string.Format("_{0}_", dataTable.Rows[i][0].ToString()));
                                }
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

        private static DataTable TablaEntidades(string Archivo)
        {
            try
            {
                DataTable dataTable = new DataTable();


                using (StreamReader sr = new StreamReader(Archivo))
                {
                    DataColumn NombreEntidad = new DataColumn("NombreEntidad");
                    DataColumn Nit = new DataColumn("Nit");
                    dataTable.Columns.Add(NombreEntidad);
                    dataTable.Columns.Add(Nit);

                    int aux = 0;
                    while (sr.Peek() >= 0)
                    {
                        string linea = sr.ReadLine();
                        string[] Arreglo = linea.Split(';');
                        if (aux > 0)
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["NombreEntidad"] = Arreglo[0];
                            dataRow["Nit"] = Arreglo[1];
                            dataTable.Rows.Add(dataRow);
                        }
                        aux = aux + 1;
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
