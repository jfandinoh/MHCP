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
    public class cslPensionados
    {
        public cslPensionados() { }

        public void CargarInformacion(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_pensionados;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_pensionados AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_f5.pe_co_dane, view_f5.pe_nro_ord, view_f5.pe_tip_doc, view_f5.pe_nro_doc, view_f5.pe_nombre, " +
                        "view_f5.pe_sexo, view_f5.pe_est_civ, view_f5.pe_tipo_pension, view_f5.pe_directo, view_f5.pe_sector " +
                        "FROM " + database + "." + Esquema + ".tbl_f5_pensionados AS view_f5 " +
                        "JOIN( " +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM " + database + "." + Esquema + ".tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport " +
                        "ON lastReport.ua_co_dane = view_f5.pe_co_dane AND lastReport.ua_nro_ord = view_f5.pe_nro_ord AND lastReport.informe = view_f5.pe_nroinforme " +
                        "WHERE pe_eliminado IS FALSE " +
                        "ORDER BY view_f5.pe_co_dane, view_f5.pe_co_dane')) " +
                    "AS DATA( " +
                            "pe_co_dane character varying(5), " +
                            "pe_nro_ord character varying(5), " +
                            "pe_tip_doc character varying(1), " +
                            "pe_nro_doc numeric, " +
                            "pe_nombre character varying(50), " +
                            "pe_sexo character varying(1), " +
                            "pe_est_civ character varying(1), " +
                            "pe_tipo_pension character varying(1), " +
                            "pe_directo character varying(1), " +
                            "pe_sector character varying(2) " +
                    "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_pensionados;";
                    Console.WriteLine("Cantidad de registros pensionados en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de pensionados {0}.", ex.Message));
            }
        }

        public void CargarInformacionFallecidos(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_pensionados_fallecidos;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_pensionados_fallecidos AS "+
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_pf.pf_co_dane, view_pf.pf_nro_ord, view_pf.pf_tip_mue, view_pf.pf_doc_mue, view_pf.pf_nom_mue, " +
                       "view_pf.pf_sexo, view_pf.pf_est_civ, view_pf.pf_tipo_pension, view_pf.pf_directo, view_pf.pf_sector " +
                       "FROM " + database + "." + Esquema + ".tbl_pensionados_fallecidos AS view_pf " +
                       "JOIN(" +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM " + database + "." + Esquema + ".tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport "+
                       "ON lastReport.ua_co_dane = view_pf.pf_co_dane AND lastReport.ua_nro_ord = view_pf.pf_nro_ord AND lastReport.informe = view_pf.pf_nroinforme "+
                       "WHERE pf_eliminado IS FALSE " +
                       "ORDER BY view_pf.pf_co_dane, view_pf.pf_co_dane')) "+
                    "AS DATA( " +
                        "pf_co_dane character varying(5), " +
                        "pf_nro_ord character varying(5), " +
                        "pf_tip_mue character varying(1), " +
                        "pf_doc_mue numeric, " +
                        "pf_nom_mue character varying(50), " +
                        "pf_sexo character varying(1), " +
                        "pf_est_civ character varying(1), " +
                        "pf_tipo_pension character varying(1), " +
                        "pf_directo character varying(1), " +
                        "pf_sector character varying(2) " +
                    "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_pensionados_fallecidos;";
                    Console.WriteLine("Cantidad de registros pensionados fallecidos en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de pensionados fallecidos {0}.", ex.Message));
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
                    Query = "DROP TABLE IF EXISTS temp_hl_pensionados;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_hl_pensionados AS "+
                    "SELECT* FROM dblink "+
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_f5a.hp_co_dane, view_f5a.hp_nro_ord, view_f5a.hp_tip_doc, view_f5a.hp_nro_doc, view_f5a.hp_nit, " +
                       "view_f5a.hp_empresa, view_f5a.hp_fec_ing, view_f5a.hp_fec_ret, view_f5a.hp_ciudad, ci_nombre, dp_nombre " +
                       "FROM pc_datos_ultenvio_',BD,'.pc_datos_', cod_depto ,E'.tbl_f5a_historia_laboral_pensionados AS view_f5a " +
                       "JOIN( " +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM pc_datos_ultenvio_',BD,'.pc_datos_', cod_depto ,E'.tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport " +
                       "ON lastReport.ua_co_dane = view_f5a.hp_co_dane AND lastReport.ua_nro_ord = view_f5a.hp_nro_ord AND lastReport.informe = view_f5a.hp_nroinforme " +
                       "LEFT JOIN pc_datos_ultenvio_',BD,'.pc_datos_indice.view_cuidades ON view_f5a.hp_ciudad = view_cuidades.ci_co_dane " +
                       "JOIN pc_datos_ultenvio_',BD,'.pc_datos_indice.view_departamentos ON view_cuidades.ci_co_dept = view_departamentos.dp_co_dept " +
                       "WHERE view_f5a.hp_fec_ing IS NOT NULL AND view_f5a.hp_fec_ret IS NOT NULL " +
                       "ORDER BY view_f5a.hp_co_dane, view_f5a.hp_co_dane')) " +
                    "AS DATA( " +
                        "hp_co_dane character varying(5), " +
                        "hp_nro_ord character varying(5), " +
                        "hp_tip_doc character varying(1), "+
                        "hp_nro_doc numeric, "+
                        "hp_nit integer, "+
                        "hp_empresa character varying(50), "+
                        "hp_fec_ing timestamp without time zone, "+
                        "hp_fec_ret timestamp without time zone, "+
                        "hp_ciudad character varying(5), "+
                        "ci_nombre character varying(100), "+
                        "dp_nombre character varying(50) "+
                    "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_hl_pensionados;";
                    Console.WriteLine("Cantidad de registros historia laboral de pensionados en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de historias laborales de pensionados {0}.", ex.Message));
            }
        }

        public void CargarInformacionHlFallecidos(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_hl_pensionados_fallecidos;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_hl_pensionados_fallecidos AS "+
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_f6a.hb_co_dane, view_f6a.hb_nro_ord, view_f6a.hb_tip_doc, view_f6a.hb_nro_doc, view_f6a.hb_nit, " +
                        "view_f6a.hb_empresa, view_f6a.hb_fec_ing, view_f6a.hb_fec_ret, view_f6a.hb_ciudad, " +
                        "ci_nombre, dp_nombre " +
                        "FROM pc_datos_ultenvio_',BD,'.pc_datos_', cod_depto ,E'.tbl_f6a_historia_laboral_pensionados_fallecidos AS view_f6a " +
                        "JOIN( " +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM pc_datos_ultenvio_',BD,'.pc_datos_', cod_depto ,E'.tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport " +
                        "ON lastReport.ua_co_dane = view_f6a.hb_co_dane AND lastReport.ua_nro_ord = view_f6a.hb_nro_ord AND lastReport.informe = view_f6a.hb_nroinforme " +
                        "LEFT JOIN pc_datos_ultenvio_',BD,'.pc_datos_indice.view_cuidades ON view_f6a.hb_ciudad = view_cuidades.ci_co_dane " +
                        "JOIN pc_datos_ultenvio_',BD,'.pc_datos_indice.view_departamentos ON view_cuidades.ci_co_dept = view_departamentos.dp_co_dept " +
                        "WHERE view_f6a.hb_fec_ing IS NOT NULL AND view_f6a.hb_fec_ret IS NOT NULL " +
                        "ORDER BY view_f6a.hb_co_dane, view_f6a.hb_co_dane')) " +
                   "AS DATA( " +
                     "hb_co_dane character varying(5), " +
                     "hb_nro_ord character varying(5), " +
                     "hb_tip_doc character varying(1), " +
                     "hb_nro_doc numeric, " +
                     "hb_nit integer, " +
                     "hb_empresa character varying(50), " +
                     "hb_fec_ing timestamp without time zone, " +
                     "hb_fec_ret timestamp without time zone, " +
                     "hb_ciudad character varying(5), " +
                     "ci_nombre character varying(100), " +
                     "dp_nombre character varying(50) " +
                   "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_hl_pensionados_fallecidos;";
                    Console.WriteLine("Cantidad de registros historia laboral de pensionados fallecidos en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de historias laborales de pensionados fallecidos {0}.", ex.Message));
            }
        }

        public void ConstruirInformacion(PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction,String tipoEntidad, String codUa)
        {
            try
            {
                String Query = string.Empty;
                String Where1 = string.Empty;
                String Where2 = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_reg3;";
                    postgreSql.EjecutarQuery(Query);

                    switch (tipoEntidad.ToUpper())
                    {
                        case "EC":
                            Where1 = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f5.pe_tip_doc NOT IN('X') " +
                                    "AND f5.pe_sexo IS NOT NULL AND f5.pe_sexo <> '' AND f5.pe_est_civ IS NOT NULL AND f5.pe_est_civ <> '' " +
                                    "AND f5.pe_nro_ord LIKE '01%' ";
                            Where2 = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f5.pf_tip_mue NOT IN('X') " +
                                    "AND f5.pf_sexo IS NOT NULL AND f5.pf_sexo <> '' AND f5.pf_est_civ IS NOT NULL AND f5.pf_est_civ <> '' " +
                                    "AND f5.pf_nro_ord LIKE '01%' ";
                            break;
                        case "ED":
                            Where1 = "WHERE (hv.tipoorganizacion NOT IN ('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f5.pe_tip_doc NOT IN ('X') "+
                                    "AND f5.pe_sexo IS NOT NULL AND f5.pe_sexo <> '' AND f5.pe_est_civ IS NOT NULL AND f5.pe_est_civ <> '' "+
                                    "AND LENGTH(f5.pe_nro_ord) = 2 AND f5.pe_nro_ord <> '01'; ";
                            Where2 = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f5.pf_tip_mue NOT IN('X') " +
                                    "AND f5.pf_sexo IS NOT NULL AND f5.pf_sexo <> '' AND f5.pf_est_civ IS NOT NULL AND f5.pf_est_civ <> '' " +
                                    "AND LENGTH(f5.pf_nro_ord) = 2 AND f5.pf_nro_ord <>'01'";
                            break;
                        case "ESE":
                            Where1 = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f5.pe_tip_doc NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f5.pe_nro_ord) > 2 ELSE(LENGTH(f5.pe_nro_ord) = 2 AND f5.pe_nro_ord <> '01') END) ";
                            Where2 = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f5.pf_tip_mue NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f5.pf_nro_ord) > 2 ELSE(LENGTH(f5.pf_nro_ord) = 2 AND f5.pf_nro_ord <> '01') END) ";
                            break;
                    }

                    Query = "CREATE TEMP TABLE temp_reg3 AS "+
                    "SELECT* FROM( "+
                        "--PV " +
                        "SELECT 3 AS tipo_registro, " +
                        "TRIM(mig.nombre) AS nombre_entidad, " +
                        "CASE WHEN LENGTH(hv.nitua) > 9 THEN SUBSTRING(hv.nitua,1,9) ELSE hv.nitua END AS nit_entidad, " +
                        "hv.dvua AS digito_verificacion, " +
                        "f5.pe_tip_doc AS tipo_documento, " +
                        "f5.pe_nro_doc AS numero_documento, " +
                        "TRIM(f5.pe_nombre) AS nombre_persona, " +
                        "f5.pe_sexo AS genero, " +
                        "f5.pe_est_civ AS estado_civil, " +
                        "COALESCE(f5.pe_tipo_pension, 'V') AS modalidad_pension, " +
                         "f5.pe_directo AS tipo_pension, " +
                        "CASE WHEN f5.pe_sector = 'SA' THEN 'S' " +
                             "WHEN f5.pe_sector = 'OS' THEN 'N' ELSE '' END AS concurrencia, " +
                        "'PV' AS tipo_pensionado " +
                        "FROM temp_pensionados f5 " +
                        "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f5.pe_co_dane = hv.codet AND f5.pe_nro_ord = hv.codua " +
                        "JOIN pruebas.entidades_migracion_inter mig ON f5.pe_co_dane = mig.\"codigoDane\" AND f5.pe_nro_ord = mig.\"unidadAdministrativa\" " +
                        Where1 +
                        "UNION " +
                        "-- PF " +
                        "SELECT 3 AS tipo_registro, " +
                        "TRIM(mig.nombre) AS nombre_entidad, " +
                        "CASE WHEN LENGTH(hv.nitua) > 9 THEN SUBSTRING(hv.nitua,1,9) ELSE hv.nitua END AS nit_entidad, " +
                        "hv.dvua AS digito_verificacion, " +
                        "f5.pf_tip_mue AS tipo_documento, " +
                        "f5.pf_doc_mue AS numero_documento, " +
                        "TRIM(f5.pf_nom_mue) AS nombre_persona, " +
                        "f5.pf_sexo AS genero, " +
                        "f5.pf_est_civ AS estado_civil, " +
                        "COALESCE(f5.pf_tipo_pension, 'V') AS modalidad_pension, " +
                         "CASE WHEN f5.pf_directo IS NULL OR f5.pf_directo = '' THEN 'D' ELSE f5.pf_directo END AS tipo_pension, " +
                         "CASE WHEN f5.pf_sector = 'SA' THEN 'S' " +
                             "WHEN f5.pf_sector = 'OS' THEN 'N' ELSE '' END AS concurrencia, " +
                        "'PF' AS tipo_pensionado " +
                        "FROM temp_pensionados_fallecidos f5 " +
                        "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f5.pf_co_dane = hv.codet AND f5.pf_nro_ord = hv.codua " +
                        "JOIN pruebas.entidades_migracion_inter mig ON f5.pf_co_dane = mig.\"codigoDane\" AND f5.pf_nro_ord = mig.\"unidadAdministrativa\" " +
                        Where2 +	
	                ")AS pensionados; ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_reg3;";
                    Console.WriteLine("Cantidad de registros pensionados + pensionados fallecidos para generar (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo informacion de pensionados + pensionados fallecidos  {0}.", ex.Message));
            }
        }

        public void ConstruirInformacionHl(PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction, String tipoEntidad, String codUa)
        {
            try
            {
                String Query = string.Empty;
                String Where1 = string.Empty;
                String Where2 = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_reg4;";
                    postgreSql.EjecutarQuery(Query);

                    switch (tipoEntidad.ToUpper())
                    {
                        case "EC":
                            Where1 = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f5a.hp_tip_doc NOT IN('X') " +
                                    "AND f5a.hp_nro_ord LIKE '01%' ";
                            Where2 = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f6a.hb_tip_doc NOT IN('X') " +
                                    "AND f6a.hb_nro_ord LIKE '01%' ";
                            break;
                        case "ED":
                            Where1 = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f5a.hp_tip_doc NOT IN('X') " +
                                    "AND LENGTH(f5a.hp_nro_ord) = 2 AND f5a.hp_nro_ord <>'01'";
                            Where2 = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f6a.hb_tip_doc NOT IN('X') " +
                                    "AND LENGTH(f6a.hb_nro_ord) = 2 AND f6a.hb_nro_ord <>'01'";
                            break;
                        case "ESE":
                            Where1 = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f5a.hp_tip_doc NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f5a.hp_nro_ord) > 2 ELSE(LENGTH(f5a.hp_nro_ord) = 2 AND f5a.hp_nro_ord <> '01') END) ";
                            Where2 = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f6a.hb_tip_doc NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f6a.hb_nro_ord) > 2 ELSE(LENGTH(f6a.hb_nro_ord) = 2 AND f6a.hb_nro_ord <> '01') END) ";
                            break;
                    }

                    Query = "CREATE TEMP TABLE temp_reg4 AS "+
                    "SELECT* FROM " +
                    "( " +
                    "SELECT 4 AS tipo_registro, " +
                    "f5a.hp_tip_doc AS tipo_documento, " +
                    "f5a.hp_nro_doc AS numero_documento, " +
                    "CASE WHEN f5a.hp_nit IS NOT NULL THEN " +
                        "CASE WHEN LENGTH(CAST(f5a.hp_nit AS text)) > 9 THEN SUBSTRING(CAST(f5a.hp_nit AS text),1,9) " +
                        "ELSE CAST(f5a.hp_nit AS text) END " +
                    "ELSE '' END AS nit_empresa, " +
                    "TRIM(f5a.hp_empresa) AS nombre_empresa, " +
                    "to_char(cast(f5a.hp_fec_ing as date), 'YYYYMMDD') AS fecha_inicial, " +
                    "to_char(cast(f5a.hp_fec_ret as date), 'YYYYMMDD') AS fecha_final, " +
                      "TRIM(ci_nombre) AS ciuidad, " +
                      "TRIM(dp_nombre) AS departamento, " +
                    "'PV' AS tipo_pensionado " +
                    "FROM temp_hl_pensionados f5a " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f5a.hp_co_dane = hv.codet AND f5a.hp_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON f5a.hp_co_dane = mig.\"codigoDane\" AND f5a.hp_nro_ord = mig.\"unidadAdministrativa\" " +
                    "JOIN temp_reg3 ON temp_reg3.tipo_documento = f5a.hp_tip_doc AND temp_reg3.numero_documento = f5a.hp_nro_doc AND temp_reg3.tipo_pensionado = 'PF' " +
                    Where1 +
                    "UNION " +
                    "SELECT 4 AS tipo_registro, " +
                    "f6a.hb_tip_doc AS tipo_documento, " +
                    "f6a.hb_nro_doc AS numero_documento, " +
                    "CASE WHEN f6a.hb_nit IS NOT NULL THEN " +
                        "CASE WHEN LENGTH(CAST(f6a.hb_nit AS text)) > 9 THEN SUBSTRING(CAST(f6a.hb_nit AS text),1,9) " +
                        "ELSE CAST(f6a.hb_nit AS text) END " +
                    "ELSE '' END AS nit_empresa, " +
                    "TRIM(f6a.hb_empresa) AS nombre_empresa, " +
                    "to_char(cast(f6a.hb_fec_ing as date), 'YYYYMMDD') AS fecha_inicial, " +
                    "to_char(cast(f6a.hb_fec_ret as date), 'YYYYMMDD') AS fecha_final, " +
                      "TRIM(ci_nombre) AS ciuidad, " +
                      "TRIM(dp_nombre) AS departamento, " +
                    "'PF' AS tipo_pensionado " +
                    "FROM temp_hl_pensionados_fallecidos f6a " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f6a.hb_co_dane = hv.codet AND f6a.hb_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON f6a.hb_co_dane = mig.\"codigoDane\" AND f6a.hb_nro_ord = mig.\"unidadAdministrativa\" " +
                    "JOIN temp_reg3 ON temp_reg3.tipo_documento = f6a.hb_tip_doc AND temp_reg3.numero_documento = f6a.hb_nro_doc AND temp_reg3.tipo_pensionado = 'PV' " +
                    Where2 +
	                ") AS historia_laboral; ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_reg4;";
                    Console.WriteLine("Cantidad de registros historia laboral pensionados para generar (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo informacion historias laborales de pensionados {0}.", ex.Message));
            }
        }
    }
}
