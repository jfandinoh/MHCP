using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Win32;
using System.Xml.Serialization;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.OpenSsl;

namespace Encriptador
{
    public class clsRSA
    {

        //Llaves pública y privada de 2048
        private const String LlavePublica = "<RSAKeyValue><Modulus>xCM3HKeJECyExFSmzJWXgCXkxHTt+yOSVsMUC4tsYKNXw1O3dUaw2lSG69E7sXPkZHM7nGld4hE3MJH6qW7zXJQzxkaQda9K1mCZSK5CD1hF/2kQhn0OdBsPd55wbpHjD8UoNHsnp1GeiVO9tKniKnhU14z7tW4QTK5Vo1iUqWtQSZ0IPnYmhgpJlweEKuu+L3QTxTI7BjXqbPu6/vtaNOdAfllwDP3TognRenoi7X6WLmoGyuKS1TegICsErh2MONmF+UR1S+FeXrnhN8DxIt8QwAeSkQomYc5eu4s/avQpZ9Ev8GkmRLkbeyMR2i9Ptn/edFZgMT7s62RZ6sefZw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private const String LlavePrivada = "<RSAKeyValue><Modulus>xCM3HKeJECyExFSmzJWXgCXkxHTt+yOSVsMUC4tsYKNXw1O3dUaw2lSG69E7sXPkZHM7nGld4hE3MJH6qW7zXJQzxkaQda9K1mCZSK5CD1hF/2kQhn0OdBsPd55wbpHjD8UoNHsnp1GeiVO9tKniKnhU14z7tW4QTK5Vo1iUqWtQSZ0IPnYmhgpJlweEKuu+L3QTxTI7BjXqbPu6/vtaNOdAfllwDP3TognRenoi7X6WLmoGyuKS1TegICsErh2MONmF+UR1S+FeXrnhN8DxIt8QwAeSkQomYc5eu4s/avQpZ9Ev8GkmRLkbeyMR2i9Ptn/edFZgMT7s62RZ6sefZw==</Modulus><Exponent>AQAB</Exponent><P>+EebXSeg0v+t7Vny9MseLVt0aKRnpuQFzw6xvNyN4Mvz+U15FIOMO8BGTMO57064AFAyYkUBLsAccNZlcpWT9iyD+evmt+NGLrdMWY6oFLsd45q7H5CTQv+t0HtzUl4SmQNKSdbFTmfOFhsWiD7CNg0nTYK1DEKYlplOUJA5FzM=</P><Q>yjyJ5VTKqRHqDdFxIdxPeonKmP03DnUrUYRmOfiJOagKOdHindCsigl7za070wHWFUAi7hq0LT+r9AC4AZaD8STA/97D5q13B5slIXKgHM2IRRggZjkmsR8OhF2Cn8i2cbf4Vak6sg9hpQPgqVpt+a2jd5ZS51bF0R/YeOLthv0=</Q><DP>35Frf5jdouKFNcPXmUMGK4W87zWL+KY/7NGojw5z28cMLKPsseI69tsO8rUUyz8xRWbx17eXdwebFKiatXGnyPcQ6I3aNiA75Hk+ES76f6B3K6r7cVL4qE4fsCpaEAOR76Mc1BzHkvC6jQFbathIfe+eQpe+fAnD6WinMriMEC0=</DP><DQ>yXcLjCkvBsRw3Lkjsa6ugo0YBdZi7YUt1TRHPkLoOUa8gSMkVUDXUV/nNFr5+NegdGUDJx31FgqS+y1oITTXVffeSEeb8oYlyK72i16MQIVD1kkVpaATemlM5fHdud2SWrEBtBw0+8M069V0DmGmow8mhHB9QDaCNDU8ShkOtOU=</DQ><InverseQ>w+Oh1AktneVW3JLfLSoaV10DVydl9Q0WaTtfmxLiq7OmCxZrkFVoIduk3wb/s2UutjIarLOu2n3YUHKnx+TSFN5IGyzm/Mb4jktEDT7H7fky6qmlzw/TM9JOsANtUlq4a4gLGMFCAiJAi67ks0DXoc2a66gUc3o5h01acf7vMDk=</InverseQ><D>Fvg0zRNFtvIvvjpXql/edTB64RL8h8u7qJOB9nQmhg67CXH1azOTJI5/moBPaFG1LppcIc4pI392oiaGX29R6sakZZT/9/hGYGLBsMRPkjJ5SBcKy2U72xcMMasJabmGCOPC+Jpvo2WollaLQewfQ3ACJ5goJjA0knZcKxwFlO16n+SN8VKEccytKGfkPGD4xUn/zNDK9982sGC/KVTErpfcbAJ1RH03SNvlMKUjF14HAfxU/Ko34Lj9DWocQU3d6Qz5eb+kaRgr5HxaFAkCD1KYwmolOPZIXJhxNercOJ6AIh80BSYbOj3bXg/5XENEgJrN1GaTyxmNH7wb/haJ6Q==</D></RSAKeyValue>";
        
        private static RSACryptoServiceProvider RSACryptoServiceProvider = new RSACryptoServiceProvider(1024);
        private RSAParameters _privateKey;
        private RSAParameters _publicKey;

        public clsRSA()
        {
            _privateKey = RSACryptoServiceProvider.ExportParameters(true);
            _publicKey = RSACryptoServiceProvider.ExportParameters(false);

            Console.WriteLine($"Public Key: {GetPublicKey()}");
            Console.WriteLine($"Private Key: {GetPrivateKey()}");

            string publica = GetPublicPEM(RSACryptoServiceProvider);
            string privada = GetPrivatePEM(RSACryptoServiceProvider);

            StringBuilder pb = new StringBuilder();
            pb.AppendLine("-----BEGIN RSA PRIVATE KEY-----");
            pb.AppendLine("MIICXAIBAAKBgQDvV5DqSbAzsDfxM7yzOTfzP/XGxv2RqaekHxoEUiPjXhROlXY1");
            pb.AppendLine("WgVJK7f4trGhTZcBbiJnKlhdnHNB/doyvCxQHw5A2+1GB8GnJ5uC6eiv2ggLfSvP");
            pb.AppendLine("PXCtVCCOTzsz1SjPOpHzxPsHXt4ztrWdzBUCc/qiKVf3wHZ06Lm+u7Nv7QIDAQAB");
            pb.AppendLine("AoGATp3tzPdteFz+0yzSY/B8j1tICQYeDnWyyjcpHZg5j5q8gt+XV4j9SX6hzIF+");
            pb.AppendLine("MWTCIEcj581B/2W/ekK/JIEo8HnUmBXiQxE5RRpTxG/M75MmaHGF4j6167+FiKNi");
            pb.AppendLine("8SwoVPL8DQvjwRjX7/9PLr3JCCfo/sRBtQpd6Hx9g8UCwCUCQQD2FBIjBvvL1gSJ");
            pb.AppendLine("Pa8WLGAJ5cPnIkjch3tUq1hg7ViPAx8gND8YpWggrkehK+jD+N7B6K605UBFtiPo");
            pb.AppendLine("KeEGuwArAkEA+P33Feo9HIJyPBSQOdkfKWJGilfrhnO+eStoIM1/w3ixbl8VuY+P");
            pb.AppendLine("a11aCPdEl8Gwc1Nus3VabGLh493Qtu0sRwJAY3QTLHLrGyPBK5Jxi92dZwKknWqe");
            pb.AppendLine("1fovnzWs/2eNjict0j8rbROUtPia3Im5hlKz/NzElzm8MzB87JzYZHb5hwJAXert");
            pb.AppendLine("mEaSZn6NuDvJawiKyIFZOjWPkVd3MR0+WaEp4AFWa9tRxnxwDH9Zxqf+J3/XnqiJ");
            pb.AppendLine("yQcq5Fcn/VtdrqInYQJBAK9i8UEBX+LHh6NxvevgNJLeVhPXVoxKl2Q2Af7IpG9a");
            pb.AppendLine("WSV7IAXBF/s1bu5Cdnk7R7pZpWF+lI7rTbUxD9fhvNE=");
            pb.AppendLine("-----END RSA PRIVATE KEY-----");

            publica = pb.ToString();
            RSACryptoServiceProvider importedProvider = ImportPrivateKey(publica);
            _privateKey = importedProvider.ExportParameters(true);
            _publicKey = importedProvider.ExportParameters(false);
            GetPublicPEM(importedProvider);
            GetPrivatePEM(importedProvider);

           
        }

        public string GetPublicKey()
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RSAParameters));
            xmlSerializer.Serialize(stringWriter, _publicKey);
            return stringWriter.ToString();
        }

        public string GetPrivateKey()
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RSAParameters));
            xmlSerializer.Serialize(stringWriter, _privateKey);
            return stringWriter.ToString();
        }

        public string Encrypt (string plainText)
        {
            RSACryptoServiceProvider RSACryptoServiceProvider = new RSACryptoServiceProvider();
            RSACryptoServiceProvider.ImportParameters(_publicKey);
            byte[] byteText = Encoding.Unicode.GetBytes(plainText);
            byte[] cypherText = RSACryptoServiceProvider.Encrypt(byteText, false);
            return Convert.ToBase64String(cypherText);
        }

        public string Decrypt(string cypherText)
        {
            byte[] dataBytes = Convert.FromBase64String(cypherText);
            RSACryptoServiceProvider.ImportParameters(_privateKey);
            byte[] byteText = RSACryptoServiceProvider.Decrypt(dataBytes, false);
            return Encoding.Unicode.GetString(byteText);
        }

        /// <summary>
        /// Método que recibe la información y la llave pública con la cual realizará la encripción
        /// </summary>
        /// <param name="datos">Información que será encriptada</param>
        /// <param name="tipoLlave">Tipo de llave a utilizar. 1 Llave pública de neurona - 2 Llave pública  del cliente</param>
        /// <returns></returns>
        public byte[] EncriptarDatos(string datos, int tipoLlave)
        {


            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(LlavePublica);
            int tamanoBufferEncripcion = ((rsa.KeySize - 384) / 8) + 37;

            byte[] bytesDatos = Encoding.UTF8.GetBytes(datos);


            using (MemoryStream memoryStream = new MemoryStream())
            {

                //Se crea un buffer con el tamaño máximo permitido

                byte[] buffer = new byte[117];

                int posicion = 0;

                int longitudCopia = buffer.Length;

                while (true)
                {

                    //Se verifica si los bytes restantes son menores al tamño del buffer,en ese caso el límite del tamaño del buffer corresponde a los bytes restantes

                    if (posicion + longitudCopia > bytesDatos.Length)

                        longitudCopia = bytesDatos.Length - posicion;

                    //Se crea el nuevo buffer con el tamaño adecuado

                    buffer = new byte[longitudCopia];

                    //Se copian los bytes que el algoritmo RSA soporta y se recorre todo el arreglo de bytes para la encripción completa de la información

                    Array.Copy(bytesDatos, posicion, buffer, 0, longitudCopia);

                    posicion += longitudCopia;

                    //Se encripta la información usando la llave pública y se agrega al buffer de memoria

                    memoryStream.Write(rsa.Encrypt(buffer, true), 0, rsa.KeySize / 8);

                    Array.Clear(buffer, 0, longitudCopia);

                    if (posicion >= bytesDatos.Length)

                        break;

                }

                return memoryStream.ToArray();

            }

        }

        /// <summary>
        /// Método encargado de realizar la desencripción con la llave privada indicada
        /// </summary>
        /// <param name="datos">Información a desencriptar</param>
        /// <param name="tipoLlave">Tipo de llave a utilizar para la desencripción. 1 Llave privada de Neurona - 2 LLave privada del cliente</param>
        /// <returns></returns>
        public string DesencriptarDatos(byte[] datos, int tipoLlave)
        {
            try
            {

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                rsa.FromXmlString(LlavePrivada);

                //Se inicializa el memory stream que contendrá  los segmentos de información encripatada(Sin embargo será más pequeño).
                using (MemoryStream memoryStream = new MemoryStream(datos.Length))
                {
                    //Buffer que contendrá  los segmentos encriptados
                    byte[] buffer = new byte[rsa.KeySize / 8];
                    int posicion = 0;
                    int copyLength = buffer.Length;
                    while (true)
                    {
                        //Se copia cada segmento que será cifrado
                        Array.Copy(datos, posicion, buffer, 0, copyLength);

                        //Se actualiza la posición inicial
                        posicion += copyLength;

                        //Se realiza la desencripciòn usando la llave privada
                        byte[] bloqueDesencriptado = rsa.Decrypt(buffer, true);

                        memoryStream.Write(bloqueDesencriptado, 0, bloqueDesencriptado.Length);

                        //Se actualizan o limpian los buffer
                        Array.Clear(bloqueDesencriptado, 0, bloqueDesencriptado.Length);

                        Array.Clear(buffer, 0, copyLength);

                        //Se verifica si se llego al final del la información a encriptar

                        if (posicion >= datos.Length)

                            break;

                    }

                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }

            }

            catch (CryptographicException criptographicException)
            {
                throw criptographicException;
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Texto">Información que será encriptada</param>
        /// <param name="modulo">Módulo usado en la llave pública</param>
        /// <param name="pubExp">Exponente de cifrado</param>
        /// <returns></returns>
        public String EncriptarDatosConModulus(string Texto, string modulo, string pubExp, bool RellenoOAEP)
        {
            try
            {
                BigInteger N = new BigInteger(modulo, 16);
                BigInteger e = new BigInteger(pubExp, 16);

                RsaKeyParameters ParametrosLlave = new RsaKeyParameters(false, N, e);
                RSAParameters LlavePublica = DotNetUtilities.ToRSAParameters(ParametrosLlave);


                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(LlavePublica);

                byte[] TextoPlano = Encoding.UTF8.GetBytes(Texto);
                byte[] ciphertexto = rsa.Encrypt(TextoPlano, RellenoOAEP);
                string Resultado = Encoding.UTF8.GetString(Base64.Encode(ciphertexto));

                return Resultado;
            }
            catch (Exception ex)
            {
                string mensajeError = "Error encriptando datos con modulus.  ";

                throw new Exception(mensajeError + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Texto">Información que será desencriptada</param>
        /// <param name="modulo">Módulo usado en la llave pública</param>
        /// <param name="pubExp">Exponente de cifrado</param>
        /// <param name="pvtExp">Exponente de decifrado</param>
        /// <param name="NumPrimo1">Número primo aleatorio usado para calcular el módulo</param>
        /// <param name="NumPrimo2">Número primo aleatorio usado para calcular el módulo</param>
        /// <param name="ExpNumPrimo1">Exponente número primo aleatorio usado para calcular el módulo</param>
        /// <param name="ExpNumPrimo2">Exponente número primo aleatorio usado para calcular el módulo</param>
        /// <param name="coeficiente">Coeficiente</param>
        /// <returns></returns>
        public String DesencriptarDatosConModulus(string Texto, string modulo, string pubExp, string pvtExp, string NumPrimo1, string NumPrimo2, string ExpNumPrimo1, string ExpNumPrimo2, string coeficiente, bool RellenoOAEP)
        {
            try
            {
                BigInteger N = new BigInteger(modulo, 16); //modulo
                BigInteger e = new BigInteger(pubExp, 16); //exponente público
                BigInteger D = new BigInteger(pvtExp, 16); //exponente privado
                BigInteger P = new BigInteger(NumPrimo1, 16); //numero primo 1
                BigInteger Q = new BigInteger(NumPrimo2, 16); //numero primo 2
                BigInteger DP = new BigInteger(ExpNumPrimo1, 16); //d mod (p - 1)
                BigInteger DQ = new BigInteger(ExpNumPrimo2, 16); //d mod (q - 1)
                BigInteger coeff = new BigInteger(coeficiente, 16); //Inverse Q (InverseQ) (q) = 1 mod p

                RsaPrivateCrtKeyParameters ParametrosLlave = new RsaPrivateCrtKeyParameters(N, e, D, P, Q, DP, DQ, coeff);
                RSAParameters LlavePrivada = DotNetUtilities.ToRSAParameters(ParametrosLlave);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(LlavePrivada);
                byte[] TextoPlano = Base64.Decode(Texto);
                byte[] ciphertexto = rsa.Decrypt(TextoPlano, RellenoOAEP);
                string Resultado = Encoding.UTF8.GetString(ciphertexto);

                return Resultado;
            }
            catch (Exception ex)
            {
                string mensajeError = "Error desencriptando datos con modulus.  ";

                throw new Exception(mensajeError + ex.Message);
            }
        }

        public static String GetPrivatePEM(RSACryptoServiceProvider csp)
        {
            TextWriter outputStream = new StringWriter();

            if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
            var parameters = csp.ExportParameters(true);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, parameters.D);
                    EncodeIntegerBigEndian(innerWriter, parameters.P);
                    EncodeIntegerBigEndian(innerWriter, parameters.Q);
                    EncodeIntegerBigEndian(innerWriter, parameters.DP);
                    EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                }
                outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
            }

            Console.WriteLine("PRIVATE KEY:");
            Console.WriteLine(outputStream.ToString());
            return outputStream.ToString();
        }

        public static String GetPublicPEM(RSACryptoServiceProvider csp)
        {
            TextWriter outputStream = new StringWriter();

            var parameters = csp.ExportParameters(false);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE

                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    innerWriter.Write((byte)0x30); // SEQUENCE
                    EncodeLength(innerWriter, 13);
                    innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                    var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                    EncodeLength(innerWriter, rsaEncryptionOid.Length);
                    innerWriter.Write(rsaEncryptionOid);
                    innerWriter.Write((byte)0x05); // NULL
                    EncodeLength(innerWriter, 0);
                    innerWriter.Write((byte)0x03); // BIT STRING
                    using (var bitStringStream = new MemoryStream())
                    {
                        var bitStringWriter = new BinaryWriter(bitStringStream);
                        bitStringWriter.Write((byte)0x00); // # of unused bits
                        bitStringWriter.Write((byte)0x30); // SEQUENCE
                        using (var paramsStream = new MemoryStream())
                        {
                            var paramsWriter = new BinaryWriter(paramsStream);
                            EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                            EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                            var paramsLength = (int)paramsStream.Length;
                            EncodeLength(bitStringWriter, paramsLength);
                            bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                        }
                        var bitStringLength = (int)bitStringStream.Length;
                        EncodeLength(innerWriter, bitStringLength);
                        innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                    }
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.WriteLine("-----BEGIN PUBLIC KEY-----");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                }
                outputStream.WriteLine("-----END PUBLIC KEY-----");

                Console.WriteLine("PUBLIC KEY:");
                Console.WriteLine(outputStream.ToString());

                return outputStream.ToString();
            }
        }

        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        /// <summary>
        /// Import OpenSSH PEM private key string into MS RSACryptoServiceProvider
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider ImportPrivateKey(string pem)
        {
            PemReader pr = new PemReader(new StringReader(pem));
            AsymmetricCipherKeyPair KeyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)KeyPair.Private);

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
            csp.ImportParameters(rsaParams);
            return csp;
        }

        /// <summary>
        /// Import OpenSSH PEM public key string into MS RSACryptoServiceProvider
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider ImportPublicKey(string pem)
        {
            PemReader pr = new PemReader(new StringReader(pem));
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
            csp.ImportParameters(rsaParams);
            return csp;
        }
    }
}



/*
 https://www.javatips.net/api/java.security.spec.rsaprivatecrtkeyspec
https://www.dcode.fr/rsa-cipher#f1
https://tekshinobi.com/reading-rsa-key-pair-from-pem-files-in-net-with-c-using-bouncy-castle/
https://www.c-sharpcorner.com/blogs/asp-net-core-encrypt-and-decrypt-public-key-and-private-key
https://gist.github.com/therightstuff/aa65356e95f8d0aae888e9f61aa29414
https://dejanstojanovic.net/aspnet/2018/june/loading-rsa-key-pair-from-pem-files-in-net-core-with-c/
https://www.baeldung.com/java-read-pem-file-keys
https://www.iteramos.com/pregunta/53297/como-leer-una-clave-privada-pem-rsa-desde-net
 */