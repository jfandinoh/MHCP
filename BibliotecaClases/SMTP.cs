using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace BibliotecaClases
{
    public class SMTP
    {
        public SMTP()
        {
            System.Console.WriteLine("Clase SMTP");
        }

        public void EnviarMail(string Destinatario)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();

                //Quien escribe el correo
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["From"].ToString());
                //A quien se envia el correo
                mailMessage.To.Add(new MailAddress(Destinatario));
                //Prioridad del correo
                mailMessage.Priority = MailPriority.High;
                //Asunto del correo
                mailMessage.Subject = ConfigurationManager.AppSettings["Subject"].ToString();
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                //Contenido del correo
                mailMessage.Body = ConfigurationManager.AppSettings["Body"].ToString();
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = true;

                //Servidor smtp
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    smtpClient.Host = ConfigurationManager.AppSettings["Host"].ToString();
                    smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
                    smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"].ToString());
                    smtpClient.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"].ToString()) * 1000;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    //Credenciales desde donde se envia el correo
                    smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["From"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());

                    smtpClient.Send(mailMessage);
                }
            }
            catch(SmtpException ex)
            {
                if (ex.InnerException != null)
                {
                    throw new System.ArgumentException("Error al enviar correo. " + ex.InnerException.ToString());
                }
                else
                {
                    throw new System.ArgumentException("Error al enviar correo. " + ex.Message.ToString());
                }
                
            }
        }

    }
}
