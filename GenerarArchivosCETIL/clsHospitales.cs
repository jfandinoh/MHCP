using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GenerarArchivosCETIL
{
    public class clsHospitales
    {
        public clsHospitales() { }

        public String Proceso(DataTable EntidadesGenerarInformacion, Int32 CantidadArchivos)
        {
            try
            {
                
                DataRow[] dataRowEntidadesGenerarInformacion = EntidadesGenerarInformacion.Select("cod_Ua <> '01' AND fecha_GeneradoCETIL = '' AND tipo_Organizacion = 'E.S.E. Empresa Social del Estado / Hospital'");

                if (dataRowEntidadesGenerarInformacion.Length > 0)
                {
                    clsInformacionEntidades clsInformacionEntidades = new clsInformacionEntidades();
                    int AuxCantidadArchivos = CantidadArchivos;
                    for (int i = 0; i < dataRowEntidadesGenerarInformacion.Length; i++)
                    {
                        if (AuxCantidadArchivos != 0)
                        {
                            clsInformacionEntidades.ActualizarFechaGeneracion(dataRowEntidadesGenerarInformacion[i][0].ToString(), dataRowEntidadesGenerarInformacion[i][1].ToString(), dataRowEntidadesGenerarInformacion[i][2].ToString());
                            AuxCantidadArchivos = AuxCantidadArchivos - 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return string.Format("Se generaron {0} archivos de hospitales", CantidadArchivos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error generando archivos de entidades hospitales. " + ex.Message);
            }
        }
    }
}
