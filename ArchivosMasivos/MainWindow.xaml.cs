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
////using para IO archivos
using System.IO;
////using para datatable
using System.Data;
////using para open file dialog
using Microsoft.Win32;
////using para usar BackGroundWorker
using System.ComponentModel;
////using para archivo configuracion (.config)
using System.Configuration;
////using para hilos
using System.Threading; 
////Using para diagnostics
using System.Diagnostics;
////Using para biblioteca de clases JAFH
using BibliotecaClases;

namespace CorreosMasivos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Declaración de variables globales
        DataTable DataTable;
        Excel Excel = new Excel();
        Word Word = new Word();
        SeleccionMenu SeleccionMenu = new SeleccionMenu();
        string IDmail;
        List<string> EnviarAdjuntos = new List<string>();

        //Declaración tarea segundo plano
        BackgroundWorker tarea = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            cmbColumnaID.IsEnabled = false;
            btnWord.IsEnabled = false;
            Btn_GenerarArchivos.IsEnabled = false;
            Btn_Enviar.IsEnabled = false;
        }

        public MainWindow(string email)
        {
            this.IDmail = email;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool funcionar = Convert.ToBoolean(ConfigurationManager.AppSettings["Setting"].ToString());
            if (!funcionar)
            { 
                throw new Exception("Error en el programa");
            }
        }
        
        private void Btn_Enviar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Se envía correo por Outlook configurado en el equipo o indicado en el archivo de configuraciones", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (!string.IsNullOrEmpty(txtExcel.Text))
            {
                Outlook Outlook = new Outlook();
                List<string> LsAdjuntos = EnviarAdjuntos;
                if (LsAdjuntos.Count <= 0)
                {
                    if(MessageBox.Show("No seleccionó ningun archivo, desea continuar?", "Archivo adjunto", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        //Se envía correo con archivo adjunto
                        Outlook.EnviarMail(IDmail, LsAdjuntos, DataTable);
                    }
                }
                else
                {
                    //Se envía correo con archivo adjunto
                    Outlook.EnviarMail(IDmail, LsAdjuntos, DataTable); 
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar archivo origen", "Enviar correo masivo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            MessageBox.Show("Se envió correo satisfactoriamente.", "Enviar correo masivo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx;*.xls;*.xlsm"; //"All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtExcel.Text = openFileDialog1.FileName;
                    string archivoExcel= txtExcel.Text.ToString();   
                    //Tarea segundo plano Forma de hacerlo 1
                    Thread thread = new Thread(() =>
                    {
                        //Obtener la información del archivo de excel
                        try
                        {
                            DataTable = Excel.ExtraerDatos(archivoExcel);
                        }                        
                        catch (Exception ex)
                        {
                            throw new System.ArgumentException("Error cargando la información. " + ex.Message.ToString());
                        }
                        Action action = () =>
                        {
                            stpanelCarga.Visibility = Visibility.Hidden; 
                            pgbcarga.Visibility = Visibility.Hidden;
                            txtcarga.Visibility = Visibility.Hidden;
                            CargarArchivoExcelFin();
                        };
                        this.Dispatcher.BeginInvoke(action);
                    });
                    thread.Start();
                    stpanelCarga.Visibility = Visibility.Visible; 
                    pgbcarga.Visibility = Visibility.Visible;
                    txtcarga.Visibility = Visibility.Visible;                    

                    ////Tarea segundo plano Forma de hacerlo 2
                    //tarea.DoWork += HacerMientras;
                    //tarea.RunWorkerCompleted += HacerAlTerminar;
                    //tarea.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error;" +ex.Message.ToString(), "Leyendo archivo Excel", MessageBoxButton.OK, MessageBoxImage.Error);
            }   
        }

        private void btnWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "Word Documents (.docx)|*.docx;*.dotx";//"All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtWord.Text = openFileDialog1.FileName;
                }  
            }
            catch { }
        }

        private void Btn_GenerarArchivos_Click(object sender, RoutedEventArgs e)
        {
            bool EnviarCorreo = false;

            try
            {
                if (!string.IsNullOrEmpty(txtExcel.Text) && !string.IsNullOrEmpty(txtWord.Text))
            {
                if(!string.IsNullOrEmpty(txtNombreArchivo.Text))
                {
                    if (cmbColumnaID.SelectedIndex != -1)
                    {
                        if (RbtnGenerar.IsChecked == true)
                        {
                            EnviarCorreo = false;
                        }
                        if (RbtnGenerarEnviar.IsChecked == true)
                        {
                            EnviarCorreo = true;
                        }
                        string strtxtWord = txtWord.Text.ToString();
                        string strtxtNombreArchivo = txtNombreArchivo.Text.ToString();
                        string strcmbColumnaID = cmbColumnaID.SelectedItem.ToString();
                        //Tarea segundo plano Forma de hacerlo 1
                        Thread thread = new Thread(() =>
                        {
                            string respuesta = Word.CrearArchivoConDataTable(DataTable, strtxtWord, strtxtNombreArchivo, strcmbColumnaID, EnviarCorreo, IDmail, EnviarAdjuntos);
                            Action action = () =>
                            {
                                stpanelMensaje.Visibility = Visibility.Hidden;
                                pgbMensaje.Visibility = Visibility.Hidden;
                                txtMensaje.Visibility = Visibility.Hidden;
                                GenerarArchivosFin(respuesta, EnviarCorreo);
                            };
                            this.Dispatcher.BeginInvoke(action);
                        });
                        thread.Start();
                        stpanelMensaje.Visibility = Visibility.Visible;
                        pgbMensaje.Visibility = Visibility.Visible;
                        txtMensaje.Visibility = Visibility.Visible;   
                    }
                    else
                    {
                        MessageBox.Show("Falta seleccionar el ID con el que se creará el archivo", "Generación Masivos", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Debe escribir un nombre para los archivos generados", "Generación Masivos", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar archivos origen", "seleccionar Archivos", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error;" + ex.Message.ToString(), "Generando Archivos", MessageBoxButton.OK, MessageBoxImage.Error);
            }   
            
        }

        private void HacerMientras(object o, DoWorkEventArgs e)
        {
            Thread.Sleep(2000);
            pgbcarga.Visibility = Visibility.Visible;
            txtcarga.Visibility = Visibility.Visible;
        }

        private void HacerAlTerminar(object o, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Hacer al terminar", "terminar", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CargarArchivoExcelFin()
        {
            try
            {
                cmbColumnaID.IsEnabled = true;
                btnWord.IsEnabled = true;
                //Llena ComboBox con información de la tabla
                foreach (DataColumn DataColumn in DataTable.Columns)
                {
                    cmbColumnaID.Items.Add(DataColumn.ColumnName.ToString());
                    SeleccionMenu.CmbIDEmail.Items.Add(DataColumn.ColumnName.ToString());

                }

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
                    TablaCorreo.View = GridView;
                    TablaCorreo.DataContext = DataTable;

                    Binding Binding = new Binding();
                    TablaCorreo.SetBinding(ListView.ItemsSourceProperty, Binding);

                    ////Filtrar filas en DataTable
                    //DataRow[] filtro = DataTable.Select("Id = 1");

                    ////Borrar datos del DataTable
                    //DataTable.Clear(); 
                }
                else
                {
                    throw new System.ArgumentException ("La tabla se encuentra vacía");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() , "Cargando archivo", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        
        private void GenerarArchivosFin(string respuesta, bool EnviarCorreo)
        {
            if (respuesta == "finalizó")
            {
                if (EnviarCorreo == true)
                {
                    MessageBox.Show("Se crearon y enviaron todos los archivos satisfactoriamente.", "Generación Masivos", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Se crearon todos los archivos satisfactoriamente.", "Generación Masivos", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(respuesta, "Generación Masivos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RbtnGenerar_Checked(object sender, RoutedEventArgs e)
        {
            Btn_GenerarArchivos.IsEnabled = true;
            Btn_Enviar.IsEnabled = false;
            IDmail = "";
        }

        private void RbtnGenerarEnviar_Checked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtExcel.Text) && !string.IsNullOrEmpty(txtWord.Text))
            {
                Btn_GenerarArchivos.IsEnabled = true;
                Btn_Enviar.IsEnabled = false;
                EnviarAdjuntos = new List<string>();

                if (SeleccionMenu.LsArchivosAdjuntos.Count > 0)
                {
                    foreach (string Arhivo in SeleccionMenu.LsArchivosAdjuntos)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.IsChecked = true;
                        checkBox.Content = Arhivo;
                        SeleccionMenu.lbAdjuntos.Items.Add(checkBox);
                    }
                }

                SeleccionMenu.ShowDialog();
                if (SeleccionMenu.email.ToString() != "" || SeleccionMenu.lbAdjuntos.Items.Count > 0)
                {
                    IDmail = SeleccionMenu.email.ToString();
                    if (SeleccionMenu.lbAdjuntos.Items.Count > 0)
                    {
                        foreach (CheckBox ArchivoAdjunto in SeleccionMenu.lbAdjuntos.Items)
                        {
                            if (!string.IsNullOrEmpty(ArchivoAdjunto.Content.ToString()))
                            {
                                if (ArchivoAdjunto.IsChecked == true)
                                {
                                    EnviarAdjuntos.Add(ArchivoAdjunto.Content.ToString());
                                }
                            }
                        }
                    }
                }
                else
                {
                    RbtnGenerarEnviar.IsChecked = false;
                    Btn_GenerarArchivos.IsEnabled = true;
                }              
            }
            else
            {
                MessageBox.Show("Debe seleccionar archivos origen", "Generar y enviar", MessageBoxButton.OK, MessageBoxImage.Information);
                RbtnGenerarEnviar.IsChecked = false;
                Btn_GenerarArchivos.IsEnabled = true;
            }
            SeleccionMenu.lbAdjuntos.Items.Clear();
            SeleccionMenu.CmbIDEmail.SelectedIndex = -1; 
        }

        private void RbtnEnviar_Checked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtExcel.Text))
            {
                Btn_GenerarArchivos.IsEnabled = false;
                Btn_Enviar.IsEnabled = true;
                EnviarAdjuntos = new List<string>();

                if (SeleccionMenu.LsArchivosAdjuntos.Count > 0)
                {
                    foreach (string Arhivo in SeleccionMenu.LsArchivosAdjuntos)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.IsChecked = true;
                        checkBox.Content = Arhivo;
                        SeleccionMenu.lbAdjuntos.Items.Add(checkBox);
                    }
                }

                SeleccionMenu.ShowDialog();
                if (SeleccionMenu.email.ToString() != "" || SeleccionMenu.lbAdjuntos.Items.Count >0)
                {
                    IDmail = SeleccionMenu.email.ToString();
                    if(SeleccionMenu.lbAdjuntos.Items.Count > 0)
                    {
                        foreach (CheckBox ArchivoAdjunto in SeleccionMenu.lbAdjuntos.Items)
                        {
                            if (!string.IsNullOrEmpty(ArchivoAdjunto.Content.ToString()))
                            {
                                if (ArchivoAdjunto.IsChecked == true)
                                {
                                    EnviarAdjuntos.Add(ArchivoAdjunto.Content.ToString());
                                }
                            }
                        }
                    }
                }
                else
                {
                    RbtnEnviar.IsChecked = false;
                    Btn_Enviar.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar archivos origen", "Generar y enviar", MessageBoxButton.OK, MessageBoxImage.Information);
                RbtnEnviar.IsChecked = false;
                Btn_Enviar.IsEnabled = false;
            }
            SeleccionMenu.lbAdjuntos.Items.Clear();
            SeleccionMenu.CmbIDEmail.SelectedIndex = -1; 
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();  
            GC.Collect();
        }
    }
}