using System;
using System.Text;
using Jose;

namespace Encriptador
{
    public class clsJsonWebTokens
    {
        public clsJsonWebTokens()
        {
        }

        /// <summary>
        /// Metodo que cifra con uso directo de una clave simetrica
        /// </summary>
        /// <param name="Texto"></param>
        /// <param name="Llave"></param>
        /// <returns></returns>
        public String JWEEncode(Object Texto, String Llave)
        {
            try
            {
                String Resultado = String.Empty;
                byte[] LlaveSecreta = Encoding.UTF8.GetBytes(Llave);
                Resultado = JWT.Encode(Texto, LlaveSecreta, JweAlgorithm.DIR, JweEncryption.A128CBC_HS256);

                return Resultado;
            }
            catch (Exception ex)
            {
                String mensajeError = String.Format("{0} {1}","Error cifrando datos con JWE.",ex.Message.ToString());

                throw new Exception(mensajeError);
            }
        }

        /// <summary>
        /// Metodo que decifra con uso directo de una clave simetrica
        /// </summary>
        /// <param name="Texto"></param>
        /// <param name="Llave"></param>
        /// <returns></returns>
        public String JWEDecode(String Texto, String Llave)
        {
            try
            {
                String Resultado = String.Empty;
                byte[] LlaveSecreta = Encoding.UTF8.GetBytes(Llave);
                Resultado = JWT.Decode(Texto, LlaveSecreta, JweAlgorithm.DIR, JweEncryption.A128CBC_HS256);

                return Resultado;
            }
            catch (Exception ex)
            {
                String mensajeError = String.Format("{0} {1}", "Error decifrando datos con JWE.", ex.Message.ToString());

                throw new Exception(mensajeError);
            }
        }
    }
}
