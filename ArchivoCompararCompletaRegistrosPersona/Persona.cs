using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchivoCompararCompletaRegistrosPersona
{
    public class Persona
    {
        public Int64 id { get; set; }
        public string tipo_documento { get; set; }
        public string numero_documento { get; set; }
        public string nombre_persona { get; set; }
        public string grupo_persona { get; set; }
    }
}
