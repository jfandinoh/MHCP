using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using BibliotecaClases;
using System.IO;
using System.Data;

namespace GenerarArchivosAsofondos
{
    public class clsActivos
    {
        public clsActivos() { }

        public void CargarInformacion(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_activos;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_activos AS " +
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

        public void CargarInformacionHl(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_hl_activos;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_hl_activos AS " +
                    "SELECT * FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_f4.ha_co_dane, view_f4.ha_nro_ord, view_f4.ha_tip_doc, view_f4.ha_nro_doc, view_f4.ha_nit, " +
                    "view_f4.ha_empresa, view_f4.ha_fec_ing, view_f4.ha_fec_ret, view_f4.ha_ciudad, " +
                    "ci_nombre, dp_nombre " +
                    "FROM " + database + "." + Esquema + ".tbl_f4_historia_laboral_empleados_activos AS view_f4 " +
                       "JOIN( " +
                           "SELECT ea_co_dane, ea_nro_ord, MAX(ea_nroinforme) AS informe " +
                           "FROM " + database + "." + Esquema + ".tbl_f3_empleados_activos " +
                           "GROUP BY ea_co_dane, ea_nro_ord) AS lastReport " +
                       "ON lastReport.ea_co_dane = view_f4.ha_co_dane AND lastReport.ea_nro_ord = view_f4.ha_nro_ord AND lastReport.informe = view_f4.ha_nroinforme " +
                       "LEFT JOIN " + database + ".pc_datos_indice.view_cuidades ON view_f4.ha_ciudad = view_cuidades.ci_co_dane " +
                       "JOIN " + database + ".pc_datos_indice.view_departamentos ON view_cuidades.ci_co_dept = view_departamentos.dp_co_dept " +
                       "WHERE view_f4.ha_fec_ing IS NOT NULL AND view_f4.ha_fec_ret IS NOT NULL " +
                       "ORDER BY view_f4.ha_co_dane, view_f4.ha_co_dane')) " +
                    "AS DATA( " +
                        "ha_co_dane character varying(5), " +
                        "ha_nro_ord character varying(5), " +
                        "ha_tip_doc character varying(1), " +
                        "ha_nro_doc numeric, " +
                        "ha_nit integer, " +
                        "ha_empresa character varying(50), " +
                        "ha_fec_ing timestamp without time zone, " +
                        "ha_fec_ret timestamp without time zone, " +
                        "ha_ciudad character varying(5), " +
                        "ci_nombre character varying(100), " +
                        "dp_nombre character varying(50) " +
                    ");";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_hl_activos;";
                    Console.WriteLine("Cantidad de registros historia laboral de activos en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de historias laborales de activos {0}.", ex.Message));
            }
        }

        public void ConstruirInformacion(PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction,String tipoEntidad, String codUa)
        {
            try
            {
                String Query = string.Empty;
                String Where = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_reg1;";
                    postgreSql.EjecutarQuery(Query);
                    
                    switch (tipoEntidad.ToUpper()) 
                    {
                        case "EC":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f3.ea_tip_doc NOT IN('X')  " +
                                    "AND f3.ea_nro_ord LIKE '01%'; ";
                            break;
                        case "ED":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f3.ea_tip_doc NOT IN('X')  " +
                                    "AND LENGTH(f3.ea_nro_ord) = 2 AND f3.ea_nro_ord <>'01'; ";
                            break;
                        case "ESE":
                            Where = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f3.ea_tip_doc NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa +"' = '01' THEN LENGTH(f3.ea_nro_ord) > 2 ELSE(LENGTH(f3.ea_nro_ord) = 2 AND f3.ea_nro_ord <> '01') END); ";
                            break;
                    }

                    Query = "CREATE TEMP TABLE temp_reg1 AS " +
                    "SELECT 1 AS tipo_registro, " +
                    "TRIM(mig.nombre) AS nombre_entidad, " +
                    "CASE WHEN LENGTH(hv.nitua) > 9 THEN SUBSTRING(hv.nitua,1,9) ELSE hv.nitua END AS nit_entidad, " +
                    "hv.dvua AS digito_verificacion, " +
                    "f3.ea_tip_doc AS tipo_documento, " +
                    "f3.ea_nro_doc AS numero_documento, " +
                    "f3.ea_nombre AS nombre_persona, " +
                    "f3.ea_sexo AS genero, " +
                    "f3.ea_est_civ AS estado_civil, " +
                    "to_char(cast(f3.ea_fec_ing as date), 'YYYYMMDD') AS fecha_inicial, " +
                    "CASE WHEN f3.ea_subsector = 'S_CONC' THEN 'S' " +
                         "WHEN f3.ea_subsector = 'S_ENTI' THEN 'N' ELSE '' END AS concurrencia " +
                    "FROM temp_activos f3 " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f3.ea_co_dane = hv.codet AND f3.ea_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON f3.ea_co_dane = mig.\"codigoDane\" AND f3.ea_nro_ord = mig.\"unidadAdministrativa\" " +
                    Where;
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_reg1;";
                    Console.WriteLine("Cantidad de registros activos para generar (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo informacion de activos {0}.", ex.Message));
            }
        }

        public void ConstruirInformacionHl(PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction,String tipoEntidad, String codUa)
        {
            try
            {
                String Query = string.Empty;
                String Where = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_reg2;";
                    postgreSql.EjecutarQuery(Query);

                    switch (tipoEntidad.ToUpper())
                    {
                        case "EC":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f3.ha_tip_doc NOT IN('X') " +
                                    "AND f3.ha_nro_ord LIKE '01%'; ";
                            break;
                        case "ED":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f3.ha_tip_doc NOT IN('X')  " +
                                    "AND LENGTH(f3.ha_nro_ord) = 2 AND f3.ha_nro_ord <>'01';";
                            break;
                        case "ESE":
                            Where = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f3.ha_tip_doc NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f3.ha_nro_ord) > 2 ELSE(LENGTH(f3.ha_nro_ord) = 2 AND f3.ha_nro_ord <> '01') END); ";
                            break;
                    }

                    Query = "CREATE TEMP TABLE temp_reg2 AS " +
                    "SELECT DISTINCT 2 AS tipo_registro, " +
                    "f3.ha_tip_doc AS tipo_documento, " +
                    "f3.ha_nro_doc AS numero_documento, " +
                    "CASE WHEN f3.ha_nit IS NOT NULL THEN " +
                        "CASE WHEN LENGTH(CAST(f3.ha_nit AS text)) > 9 THEN SUBSTRING(CAST(f3.ha_nit AS text),1,9) " +
                        "ELSE CAST(f3.ha_nit AS text) END " +
                    "ELSE '' END AS nit_empresa, " +
                    "TRIM(f3.ha_empresa) AS nombre_empresa, " +
                    "to_char(cast(f3.ha_fec_ing as date), 'YYYYMMDD') AS fecha_inicial, " +
                    "to_char(cast(f3.ha_fec_ret as date), 'YYYYMMDD') AS fecha_final, " +
                    "TRIM(ci_nombre) AS ciuidad, " +
                    "TRIM(dp_nombre) AS departamento " +
                    "FROM temp_hl_activos f3 " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f3.ha_co_dane = hv.codet AND f3.ha_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON f3.ha_co_dane = mig.\"codigoDane\" AND f3.ha_nro_ord = mig.\"unidadAdministrativa\" " +
                    "JOIN temp_reg1 activos ON activos.tipo_documento = f3.ha_tip_doc AND activos.numero_documento = f3.ha_nro_doc " +
                    Where;
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_reg2;";
                    Console.WriteLine("Cantidad de registros historia laboral activos para generar (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo informacion historias laborales de activos {0}.", ex.Message));
            }
        }
    }
}
