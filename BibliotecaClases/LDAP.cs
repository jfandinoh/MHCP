using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
//using System.Windows.Forms;
//using System.DirectoryServices;

namespace BibliotecaClases
{
    /*public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = @"LDAP://Nombre_del _servidor";       //CAMBIAR POR VUESTRO PATH (URL DEL SERVICIO DE DIRECTORIO LDAP)
                                                                //Por ejemplo: 'LDAP://ejemplo.lan.es'
            string dominio = @"Nombre_del_dominio";             //CAMBIAR POR VUESTRO DOMINIO
            string usu = usuario.Text.Trim();                   //USUARIO DEL DOMINIO
            string pass = clave.Text.Trim();                    //PASSWORD DEL USUARIO
            string domUsu = dominio + @"\" + usu;               //CADENA DE DOMINIO + USUARIO A COMPROBAR

            bool permiso = AutenticaUsuario(path, domUsu, pass);
            if (permiso)
            {
                permitido.BackColor = Color.Green;
                permitido.ForeColor = Color.White;
                permitido.Text = "Acceso permitido";
            }
            else
            {
                permitido.BackColor = Color.Red;
                permitido.ForeColor = Color.Black;
                permitido.Text = "Acceso denegado";
            }
        }

        private bool AutenticaUsuario(String path, String user, String pass)
        {
            //Los datos que hemos pasado los 'convertimos' en una entrada de Active Directory para hacer la consulta
            DirectoryEntry de = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
            try
            {
                //Inicia el chequeo con las credenciales que le hemos pasado
                //Si devuelve algo significa que ha autenticado las credenciales
                DirectorySearcher ds = new DirectorySearcher(de);
                ds.FindOne();
                return true;
            }
            catch
            {
                //Si no devuelve nada es que no ha podido autenticar las credenciales
                //ya sea porque no existe el usuario o por que no son correctas
                return false;
            }
        }
    }*/
}
