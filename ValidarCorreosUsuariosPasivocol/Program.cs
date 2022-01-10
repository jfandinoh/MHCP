using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaClases;
using System.Configuration;

namespace ValidarCorreosUsuariosPasivocol
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ValidarCorreos();
                Console.WriteLine("Se termina el envío de correos a validar");
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        static void ValidarCorreos()
        {
            try
            {
                SMTP SMTP = new SMTP();

                SMTP.EnviarMail("jfandinoh93@gmail.com");
            }
            catch(Exception ex) 
            {
                throw new Exception("Error validando correos: " + ex.Message);
            }
        }
    }
}
