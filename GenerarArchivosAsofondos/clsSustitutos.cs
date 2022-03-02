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
    public class clsSustitutos
    {
        public clsSustitutos() { }

        public void CargarInformacionPensionadosFallecidos(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
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
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_pf.pf_co_dane, view_pf.pf_nro_ord, view_pf.pf_tip_mue, view_pf.pf_doc_mue " +
                       "FROM " + database + "." + Esquema + ".tbl_pensionados_fallecidos AS view_pf " +
                       "JOIN( " +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM " + database + "." + Esquema + ".tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport " +
                       "ON lastReport.ua_co_dane = view_pf.pf_co_dane AND lastReport.ua_nro_ord = view_pf.pf_nro_ord AND lastReport.informe = view_pf.pf_nroinforme " +
                       "WHERE pf_eliminado IS FALSE " +
                       "ORDER BY view_pf.pf_co_dane, view_pf.pf_co_dane')) " +
                   "AS DATA( " +
                     "pf_co_dane character varying(5), " +
                     "pf_nro_ord character varying(5), " +
                     "pf_tip_mue character varying(1), " +
                     "pf_doc_mue numeric " +
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
                throw new Exception(string.Format("Error cargando informacion de pensionados {0}.", ex.Message));
            }
        }

        public void CargarInformacion(String database, String Esquema, PostgreSql postgreSql, NpgsqlTransaction npgsqlTransaction)
        {
            try
            {
                String Query = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    Query = "DROP TABLE IF EXISTS temp_sustitutos;";
                    postgreSql.EjecutarQuery(Query);

                    Query = "CREATE TEMP TABLE temp_sustitutos AS "+
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('SELECT DISTINCT view_f6.be_co_dane, view_f6.be_nro_ord, view_f6.be_tip_mue, view_f6.be_doc_mue, view_pf.pf_nom_mue, " +
                       "view_pf.pf_sexo, view_pf.pf_est_civ, view_pf.pf_tipo_pension, view_f6.be_tip_doc, view_f6.be_nro_doc, " +
                       "view_f6.be_nombre, view_f6.be_sexo, view_f6.be_directo, view_pf.pf_sector " +
                       "FROM " + database + "." + Esquema + ".tbl_f6_beneficiarios AS view_f6 " +
                       "JOIN( " +
                           "SELECT ua_co_dane, ua_nro_ord, MAX(ua_nroinforme) AS informe " +
                           "FROM " + database + "." + Esquema + ".tbl_f1_unidades_administrativas " +
                           "GROUP BY ua_co_dane, ua_nro_ord) AS lastReport " +
                       "ON lastReport.ua_co_dane = view_f6.be_co_dane AND lastReport.ua_nro_ord = view_f6.be_nro_ord AND lastReport.informe = view_f6.be_nroinforme " +
                       "JOIN " + database + "." + Esquema + ".tbl_pensionados_fallecidos AS view_pf " +
                       "ON view_f6.be_tip_mue = view_pf.pf_tip_mue AND view_f6.be_doc_mue = view_pf.pf_doc_mue AND view_f6.be_nroinforme = view_pf.pf_nroinforme " +
                       "AND view_f6.be_co_dane = view_pf.pf_co_dane AND view_f6.be_nro_ord = view_pf.pf_nro_ord " +
                       "WHERE be_eliminado IS FALSE AND view_pf.pf_eliminado IS FALSE " +
                       "ORDER BY view_f6.be_co_dane, view_f6.be_co_dane')) " +
                    "AS DATA( " +
                     "be_co_dane character varying(5), " +
                     "be_nro_ord character varying(5), " +
                     "be_tip_mue character varying(1), "+
                     "be_doc_mue double precision, "+
                     "pf_nom_mue character varying(50), "+
                     "pf_sexo character varying(1), "+
                     "pf_est_civ character varying(1), "+
                     "pf_tipo_pension character varying(1), "+
                     "be_tip_doc character varying(1), "+
                     "be_nro_doc numeric, "+
                     "be_nombre character varying(50), "+
                     "be_sexo character varying(1), "+
                     "be_directo character varying(1), "+
                     "pf_sector character varying(2) "+
                    "); ";
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_sustitutos;";
                    Console.WriteLine("Cantidad de registros sustitutos en " + database + "." + Esquema + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion de sustitutos {0}.", ex.Message));
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
                    Query = "DROP TABLE IF EXISTS temp_reg5;";
                    postgreSql.EjecutarQuery(Query);

                    switch (tipoEntidad.ToUpper())
                    {
                        case "EC":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f6.be_tip_doc NOT IN('X') AND f6.be_tip_mue NOT IN('X') " +
                                    "AND f6.be_sexo IS NOT NULL AND f6.be_sexo <> '' AND f6.pf_sexo IS NOT NULL AND f6.pf_sexo <> '' AND f6.pf_est_civ IS NOT NULL AND f6.pf_est_civ <> '' " +
                                    "AND f6.be_nro_ord LIKE '01%'; ";
                            break;
                        case "ED":
                            Where = "WHERE(hv.tipoorganizacion NOT IN('E.S.E.') OR hv.tipoorganizacion IS NULL) AND f6.be_tip_doc NOT IN('X') AND f6.be_tip_mue NOT IN('X') " +
                                    "AND f6.be_sexo IS NOT NULL AND f6.be_sexo <> '' AND f6.pf_sexo IS NOT NULL AND f6.pf_sexo <> '' AND f6.pf_est_civ IS NOT NULL AND f6.pf_est_civ <> '' " +
                                    "AND LENGTH(f6.be_nro_ord) = 2 AND f6.be_nro_ord <>'01';";
                            break;
                        case "ESE":
                            Where = "WHERE hv.tipoorganizacion = 'E.S.E.' AND f6.be_tip_doc NOT IN ('X') AND f6.be_tip_mue NOT IN ('X') "+
                                    "AND(CASE WHEN '" + codUa + "' = '01' THEN LENGTH(f6.be_nro_ord) > 2 ELSE(LENGTH(f6.be_nro_ord) = 2 AND f6.be_nro_ord <> '01') END); ";
                            break;
                    }

                    Query = "CREATE TEMP TABLE temp_reg5 AS " +
                    "SELECT DISTINCT 5 AS tipo_registro, " +
                    "TRIM(mig.nombre) AS nombre_entidad, " +
                    "CASE WHEN LENGTH(hv.nitua) > 9 THEN SUBSTRING(hv.nitua,1,9) ELSE hv.nitua END AS nit_entidad, " +
                    "hv.dvua AS digito_verificacion, " +
                    "f6.be_tip_doc AS tipo_documento, " +
                    "f6.be_nro_doc AS numero_documento, " +
                    "TRIM(f6.be_nombre) AS nombre_persona, " +
                    "f6.be_sexo AS genero, " +
                    "f6.be_tip_mue AS tipo_documento_causante, " +
                    "f6.be_doc_mue AS numero_documento_causante, " +
                    "TRIM(f6.pf_nom_mue) AS nombre_causante, " +
                    "f6.pf_sexo AS genero_causante, " +
                    "f6.pf_est_civ AS estado_civil_causante, " +
                    "COALESCE(f6.pf_tipo_pension, 'V') AS modalidad_pension, " +
                    "f6.be_directo AS tipo_pension, " +
                    "CASE WHEN f6.pf_sector = 'SA' THEN 'S' " +
                         "WHEN f6.pf_sector = 'OS' THEN 'N' ELSE '' END AS concurrencia " +
                    "FROM temp_sustitutos f6 " +
                    //"--JOIN temp_pf pf ON f6.be_tip_mue = pf.tipo_documento AND f6.be_doc_mue = pf.numero_documento " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON f6.be_co_dane = hv.codet AND f6.be_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON f6.be_co_dane = mig.\"codigoDane\" AND f6.be_nro_ord = mig.\"unidadAdministrativa\" " +
                    Where;
                    postgreSql.EjecutarQuery(Query);

                    Query = "SELECT COUNT(*) FROM temp_reg5;";
                    Console.WriteLine("Cantidad de registros sustitutos para generar (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(Query));

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
    }
}
