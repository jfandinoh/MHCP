using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using BibliotecaClases;
using System.IO;
using System.Data;


namespace GenerarArchivosCETIL
{
    public class clsActualizarEntidadesUltimoEnvio
    {
        public clsActualizarEntidadesUltimoEnvio() { }

        public void ExisteInformacion(String database, String esquema, String nombreTabla, PostgreSql postgreSql)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();
                try
                {
                    Query = "SELECT * FROM information_schema.columns where table_catalog = 'pc_pruebas' AND table_schema = 'public' and table_name='" + nombreTabla + "';";
                    dataTable = postgreSql.ConsultarDatos(Query);

                    if(dataTable.Rows.Count > 0)
                    {
                        Query = "DROP TABLE IF EXISTS " + nombreTabla + ";";
                        postgreSql.EjecutarQuery(Query);

                        Query = "CREATE TABLE " + nombreTabla + " AS " +
                        "SELECT * FROM dblink " +
                        "('demodbrnd', CONCAT('SELECT DISTINCT f3_view.ea_co_dane, f3_view.ea_nro_ord, f3_view.ea_tip_doc, f3_view.ea_nro_doc, f3_view.ea_nombre," +
                        "f3_view.ea_sexo, f3_view.ea_est_civ, f3_view.ea_fec_ing, f3_view.ea_subsector " +
                        "FROM " + database + "." + esquema + ".tbl_f3_empleados_activos AS f3_view " +
                            "JOIN( " +
                                   "SELECT ea_co_dane, ea_nro_ord, MAX(ea_nroinforme) AS informe " +
                                   "FROM " + database + "." + esquema + ".tbl_f3_empleados_activos " +
                                   "GROUP BY ea_co_dane, ea_nro_ord) AS lastReport " +
                                   "ON lastReport.ea_co_dane = f3_view.ea_co_dane AND lastReport.ea_nro_ord = f3_view.ea_nro_ord AND lastReport.informe = f3_view.ea_nroinforme " +
                            "ORDER BY f3_view.ea_co_dane, f3_view.ea_nro_ord')) " +
                        "AS DATA( " +
                         "ea_co_dane character varying(5), " +
                         "ea_nro_ord character varying(5), " +
                         "ea_tip_doc character varying(1), " +
                         "ea_nro_doc numeric, " +
                         "ea_nombre character varying(50), " +
                         "ea_sexo character varying(1), " +
                         "ea_est_civ character varying(1), " +
                         "ea_fec_ing timestamp without time zone, " +
                         "ea_subsector character varying(10) " +
                       "); ";
                        postgreSql.EjecutarQuery(Query);
                    }

                    

                    Query = "SELECT COUNT(*) FROM temp_activos;";
                    Console.WriteLine("Cantidad de registros activos en " + database + "." + esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de activos {0}.", ex.Message));
            }
        }

        public void CargarInformacion(String database, String Esquema, PostgreSql postgreSql)
        {
            try
            {
                String Query = string.Empty;

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_ultimoenvioConsegui;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TABLE temp_activos AS " +
                    "SELECT * FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT f3_view.ea_co_dane, f3_view.ea_nro_ord, f3_view.ea_tip_doc, f3_view.ea_nro_doc, f3_view.ea_nombre," +
                    "f3_view.ea_sexo, f3_view.ea_est_civ, f3_view.ea_fec_ing, f3_view.ea_subsector " +
                    "FROM " + database + "." + Esquema + ".tbl_f3_empleados_activos AS f3_view " +
                        "JOIN( " +
                               "SELECT ea_co_dane, ea_nro_ord, MAX(ea_nroinforme) AS informe " +
                               "FROM " + database + "." + Esquema + ".tbl_f3_empleados_activos " +
                               "GROUP BY ea_co_dane, ea_nro_ord) AS lastReport " +
                               "ON lastReport.ea_co_dane = f3_view.ea_co_dane AND lastReport.ea_nro_ord = f3_view.ea_nro_ord AND lastReport.informe = f3_view.ea_nroinforme " +
                        "ORDER BY f3_view.ea_co_dane, f3_view.ea_nro_ord')) " +
                    "AS DATA( " +
                     "ea_co_dane character varying(5), " +
                     "ea_nro_ord character varying(5), " +
                     "ea_tip_doc character varying(1), " +
                     "ea_nro_doc numeric, " +
                     "ea_nombre character varying(50), " +
                     "ea_sexo character varying(1), " +
                     "ea_est_civ character varying(1), " +
                     "ea_fec_ing timestamp without time zone, " +
                     "ea_subsector character varying(10) " +
                   "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_activos;";
                    Console.WriteLine("Cantidad de registros activos en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de activos {0}.", ex.Message));
            }
        }
    }
}
