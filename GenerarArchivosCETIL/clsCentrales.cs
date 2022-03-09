using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BibliotecaClases;

namespace GenerarArchivosCETIL
{
    public class clsCentrales
    {
        public clsCentrales() { }

        public String Proceso(DataTable EntidadesGenerarInformacion, Int32 CantidadArchivos)
        {
            try
            {
                DataRow[] dataRowEntidadesGenerarInformacion = EntidadesGenerarInformacion.Select("cod_Ua = '01' AND fecha_GeneradoCETIL = ''");

                if(dataRowEntidadesGenerarInformacion.Length > 0)
                {
                    clsInformacionEntidades clsInformacionEntidades = new clsInformacionEntidades();
                    PostgreSql postgreSql = new PostgreSql();
                    int AuxCantidadArchivos = CantidadArchivos;

                    String pc_datos_ultenvio_ac = "pc_datos_ultenvio_ac";
                    clsConexion clsConexion = new clsConexion();

                    postgreSql.AbrirConexion("pc_pruebas");

                    try
                    {
                        for (int i = 0; i < dataRowEntidadesGenerarInformacion.Length; i++)
                        {
                            if (AuxCantidadArchivos != 0)
                            {
                                clsCargarInformacion clsCargarInformacion = new clsCargarInformacion();

                                clsCargarInformacion.InformacionSalarioBase(postgreSql);

                                clsConexion.CrearConexiondb(pc_datos_ultenvio_ac, postgreSql);
                                clsCargarInformacion.InformacionEntidad(pc_datos_ultenvio_ac,postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                clsCargarInformacion.InformacionActivos(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                clsCargarInformacion.InformacionRetirados(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                clsCargarInformacion.InformacionPensionados(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                clsCargarInformacion.InformacionSustitutos(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                clsCargarInformacion.InformacionPensionadosFallecidos(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                clsCargarInformacion.InformacionRegistraduria(pc_datos_ultenvio_ac, postgreSql);
                                clsCargarInformacion.InformacionCiudades(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString());
                                clsCargarInformacion.InformacionDepartamentos(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString());
                                clsCargarInformacion.InformacionHistoriaLaboralActivos(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());
                                clsCargarInformacion.InformacionHistoriaLaboralRetirados(pc_datos_ultenvio_ac, postgreSql, dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][7].ToString());

                                clsConstruirInformacion clsConstruirInformacion = new clsConstruirInformacion(dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString(), dataRowEntidadesGenerarInformacion[i][6].ToString());
                                clsConstruirInformacion.InformacionR1(pc_datos_ultenvio_ac, postgreSql);
                                clsConstruirInformacion.InformacionR2(pc_datos_ultenvio_ac, postgreSql);
                                clsConstruirInformacion.InformacionTemporal(pc_datos_ultenvio_ac, postgreSql);
                                clsConstruirInformacion.InformacionR4(pc_datos_ultenvio_ac, postgreSql);
                                clsConstruirInformacion.InformacionR5(pc_datos_ultenvio_ac, postgreSql);
                                clsConstruirInformacion.InformacionR6(pc_datos_ultenvio_ac, postgreSql);
                                clsConstruirInformacion.InformacionR7(pc_datos_ultenvio_ac, postgreSql);
                                clsConstruirInformacion.InformacionR8(pc_datos_ultenvio_ac, postgreSql);
                                String NombreArchivo = string.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), clsConstruirInformacion.nit);
                                clsConstruirInformacion.EscribirArchivo(NombreArchivo, postgreSql);

                                clsInformacionEntidades.ActualizarFechaGeneracion(dataRowEntidadesGenerarInformacion[i][0].ToString(), dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString());
                                AuxCantidadArchivos = AuxCantidadArchivos - 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        postgreSql.CerrarConexion();
                    }
                }

                return string.Format("Se generaron {0} archivos de entidades centrales", CantidadArchivos);
            }
            catch(Exception ex)
            {
                throw new Exception("Error generando archivos de entidades centrales. " + ex.Message);
            }
        }
    }
}
