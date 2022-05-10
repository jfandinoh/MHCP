using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHCP.DGT.SUPPT.ValidaArchivoCetil.Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ValidarArchivoCetilBL _ValidarArchivoCetilBL = new ValidarArchivoCetilBL();
            _ValidarArchivoCetilBL.Validar();
            //_ValidarArchivoCetilBL.GuardarAuditoriaPostgres(true, "20192911150410_000000001.csv");
        }
    }
}
