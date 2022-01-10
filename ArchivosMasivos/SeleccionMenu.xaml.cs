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
using System.Windows.Shapes;
//using para open file dialog
using Microsoft.Win32;


namespace CorreosMasivos
{
    /// <summary>
    /// Interaction logic for SeleccionMenu.xaml
    /// </summary>
    public partial class SeleccionMenu : Window
    {
        public string email="";
        public List<string> LsArchivosAdjuntos = new List<string>();
        public SeleccionMenu()
        {
            InitializeComponent();
            lbAdjuntos.IsEnabled = false;
            btnAdjunto.IsEnabled = false;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            MainWindow menu = new MainWindow("correo");
            email = CmbIDEmail.Text.ToString();
            LsArchivosAdjuntos = new List<string>();

            if (lbAdjuntos.Items.Count > 0)
            {
                foreach(CheckBox ArchivoAdjunto in lbAdjuntos.Items)
                {
                    if (!string.IsNullOrEmpty(ArchivoAdjunto.Content.ToString()))
                    {
                        if(ArchivoAdjunto.IsChecked == true)
                        {
                            LsArchivosAdjuntos.Add(ArchivoAdjunto.Content.ToString());
                        }
                    }
                }
            }
            if (email == "")
            {
                MessageBox.Show("No seleccionó ningun correo", "Seleccionar correo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                this.Hide();
            }
        }

        private void btnAdjunto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                if (openFileDialog1.ShowDialog() == true)
                {
                    foreach(string Arhivo in openFileDialog1.FileNames)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.IsChecked = true;
                        checkBox.Content = Arhivo;
                        lbAdjuntos.Items.Add(checkBox);
                    }
                }

                if(lbAdjuntos.Items.Count > 0)
                {
                    lbAdjuntos.IsEnabled = true;
                }
            }
            catch { }
        }

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); 
            email="";
            lbAdjuntos.Items.Clear();
            lbAdjuntos.IsEnabled = false;
            btnAdjunto.IsEnabled = false;
            RbtnCorreoMasivo.IsChecked = false;
            LsArchivosAdjuntos = new List<string>();
        }

        private void RbtnCorreoMasivo_Checked(object sender, RoutedEventArgs e)
        {
            lbAdjuntos.IsEnabled = true;
            btnAdjunto.IsEnabled = true;
        }

        private void RbtnCorreoMasivo_Unchecked(object sender, RoutedEventArgs e)
        {
            lbAdjuntos.IsEnabled = false;
            btnAdjunto.IsEnabled = false;
        }

        private void lbAdjuntos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
