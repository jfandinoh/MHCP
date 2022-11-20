using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Encriptador
{
    public partial class Form1 : Form
    {
        clsRSA clsRSA = new clsRSA();

        public Form1()
        {
            InitializeComponent();

            cmbEsquema.Items.Insert(0, "AES");
            cmbEsquema.Items.Insert(1, "Base64");
            cmbEsquema.Items.Insert(2, "3Des");
            cmbEsquema.Items.Insert(3, "JWT");
            cmbEsquema.Items.Insert(4, "RSA");
            cmbEsquema.SelectedIndex = 0;

            cmbProceso.Items.Insert(0,"Encriptar");
            cmbProceso.Items.Insert(1,"Desencriptar");
            cmbProceso.SelectedIndex = 0;

            txtLlave.Text = string.Format("{0}-{1}", "656a89da-5a56-4a11-9478-c8987fa0d516", DateTime.Now.Date.ToString("yyyy-MM-dd"));


            double juliano = 44769.6134419792;
            DateTime dateTime = DateTime.FromOADate(juliano);


            DateTime hoy = DateTime.Now;
            double Julianohoy = hoy.ToOADate();
            decimal Julianohoydec = Convert.ToDecimal( hoy.ToOADate());
            string JualianohoyStr = Julianohoy.ToString("R");

            Console.WriteLine(dateTime.ToString("yyyy-MM-dd HH:mm:ss fff"));

        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            //Limpiar campos
            txtSalida.Text = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(cmbEsquema.Text))
                {
                    throw new Exception("Debe seleccionar un esquema de encripción");
                }
                else if (string.IsNullOrEmpty(cmbProceso.Text))
                {
                    throw new Exception("Debe seleccionar un proceso a realizar");
                }
                else if (string.IsNullOrEmpty(txtEntrada.Text))
                {
                    throw new Exception("Debe ingresar un texto a procesar");
                }
                else
                {
                    switch (cmbProceso.SelectedIndex)
                    {
                        //Encriptar
                        case 0:
                            switch (cmbEsquema.SelectedIndex)
                            {
                                //AES
                                case 0:
                                    clsRijndael clsRijndael = new clsRijndael();
                                    //clsAES clsAES = new clsAES(txtLlave.Text, ASCIIEncoding.UTF8.GetBytes(txtVector.Text));
                                    //string hash = string.Empty;
                                    //txtSalida.Text = clsAES.Encriptar(txtEntrada.Text, CipherMode.CBC, PaddingMode.PKCS7, ref hash);

                                    txtSalida.Text = clsRijndael.Encriptar(txtEntrada.Text, txtLlave.Text, txtVector.Text, 32, 16, CipherMode.CBC, PaddingMode.PKCS7);
                                    break;
                                    
                                //Base 64
                                case 1:
                                    clsBase64 clsBase64 = new clsBase64();
                                    txtSalida.Text = clsBase64.Cifrar(txtEntrada.Text);
                                    break;

                                //3Des
                                case 2:
                                    cls3Des cls3Des = new cls3Des();
                                    throw new Exception("Esquema de cifrado no implementado");

                                //JWT
                                case 3:
                                    clsJsonWebTokens clsJsonWebTokens = new clsJsonWebTokens();
                                    txtSalida.Text = clsJsonWebTokens.JWEEncode(txtEntrada.Text,txtLlave.Text);
                                    break;

                                //RSA
                                case 4:
                                    string modulo = txtLlave.Text;
                                    string expPublico = "010001";
                                    txtSalida.Text = clsRSA.Encrypt(txtEntrada.Text);
                                    break;
                                default:
                                    throw new Exception("Esquema no implementado");
                            }
                            break;

                        //Desencriptar
                        case 1:
                            switch (cmbEsquema.SelectedIndex)
                            {
                                //AES
                                case 0:
                                    clsRijndael clsRijndael = new clsRijndael();
                                    txtSalida.Text = clsRijndael.Desencriptar(txtEntrada.Text, txtLlave.Text, txtVector.Text, 32, 16, CipherMode.CBC, PaddingMode.PKCS7);
                                    break;

                                //Base 64
                                case 1:
                                    clsBase64 clsBase64 = new clsBase64();
                                    txtSalida.Text = clsBase64.Descifrar(txtEntrada.Text);
                                    break;

                                //3Des
                                case 2:
                                    cls3Des cls3Des = new cls3Des();
                                    txtSalida.Text = cls3Des.Desencriptar(txtEntrada.Text);
                                    break;

                                //JWT
                                case 3:
                                    clsJsonWebTokens clsJsonWebTokens = new clsJsonWebTokens();
                                    txtSalida.Text = clsJsonWebTokens.JWEDecode(txtEntrada.Text, txtLlave.Text);
                                    break;

                                //RSA
                                case 4:
                                    
                                    txtSalida.Text = clsRSA.Decrypt(txtEntrada.Text);
                                    //txtSalida.Text = clsRSA.DesencriptarDatosConModulus(txtEntrada.Text,
                                    //    "b4a7e46170574f16a97082b22be58b6a2a629798419be12872a4bdba626cfae9900f76abfb12139dce5de56564fab2b6543165a040c606887420e33d91ed7ed7",
                                    //    "AQAB",
                                    //    "CbUODZ5VQyM9wwy/Msoy7hl+ZwW9sq5NnZ7onV7d61w1z7LeRvxUrgvPVHBfGATQtHluQyZlS2uW+XHWmqvVAGrSSgBOVTltPzFQQFbCWCYmGAzO2uMQKwH0tkm++NZw9QVDvSsgztt0Ls3oh9BzJuYfPZaUUN6QlQl1yqx87OZovJEa/5onndHLU/ywSzgmvtchwLTh9Bo5rOp+dV7XJcrzXsBKVAmGevIrv1aJmoLhsJT/tHXlg1Kr916obCQMfDRT6N8f7wOAKmZKWR9/KLVfmlwpCqaVoYhUIm1M2nYs/WqIrN8SLn/rAfINV3s7NnSg++KZbjtAQM9EYxL2bQ==",
                                    //    "wgbUFLylOWViviOhpiCZhE9Lsd90XHsYpS069jACwCiEDJXqpgv4gAXB9gAN0QjI/QnjC0UuKOBrZWkaEw5zIh9O6KB+HokopzWjDEjrwF+zPKloU7VDSAC4KY2W4XYKZ3sF68w+gOwsfqbxLF6YWKCmni0ak+Lh4/vkeNc1Jxc=",
                                    //    "yk9l9oa1CGEs79uf7pIlcAel908EINITw9YMBJT5S0ypeVEpcdFx2vzkHghJ3+wgoyOvisPqYX8VR2PLNoNwI2SYEN58Dgt1zLNNFHlvXKw0dfHxVc/U0QHr/qhZQT9qmpygqSnDVGPdYoGeYO6IasGexiQt+riHMElIqD+Wzxs=",
                                    //    "bMn7W+0yDu6D4OyFlGou6Xq0jHUn/zqd5O9pYeQlhqyw7YWZXaTUnqObz1zUYmI0n3xNq2IMsaqzTNcTYZ/2eilnyxNB48kv3CE83IR+ewytqNOrxVOa9+I/YD0dXikbSE44Ua/hU9QYVqMwEpqvfPIGuiW8AzNxSK1Ru1efW90=",
                                    //    "QPMcmPjGRdpswtSPk9f3jO/nqG4FtUO8dLLTSOVK45HJFtmnpJLJKrTlx36BuRHlqHpccQBUztgtzK2NIAMyjSpOZ5wF17x2JTGrHVefGPFwJi12fUYr3K3xReBmRzxEzT5blfOCciVdfbJ/Vj8+LIfcoWAcItH7FNVyLhG8KOE=",
                                    //    "Lp85K2K5SqIVUZroeIOiE1Zs20L60BCY2UoGnIEVs17ygaZMhpxH8FdeQ7HbEwRvdBEs7YJY9A6Aks5n2LrYLV6qsES2Bhh1C9U5WAwnvT3nd/AKlOc51/diKvI8dlsR6v1qiQehbEjM4Su37qU3DJouS5vUa1APUrCkyemBfJ4=", false);
                                    break;
                                default:
                                    throw new Exception("Esquema no implementado");
                            }
                            break;
                        default:
                            throw new Exception("Proceso no implementado");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Encriptador", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }


        }

        private void cmbEsquema_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbEsquema.SelectedIndex)
            {
                //AES
                case 0:
                    lblLlave.Visible = true;
                    lblVector.Visible = true;

                    txtLlave.Visible = true;
                    txtVector.Visible = true;
                    break;

                //Base 64
                case 1:
                    lblLlave.Visible = false;
                    lblVector.Visible = false;

                    txtLlave.Visible = false;
                    txtVector.Visible = false;
                    break;

                //3Des
                case 2:
                    lblLlave.Visible = false;
                    lblVector.Visible = false;

                    txtLlave.Visible = false;
                    txtVector.Visible = false;
                    break;

                //JWT
                case 3:
                    lblLlave.Visible = true;
                    lblVector.Visible = false;

                    txtLlave.Visible = true;
                    txtVector.Visible = false;
                    break;

                //RSA
                case 4:
                    lblLlave.Visible = true;
                    lblVector.Visible = false;

                    txtLlave.Visible = true;
                    txtVector.Visible = false;
                    break;

                default:
                    throw new Exception("Opción no implementado");
            }
        }
    }
}
