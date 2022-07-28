using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encriptador
{
    public class clsRijndael
    {
        public Byte[] Key = {2, 8, 48, 8, 12, 6, 8, 1, 4, 9, 17, 5, 7, 7, 9, 11};
        public Byte[] IV  = {10, 87, 66, 4, 50, 2, 17, 80, 9, 23, 11, 120, 30, 40, 10, 106};

        /// <summary>
        /// Encripta un mensaje con el cifrado Rijndael.
        /// </summary>
        /// <param name="mensaje">Mensafe a cifrar</param>
        /// <param name="strKey">Llave secreta </param>
        /// <param name="strIV"> Vecto de inicializacion</param>
        /// <param name="LonKey">Longitud llave secreta. Valores permitidos: 16,24 o 32 </param>
        /// <param name="LonIV"> Tamaño de bloque. Valores permitidos: 16,24 o 32</param>
        /// <param name="Modo">Modo de cifrado</param>
        /// <param name="Padding">Modo de relleno</param>
        /// <returns>Retorna un string con el mensaje encriptado</returns>
        public string Encriptar(string mensaje, string strKey, string strIV, int LonKey, int LonIV, CipherMode Modo, PaddingMode Padding)
        {
            string mensajeOrig = "";

            try
            {
                string resultado = "";
                Byte[] arreglo = null;

                if (!string.IsNullOrEmpty(strKey))
                {
                    Key = ASCIIEncoding.UTF8.GetBytes(strKey);
                    Array.Resize(ref Key, LonKey);
                }

                if (!string.IsNullOrEmpty(strIV))
                {
                    IV = ASCIIEncoding.UTF8.GetBytes(strIV);
                    Array.Resize(ref IV, LonIV);
                }                

                RijndaelManaged encriptador = new RijndaelManaged();
                encriptador.KeySize = Key.Length * 8;
                encriptador.Key = Key;
                encriptador.BlockSize = IV.Length * 8;
                encriptador.IV = IV;
                encriptador.Mode = Modo;
                encriptador.Padding = Padding;

                ICryptoTransform encryptor = null;
                encryptor = encriptador.CreateEncryptor(encriptador.Key, encriptador.IV);

                MemoryStream msEncrypt = new MemoryStream();
                CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                StreamWriter swEncrypt = new StreamWriter(csEncrypt);

                mensajeOrig = mensaje;

                if (string.IsNullOrEmpty(mensajeOrig))
                    mensajeOrig = "";


                swEncrypt.Write(mensajeOrig);

                if (swEncrypt != null)
                    swEncrypt.Close();

                if (csEncrypt != null)
                    csEncrypt.Close();

                if (msEncrypt != null)
                    msEncrypt.Close();

                if (encriptador != null)
                    encriptador.Clear();

                arreglo = msEncrypt.ToArray();

                resultado = Convert.ToBase64String(arreglo);

                return resultado;


            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error encriptando mensaje con cifrado Rijndael. {0}", ex.Message));
            }
        }



        /// <summary>
        /// Desencripta un mensaje con el cifrado Rijndael. 
        /// </summary>
        /// <param name="mensaje">Mensafe a cifrar</param>
        /// <param name="strKey">Llave secreta </param>
        /// <param name="strIV"> Vecto de inicializacion</param>
        /// <param name="LonKey">Longitud llave secreta. Valores permitidos: 16,24 o 32 </param>
        /// <param name="LonIV"> Tamaño de bloque. Valores permitidos: 16,24 o 32</param>
        /// <param name="Modo">Modo de cifrado</param>
        /// <param name="Padding">Modo de relleno</param>
        /// <returns>Retorna un string con el mensaje desencriptado</returns>
        public string Desencriptar(string mensaje, string strKey, string strIV, int LonKey, int LonIV, CipherMode Modo, PaddingMode Padding)
        {
            try
            {
                string resultado = "";
                Byte[] arreglo = null;

                if (!string.IsNullOrEmpty(strKey))
                {
                    Key = ASCIIEncoding.UTF8.GetBytes(strKey);
                    Array.Resize(ref Key, LonKey);
                }

                if (!string.IsNullOrEmpty(strIV))
                {
                    IV = ASCIIEncoding.UTF8.GetBytes(strIV);
                    Array.Resize(ref IV, LonIV);
                }    

                RijndaelManaged desencriptador = new RijndaelManaged();
                desencriptador.KeySize = Key.Length * 8;
                desencriptador.Key = Key;
                desencriptador.BlockSize = IV.Length * 8;
                desencriptador.IV = IV;
                desencriptador.Mode = Modo;
                desencriptador.Padding = Padding;
                arreglo = Convert.FromBase64String(mensaje);

                ICryptoTransform decryptor = null;
                decryptor = desencriptador.CreateDecryptor(desencriptador.Key, desencriptador.IV);

                MemoryStream msDecrypt = new MemoryStream(arreglo);
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                StreamReader srDecrypt = new StreamReader(csDecrypt);

                resultado = srDecrypt.ReadToEnd();

                if (srDecrypt != null)
                    srDecrypt.Close();

                if (csDecrypt != null)
                    csDecrypt.Close();

                if (msDecrypt != null)
                    msDecrypt.Close();

                if (desencriptador != null)
                    desencriptador.Clear();


                return resultado;


            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error desencriptando mensaje con cifrado Rijndael. {0}", ex.Message));
            }
        }  

    }
}
