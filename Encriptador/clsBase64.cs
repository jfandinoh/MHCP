using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encriptador
{
    class clsBase64
    {

        /// <summary>
        /// Metodo para cifrar con base64
        /// </summary>
        /// <param name="textoPlano">Mensaje a cifrar</param>
        /// <returns></returns>
        public static string Cifrar(string textoPlano)
        {
            try
            {
                byte[] BytesTextoPlano = System.Text.Encoding.UTF8.GetBytes(textoPlano);
                byte[] BytesTextoEncriptado = BytesTextoPlano;
                string Base64TextoPlano = Convert.ToBase64String(BytesTextoEncriptado);
                return Base64TextoPlano;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} {1}", "Error en proceso de crifrado", ex.Message));
            }
        }

        /// <summary>
        /// Metodo para descifrar con base64
        /// </summary>
        /// <param name="textoBase64">Mensaje a descifrar</param>
        /// <returns></returns>
        public static string Descifrar(string textoBase64)
        {
            try
            {
                byte[] BytesTextoBase64 = Convert.FromBase64String(textoBase64);
                byte[] BytesTextoPlano = BytesTextoBase64;
                string TextoPlano = System.Text.Encoding.UTF8.GetString(BytesTextoBase64);
                return TextoPlano;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} {1}", "Error en proceso de descrifrado", ex.Message));
            }
        }
    }
}
