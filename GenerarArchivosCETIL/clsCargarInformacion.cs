using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BibliotecaClases;

namespace GenerarArchivosCETIL
{
    public class clsCargarInformacion
    {
        public clsCargarInformacion() { }

        public void InformacionSalarioBase(PostgreSql postgreSql)
        {
            try
            {
                String pc_datos = "pc_datos";

                String queryPostgrSql = string.Empty;
                clsConexion clsConexion = new clsConexion();

                try
                {
                    clsConexion.CrearConexiondb(pc_datos, postgreSql);

                    queryPostgrSql = "DROP TABLE IF EXISTS temp_salariobase;";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_salariobase AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select \"TIPO_DOCUMENTO_PK2\", \"NUMERO_DOCUMENTO_PK1\", \"FECHA_SALARIOBASEBONO\", \"SALARIOBASEBONO\" from " + pc_datos + ".pc_cedulas_registraduria.\"tbl_cedulasSalarioBaseBono\"', '')) " +
                    "AS DATA( " +
                        "\"TIPO_DOCUMENTO_PK2\" character varying(1), " +
                        "\"NUMERO_DOCUMENTO_PK1\" numeric, " +
                        "\"FECHA_SALARIOBASEBONO\" date , " +
                        "\"SALARIOBASEBONO\" numeric " +
                    "); " +
                    "CREATE INDEX ON temp_salariobase(\"TIPO_DOCUMENTO_PK2\", \"NUMERO_DOCUMENTO_PK1\"); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_salariobase;";
                    Console.WriteLine("Cantidad de registros salario base en " + pc_datos + "." + "pc_cedulas_registraduria" + " (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion salario base. {0}", ex.Message));
            }
        }

        public void InformacionEntidad(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt, String codUa, String NroInforme)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_unidades_administrativas;";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_unidades_administrativas AS " +
                    "SELECT * FROM dblink " +
                    "('demodbrnd', 'select * from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f1_unidades_administrativas WHERE ua_co_dane =\''" + codEt + "'\' AND ua_nro_ord LIKE \''" + codUa + "%'\' AND ua_nroinforme = \''" + NroInforme + "'\'') " +
                    "AS DATA( " +
                      "ua_co_dane character varying(5), " +
                      "ua_nro_ord character varying(5), " +
                      "ua_nombre character varying(500), " +
                      "ua_nit integer, " +
                      "ua_dig_ver smallint, " +
                      "ua_ind_tel character varying(5), " +
                      "ua_nro_tel character varying(7), " +
                      "ua_ind_fax character varying(5), " +
                      "ua_nro_fax character varying(7), " +
                      "ua_direc character varying(50), " +
                      "ua_ciudad character varying(5), " +
                      "ua_nom_fun character varying(50), " +
                      "ua_emp_pub smallint, " +
                      "ua_nom_pub numeric, " +
                      "ua_tra_ofi smallint, " +
                      "ua_nom_ofi numeric, " +
                      "ua_sum_emp double precision, " +
                      "ua_sum_nom numeric, " +
                      "ua_nro_pen double precision, " +
                      "ua_nom_pen numeric, " +
                      "ua_nro_ben double precision, " +
                      "ua_nom_ben numeric, " +
                      "ua_nro_rf7 double precision, " +
                      "ua_nom_rf9 numeric, " +
                      "ua_cta2720 numeric, " +
                      "ua_tipo_unidad character varying(1), " +
                      "ua_ley_33 boolean, " +
                      "ua_ley_6 boolean , " +
                      "ua_otr_ley boolean, " +
                      "ua_nro_ley smallint, " +
                      "ua_ano_ley smallint, " +
                      "ua_hay_con boolean , " +
                      "ua_pac_con character varying(50), " +
                      "ua_fec_des timestamp without time zone, " +
                      "ua_fec_has timestamp without time zone, " +
                      "ua_desc_con text, " +
                      "ua_hay_reg boolean , " +
                      "ua_acuerdo boolean, " +
                      "ua_nro_acu smallint, " +
                      "ua_ano_acu smallint, " +
                      "ua_desc_acu text, " +
                      "ua_resoluc boolean, " +
                      "ua_nro_res smallint, " +
                      "ua_ano_res smallint, " +
                      "ua_desc_res text, " +
                      "ua_ordenan boolean, " +
                      "ua_nro_odz smallint, " +
                      "ua_ano_odz smallint, " +
                      "ua_desc_odz text, " +
                      "ua_otr_reg boolean, " +
                      "ua_nro_otr smallint, " +
                      "ua_ano_otr smallint, " +
                      "ua_desc_otr text, " +
                      "ua_fec_sgp timestamp without time zone, " +
                      "ua_fec_cre timestamp without time zone, " +
                      "ua_afi_ant boolean, " +
                      "ua_af_ant1 boolean , " +
                      "ua_af_ant2 boolean, " +
                      "ua_af_ant3 boolean , " +
                      "ua_af_ant4 boolean, " +
                      "ua_af_ant5 boolean , " +
                      "ua_af_ant6 boolean,  " +
                      "ua_af_ant7 boolean , " +
                      "ua_cual_es character varying(50), " +
                      "ua_af_anc1 boolean, " +
                      "ua_af_anc2 boolean , " +
                      "ua_af_anc3 boolean, " +
                      "ua_af_anc4 boolean , " +
                      "ua_af_anc6 boolean, " +
                      "ua_af_anc7 boolean , " +
                      "ua_af_anc5 boolean, " +
                      "ua_cual_anc5 character varying(50), " +
                      "ua_fliq_cjn timestamp without time zone, " +
                      "ua_fliq_fpm timestamp without time zone, " +
                      "ua_fliq_cpm timestamp without time zone, " +
                      "ua_fliq_cpd timestamp without time zone, " +
                      "ua_fliq_otr timestamp without time zone, " +
                      "ua_fec_iss timestamp without time zone, " +
                      "ua_pen_com boolean, " +
                      "ua_cot_iss boolean , " +
                      "ua_fac_s_p text, " +
                      "ua_fac_s_o text, " +
                      "f_creacion timestamp without time zone, " +
                      "usu_cre character varying(20), " +
                      "f_modifica timestamp without time zone, " +
                      "usu_mod character varying(20), " +
                      "version character varying(5), " +
                      "f_corte timestamp without time zone, " +
                      "ua_anoinforme character(4) , " +
                      "ua_mesinforme character(2) , " +
                      "ua_calculo boolean, " +
                      "ua_nroinforme double precision, " +
                      "\"UA_VAL_PAS_FONPET_CORTE\" double precision, " +
                      "\"UA_VAL_APO_FONPET_CORTE\" double precision, " +
                      "\"UA_VAL_APO_PAT_AUTONOMO\" double precision, " +
                      "ea_ubica_doc_fisica text, " +
                      "ea_id double precision, " +
                      "ea_subsector character varying(10), " +
                      "ea_fec_sal_92 timestamp without time zone, " +
                      "ea_cod_cargo double precision " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_unidades_administrativas;";
                    Console.WriteLine("Cantidad de registros unidades administrativas en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f1_unidades_administrativas " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion entidad. {0}", ex.Message));
            }
        }

        public void InformacionActivos(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt, String codUa, String NroInforme)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_activos; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_activos AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select * from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f3_empleados_activos  WHERE ea_co_dane =\''" + codEt + "'\' AND ea_nro_ord LIKE \''" + codUa + "%'\' AND ea_nroinforme = \''" + NroInforme + "'\'')) " +
                        "AS DATA( " +
                        "ea_co_dane character varying(5), " +
                        "ea_nro_ord character varying(5), " +
                        "ea_tip_doc character varying(1), " +
                        "ea_nro_doc numeric, " +
                        "ea_nombre character varying(50), " +
                        "ea_tip_emp character varying(1), " +
                        "ea_sexo character varying(1), " +
                        "ea_est_civ character varying(1), " +
                        "ea_sexo_cony character varying(1), " +
                        "ea_fec_nac timestamp without time zone, " +
                        "ea_f_nac_c timestamp without time zone, " +
                        "ea_cony_a smallint, " +
                        "ea_salario numeric, " +
                        "ea_por_jub double precision, " +
                        "ea_por_sob double precision, " +
                        "ea_edad_ju smallint, " +
                        "ea_tie_jub smallint, " +
                        "ea_tie_sub smallint, " +
                        "ea_regimen smallint, " +
                        "ea_fec_ing timestamp without time zone, " +
                        "ea_dia_lic smallint, " +
                        "ea_sal_iss numeric, " +
                        "ea_fec_iss timestamp without time zone, " +
                        "ea_fec_afp timestamp without time zone, " +
                        "ea_ult_f_iss timestamp without time zone, " +
                        "ea_ult_f_afp timestamp without time zone, " +
                        "ea_fec_otr timestamp without time zone, " +
                        "ea_cod_ent_seg character varying(50), " +
                        "ea_sal_92 numeric, " +
                        "f_creacion timestamp without time zone, " +
                        "usu_cre character varying(200), " +
                        "f_modifica timestamp without time zone, " +
                        "usu_mod character varying(200), " +
                        "version character varying(5), " +
                        "f_corte timestamp without time zone, " +
                        "ea_sector character varying(2), " +
                        "ea_mesadas_junio double precision, " +
                        "ea_mesadas_dic double precision, " +
                        "ea_cargo character varying(100), " +
                        "ea_ano_informe character(4), " +
                        "ea_mesinforme character(2), " +
                        "ea_calculo boolean, " +
                        "ea_cod_ua character(2), " +
                        "ea_nroinforme double precision, " +
                        "verifregistraduria character varying(1), " +
                        "ea_ubica_doc_fisica text, " +
                        "ea_id double precision, " +
                        "ea_subsector character varying(10), " +
                        "ea_fec_sal_92 timestamp without time zone, " +
                        "ea_cod_cargo double precision " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "DELETE FROM temp_activos WHERE UPPER(ea_tip_doc) NOT IN ('C','E','P','T','R','O'); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_activos;";
                    Console.WriteLine("Cantidad de registros activos en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f3_empleados_activos " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion activos. {0}", ex.Message));
            }
        }

        public void InformacionRetirados(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt, String codUa, String NroInforme)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_retirados_completo; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_retirados_completo AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select * from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f9_retirados  WHERE rs_co_dane =\''" + codEt + "'\' AND rs_nro_ord LIKE \''" + codUa + "%'\' AND rs_nroinforme = \''" + NroInforme + "'\'')) " +
                    "AS DATA( " +
                        "rs_co_dane character varying(5), " +
                        "rs_nro_ord character varying(5), " +
                        "rs_tip_doc character varying(1), " +
                        "rs_nro_doc numeric, " +
                        "rs_nombre character varying(90), " +
                        "rs_est_ret boolean, " +
                        "rs_tip_emp character varying(1), " +
                        "rs_sexo character varying(1), " +
                        "rs_est_civ character varying(1), " +
                        "rs_fec_nac timestamp without time zone, " +
                        "rs_f_nac_c timestamp without time zone, " +
                        "rs_cony_a smallint, " +
                        "rs_salario numeric, " +
                        "rs_por_jub double precision, " +
                        "rs_por_sob double precision, " +
                        "rs_edad_ju smallint, " +
                        "rs_tie_jub smallint, " +
                        "rs_tie_sub smallint, " +
                        "rs_regimen smallint, " +
                        "rs_fec_ing timestamp without time zone, " +
                        "rs_fec_ret timestamp without time zone, " +
                        "rs_dia_lic smallint, " +
                        "rs_fec_iss timestamp without time zone, " +
                        "rs_fec_afp timestamp without time zone, " +
                        "rs_ult_f_iss timestamp without time zone, " +
                        "rs_ult_f_afp timestamp without time zone, " +
                        "rs_fec_otr timestamp without time zone, " +
                        "rs_cod_ent_seg character varying(50), " +
                        "rs_sal_92 numeric, " +
                        "f_creacion timestamp without time zone, " +
                        "usu_cre character varying(200), " +
                        "f_modifica timestamp without time zone, " +
                        "usu_mod character varying(200), " +
                        "version character varying(5), " +
                        "f_corte timestamp without time zone, " +
                        "rs_sector character varying(2), " +
                        "rs_cargo character varying(100), " +
                        "rs_tipo_ret character varying(1), " +
                        "rs_anoinforme character(4), " +
                        "rs_mesinforme character(2), " +
                        "rs_calculo boolean, " +
                        "rs_cod_ua character(2), " +
                        "rs_nroinforme double precision, " +
                        "verifregistraduria character varying(1), " +
                        "rs_ubica_doc_fisica text, " +
                        "rs_id double precision, " +
                        "rs_fec_sal_92 timestamp without time zone, " +
                        "rs_cod_cargo double precision, " +
                        "rs_eliminado boolean " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "DELETE FROM temp_retirados_completo WHERE UPPER(rs_tip_doc) NOT IN ('C','E','P','T','R','O');";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_retirados_completo;";
                    Console.WriteLine("Cantidad de registros retirados total en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f9_retirados " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));

                    
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_retirados;";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    /*
                    queryPostgrSql = "CREATE TEMP TABLE temp_retirados AS " +
                    "SELECT t1.*FROM temp_retirados_completo t1 " +
                    "JOIN(SELECT rs_tip_doc, rs_nro_doc, MAX(f_modifica)AS fec_mod " +
                         "FROM temp_retirados_completo " +
                         "WHERE rs_eliminado IS FALSE " +
                         "GROUP BY rs_tip_doc, rs_nro_doc)AS no_depurados " +
                    "ON no_depurados.rs_tip_doc = t1.rs_tip_doc AND no_depurados.rs_nro_doc = t1.rs_nro_doc AND COALESCE(no_depurados.fec_mod, NOW()) = COALESCE(t1.f_modifica, NOW()); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);
                    */

                    queryPostgrSql = "CREATE TEMP TABLE temp_retirados AS " +
                    "SELECT * FROM temp_retirados_completo where rs_eliminado IS FALSE; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_retirados;";
                    Console.WriteLine("Cantidad de registros retirados antes de la depuracion en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f9_retirados " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));

                    queryPostgrSql = "CREATE UNIQUE INDEX temp_retirados_idx ON temp_retirados (rs_tip_doc,rs_nro_doc,rs_nro_ord);";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    /*
                    queryPostgrSql = "INSERT INTO temp_retirados " +
                    "SELECT t1.* " +
                    "FROM temp_retirados_completo t1 " +
                    "JOIN(SELECT CONCAT(rs_tip_doc, rs_nro_doc) AS persona, MAX(f_modifica) " +
                          "FROM temp_retirados_completo " +
                          "WHERE rs_eliminado IS TRUE " +
                          "GROUP BY rs_tip_doc, rs_nro_doc)AS depurados " +
                    "ON CONCAT(t1.rs_tip_doc, t1.rs_nro_doc) = depurados.persona " +
                    "ON CONFLICT(rs_tip_doc, rs_nro_doc) DO NOTHING; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_retirados;";
                    Console.WriteLine("Cantidad de registros retirados en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f9_retirados " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                    */
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion retirados. {0}", ex.Message));
            }
        }

        public void InformacionPensionados(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt, String codUa, String NroInforme)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_pensionados; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_pensionados AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select * from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f5_pensionados WHERE pe_co_dane =\''" + codEt + "'\' AND pe_nro_ord LIKE \''" + codUa + "%'\' AND pe_nroinforme =\''" + NroInforme + "'\'')) " +
                    "AS DATA( " +
                        "pe_co_dane character varying(5), " +
                        "pe_nro_ord character varying(5), " +
                        "pe_tip_doc character varying(1), " +
                        "pe_nro_doc numeric, " +
                        "pe_nombre character varying(50), " +
                        "pe_sexo character varying(1), " +
                        "pe_est_civ character varying(1), " +
                        "pe_fec_nac timestamp without time zone, " +
                        "pe_f_nac_c timestamp without time zone, " +
                        "pe_cony_a smallint, " +
                        "pe_est_pen boolean, " +
                        "pe_directo character varying(1), " +
                        "pe_pension numeric, " +
                        "pe_com_iss boolean, " +
                        "pe_fut_iss boolean, " +
                        "pe_fec_iss timestamp without time zone, " +
                        "pe_por_pen double precision, " +
                        "pe_fec_pen timestamp without time zone, " +
                        "pe_tip_rep character varying(1), " +
                        "pe_nro_rep double precision, " +
                        "f_creacion timestamp without time zone, " +
                        "usu_cre character varying(200), " +
                        "f_modifica timestamp without time zone, " +
                        "usu_mod character varying(200), " +
                        "version character varying(5), " +
                        "f_corte timestamp without time zone, " +
                        "pe_sector character varying(2), " +
                        "pe_mesadas_junio smallint, " +
                        "pe_mesadas_dic smallint, " +
                        "pe_fec_res timestamp without time zone, " +
                        "pe_nro_res integer, " +
                        "pe_anoinforme character(4), " +
                        "pe_mesinforme character(2), " +
                        "pe_calculo boolean, " +
                        "pe_cod_ua character(2), " +
                        "pe_nroinforme double precision, " +
                        "verifregistraduria character varying(1), " +
                        "pe_tipo_pension character varying(1), " +
                        "pe_pension_resolucion double precision, " +
                        "pe_pago_salud_razon character varying(4), " +
                        "pe_pago_salud_porcentaje double precision, " +
                        "pe_pago_salud_monto double precision, " +
                        "pe_pago_salud_desc_razon text, " +
                        "pe_ubica_doc_fisica text, " +
                        "pe_id double precision, " +
                        "pe_eliminado boolean, " +
                        "pe_subsector varchar(15), " +
                        "pe_lug_exp varchar(150),  " +
                        "pe_fec_exp timestamp, " +
                        "idfonpet numeric, " +
                        "nombre_fonpet varchar(255), " +
                        "idpacto numeric, " +
                        "descpacto varchar(255), " +
                        "pe_nrodoc_cony numeric, " +
                        "pe_nom_cony varchar(150) " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "DELETE FROM temp_pensionados WHERE UPPER(pe_tip_doc) NOT IN ('C','E','P','T','R','O');";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    //Se quitan registros eliminados
                    queryPostgrSql = "DELETE FROM temp_pensionados WHERE pe_eliminado IS TRUE;";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_pensionados;";
                    Console.WriteLine("Cantidad de registros pensionados en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f5_pensionados " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion pensionados. {0}", ex.Message));
            }
        }

        public void InformacionSustitutos(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt, String codUa, String NroInforme)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_sustitutos; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_sustitutos AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select * from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f6_beneficiarios  WHERE be_co_dane =\''" + codEt + "'\' AND be_nro_ord LIKE \''" + codUa + "%'\' AND be_nroinforme = \''" + NroInforme + "'\'')) " +
                    "AS DATA( " +
                        "be_co_dane character varying(5), " +
                        "be_nro_ord character varying(5), " +
                        "be_secuenc smallint, " +
                        "be_tip_doc character varying(1), " +
                        "be_nro_doc numeric, " +
                        "be_nombre character varying(50), " +
                        "be_sexo character varying(1), " +
                        "be_fec_nac timestamp without time zone, " +
                        "be_tip_ben character varying(1), " +
                        "be_est_ben boolean, " +
                        "be_directo character varying(1), " +
                        "be_pension numeric, " +
                        "be_por_pen double precision, " +
                        "be_tip_mue character varying(1), " +
                        "be_doc_mue double precision, " +
                        "be_tip_rep character varying(1), " +
                        "be_nro_rep double precision, " +
                        "f_creacion timestamp without time zone, " +
                        "usu_cre character varying(200), " +
                        "f_modifica timestamp without time zone, " +
                        "usu_mod character varying(200), " +
                        "version character varying(5), " +
                        "f_corte timestamp without time zone, " +
                        "be_mesadas_junio double precision, " +
                        "be_mesadas_dic double precision, " +
                        "be_parentesco character varying(1), " +
                        "be_porc_benef double precision, " +
                        "be_anoinforme character(4), " +
                        "be_mesinforme character(2), " +
                        "be_calculo boolean, " +
                        "be_cod_ua character(2), " +
                        "be_nroinforme double precision, " +
                        "verifregistraduria character varying(1), " +
                        "be_pago_salud_razon character varying(4), " +
                        "be_pago_salud_porcentaje double precision, " +
                        "be_pago_salud_monto double precision, " +
                        "be_pago_salud_desc_razon text, " +
                        "be_ubica_doc_fisica text, " +
                        "be_id double precision, " +
                        "be_com_iss boolean, " +
                        "be_hijos_fallecido boolean, " +
                        "be_pension_resolucion double precision, " +
                        "be_eliminado boolean, " +
                        "be_lug_exp varchar(150), " +
                        "be_fec_exp timestamp, " +
                        "idfonpet numeric, " +
                        "nombre_fonpet varchar(255), " +
                        "idpacto numeric, " +
                        "descpacto varchar(255) " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "DELETE FROM temp_sustitutos WHERE UPPER(be_tip_doc) NOT IN ('C','E','P','T','R','O') OR UPPER(be_tip_mue) NOT IN ('C','E','P','T','R','O');";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    //Se quitan registros eliminados
                    queryPostgrSql = "DELETE FROM temp_sustitutos WHERE be_eliminado IS TRUE;";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_sustitutos;";
                    Console.WriteLine("Cantidad de registros sustitutos en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f6_beneficiarios " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion beneficiarios. {0}", ex.Message));
            }
        }

        public void InformacionPensionadosFallecidos(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt, String codUa, String NroInforme)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_pensionadosfallecidos; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_pensionadosfallecidos AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select * from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_pensionados_fallecidos  WHERE pf_co_dane =\''" + codEt + "'\' AND pf_nro_ord LIKE \''" + codUa + "%'\' AND pf_nroinforme = \''" + NroInforme + "'\'')) " +
                    "AS DATA( " +
                      "pf_co_dane character varying(5), " +
                      "pf_nro_ord character varying(5), " +
                      "pf_tip_mue character varying(1), " +
                      "pf_doc_mue double precision, " +
                      "pf_nom_mue character varying(50), " +
                      "pf_fec_pen timestamp without time zone, " +
                      "f_creacion timestamp without time zone, " +
                      "usu_cre character varying(200), " +
                      "f_modifica timestamp without time zone, " +
                      "usu_mod character varying(200), " +
                      "version character varying(5), " +
                      "f_corte timestamp without time zone, " +
                      "pf_sector character varying(2), " +
                      "pf_fec_res timestamp without time zone, " +
                      "pf_nro_res integer, " +
                      "pf_anoinforme character(4), " +
                      "pf_mesinforme character(2), " +
                      "pf_calculo boolean, " +
                      "pf_cod_ua character(2), " +
                      "pf_nroinforme double precision, " +
                      "pf_sexo character varying(1), " +
                      "pf_est_civ character varying(1), " +
                      "pf_fec_nac timestamp without time zone, " +
                      "pf_f_nac_c timestamp without time zone, " +
                      "pf_est_pen boolean, " +
                      "pf_directo character varying(1), " +
                      "pf_ultima_pension numeric, " +
                      "pf_com_iss boolean, " +
                      "pf_fut_iss boolean, " +
                      "pf_fec_iss timestamp without time zone, " +
                      "pf_por_pen double precision, " +
                      "pf_mesadas_junio smallint, " +
                      "pf_mesadas_dic smallint, " +
                      "pf_ano_vr_pension double precision, " +
                      "pf_fec_fallecimiento timestamp without time zone, " +
                      "pf_tip_doc_fallecimiento character varying(2), " +
                      "pf_ano_res_fallecimiento integer, " +
                      "pf_nro_res_fallecimiento integer, " +
                      "pf_tipo_pension character varying(1), " +
                      "pf_pension_resolucion double precision, " +
                      "pf_pago_salud_razon character varying(4), " +
                      "pf_pago_salud_porcentaje double precision, " +
                      "pf_pago_salud_monto double precision, " +
                      "pf_pago_salud_desc_razon text, " +
                      "pf_ubica_doc_fisica text, " +
                      "pf_id double precision, " +
                      "pf_eliminado boolean, " +
                      "pf_subsector varchar(15), " +
                      "pf_lug_exp varchar(150), " +
                      "pf_fec_exp timestamp, " +
                      "idfonpet numeric, " +
                      "nombre_fonpet varchar(255), " +
                      "idpacto numeric, " +
                      "descpacto varchar(255), " +
                      "pf_nrodoc_cony numeric, " +
                      "pf_nom_cony varchar(150) " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "DELETE FROM temp_pensionadosfallecidos WHERE UPPER(pf_tip_mue) NOT IN ('C','E','P','T','R','O');";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    //Se quitan registros eliminados
                    queryPostgrSql = "DELETE FROM temp_pensionadosfallecidos WHERE pf_eliminado IS TRUE;";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_pensionadosfallecidos;";
                    Console.WriteLine("Cantidad de registros pensionados fallecidos en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_pensionados_fallecidos " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion pensionados fallecidos. {0}", ex.Message));
            }
        }

        public void InformacionRegistraduria(String pc_datos_ultenvio, PostgreSql postgreSql)
        {
            try
            {
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_registraduria; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_registraduria AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select * from " + pc_datos_ultenvio + ".pc_cedulas_registraduria.\"tbl_cedulasRegistraduria\"', '')) " +
                    "AS DATA( " +
                        "\"TIPO_DOCUMENTO_PK2\" character varying(1), " +
                        "\"NUMERO_DOCUMENTO_PK1\" numeric, " +
                        "\"PRIMER_NOMBRE\" character varying(55) , " +
                        "\"SEGUNDO_NOMBRE\" character varying(55), " +
                        "\"PRIMER_APELLIDO\" character varying(55) , " +
                        "\"SEGUNDO_APELLIDO\" character varying(55), " +
                        "\"VIGENCIA\" character varying(55), " +
                        "\"TIPO_CANCELACION\" character varying(55), " +
                        "\"FECHA_EXPEDICION\" date, " +
                        "\"FECHA_NACIMIENTO\" date, " +
                        "\"LUGAR_EXPEDICION\" character varying(100), " +
                        "\"CODIGO_VIGENCIA\" character varying(2) " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE INDEX ON temp_registraduria (\"TIPO_DOCUMENTO_PK2\", \"NUMERO_DOCUMENTO_PK1\");";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_registraduria;";
                    Console.WriteLine("Cantidad de registros registraduria en " + pc_datos_ultenvio + ".pc_cedulas_registraduria " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion registraduria. {0}", ex.Message));
            }
        }

        public void InformacionCiudades(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_ciudades; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_ciudades AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select ci_co_dane, ci_nombre from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_ciudades WHERE ci_co_dane =\''" + codEt + "'\'')) " +
                    "AS DATA( " +
                        "ci_co_dane character varying(5), " +
                        "ci_nombre character varying(50) " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_ciudades;";
                    Console.WriteLine("Cantidad de registros ciudades en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_ciudades " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion ciudades. {0}", ex.Message));
            }
        }

        public void InformacionDepartamentos(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_departamentos; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_departamentos AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select dp_co_dept, dp_nombre from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_departamentos')) " +
                    "AS DATA( " +
                        "dp_co_dept character varying(2), " +
                        "dp_nombre character varying(50) " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_departamentos;";
                    Console.WriteLine("Cantidad de registros departamentos en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_departamentos " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion departamentos. {0}", ex.Message));
            }
        }

        public void InformacionHistoriaLaboralActivos(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt, String codUa, String NroInforme)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_historialaboral_activos; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_historialaboral_activos AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select * from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f4_historia_laboral_empleados_activos  WHERE ha_co_dane =\''" + codEt + "'\' AND ha_nro_ord LIKE \''" + codUa + "%'\' AND ha_nroinforme = \''" + NroInforme + "'\'')) " +
                    "AS DATA( " +
                      "ha_co_dane character varying(5), " +
                      "ha_nro_ord character varying(5), " +
                      "ha_tip_doc character varying(1), " +
                      "ha_nro_doc numeric, " +
                      "ha_nro_his smallint, " +
                      "ha_nit integer, " +
                      "ha_dig_ver smallint, " +
                      "ha_empresa character varying(250), " +
                      "ha_ciudad character varying(5), " +
                      "ha_misma_ua boolean, " +
                      "ha_sec_pub boolean, " +
                      "ha_otr_pub boolean, " +
                      "ha_sec_pri boolean, " +
                      "ha_fec_ing timestamp without time zone, " +
                      "ha_fec_ret timestamp without time zone, " +
                      "ha_dia_lic smallint, " +
                      "ha_labor_a smallint, " +
                      "ha_labor_m smallint, " +
                      "ha_cot_sem smallint, " +
                      "ha_cot_ent smallint, " +
                      "f_creacion timestamp without time zone, " +
                      "usu_cre character varying(200), " +
                      "f_modifica timestamp without time zone, " +
                      "usu_mod character varying(200), " +
                      "version character varying(5), " +
                      "f_corte timestamp without time zone, " +
                      "ha_anoinforme character(4), " +
                      "ha_mesinforme character(2), " +
                      "ha_calculo boolean, " +
                      "ha_nroinforme double precision " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "DELETE FROM temp_historialaboral_activos WHERE UPPER(ha_tip_doc) NOT IN ('C','E','P','T','R','O');";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_historialaboral_activos;";
                    Console.WriteLine("Cantidad de registros historia laboral activos en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f4_historia_laboral_empleados_activos " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion historia laboral activos. {0}", ex.Message));
            }
        }

        public void InformacionHistoriaLaboralRetirados(String pc_datos_ultenvio, PostgreSql postgreSql, String codEt, String codUa, String NroInforme)
        {
            try
            {
                String codigoDepto = codEt.Substring(0, 2);
                String queryPostgrSql = string.Empty;

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_historialaboral_retirados; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_historialaboral_retirados AS " +
                    "SELECT* FROM dblink " +
                    "('demodbrnd', CONCAT('select * from " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f9a_historia_laboral_retirados WHERE hs_co_dane =\''" + codEt + "'\' AND hs_nro_ord LIKE \''" + codUa + "%'\' AND hs_nroinforme = \''" + NroInforme + "'\'')) " +
                    "AS DATA( " +
                      "hs_co_dane character varying(5), " +
                      "hs_nro_ord character varying(5), " +
                      "hs_tip_doc character varying(1), " +
                      "hs_nro_doc numeric, " +
                      "hs_nro_his smallint, " +
                      "hs_nit integer, " +
                      "hs_dig_ver smallint, " +
                      "hs_empresa character varying(250), " +
                      "hs_ciudad character varying(5), " +
                      "hs_misma_ua boolean, " +
                      "hs_sec_pub boolean, " +
                      "hs_otr_pub boolean, " +
                      "hs_sec_pri boolean, " +
                      "hs_fec_ing timestamp without time zone, " +
                      "hs_fec_ret timestamp without time zone, " +
                      "hs_dia_lic smallint, " +
                      "hs_labor_a smallint, " +
                      "hs_labor_m smallint, " +
                      "hs_cot_sem smallint, " +
                      "hs_cot_ent smallint, " +
                      "f_creacion timestamp without time zone, " +
                      "usu_cre character varying(200), " +
                      "f_modifica timestamp without time zone, " +
                      "usu_mod character varying(200), " +
                      "version character varying(5), " +
                      "f_corte timestamp without time zone, " +
                      "hs_anoinforme character(4), " +
                      "hs_mesinforme character(2), " +
                      "hs_calculo boolean, " +
                      "hs_nroinforme double precision " +
                    "); ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "DELETE FROM temp_historialaboral_retirados WHERE UPPER(hs_tip_doc) NOT IN ('C','E','P','T','R','O');";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_historialaboral_retirados;";
                    Console.WriteLine("Cantidad de registros historia laboral retirados en " + pc_datos_ultenvio + ".pc_datos_" + codigoDepto + ".tbl_f9a_historia_laboral_retirados " + "(PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error cargando informacion historia laboral retirados. {0}", ex.Message));
            }
        }
    }
}
