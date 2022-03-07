using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace BibliotecaClases
{
    public class Comprimir
    {
        public Comprimir() 
        {
            System.Console.WriteLine("Clase Comprimir");
        }

        public String Carpeta(String rutaCarpeta)
        {
            try
            {
                String rutaCarpetaComprimida = rutaCarpeta + ".zip";

                ZipFile.CreateFromDirectory(rutaCarpeta, rutaCarpetaComprimida);

                return rutaCarpetaComprimida;
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error comprimiendo carpeta '{0}'. Error: ", rutaCarpeta, ex.Message));
            }
        }

    }
}
