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
    public class cslRetirados
    {
        public cslRetirados() { }

        public void CargarInformacion(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_retirados;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_retirados AS "+
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_f9.rs_co_dane, view_f9.rs_nro_ord, view_f9.rs_tip_doc, view_f9.rs_nro_doc, view_f9.rs_nombre, " +
                       "view_f9.rs_sexo, view_f9.rs_est_civ, view_f9.rs_fec_ing, view_f9.rs_fec_ret, view_f9.rs_sector " +
                       "FROM " + database + "." + Esquema + ".tbl_f9_retirados AS view_f9 " +
                       "JOIN( " +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM " + database + "." + Esquema + ".tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport " +
                       "ON lastReport.ua_co_dane = view_f9.rs_co_dane AND lastReport.ua_nro_ord = view_f9.rs_nro_ord AND lastReport.informe = view_f9.rs_nroinforme " +
                       "WHERE view_f9.rs_eliminado IS FALSE AND view_f9.rs_fec_ing IS NOT NULL AND view_f9.rs_fec_ret IS NOT NULL " +
                       "ORDER BY view_f9.rs_co_dane, view_f9.rs_co_dane')) " +
                    "AS DATA( " +
                     "rs_co_dane character varying(5), " +
                     "rs_nro_ord character varying(5), " +
                     "rs_tip_doc character varying(1), " +
                     "rs_nro_doc double precision, " +
                     "rs_nombre character varying(50), " +
                     "rs_sexo character varying(1), " +
                     "rs_est_civ character varying(1), " +
                     "rs_fec_ing timestamp without time zone, " +
                     "rs_fec_ret timestamp without time zone, " +
                     "rs_sector character varying(2) " +
                    "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_retirados;";
                    Console.WriteLine("Cantidad de registros retirados en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de retirados {0}.", ex.Message));
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
                    Query = "DROP TABLE IF EXISTS temp_hl_retirados;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_hl_retirados AS "+
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_f9a.hs_co_dane, view_f9a.hs_nro_ord, view_f9a.hs_tip_doc, view_f9a.hs_nro_doc, view_f9a.hs_nit, " +
                        "view_f9a.hs_empresa, view_f9a.hs_fec_ing, view_f9a.hs_fec_ret, view_f9a.hs_ciudad, " +
                        "ci_nombre, dp_nombre " +
                        "FROM " + database + "." + Esquema + ".tbl_f9a_historia_laboral_retirados AS view_f9a " +
                        "JOIN( " +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM " + database + "." + Esquema + ".tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport " +
                        "ON lastReport.ua_co_dane = view_f9a.hs_co_dane AND lastReport.ua_nro_ord = view_f9a.hs_nro_ord AND lastReport.informe = view_f9a.hs_nroinforme " +
                        "LEFT JOIN " + database + ".pc_datos_indice.view_cuidades ON view_f9a.hs_ciudad = view_cuidades.ci_co_dane " +
                        "JOIN " + database + ".pc_datos_indice.view_departamentos ON view_cuidades.ci_co_dept = view_departamentos.dp_co_dept " +
                        "JOIN " + database + "." + Esquema + ".tbl_f9_retirados AS view_f9 " +
                        "ON view_f9a.hs_tip_doc = view_f9.rs_tip_doc AND view_f9a.hs_nro_doc = view_f9.rs_nro_doc AND view_f9.rs_nroinforme = view_f9a.hs_nroinforme " +
                        "AND view_f9.rs_co_dane = view_f9a.hs_co_dane AND  view_f9.rs_nro_ord = view_f9a.hs_nro_ord " +
                        "WHERE view_f9a.hs_fec_ing IS NOT NULL AND view_f9a.hs_fec_ret IS NOT NULL AND view_f9.rs_eliminado IS FALSE " +
                        "ORDER BY view_f9a.hs_co_dane, view_f9a.hs_co_dane')) " +
                    "AS DATA( " +
                        "hs_co_dane character varying(5), " +
                        "hs_nro_ord character varying(5), " +
                        "hs_tip_doc character varying(1), " +
                        "hs_nro_doc numeric, " +
                        "hs_nit integer, " +
                        "hs_empresa character varying(50), " +
                        "hs_fec_ing timestamp without time zone, " +
                        "hs_fec_ret timestamp without time zone, " +
                        "hs_ciudad character varying(5), " +
                        "ci_nombre character varying(100), " +
                        "dp_nombre character varying(50) " +
                    "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_hl_retirados;";
                    Console.WriteLine("Cantidad de registros historia laboral de retirados en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de historias laborales de retirados {0}.", ex.Message));
            }
        }

        public void CargarInformacionAFP(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_hl_afp_retirados;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_hl_afp_retirados AS "+
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_afp_retirados.pb_co_dane, view_afp_retirados.pb_nro_ord, view_afp_retirados.pb_tip_doc,  " +
                        "view_afp_retirados.pb_nro_doc, view_afp_retirados.pb_afp, view_afp_retirados.pb_fec_afiliacion, " +
                        "view_afp_retirados.pb_fec_fin_afiliacion " +
                        "FROM " + database + "." + Esquema + ".tbl_afiliaciones_afp_retirados AS view_afp_retirados " +
                        "JOIN( " +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM " + database + "." + Esquema + ".tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport " +
                        "ON lastReport.ua_co_dane = view_afp_retirados.pb_co_dane AND lastReport.ua_nro_ord = view_afp_retirados.pb_nro_ord AND lastReport.informe = view_afp_retirados.pb_nroinforme " +
                        "JOIN " + database + "." + Esquema + ".tbl_f9_retirados AS view_f9 " +
                        "ON view_afp_retirados.pb_tip_doc = view_f9.rs_tip_doc AND view_afp_retirados.pb_nro_doc = view_f9.rs_nro_doc " +
                        "AND view_afp_retirados.pb_nroinforme = view_f9.rs_nroinforme AND view_afp_retirados.pb_co_dane = view_f9.rs_co_dane AND view_afp_retirados.pb_nro_ord = view_f9.rs_nro_ord " +
                        "WHERE view_afp_retirados.pb_fec_afiliacion IS NOT NULL AND view_f9.rs_eliminado IS FALSE " +
                        "ORDER BY view_afp_retirados.pb_co_dane, view_afp_retirados.pb_co_dane')) " +
                    "AS DATA( " +
                        "pb_co_dane character varying(5), " +
                        "pb_nro_ord character varying(5), " +
                        "pb_tip_doc character varying(1), " +
                        "pb_nro_doc numeric, " +
                        "pb_afp integer, " +
                        "pb_fec_afiliacion timestamp without time zone, " +
                        "pb_fec_fin_afiliacion timestamp without time zone " +
                    "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_hl_afp_retirados;";
                    Console.WriteLine("Cantidad de registros historia laboral AFP de retirados en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de historias laborales de retirados {0}.", ex.Message));
            }
        }

        public void ConstruirInformacion(PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction, String tipoEntidad, String codUa)
        {
            try
            {
                String Query = string.Empty;
                String Where = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_reg6;";
                    postgreSql.EjecutarQuery(Query);

                    switch (tipoEntidad.ToUpper())
                    {
                        case "EC":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f6.rs_tip_doc NOT IN('X') " +
                                    "AND f6.rs_sexo IS NOT NULL AND f6.rs_sexo <> '' AND f6.rs_est_civ IS NOT NULL AND f6.rs_est_civ <> '' " +
                                    "AND f6.rs_nro_ord LIKE '01%'; "; 
                            break;
                        case "ED":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f6.rs_tip_doc NOT IN('X') " +
                                    "AND f6.rs_sexo IS NOT NULL AND f6.rs_sexo <> '' AND f6.rs_est_civ IS NOT NULL AND f6.rs_est_civ <> '' " +
                                    "AND LENGTH(f6.rs_nro_ord) = 2 AND f6.rs_nro_ord <>'01'; ";
                            break;
                        case "ESE":
                            Where = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f6.rs_tip_doc NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f6.rs_nro_ord) > 2 ELSE(LENGTH(f6.rs_nro_ord) = 2 AND f6.rs_nro_ord <> '01') END); ";
                            break;
                    }

                    Query = "CREATE TEMP TABLE temp_reg6 AS " +
                    "SELECT 6 AS tipo_registro, " +
                    "TRIM(mig.nombre) AS nombre_entidad, " +
                    "CASE WHEN LENGTH(hv.nitua) > 9 THEN SUBSTRING(hv.nitua,1,9) ELSE hv.nitua END AS nit_entidad, " +
                    "hv.dvua AS digito_verificacion, " +
                    "f6.rs_tip_doc AS tipo_documento, " +
                    "f6.rs_nro_doc AS numero_documento, " +
                    "TRIM(f6.rs_nombre) AS nombre_persona, " +
                    "CASE WHEN f6.rs_sexo = 'f' THEN 'F' ELSE f6.rs_sexo END AS genero, " +
                    "CASE WHEN f6.rs_est_civ = 's' THEN 'S' ELSE f6.rs_est_civ END AS estado_civil, " +
                    "to_char(cast(f6.rs_fec_ing as date), 'YYYYMMDD') AS fecha_inicial, " +
                     "to_char(cast(f6.rs_fec_ret as date), 'YYYYMMDD') AS fecha_final, " +
                      "CASE WHEN f6.rs_sector = 'SA' THEN 'S' " +
                         "WHEN f6.rs_sector = 'OS' OR f6.rs_sector = 'ED' THEN 'N' ELSE '' END AS concurrencia " +
                    "FROM temp_retirados f6 " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f6.rs_co_dane = hv.codet AND f6.rs_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON f6.rs_co_dane = mig.\"codigoDane\" AND f6.rs_nro_ord = mig.\"unidadAdministrativa\" " +
                    Where;

                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_reg6;";
                    Console.WriteLine("Cantidad de registros retirados para generar (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo informacion de sustitutos {0}.", ex.Message));
            }
        }

        public void ConstruirInformacionHl(PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction, String tipoEntidad, String codUa)
        {
            try
            {
                String Query = string.Empty;
                String Where = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_reg7;";
                    postgreSql.EjecutarQuery(Query);

                    switch (tipoEntidad.ToUpper())
                    {
                        case "EC":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f9a.hs_tip_doc NOT IN('X') " +
                                    "AND f9a.hs_nro_ord LIKE '01%'; ";
                            break;
                        case "ED":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f9a.hs_tip_doc NOT IN('X') " +
                                    "AND LENGTH(f9a.hs_nro_ord) = 2 AND f9a.hs_nro_ord <>'01'; ";
                            break;
                        case "ESE":
                            Where = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f9a.hs_tip_doc NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f9a.hs_nro_ord) > 2 ELSE(LENGTH(f9a.hs_nro_ord) = 2 AND f9a.hs_nro_ord <> '01') END); ";
                            break;
                    }

                    Query = "CREATE TEMP TABLE temp_reg7 AS " +
                    "SELECT 7 AS tipo_registro, " +
                    "f9a.hs_tip_doc AS tipo_documento, " +
                    "f9a.hs_nro_doc AS numero_documento, " +
                    "CASE WHEN f9a.hs_nit IS NOT NULL THEN " +
                        "CASE WHEN LENGTH(CAST(f9a.hs_nit AS text)) > 9 THEN SUBSTRING(CAST(f9a.hs_nit AS text),1,9) " +
                        "ELSE CAST(f9a.hs_nit AS text) END " +
                    "ELSE '' END AS nit_empresa, " +
                    "TRIM(f9a.hs_empresa) AS nombre_empresa, " +
                    "to_char(cast(f9a.hs_fec_ing as date), 'YYYYMMDD') AS fecha_inicial, " +
                     "to_char(cast(f9a.hs_fec_ret as date), 'YYYYMMDD') AS fecha_final, " +
                      "TRIM(ci_nombre) AS ciuidad, " +
                      "TRIM(dp_nombre) AS departamento " +
                    "FROM temp_hl_retirados f9a " +
                    "--JOIN temp_reg6 f9 ON f9.tipo_documento = f9a.hs_tip_doc AND f9.numero_documento = f9a.hs_nro_doc " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f9a.hs_co_dane = hv.codet AND f9a.hs_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON f9a.hs_co_dane = mig.\"codigoDane\" AND f9a.hs_nro_ord = mig.\"unidadAdministrativa\" " +
                    Where;
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_reg7;";
                    Console.WriteLine("Cantidad de registros historia laboral retirados para generar (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo informacion historias laborales de retirados {0}.", ex.Message));
            }
        }

        public void ConstruirInformacionHlAFP(PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction, String tipoEntidad, String codUa)
        {
            try
            {
                String Query = string.Empty;
                String Where = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_reg8;";
                    postgreSql.EjecutarQuery(Query);

                    switch (tipoEntidad.ToUpper())
                    {
                        case "EC":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f8.pb_tip_doc NOT IN('X') " +
                                    "AND f8.pb_fec_afiliacion<> f8.pb_fec_fin_afiliacion AND f8.pb_nro_ord LIKE '01%'; ";
                            break;
                        case "ED":
                            Where = "WHERE (hv.tipoorganizacion NOT IN ('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f8.pb_tip_doc NOT IN ('X') "+
                                    "AND f8.pb_fec_afiliacion<> f8.pb_fec_fin_afiliacion AND LENGTH(f8.pb_nro_ord) = 2 AND f8.pb_nro_ord <> '01'; ";
                            break;
                        case "ESE":
                            Where = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f8.pb_tip_doc NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f8.pb_nro_ord) > 2 ELSE(LENGTH(f8.pb_nro_ord) = 2 AND f8.pb_nro_ord <> '01') END); ";
                            break;
                    }

                    Query = "CREATE TEMP TABLE temp_reg8 AS " +
                    "SELECT 8 AS tipo_registro, " +
                    "f8.pb_tip_doc AS tipo_documento, " +
                    "f8.pb_nro_doc AS numero_documento, " +
                    "CASE WHEN fondos.id IS NOT NULL THEN fondos.nombre ELSE 'No Registra' END AS nombre_fondo_aportes, " +
                    "to_char(cast(f8.pb_fec_afiliacion as date), 'YYYYMMDD') AS fecha_inicial, " +
                     "CASE WHEN f8.pb_fec_fin_afiliacion IS NOT NULL THEN to_char(cast(f8.pb_fec_fin_afiliacion as date),'YYYYMMDD') " +
                         "ELSE ''  END AS fecha_final " +
                    "FROM temp_hl_afp_retirados f8 " +
                    "--JOIN temp_reg6 f9 ON f9.tipo_documento = f8.pb_tip_doc AND f9.numero_documento = f8.pb_nro_doc " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f8.pb_co_dane = hv.codet AND f8.pb_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON f8.pb_co_dane = mig.\"codigoDane\" AND f8.pb_nro_ord = mig.\"unidadAdministrativa\" " +
                    "LEFT JOIN pruebas.fondos_aportes AS fondos ON fondos.id = f8.pb_afp " +
                    Where;
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_reg8;";
                    Console.WriteLine("Cantidad de registros historia laboral AFP retirados para generar (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo informacion historias laborales de retirados {0}.", ex.Message));
            }
        }
    }
}
