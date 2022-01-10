using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ArchivoCompararCompletaRegistrosPersona
{
    class Program
    {
       
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(Proceso());
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error procesando archivo CRUCE_NITS_PASIVOCOL.txt: {0}", ex.Message));
            }
        }

        private static string Proceso()
        {
            try
            {
                string ArchivoAC = @"D:\jfandino\Documentos\05. CETIL\Consultas Cruces 2022\Ultimo_Envio_AC.csv";
                string ArchivoNuevo = @"D:\jfandino\Documentos\05. CETIL\Consultas Cruces 2022\RegistrosPersona.txt";
                string[] Archivos = { @"D:\jfandino\Documentos\05. CETIL\Consultas Cruces 2022\Ultimo_Envio_AC.csv", @"D:\jfandino\Documentos\05. CETIL\Consultas Cruces 2022\Ultimo_Envio_DC.csv" };

                bool encabezado = true;
                List<string> LsRegistros = new List<string>();
                int contadorPersona = 0;
                List<Persona> personas = new List<Persona>();

                foreach (string archivo in Archivos.ToList())
                {
                    using (FileStream fs = new FileStream(ArchivoNuevo, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            using (StreamReader sr = new StreamReader(archivo, Encoding.UTF8))
                            {
                                while (sr.Peek() >= 0)
                                {
                                    string linea = sr.ReadLine();
                                    string[] Arreglo = linea.Split(';');
                                    Persona persona = new Persona();

                                    if (encabezado)
                                    {
                                        encabezado = false;
                                    }
                                    else
                                    {
                                        if (archivo.Equals(ArchivoAC))
                                        {
                                            try
                                            {
                                                persona.id = contadorPersona + 1;
                                                persona.tipo_documento = Arreglo[0];
                                                persona.numero_documento = Arreglo[1];
                                                persona.nombre_persona = Arreglo[2];
                                                persona.grupo_persona = Arreglo[3];

                                                contadorPersona += 1;
                                                personas.Add(persona);

                                                sw.WriteLine(string.Format("{0};{1};{2};{3}",persona.tipo_documento,persona.numero_documento,persona.nombre_persona,persona.grupo_persona));

                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("Error linea " + contadorPersona.ToString());
                                            }
                                        }
                                        else
                                        {
                                            int indice = personas.FindIndex(p => p.numero_documento == Arreglo[1] && p.tipo_documento == Arreglo[0]);

                                            if (indice != -1)
                                            {
                                                Console.WriteLine(string.Format("El tipo {0} y numero de documento {1} ya existe en el objeto Persona", Arreglo[0], Arreglo[1]));
                                            }
                                            else
                                            {
                                                persona.id = contadorPersona + 1;
                                                persona.tipo_documento = Arreglo[0];
                                                persona.numero_documento = Arreglo[1];
                                                persona.nombre_persona = Arreglo[2];
                                                persona.grupo_persona = Arreglo[3];

                                                contadorPersona += 1;
                                                personas.Add(persona);

                                                sw.WriteLine(string.Format("{0};{1};{2};{3}", persona.tipo_documento, persona.numero_documento, persona.nombre_persona, persona.grupo_persona));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }                   
                }

                return "Proceso exitoso";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
