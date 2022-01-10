using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using para datatable
using System.Data;
//using para excel
using excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;

namespace BibliotecaClases
{
    public class Excel
    {
        public Excel()
        {
            System.Console.WriteLine("Clase Excel");
        }

        /*
         * Esta función obtiene el rango de celdas con datos, las recorre
         * extrae e imprime en consola el dato que contiene cada celda.
         */
        public DataTable ExtraerDatos(string ArchivoExcel)
        {
            excel.Application objexcel = new excel.Application();
            excel.Workbook objlibro;
            excel.Worksheet objhoja;
            DataTable dt = new DataTable();

            try
            {
                object missing = System.Reflection.Missing.Value;
                objlibro = objexcel.Workbooks.Open(ArchivoExcel, missing, missing, missing, missing,
                                        missing, missing, missing, missing, missing, missing,
                                        missing, missing, missing, missing);
                objhoja = (Microsoft.Office.Interop.Excel.Worksheet)objlibro.Worksheets.get_Item(1);

                int ultimaFilaConDatos = objhoja.get_Range("A" + objhoja.Rows.Count).get_End(Microsoft.Office.Interop.Excel.XlDirection.xlUp).Row;
                int ultimaColumnaConDatos = objhoja.Cells.Find("*", missing,
                    Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues,
                    Microsoft.Office.Interop.Excel.XlLookAt.xlWhole,
                    Microsoft.Office.Interop.Excel.XlSearchOrder.xlByColumns,
                    Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious, false, false, missing).Column;

                //System.Console.WriteLine("Nombre campos");
                for (int row = 1; row <= 1; row++)
                {
                    for (int col = 1; col <= ultimaColumnaConDatos; col++)
                    {
                        // lectura como cadena
                        string str_value = ((excel.Range)objhoja.Cells[row, col] as excel.Range).Value2.ToString();
                        dt.Columns.Add(str_value);
                        //System.Console.WriteLine(str_value);
                    }
                }

                //System.Console.WriteLine("Información campos");
                for (int row = 2; row <= ultimaFilaConDatos; row++)
                {
                    string[] datos = new string[ultimaColumnaConDatos];  //ultimaColumnaConDatos elementos
                    for (int col = 1; col <= ultimaColumnaConDatos; col++)
                    {
                        string str_value;
                        // lectura como cadena        
                        if (((excel.Range)objhoja.Cells[row, col] as excel.Range).Value2 != null)
                        {
                            str_value = ((excel.Range)objhoja.Cells[row, col] as excel.Range).Value2.ToString();
                        }
                        else
                        {
                            str_value = "";   
                        }               
                        //System.Console.WriteLine(str_value);
                        datos[col - 1] = str_value;
                    }
                    dt.Rows.Add(datos);
                }                
                objlibro.Close(false, missing, missing);
                objhoja = null;
                objlibro = null;
                objexcel.Quit();
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(objhoja);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(objlibro);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(objexcel);
            }
            catch (Exception e)
            {
                string error = e.Message.ToString(); 
            }
            return dt;
        }

        /*
         * Esta función busca un texto indicado, en el archivo excel seleccionado, y obtiene toda la información que
         * tiene en la fila donde se encuentra.
         */
        public void BuscarEnArchivoExcel(string TextoBuscar)
        {
            excel.Application objexcel = new excel.Application();
            excel.Workbook objlibro;
            excel.Worksheet objhoja;

            try
            {
                object missing = System.Reflection.Missing.Value;
                objlibro = objexcel.Workbooks.Open("C:\\prueba.xlsm", missing, missing, missing, missing,
                                        missing, missing, missing, missing, missing, missing,
                                        missing, missing, missing, missing);
                objhoja = (Microsoft.Office.Interop.Excel.Worksheet)objlibro.Worksheets.get_Item(1);

                excel.Range oRng = ObtenerFila(TextoBuscar, objhoja);
                if (oRng != null)
                {
                    //Extraer de la fila la columna indicada
                    excel.Range name = (excel.Range)objhoja.Cells[oRng.Row, 3];
                    string strname = name.get_Value(missing).ToString();
                    string nombre = strname;
                }
                else
                {
                    //MessageBox.Show("El usuario no existe en la base de datos", "Información");
                }
                objlibro.Close(false, missing, missing);
                objhoja = null;
                objlibro = null;
                objexcel.Quit();
            }
            catch { }
        }

        /*
         * Esta función obtiene el rango de celdas que han sido usadas, las recorre
         * e imprime en consola el dato que contiene cada celda.
         */
        public void RecorreCeldas()
        {
            excel.Application objexcel = new excel.Application();
            excel.Workbook objlibro;
            excel.Worksheet objhoja;
            try
            {
                object missing = System.Reflection.Missing.Value;
                objlibro = objexcel.Workbooks.Open("C:\\prueba.xlsm", missing, missing, missing, missing,
                                        missing, missing, missing, missing, missing, missing,
                                        missing, missing, missing, missing);
                objhoja = (Microsoft.Office.Interop.Excel.Worksheet)objlibro.Worksheets.get_Item(1);

                excel.Range range = objhoja.UsedRange;
                // leer las celdas
                int rows = range.Rows.Count;
                int cols = range.Columns.Count;

                for (int row = 1; row <= rows; row++)
                {
                    for (int col = 1; col <= cols; col++)
                    {

                        // lectura como cadena
                        string str_value = (range.Cells[row, col] as excel.Range).Value2.ToString();
                        System.Console.WriteLine(str_value);
                    }
                }

                objlibro.Close(false, missing, missing);
                objhoja = null;
                objlibro = null;
                objexcel.Quit();
            }
            catch { }
        }
        
        /*
         * Esta función obtiene el número de la ultima fila con datos en el archivo excel.
         */
        public int ObtenerNumeroFilas()
        {
            excel.Application objexcel = new excel.Application();
            excel.Workbook objlibro;
            excel.Worksheet objhoja;
            int ultimaFilaConDatos= 0;
            try
            {
                object missing = System.Reflection.Missing.Value;
                objlibro = objexcel.Workbooks.Open("C:\\prueba.xlsm", missing, missing, missing, missing,
                                        missing, missing, missing, missing, missing, missing,
                                        missing, missing, missing, missing);
                objhoja = (Microsoft.Office.Interop.Excel.Worksheet)objlibro.Worksheets.get_Item(1);
                ultimaFilaConDatos = objhoja.get_Range("A" + objhoja.Rows.Count).get_End(Microsoft.Office.Interop.Excel.XlDirection.xlUp).Row;
                
                objlibro.Close(false, missing, missing);
                objhoja = null;
                objlibro = null;
                objexcel.Quit();
            }
            catch{}

            return ultimaFilaConDatos;
        }

        /*
         * Esta función obtiene el número de columnas con datos en el archivo excel.
         */
        public int ObtenerNumeroColumnas()
        {
            excel.Application objexcel = new excel.Application();
            excel.Workbook objlibro;
            excel.Worksheet objhoja;
            int ultimaColumnaConDatos = 0;
            try
            {
                object missing = System.Reflection.Missing.Value;
                objlibro = objexcel.Workbooks.Open("C:\\prueba.xlsm", missing, missing, missing, missing,
                                        missing, missing, missing, missing, missing, missing,
                                        missing, missing, missing, missing);
                objhoja = (Microsoft.Office.Interop.Excel.Worksheet)objlibro.Worksheets.get_Item(1);

                ultimaColumnaConDatos = objhoja.Cells.Find("*", missing,
                    Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues,
                    Microsoft.Office.Interop.Excel.XlLookAt.xlWhole,
                    Microsoft.Office.Interop.Excel.XlSearchOrder.xlByColumns,
                    Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious, false, false, missing).Column;

                objlibro.Close(false, missing, missing);
                objhoja = null;
                objlibro = null;
                objexcel.Quit();
            }
            catch { }

            return ultimaColumnaConDatos;
        }

        /*
         * Esta función obtiene la fila donde se encuentra el texto buscado
         */
        private excel.Range ObtenerFila(string TextoBuscar, excel.Worksheet objhoja)
        {
            object missing = System.Reflection.Missing.Value;
            excel.Range currentFind = null;
            excel.Range firstFind = null;
            currentFind = objhoja.get_Range("A1", "AM1000").Find(TextoBuscar, missing,
                           Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues,
                           Microsoft.Office.Interop.Excel.XlLookAt.xlWhole,
                           Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                           Microsoft.Office.Interop.Excel.XlSearchDirection.xlNext, false, missing, missing);
            return currentFind;
        }
    }
}
