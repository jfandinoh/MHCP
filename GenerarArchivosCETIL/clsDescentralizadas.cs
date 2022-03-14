using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BibliotecaClases;
using System.IO;


namespace GenerarArchivosCETIL
{
    public class clsDescentralizadas
    {
        public clsDescentralizadas() { }

        public String Proceso(DataTable EntidadesGenerarInformacion, Int32 CantidadArchivos)
        {
            try
            {
                String mensaje = string.Empty;
                DataRow[] dataRowEntidadesGenerarInformacion = EntidadesGenerarInformacion.Select("cod_Ua <> '01' AND fecha_GeneradoCETIL = '' AND tipo_Organizacion NOT IN('','E.S.E. Empresa Social del Estado / Hospital')");

                if (dataRowEntidadesGenerarInformacion.Length > 0)
                {
                    clsInformacionEntidades clsInformacionEntidades = new clsInformacionEntidades();
                    PostgreSql postgreSql = new PostgreSql();
                    int AuxCantidadArchivos = CantidadArchivos;

                    String pc_datos_ultenvio_dc = "pc_datos_ultenvio_dc";
                    clsConexion clsConexion = new clsConexion();

                    postgreSql.AbrirConexion("pc_pruebas");

                    try
                    {
                        string pathCentrales = Directory.GetCurrentDirectory() + @"\IndiciosPasivocol\Descentralizadas";
                        if (!Directory.Exists(pathCentrales))
                        {
                            Directory.CreateDirectory(pathCentrales);
                        }

                        String[] directorios = Directory.GetDirectories(pathCentrales);
                        Int32 cantidadDirectorios = directorios.Length;

                        string path = Directory.GetCurrentDirectory() + @"\IndiciosPasivocol\Descentralizadas\Paquete_" + (cantidadDirectorios + 1).ToString();
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        for (int i = 0; i < dataRowEntidadesGenerarInformacion.Length; i++)
                        {
                            if (AuxCantidadArchivos != 0)
                            {
                                if (!string.IsNullOrEmpty(dataRowEntidadesGenerarInformacion[i][7].ToString()))
                                {
                                    clsCargarInformacion clsCargarInformacion = new clsCargarInformacion();

                                    clsCargarInformacion.InformacionSalarioBase(postgreSql);

                                    clsConexion.CrearConexiondb(pc_datos_ultenvio_dc, postgreSql);
                                    clsCargarInformacion.InformacionEntidad(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                    clsCargarInformacion.InformacionActivos(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                    clsCargarInformacion.InformacionRetirados(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                    clsCargarInformacion.InformacionPensionados(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                    clsCargarInformacion.InformacionSustitutos(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                    clsCargarInformacion.InformacionPensionadosFallecidos(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                    clsCargarInformacion.InformacionRegistraduria(pc_datos_ultenvio_dc, postgreSql);
                                    clsCargarInformacion.InformacionCiudades(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString());
                                    clsCargarInformacion.InformacionDepartamentos(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString());
                                    clsCargarInformacion.InformacionHistoriaLaboralActivos(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                    clsCargarInformacion.InformacionHistoriaLaboralRetirados(pc_datos_ultenvio_dc, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());

                                    clsConstruirInformacion clsConstruirInformacion = new clsConstruirInformacion(dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][6].ToString());
                                    clsConstruirInformacion.InformacionR1(pc_datos_ultenvio_dc, postgreSql);
                                    clsConstruirInformacion.InformacionR2(pc_datos_ultenvio_dc, postgreSql);
                                    clsConstruirInformacion.InformacionTemporal(pc_datos_ultenvio_dc, postgreSql);
                                    clsConstruirInformacion.InformacionR4(pc_datos_ultenvio_dc, postgreSql);
                                    clsConstruirInformacion.InformacionR5(pc_datos_ultenvio_dc, postgreSql);
                                    clsConstruirInformacion.InformacionR6(pc_datos_ultenvio_dc, postgreSql);
                                    clsConstruirInformacion.InformacionR7(pc_datos_ultenvio_dc, postgreSql);
                                    clsConstruirInformacion.InformacionR8(pc_datos_ultenvio_dc, postgreSql);


                                    String nombreArchivo = string.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), clsConstruirInformacion.nit);

                                    string pathArchivo = System.IO.Path.Combine(path, nombreArchivo + ".csv");
                                    if (clsConstruirInformacion.EscribirArchivo(pathArchivo, postgreSql))
                                    {
                                        clsInformacionEntidades.ActualizarFechaGeneracion(dataRowEntidadesGenerarInformacion[i][0].ToString(), dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), pathArchivo);

                                        Console.WriteLine(string.Format("CodEt: {0}, CodUa: {1}, Archivo: {2}", dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), pathArchivo));
                                    }
                                }
                                AuxCantidadArchivos = AuxCantidadArchivos - 1;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (Directory.GetFiles(path).Length != 0)
                        {
                            Comprimir comprimir = new Comprimir();
                            comprimir.Carpeta(path);
                            mensaje = string.Format("Se generaron {0} archivos de entidades descentralizadas", Directory.GetFiles(path).Length);
                        }
                        else
                        {
                            mensaje = "No se generaron archivos";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        postgreSql.CerrarConexion();
                    }
                }

                return mensaje;
            }
            catch (Exception ex)
            {
                throw new Exception("Error generando archivos de entidades descentralizadas. " + ex.Message);
            }
        }
    }
}
