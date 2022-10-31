using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace Encriptador
{
    class clsAES
    {
        private byte[] bIV = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // Vector Iniciador
        private byte[] bKey;  // Llave de encripción

        // Constructor
        public clsAES(string strKey, byte[] bLocalIV = null)
        {
            if (strKey.Length == 64)
                bKey = Enumerable.Range(0, strKey.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(strKey.Substring(x, 2), 16)).ToArray();
            else
                bKey = new ASCIIEncoding().GetBytes(strKey);

            if (bKey.Length < 16)
                Array.Resize(ref bKey, 16);
            if (bLocalIV != null)
                bIV = bLocalIV;
        }

        // Constructor
        public clsAES(byte[] bKey, byte[] bLocalIV = null)
        {
            this.bKey = bKey;

            if (bLocalIV != null)
                bIV = bLocalIV;
        }

        // Descripta la cadena enviada por parámetro y la retorna 
        public string Desencriptar(string strCadena)
        {
            System.Text.Encoding textConverter = Encoding.GetEncoding("Windows-1252");
            RijndaelManaged myRijndael = new RijndaelManaged();
            byte[] fromEncrypt;
            byte[] encrypted;

            encrypted = Convert.FromBase64String(strCadena);

            myRijndael.KeySize = bKey.Length * 8;
            myRijndael.BlockSize = 128;
            myRijndael.Padding = PaddingMode.PKCS7;

            // Get a decryptor that uses the same key and IV as the encryptor.
            ICryptoTransform decryptor = myRijndael.CreateDecryptor(bKey, bIV);

            // Now decrypt the previously encrypted message using the decryptor
            // obtained in the above step.
            MemoryStream msDecrypt = new MemoryStream(encrypted);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);


            fromEncrypt = new byte[encrypted.Length + 1];

            // Read the data out of the crypto stream.
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            msDecrypt.Close();

            return textConverter.GetString(fromEncrypt).Replace((char)0, (char)32);
        }

        public string Desencriptar(string strCadena, CipherMode Mode, PaddingMode Padding, string Hash)
        {
            System.Text.Encoding textConverter = Encoding.GetEncoding("Windows-1252");
            RijndaelManaged myRijndael = new RijndaelManaged();
            byte[] fromEncrypt;
            byte[] encrypted;

            encrypted = Convert.FromBase64String(strCadena);

            myRijndael.KeySize = bKey.Length * 8;
            myRijndael.BlockSize = 128;

            myRijndael.Mode = Mode;
            myRijndael.Padding = Padding;

            // Get a decryptor that uses the same key and IV as the encryptor.
            ICryptoTransform decryptor = myRijndael.CreateDecryptor(bKey, bIV);

            // Now decrypt the previously encrypted message using the decryptor
            // obtained in the above step.
            MemoryStream msDecrypt = new MemoryStream(encrypted);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);


            fromEncrypt = new byte[encrypted.Length + 1];

            // Read the data out of the crypto stream.
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            msDecrypt.Close();

            return textConverter.GetString(fromEncrypt).Replace((char)0, (char)32);
        }

        // Encripta la cadena pasada por parámetro la retorna
        public string Encriptar(string strCadena)
        {
            RijndaelManaged myRijndael = new RijndaelManaged();
            byte[] encrypted;
            byte[] toEncrypt;
            System.Text.Encoding textConverter = Encoding.GetEncoding("Windows-1252");

            myRijndael.KeySize = bKey.Length * 8;
            myRijndael.BlockSize = 128;
            myRijndael.Padding = PaddingMode.PKCS7;

            // Get an encryptor.
            ICryptoTransform encryptor = myRijndael.CreateEncryptor(bKey, bIV);

            // Encrypt the data.
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            // Convert the data to a byte array.
            toEncrypt = textConverter.GetBytes(strCadena);

            // Write all data to the crypto stream and flush it.
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            // Get encrypted array of bytes.
            encrypted = msEncrypt.ToArray();

            msEncrypt.Close();

            return Convert.ToBase64String(encrypted);
        }

        public string Encriptar(string strCadena, CipherMode Mode, PaddingMode Padding, ref string Hash)
        {
            RijndaelManaged myRijndael = new RijndaelManaged();
            byte[] encrypted;
            byte[] toEncrypt;
            System.Text.Encoding textConverter = Encoding.GetEncoding("Windows-1252");

            myRijndael.KeySize = bKey.Length * 8;
            myRijndael.Key = bKey;
            myRijndael.BlockSize = bIV.Length * 8;
            myRijndael.IV = bIV;
            myRijndael.Mode = Mode;
            myRijndael.Padding = Padding;

            // Get an encryptor.
            ICryptoTransform encryptor = myRijndael.CreateEncryptor(bKey, bIV);

            // Encrypt the data.
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            // Convert the data to a byte array.
            toEncrypt = textConverter.GetBytes(strCadena);

            // Write all data to the crypto stream and flush it.
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            Hash = GetSHA1(strCadena);

            // Get encrypted array of bytes.
            encrypted = msEncrypt.ToArray();

            msEncrypt.Close();

            return Convert.ToBase64String(encrypted);
        }
        // Encriptar archivo 
        public string EncriptarArchivo(string StrArchivo, string StrRuta, string strBackUp, string strTransit) // Código de Error
        {
            string strCadena;
            System.IO.StreamReader oFileR;
            Stream oFileW;
            StreamWriter oFSW;
            string strFilename = StrArchivo.Split('/')[(StrArchivo.Split('/').Length - 1)];
            string strFilenameBkp;
            string strFilenameEnc;
            string[] arr;
            byte[] b;

            try
            {
                arr = strFilename.Split('.');
                if (arr.Length == 1 | arr[0] == "")
                {
                    strFilenameEnc = strFilename + ".ENC";
                    strFilenameBkp = strFilename + ".AX";
                }
                else
                {
                    arr[arr.Length - 1] = "ENC";

                    strFilenameEnc = string.Join(".", arr);

                    arr[arr.Length - 1] = "AX";

                    strFilenameBkp = string.Join(".", arr);

                    if (strTransit == "032")
                    {
                        ;/* Cannot convert AssignmentStatementSyntax, CONVERSION ERROR: Conversion for MidExpression not implemented, please report this issue in 'Mid(strFilenameEnc, 4, 1)' at character 6988
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMidExpression(MidExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMidExpression(MidExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 

                    Mid(strFilenameEnc, 4, 1) = "R"

 */
                        ;/* Cannot convert AssignmentStatementSyntax, CONVERSION ERROR: Conversion for MidExpression not implemented, please report this issue in 'Mid(strFilenameBkp, 4, 1)' at character 7041
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMidExpression(MidExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMidExpression(MidExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 

                    Mid(strFilenameBkp, 4, 1) = "R"

 */
                    }
                }

                // Lectura del archivo
                oFileR = new StreamReader(StrArchivo, Encoding.GetEncoding("Windows-1252"));

                strCadena = oFileR.ReadToEnd();

                oFileR.Close();

                oFileR = null;

                strCadena = Encriptar(strCadena);

                b = BitConverter.GetBytes(strCadena.Length);

                Array.Reverse(b);

                // Escritura de archivo
                oFileW = new FileStream(StrRuta + @"\" + strFilenameEnc, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

                oFileW.Write(b, 0, b.Length);

                oFSW = new StreamWriter(oFileW, Encoding.GetEncoding("Windows-1252"));

                oFSW.WriteLine(strCadena);

                oFSW.Close();

                oFileW.Close();

                if (File.Exists(strBackUp + @"\" + strFilenameBkp))
                    File.Delete(strBackUp + @"\" + strFilenameBkp);

                File.Move(StrArchivo, strBackUp + @"\" + strFilenameBkp);

                return StrRuta + @"\" + strFilenameEnc;
            }
            catch (FileNotFoundException ex1)
            {
                throw new Exception("3 - No es posible tener acceso al archivo que intenta encriptar. Error:" + ex1.Message);
            }
            catch (FileLoadException ex2)
            {
                throw new Exception("4 - No es posible leer el contenido del archivo que intenta encriptar. Error:" + ex2.Message);
            }

            // Catch ex3 As AccessViolationException

            // Throw New Exception("8 - No es posible entregar el archivo desencriptado en el destino")
            catch (Exception ex)
            {
                throw new Exception("5 - No es posible encriptar el archivo. Error: " + ex.Message);
            }
            finally
            {
                oFSW = null;
                oFileW = null;
                oFileR = null;
            }
        }

        // Desencriptar archivo
        public string DesencriptarArchivo(string StrArchivo, string StrRuta, string strBackUp, string strTransit)
        {
            string strCadena;
            StreamReader oFileR;
            StreamWriter oFileW;
            string strFilename = StrArchivo.Split('/')[(StrArchivo.Split('/').Length - 1)];
            string strFilenameBkp;
            string strFilenameDesEnc;
            string[] arr;
            try
            {
                arr = strFilename.Split('.');
                if (arr.Length == 1 | arr[0] == "")
                {
                    strFilenameDesEnc = strFilename + ".IE";
                    strFilenameBkp = strFilename + ".ENC";
                }
                else
                {
                    arr[arr.Length - 1] = "IE";

                    strFilenameDesEnc = string.Join(".",arr);

                    arr[arr.Length - 1] = "ENC";

                    strFilenameBkp = string.Join(".", arr);
                }

                // Lectura del archivo
                oFileR = new StreamReader(StrArchivo, Encoding.GetEncoding("Windows-1252"));

                for (int i = 1; i <= 4; i++)

                    oFileR.Read();

                strCadena = oFileR.ReadToEnd();

                oFileR.Close();

                strCadena = Desencriptar(strCadena);

                // Escritura de archivo
                oFileW = new StreamWriter(StrRuta + @"\" + strFilenameDesEnc, false, Encoding.GetEncoding("Windows-1252"));

                oFileW.Write(strCadena);

                oFileW.Close();

                if (File.Exists(strBackUp + @"\" + strFilenameBkp))

                    // File.Delete(strBackUp & "\" & strFilenameBkp)
                    File.Copy(strBackUp + @"\" + strFilenameBkp, strBackUp + @"\" + strFilenameBkp + "." + DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "-" + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second);

                File.Move(StrArchivo, strBackUp + @"\" + strFilenameBkp);

                return StrRuta + @"\" + strFilenameDesEnc;
            }
            catch (FileNotFoundException ex1)
            {
                throw new Exception("3 - No es posible tener acceso al archivo encriptado. Error: " + ex1.Message);
            }
            catch (FileLoadException ex2)
            {
                throw new Exception("4 - No es posible leer el contenido del archivo encriptado. Error: " + ex2.Message);
            }

            catch (Exception ex)
            {
                throw new Exception("5 - No es posible desencriptar el archivo encriptado. Error: " + ex.Message);
            }
            finally
            {
                oFileW = null;
                oFileR = null;
            }
        }

        private string GetSHA1(string sInp)
        {
            byte[] vInp;
            byte[] vOut;
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();

            vInp = Encoding.Default.GetBytes(sInp);
            vOut = sha.ComputeHash(vInp);
            return BitConverter.ToString(vOut).Replace("-", string.Empty);
        }
    }
}