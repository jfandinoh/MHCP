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
using System.Net;
using System.Net.Mail;

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace BibliotecaClases
{/*
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // <summary>
        /// Default SMTP Port.
        /// </summary>
        public static int SmtpPort = 25; 

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //MailMessage msg = new MailMessage();
            //SmtpClient client = new SmtpClient();

            //msg.To.Add("jfandinoh@hotmail.com");
            //msg.From = new MailAddress("jfandinoh93@gmail.com", "Jaime Fandiño", System.Text.Encoding.UTF8);
            //msg.Subject = "Aqui va el asunto";
            //msg.SubjectEncoding = System.Text.Encoding.UTF8;
            //msg.Body = "Y aqui el contenido";
            //msg.BodyEncoding = System.Text.Encoding.UTF8;
            //msg.IsBodyHtml = false; //Si vas a enviar un correo con contenido html entonces cambia el valor a true

            //client.Credentials = new System.Net.NetworkCredential("jfandinoh93@gmail.com", "1010211430");
            //client.Port = 587;
            //client.Host = "smtp.gmail.com";//"mhproxysg.minhacienda.red";//Este es el smtp valido para Gmail
            //client.EnableSsl = true; //Esto es para que vaya a través de SSL que es obligatorio con GMail

            //try
            //{
            //    client.Send(msg);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error" + ex.Message.ToString());
            //}

            try
            {
                CDO.Message oMsg = new CDO.Message();
                CDO.Configuration iConfg = new CDO.Configuration();
                ADODB.Fields oFields;
                ADODB.Field oField;

                oFields = iConfg.Fields;

                // Set Proxy properties
                oField = oFields["http://schemas.microsoft.com/cdo/configuration/urlproxyserver"];
                oField.Value = "mhproxysg.minhacienda.red";

                oField = oFields["http://schemas.microsoft.com/cdo/configuration/proxyserverport"];
                oField.Value = 8080;

                oField =
                oFields["http://schemas.microsoft.com/cdo/configuration/smtpsserver"];
                oField.Value = "smtp.gmail.com";

                            oField =
                oFields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
                            oField.Value = 587;

                            oFields.Update();

                            oMsg.Configuration = iConfg;

                            // Set common properties for Message
                            oMsg.Subject = "Test SMTP";
                            oMsg.From = "jfandinoh93@gmail.com";
                            oMsg.To = "jfandinoh@hotmail.com";

                            oMsg.Send();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("{0} Exception caught.", e);
                        }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            try
            {
                MailAddress fromAddress = new MailAddress("jaime.fandino@minhacienda.gov.co");
                message.From = fromAddress;
                message.To.Add("jfandinoh93@gmail.com");

                message.Subject = "prueba";

                message.IsBodyHtml = true;
                message.Body = "prueba";
                smtpClient.Host = "correoweb.minhacienda.gov.co"; //exchange server name
                smtpClient.Port = 25;

                //smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("jfandino", "Diciembre10", "minhacienda.red");
                smtpClient.EnableSsl = true;
                //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                ServicePointManager.ServerCertificateValidationCallback =
                   delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                   { return true; };

                smtpClient.Send(message);
                message.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exchange" + ex.Message.ToString());
            }
        }

    }
*/}
