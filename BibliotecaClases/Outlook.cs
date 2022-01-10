using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using para outlook
using outlook = Microsoft.Office.Interop.Outlook;
//using para archivo configuracion (.config)
using System.Configuration;
//using para datatable
using System.Data;


namespace BibliotecaClases
{
    public class Outlook
    {
        public Outlook()
        {
            System.Console.WriteLine("Clase Outlook");
        }

        /*
         * Esta función envía un correo electrónico desde la cuenta de outlook configurada en el equipo.
         */
        public void EnviarMail(string destino, List<string> Adjuntos)
        {
            try
            {
                //Configuration App = ConfigurationManager.OpenExeConfiguration(App.FilePath);

                //----------------------Se envía correo por Outlook configurado en el equipo------------------------
                outlook.Application app = new outlook.Application();
                outlook.MailItem mail = (outlook.MailItem)app.CreateItem(outlook.OlItemType.olMailItem);

                outlook.Accounts accounts = app.Session.Accounts;

                //Cuenta para envío de correo cuando hay más de una cuenta configurada en el equipo
                if (ConfigurationManager.AppSettings["CuentaAsociada"].ToString() != "")
                {
                    foreach (outlook.Account cuenta in accounts)
                    {
                        if (cuenta.SmtpAddress == ConfigurationManager.AppSettings["CuentaAsociada"].ToString())
                        {
                            mail.SendUsingAccount = cuenta;
                        }
                    }
                }

                ////Envío desde un buzon compartido
                //if (ConfigurationManager.AppSettings["CuentaAsociada"].ToString() != "")
                //{
                //    mail.SentOnBehalfOfName = ConfigurationManager.AppSettings["CuentaAsociada"].ToString();
                //}

                if (destino.Contains(";") || destino.Contains("|") || destino.Contains("?") ||
                        destino.Contains("/") || destino.Contains("\t") || destino.Contains(","))
                {
                    char[] separador = new char[] { ';', '|', '?', '/', '\t', ',' };
                    String[] substrings = destino.Split(separador);
                    string[] destinos = new string[substrings.Length];
                    int i = 0;
                    foreach (string substring in substrings)
                    {
                        Console.WriteLine(substring);
                        destinos[i] = substring;
                        i = i + 1;
                    }
                    //Destino
                    mail.To = string.Join(";", destinos);
                }
                else
                {
                    //Destino
                    mail.To = destino;
                }              
                //Asunto del mail
                mail.Subject = ConfigurationManager.AppSettings["Asunto"].ToString();
                //Cuerpo del mail
                mail.Body = ConfigurationManager.AppSettings["Mensaje"].ToString();
                //Importancia del mail
                mail.Importance = outlook.OlImportance.olImportanceHigh;
                //Se agregan adjuntos al mail
                foreach (string Adjunto in Adjuntos)
                {
                    if (Adjunto != "")
                    {
                        mail.Attachments.Add(Adjunto, Type.Missing, Type.Missing, Type.Missing);
                    }
                }
                ((outlook._MailItem)mail).Send();
            }
            catch (Exception e)
            {
                throw new System.ArgumentException("Error al enviar correo. " + e.Message.ToString());
            }
        }

        public void EnviarMail(string destino, List<string> LsAdjuntos, DataTable TablaInformacion)
        {
            try
            {
                foreach (DataRow DataRow in TablaInformacion.Rows)
                {
                    //Extraer información de columna indicada
                    System.Console.WriteLine("Correos electrónicos destino: " + DataRow[destino]);                
                    //----------------------Se envía correo por Outlook configurado en el equipo------------------------
                    outlook.Application app = new outlook.Application();
                    outlook.MailItem mail = (outlook.MailItem)app.CreateItem(outlook.OlItemType.olMailItem);
                    outlook.Accounts accounts = app.Session.Accounts;

                    //Cuenta para envío de correo cuando hay más de una cuenta configurada en el equipo
                    if (ConfigurationManager.AppSettings["CuentaAsociada"].ToString() != "")
                    {
                        System.Console.WriteLine("Correo electrónico origen: " + ConfigurationManager.AppSettings["CuentaAsociada"].ToString());
                        foreach (outlook.Account cuenta in accounts)
                        {
                            if (cuenta.SmtpAddress == ConfigurationManager.AppSettings["CuentaAsociada"].ToString())
                            {
                                mail.SendUsingAccount = cuenta;
                                System.Console.WriteLine("Cuenta de usuario parametrizada en mail.SendUsingAccount: "+ cuenta.UserName);
                            }
                        }
                    }

                    ////Envío desde un buzon compartido
                    //if (ConfigurationManager.AppSettings["CuentaAsociada"].ToString() != "")
                    //{
                    //    mail.SentOnBehalfOfName = ConfigurationManager.AppSettings["CuentaAsociada"].ToString();
                    //}

                    if (DataRow[destino].ToString().Contains(";") || DataRow[destino].ToString().Contains("|") || DataRow[destino].ToString().Contains("?") ||
                        DataRow[destino].ToString().Contains("/") || DataRow[destino].ToString().Contains("\t") || DataRow[destino].ToString().Contains(","))
                    {
                        char[] separador = new char[] { ';', '|', '?', '/', '\t', ',' };
                        String[] substrings = DataRow[destino].ToString().Split(separador);
                        string[] destinos = new string[substrings.Length];
                        int i = 0;
                        foreach (string substring in substrings)
                        {
                            Console.WriteLine(substring);
                            destinos[i] =substring; 
                            i= i+1;
                        }
                        //Destino
                        mail.To = string.Join(";", destinos);
                    }
                    else
                    {
                        //Destino
                        mail.To = DataRow[destino].ToString(); 
                    }                    
                    //Asunto del mail
                    mail.Subject = ConfigurationManager.AppSettings["AsuntoMasivo"].ToString();
                    //Cuerpo del mail
                    mail.Body = ConfigurationManager.AppSettings["MensajeMasivo"].ToString();
                    //Importancia del mail
                    mail.Importance = outlook.OlImportance.olImportanceHigh;
                    //Se agregan adjuntos al mail
                    foreach (string Adjunto in LsAdjuntos)
                    {
                        if (Adjunto != "")
                        {
                            mail.Attachments.Add(Adjunto, Type.Missing, Type.Missing, Type.Missing);
                        }
                    }
                    
                    ((outlook._MailItem)mail).Send();
                }
            }
            catch (Exception e)
            {
                throw new System.ArgumentException("Error al enviar correo. " + e.Message.ToString());
            }
        }

    }
}
