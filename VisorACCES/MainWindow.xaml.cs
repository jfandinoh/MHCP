using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
//using para excel
using Excel = Microsoft.Office.Interop.Excel;

namespace VisorACCES
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        conectar conectar = new conectar();
        DataSet DataSet = new DataSet();
        DataTable DataTable = new DataTable();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

            string estado= conectar.AbrirConexion();
            MessageBox.Show(estado);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string estado = conectar.CerrarConexion();
            MessageBox.Show(estado);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DataTable = conectar.consultar();

            //Mostrar DataTable en un listView
            GridView GridView = new GridView();
            if (DataTable.Rows.Count != 0)
            {
                foreach (DataColumn DataColumn in DataTable.Columns)
                {
                    GridViewColumn GridViewColumn = new GridViewColumn();
                    GridViewColumn.DisplayMemberBinding = new Binding(DataColumn.ColumnName);
                    GridViewColumn.Header = DataColumn.ColumnName;
                    GridView.Columns.Add(GridViewColumn);
                }
                Tabla.View = GridView;
                Tabla.DataContext = DataTable;

                Binding Binding = new Binding();
                Tabla.SetBinding(ListView.ItemsSourceProperty, Binding);

                ////Filtrar filas en DataTable
                //DataRow[] filtro = DataTable.Select("Id = 1");

                ////Borrar datos del DataTable
                //DataTable.Clear(); 
            }
        }

        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application app = new Excel.Application();
            app.Visible = true;
            Excel.Workbook wb = app.Workbooks.Add(1);
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = "Tabla 1";
            int incolumn = 1;
            int inrows = 1;
            int inHeaderLength=0;

            DataSet.Tables.Add(DataTable); 
            foreach (DataTable dt in DataSet.Tables)
            {
                Excel.Worksheet ws2 = wb.Sheets.Add(System.Reflection.Missing.Value, wb.Sheets[wb.Sheets.Count], 1, System.Reflection.Missing.Value);
                ws2.Name = "Tabla 2";

                //Escribir nombre columna
                for(int i = 0; i<DataTable.Columns.Count; i ++ )
                {
                    ws.Cells[inHeaderLength + 1, i + 1] = DataTable.Columns[i].ColumnName.ToUpper();
                }
                
                //Escribir Celdas
                for (int m = 0; m < DataTable.Rows.Count; m ++ )
                {
                    for (int n = 0; n < DataTable.Columns.Count; n++)
                    {
                        incolumn = n  + 1;
                        inrows = inHeaderLength + 2 + m;
                        ws.Cells[inrows, incolumn] = DataTable.Rows[m].ItemArray[n].ToString();
                        //if(m%2 ==0)
                        //{
                        //    ws.get_Range("A"+ inrows.ToString(), )
                        //}
                    }
                }

                //i = 1;
                //foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                //{
                //    ws.Cells[i2, i] = lvs.Text;
                //    i++;
                //}
                //i2++;
            }
        }
    }
}
