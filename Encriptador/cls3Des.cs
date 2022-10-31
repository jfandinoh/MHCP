using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Encriptador
{
    public class cls3Des
    {
        //Definir objteto de tipo 3Des
        private TripleDESCryptoServiceProvider obj3Des = new TripleDESCryptoServiceProvider();
        private UTF8Encoding objUtf8 = new UTF8Encoding();
        private byte[] m_key;
        private byte[] m_iv;


        public cls3Des(byte[] key, byte[] iv)
        {
            this.m_key = key;
            this.m_iv = iv;
        }


        public cls3Des()
        {
            this.m_key = getKey();
            this.m_iv = getIV();
        }


        /// <summary>
        /// Desencripta un mensaje con el cifrado 3Des. 
        /// </summary>
        /// <param name="rutaArchivo">Ruta del archivo a desencriptar</param>
        public void DesencriptarArchivo(string rutaArchivo)
        {
            try
            {
                if (System.IO.File.Exists(rutaArchivo))
                {
                    StreamReader objStream = new StreamReader(rutaArchivo);
                    string objArchivo;

                    // Leer(archivo)
                    objArchivo = objStream.ReadToEnd();
                    objStream.Close();

                    //Desencriptando el archivo
                    byte[] input = Convert.FromBase64String(objArchivo);
                    obj3Des.Padding = PaddingMode.PKCS7;
                    obj3Des.Mode = CipherMode.CBC;
                    byte[] output = Transform(input, obj3Des.CreateDecryptor(m_key, m_iv));
                    objArchivo = objUtf8.GetString(output);

                    //Sacando copia del archivo original
                    System.IO.File.Move(rutaArchivo, rutaArchivo + ".3DesOriginal"); 

                    //Escribir archivo sin Encriptar
                    StreamWriter objNuevoArchivo = new StreamWriter(rutaArchivo);
                    objNuevoArchivo.Write(objArchivo);
                    objNuevoArchivo.Close();
                }
                else
                    throw new Exception("El archivo a desencriptar no existe");

            }
            catch (Exception ex)
            {
                string mensajeError = "Error desencriptando el archivo con cifrado 3Des.";

                throw new Exception(mensajeError);
            }
        }

        public string Desencriptar(string strTexto)
        {
            try
            {
                //Desencriptando el archivo
                byte[] input = Convert.FromBase64String(strTexto);
                obj3Des.Padding = PaddingMode.PKCS7;
                obj3Des.Mode = CipherMode.CBC;
                byte[] output = Transform(input, obj3Des.CreateDecryptor(m_key, m_iv));
                return objUtf8.GetString(output);
            }
            catch (Exception ex)
            {
                throw new Exception("Error desencriptando el archivo con cifrado 3Des.");
            }
        }


        private byte[] Transform(byte[] input, ICryptoTransform CryptoTransform)
        {
            //Crea el objeto MemoryStream
            MemoryStream objMemoriaStream = new MemoryStream();
            CryptoStream objCryptStream = new CryptoStream(objMemoriaStream, CryptoTransform, CryptoStreamMode.Write);

            objCryptStream.Write(input, 0, input.Length);
            objCryptStream.FlushFinalBlock();

            objMemoriaStream.Position = 0;
            byte[] objResultado = objMemoriaStream.ToArray();

            objMemoriaStream.Close();
            objCryptStream.Close();

            return objResultado;
        }


        private byte[] getKey()
        {
            UTF8Encoding objUtf8 = new UTF8Encoding();
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            CultureInfo ci_spa = new CultureInfo("es-CO");
            string valKey = DateTime.Now.ToString("yyyyMMdd") + "Transaccion@" + textInfo.ToTitleCase(DateTime.Now.ToString("MMMM", ci_spa));
            valKey = valKey.Substring(0, 24);
            return objUtf8.GetBytes(valKey);
        }


        private byte[] getIV()
        {
            UTF8Encoding objUtf8 = new UTF8Encoding();
            string valKey = "@COP.#1$";
            byte[] vector = objUtf8.GetBytes(valKey);
            return vector;
        }
    }
}
