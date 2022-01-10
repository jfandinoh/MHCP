using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
//using para datatable
using System.Data;
//using para IO archivos
using System.IO;
//using para word
using word = Microsoft.Office.Interop.Word;
//using para archivo configuracion (.config)
using System.Configuration;
////using para hilos
using System.Threading; 

namespace BibliotecaClases
{
    public class Word
    {
        //Clase para enviar correo por outlook
        Outlook Outlook = new Outlook();

        //Declaración de variables globales

        //Ruta de mis documentos
        string MisDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //Carpeta de la Aplicación
        string CarpetaApp = Directory.GetCurrentDirectory(); 
        //Declarar Carpeta
        string carpeta;
        string texto2;

        public Word()
        {
            System.Console.WriteLine("Clase Word");
        }

        /*
         * Esta función recibe un datatable, una plantilla de word y crea un archivo word por cada fila del datatablea.
         */
        public string CrearArchivoConDataTable(DataTable TablaInformacion, string plantillaWord, string NombreArchivo, string ColumnaID, bool EnviarCorreo, string ColumnaIDCorreo, List<string> ArchivosAdjuntos)
        {
            string proceso = "finalizó";
            try
            {   
                //Seleccionar en que ruta se crearan los archivos                
                string ValorCarpetaApp = ConfigurationManager.AppSettings["CarpetaApp"].ToString();
                if (ValorCarpetaApp != "0")
                {
                    carpeta = CarpetaApp + @"\ArchivosGenerados" + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    carpeta = MisDocumentos + @"\ArchivosMasivos" + "\\" + DateTime.Now.ToString("yyyy-MM-dd");   
                }
                
                //Recorrer Datatable
                int count = 1;
                string[] campos = new string[TablaInformacion.Columns.Count];  //cantidad elementos
                foreach (DataColumn DataColumn in TablaInformacion.Columns)
                {
                    //Extraer nombre de columna 
                    System.Console.WriteLine(DataColumn.ColumnName);
                    campos[count - 1] = DataColumn.ColumnName.ToString();
                    count++;
                }

                foreach (DataRow DataRow in TablaInformacion.Rows)
                {
                    string fechaHora = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt");
                    string ArchivoSalida;                    
                    object objmiss = System.Reflection.Missing.Value;
                    word.Application objword = new word.Application();
                    object filename = plantillaWord;
                    object missing = Type.Missing;
                    word.Document objdoc = objword.Documents.Open(ref filename, ref objmiss, ref objmiss, ref objmiss,
                                               ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss,
                                               ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss);

                    for (int col = 0; col <= campos.Length - 1; col++)
                    {

                        objword.Selection.Find.ClearFormatting();
                        objword.Selection.Find.Text = "[" + campos[col].ToString() + "]";
                        objword.Selection.Find.Forward = true;   
                        objword.Selection.Find.Replacement.ClearFormatting();
                        if (DataRow[campos[col].ToString()].ToString().Length > 255)
                        {
                            string texto = DataRow[campos[col].ToString()].ToString();
                            texto2 = texto;
                            Thread newThread = new Thread(new ThreadStart(LlamarMetodo));
                            newThread.SetApartmentState(ApartmentState.STA);
                            newThread.Start();
                            objword.Selection.Find.Replacement.Text = "^c";
                        }
                        else
                        {
                            objword.Selection.Find.Replacement.Text = DataRow[campos[col].ToString()].ToString();
                        }

                        object replaceAll = word.WdReplace.wdReplaceAll;
                        objword.Selection.Find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                             ref missing, ref missing, ref missing, ref missing, ref missing,
                             ref replaceAll, ref missing, ref missing, ref missing, ref missing);

                        //Extraer información de columna indicada
                        //System.Console.WriteLine(DataRow[campos[col].ToString()]);
                    }
                    objword.Visible = false;
                    //objdoc.Password = "";
                    objdoc.ReadOnlyRecommended = false;

                    string Archivo = string.Concat (NombreArchivo,DataRow[ColumnaID]);

                    //Valida existencia de carpeta o la crea de ser necesario
                    if (Directory.Exists(carpeta))
                    {
                        ArchivoSalida = carpeta + "\\" + Archivo + ".pdf";
                        if (File.Exists(ArchivoSalida))
                        {
                            ArchivoSalida = carpeta + "\\" + Archivo + "_" + fechaHora + " .pdf"; 
                        }                        
                    }
                    else
                    {
                        Directory.CreateDirectory(carpeta);
                        ArchivoSalida = carpeta + "\\" + Archivo + ".pdf";
                    }
                    object savefile = (object)ArchivoSalida;
                    object FileFormat = word.WdSaveFormat.wdFormatPDF;
                    objdoc.SaveAs2(ref savefile, ref FileFormat, ref objmiss, ref objmiss, ref objmiss,
                                   ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss,
                                   ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss);
                    objdoc.Close(false , ref objmiss, ref objmiss);
                    objword.Quit(ref objmiss, ref objmiss, ref objmiss);

                    if (EnviarCorreo == true)
                    {
                        List<string> LsArchivoAdjuntos = new List<string>();
                        //Se agrega archivo generado a la lista de archivos que se adjuntan al correo
                        foreach(string RutaArchivo in ArchivosAdjuntos)
                        {
                            LsArchivoAdjuntos.Add(RutaArchivo);
                        }
                        LsArchivoAdjuntos.Add(ArchivoSalida);
                        //Se envía correo con archivo adjunto
                        Outlook.EnviarMail(DataRow[ColumnaIDCorreo].ToString(), LsArchivoAdjuntos); 
                    }                    
                }
                proceso = "finalizó";
            }
            catch (Exception ex)
            {
                 //throw new System.ArgumentException ("Error en la creación de archivos. " + ex.Message.ToString());
                 proceso = ex.Message.ToString();
            }
            return proceso;
        }

        /*
         * Esta función crea un archivo word a partir de una plantilla
         */
        public void CrearArchivo()
        {
            try
            {
                object objmiss = System.Reflection.Missing.Value;
                word.Application objword = new word.Application();
                //Plantilla de word
                object filename = "C:\\CartaLuisa.dotx";
                object missing = Type.Missing;
                word.Document objdoc = objword.Documents.Open(ref filename, ref objmiss, ref objmiss, ref objmiss,
                                       ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss,
                                       ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss);

                objword.Selection.Find.ClearFormatting();
                objword.Selection.Find.Text = "texto a buscar";
                objword.Selection.Find.Replacement.ClearFormatting();
                objword.Selection.Find.Replacement.Text = "texto nuevo";

                object replaceAll = word.WdReplace.wdReplaceAll;
                objword.Selection.Find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing, ref missing,
                        ref replaceAll, ref missing, ref missing, ref missing, ref missing);
               
                objword.Visible = false;
                //objdoc.Password = "";
                objdoc.ReadOnlyRecommended = false;
                object savefile = (object)"C:\\Users\\jfandino\\Desktop\\jaime\\prueba1.doc";
                objdoc.SaveAs2(ref savefile, ref objmiss, ref objmiss, ref objmiss, ref objmiss,
                               ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss,
                               ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss, ref objmiss);
                objdoc.Close(ref objmiss, ref objmiss, ref objmiss);
                objword.Quit(ref objmiss, ref objmiss, ref objmiss);
            }
            catch { }
        }

        static void CopiarPortapapeles(string a)
        {
            Clipboard.Clear();
            Clipboard.SetText(a);
            //System.Console.WriteLine(Clipboard.GetText().ToString());            
        }

        private void LlamarMetodo()
        {
            CopiarPortapapeles(texto2);
        }
    }
}
