using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace BibliotecaClases
{
    public class Log
    {
        public Log()
        {
            System.Console.WriteLine("Clase Log");
        }

        public void EscribirLog(string Mensaje)
        {
            string fecha = System.DateTime.Now.ToString("yyyyMMdd");
            string hora = System.DateTime.Now.ToString("HH:mm:ss");
            string path = Directory.GetCurrentDirectory();
            string pathString = System.IO.Path.Combine(path, @"LogArchivosMasivos.txt");

            StreamWriter sw = new StreamWriter(pathString, true);

            StackTrace stacktrace = new StackTrace();
            sw.WriteLine(Mensaje);
            sw.WriteLine("");

            sw.Flush();
            sw.Close();
        }
    }
}
