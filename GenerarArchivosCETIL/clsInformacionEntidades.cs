using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BibliotecaClases;

namespace GenerarArchivosCETIL
{
    public class clsInformacionEntidades
    {
        public clsInformacionEntidades()
        {

        }

        public DataTable Consultar()
        {
            try
            {
                return entidadesMySql(entidadesPostgrSql());
            }
            catch(Exception ex)
            {
                throw new Exception("Error consultando informacion de entidades. " + ex.Message);
            }
        }

        public void ActualizarFechaGeneracion(String id, String codEt, String codUa,String nombreArchivo)
        {
            try
            {
                MySQL mySQL = new MySQL();
                mySQL.AbrirConexion();
                String queryMySql = string.Empty;
                DataTable dataTable = new DataTable();
                try
                {
                    
                    queryMySql = "UPDATE test.EntidadesEntregadasCETIL " +
                    "SET fecha_GeneradoCETIL = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "', nombre_Archivo ='" + nombreArchivo + "' "+
                    "WHERE id = '" + id + "' AND cod_Et = '" + codEt + "' AND cod_Ua = '" + codUa + "'";
                    mySQL.EjecutarQuery(queryMySql);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    mySQL.CerrarConexion();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error actualizando fecha generacion entidad a CETIL. " + ex.Message);
            }
        }

        private DataTable entidadesPostgrSql()
        {
            String queryPostgreSql = string.Empty;
            DataTable AE_SeguimientoUnidadesAdtvas = new DataTable();
            PostgreSql postgreSql = new PostgreSql();

            try
            {
                postgreSql.AbrirConexion("pc_datos");

                try
                {
                    queryPostgreSql = "SELECT \"CodET\", \"CodUA\", MAX(\"NroInforme\") AS NroInforme, MAX(\"AñoInforme\") AS AnoInforme, " +
                    "MAX(\"FechaCorte\") AS FechaCorte " +
                    "FROM pc_consegui.\"AE_SeguimientoUnidadesAdtvas\" " +
                    "GROUP BY \"CodET\", \"CodUA\" " +
                    "ORDER BY \"CodET\", \"CodUA\"";

                    AE_SeguimientoUnidadesAdtvas = postgreSql.ConsultarDatos(queryPostgreSql);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                return AE_SeguimientoUnidadesAdtvas;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error consultando informacion de AE_SeguimientoUnidadesAdtvas. ", ex.Message));
            }
            finally
            {
                postgreSql.CerrarConexion();
            }
        }

        private DataTable entidadesMySql(DataTable AE_SeguimientoUnidadesAdtvas)
        {
            MySQL mySQL = new MySQL();
            mySQL.AbrirConexion();
            String queryMySql = string.Empty;
            DataTable dataTable = new DataTable();
            try
            {
                queryMySql = "SELECT * FROM information_schema.tables WHERE table_schema = 'test' AND table_name = 'EntidadesEntregadasCETIL' ";
                dataTable = mySQL.ConsultarDatos(queryMySql);
                if (dataTable.Rows.Count == 0)
                {
                    queryMySql = "DROP TABLE IF EXISTS test.EntidadesEntregadasCETIL; " +
                    "CREATE TABLE test.EntidadesEntregadasCETIL( " +
                      "id int AUTO_INCREMENT, " +
                      "cod_Et varchar(5) NOT NULL, " +
                      "cod_Ua varchar(2) NOT NULL, " +
                      "nit_Ua varchar(25) NOT NULL, " +
                      "nombre_Ua varchar(500) DEFAULT NULL, " +
                      "tipo_Organizacion varchar(50) NOT NULL DEFAULT '', " +
                      "fecha_Corte char(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '', " +
                      "nro_Informe char(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '', " +
                      "anio_Informe char(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '', " +
                      "fecha_GeneradoCETIL char(25) CHARACTER SET utf8mb4 NOT NULL DEFAULT '', " +
                      "nombre_Archivo text, " +
                      "PRIMARY KEY(id) " +
                    "); " +
                    "INSERT INTO test.EntidadesEntregadasCETIL(cod_Et, cod_Ua,nit_Ua, nombre_Ua, tipo_Organizacion) " +
                    "SELECT cod_et AS cod_Et, cod_ua AS cod_Ua,IFNULL(nit_ua,'') AS nit_Ua, nombre_ua AS nombre_Ua, " +
                    "IFNULL(b.descrip_organizacion_ua, '') AS tipo_Organizacion " +
                    "FROM bd_webpasivocol.tbl_directorio_unidades_adtvas a " +
                    "LEFT OUTER JOIN bd_webpasivocol.tbl_tipo_organizacion_ua b " +
                    "ON a.tipo_organizacion = b.tipo_organizacion_ua " +
                    "WHERE a.estado_registro <> 'A' " +
                    "ORDER BY cod_et,cod_ua; ";

                    mySQL.EjecutarQuery(queryMySql);

                    queryMySql = "SELECT * FROM test.EntidadesEntregadasCETIL";
                    dataTable = mySQL.ConsultarDatos(queryMySql);

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataRow[] drAE_SeguimientoUnidadesAdtvas = AE_SeguimientoUnidadesAdtvas.Select("CodET = '" + dataTable.Rows[i][1] + "' AND CodUA = '" + dataTable.Rows[i][2] + "'");
                        if (drAE_SeguimientoUnidadesAdtvas.Length == 1)
                        {
                            queryMySql = "UPDATE test.EntidadesEntregadasCETIL " +
                            "SET fecha_Corte = '" + StringToDatetime(drAE_SeguimientoUnidadesAdtvas[0][4].ToString()).ToString("yyyyMMdd") + "', " +
                            "nro_Informe = '" + drAE_SeguimientoUnidadesAdtvas[0][2].ToString() + "', " +
                            "anio_Informe = '" + drAE_SeguimientoUnidadesAdtvas[0][3].ToString() + "' " +
                            "WHERE id = '" + dataTable.Rows[i][0] + "' AND cod_Et = '" + drAE_SeguimientoUnidadesAdtvas[0][0] + "' AND cod_Ua = '" + drAE_SeguimientoUnidadesAdtvas[0][1] + "'";

                            mySQL.EjecutarQuery(queryMySql);
                        }
                    }
                }

                queryMySql = "SELECT * FROM test.EntidadesEntregadasCETIL";
                dataTable = mySQL.ConsultarDatos(queryMySql);

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                mySQL.CerrarConexion();
            }
        }

        private DateTime StringToDatetime(string strFecha)
        {
            string[] formatos = new[] { "dd-MM-yyyy", "dd/MM/yyyy" };
            DateTime fechaValida;
            DateTime.TryParseExact(strFecha, formatos, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fechaValida);
            return fechaValida;
        }
    }
}
