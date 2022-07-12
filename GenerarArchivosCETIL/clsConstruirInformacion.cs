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
    public class clsConstruirInformacion
    {
		String fechacreacion_central = string.Empty;
		String fechaiss_central = string.Empty;
		String fechasgp_central = string.Empty;
		String fechaactualizacion_central = string.Empty;
		String codEt = string.Empty;
		String codUa = string.Empty;
		String anio_corte = string.Empty;
		String fechaCorte = string.Empty;
		String cantidad = string.Empty;
		String cantidad_registros2 = string.Empty;
		Int64 cantidad_lineas = 0; 
		String nitiss = "860013816";
		String ultima_fecha_rnec = "20200505";		//Fecha del último cruce de registraduría con CETIL
		String fechadef_depuracion = "20191025";	// Fecha por defecto para las depuraciones: Temporal

		public String nit = string.Empty;

		public clsConstruirInformacion(String codEt, String codUa, String fechaCorte) 
		{
			this.codEt = codEt;
			this.codUa = codUa;
			this.anio_corte = StringToDatetime(fechaCorte).Year.ToString();
			this.fechaCorte = fechaCorte;

			Console.WriteLine(string.Format("***Generando informacion CodEt: {0}, CodUa: {1}, fechaCorte: {2}***", codEt,codUa, fechaCorte));
		}

        public void InformacionR1(String pc_datos_ultenvio, PostgreSql postgreSql)
        {
            try
            {
                String queryPostgrSql = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_reg1; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "CREATE TEMP TABLE temp_reg1 AS " +
                    "SELECT 1 AS tipo_registro, " +
                    "CASE WHEN LENGTH(hv.nitua) > 9 THEN SUBSTRING(hv.nitua,1,9) ELSE hv.nitua END AS nit_entidad, " +
					"CASE WHEN dian.\"RAZON_SOCIAL\" IS NOT NULL THEN concat(TRIM(mig.nombre), '-', dian.\"RAZON_SOCIAL\") ELSE TRIM(mig.nombre) END AS nombre_entidad, " +
                    //"TRIM(mig.nombre) AS nombre_entidad, " +
                    "mig.seccional AS seccional, " +
                    "CONCAT('01', mig.seccional) AS codigoPasivocol, " +
                    "CASE WHEN mig.seccional = '000' AND LENGTH(hv.codua) = 2 AND hv.codua != '01' THEN 'D' ELSE 'C' END AS tipo_entidad, " +
                    "CASE WHEN mig.seccional = '000' AND(tipoorganizacion IS NULL OR tipoorganizacion <> 'E.S.E.') THEN 'CEN' WHEN(mig.seccional BETWEEN '800' AND '899' OR hv.tipoorganizacion = 'E.S.E.') THEN 'HSP' WHEN mig.seccional IN('938','939','942','943','951') THEN 'SPB' ELSE 'OTR' END AS tipo_empleador, " +
                    "CASE WHEN hv.feccreaua IS NOT NULL THEN SUBSTRING(replace(replace(replace(hv.feccreaua, ' ', ''), ':', ''), '-', ''), 1, 8) ELSE NULL END AS fecha_creacion, " +
                    "CASE WHEN hv.feciss IS NOT NULL THEN replace(replace(hv.feciss, ' ', ''), '-', '') ELSE NULL END AS fecha_afiliacion_iss, " +
                    "CASE WHEN hv.fecsgp IS NOT NULL THEN replace(replace(hv.fecsgp, ' ', ''), '-', '') ELSE NULL END AS fecha_vigencia_sgp, " +
                    "hv.codet AS codigo_dane, " +
                    "UPPER(TRIM(temp.ua_nom_fun)) AS nombre_representante_legal, " +
                    "CASE WHEN SUBSTRING(hv.codet,3, 3) = '000' AND hv.codua = '01' THEN 'GOB' " +
                         "WHEN SUBSTRING(hv.codet,3, 3) <> '000' AND hv.codua = '01' THEN 'ALC' " +
                         "ELSE 'GTE' END AS cargo_representante_legal, " +
                    "REPLACE(TRIM(hv.emailua), ',', '.') AS email_representante_legal,  " +
                    "UPPER(COALESCE(TRIM(hv.nomcon), 'No registra')) AS nombre_coordinador_pasivocol, " +
                    "LPAD(CAST('20' AS CHAR(3)), 3, '0') AS cargo_coordinador_pasivocol, " +
                      "regexp_replace(COALESCE(substring(temp.ua_nro_tel from 0 for 10),'0000000000') , '[^0-9]', '', 'g') AS telefono_coordinador_pasivocol, " +
                      "COALESCE(REPLACE(TRIM(hv.emailcon), ',', '.'), 'No registra') AS email_coordinador_pasivocol, " +
                    "COALESCE(temp.ua_direc, 'No registra') AS direccion_entidad, " +
                    "ciudad.ci_nombre AS nombre_ciudad, " +
                    "COALESCE(substring(temp.ua_nro_tel from 0 for 10),'No registra') AS telefonos, " +
                    "REPLACE(TRIM(hv.webua), ',', '.') AS pagina_web, " +
                    "CONCAT(TRIM(hv.tipoactoadministrativo), '-', TRIM(hv.nroactoadministrativo)) AS tipo_numero_acto_administrativo,  " +
                     "CASE WHEN hv.liqua = 'S' AND hv.estadoua <> 'AC' THEN 'LIQ' ELSE 'ACT' END AS estado_entidad, " +
                    "CASE WHEN hv.liqua = 'S' AND hv.estadoua <> 'AC' THEN replace(hv.fecliqua, '-', '') ELSE '' END AS fecha_liquidacion_entidad, " +
                    "CASE WHEN hv.liqua = 'S' AND hv.estadoua <> 'AC' THEN TRIM(hv.nroactoliquidacion) ELSE '' END AS numero_liquidacion_entidad, " +
                    "hv.codcontua AS codigo_contaduria, " +
                    "CASE WHEN hv.fecmodreg <> '' AND hv.fecmodreg IS NOT NULL THEN substring(replace(replace(replace(hv.fecmodreg, ' ', ''), ':', ''), '-', '') from 1 for 12) ELSE NULL END AS fecha_actualizacion, " +
                    "'A' AS estado_revision, hv.codua " +
                    "FROM temp_unidades_administrativas temp " +
                    "LEFT JOIN temp_ciudades ciudad ON temp.ua_co_dane = ciudad.ci_co_dane " +
                    "JOIN pruebas.tblhojavidaunidadesadministrativas hv ON temp.ua_co_dane = hv.codet AND temp.ua_nro_ord = hv.codua " +
                    "JOIN pruebas.entidades_migracion_inter mig ON temp.ua_co_dane = mig.\"codigoDane\" AND temp.ua_nro_ord = mig.\"unidadAdministrativa\" " +
					"LEFT OUTER JOIN public.nits_dian dian ON dian.\"NIT\" = hv.nitua; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

					//Se actualiza codigo seccional a 000 cuando el nit es unico
					queryPostgrSql = "UPDATE temp_reg1 AS A "+
					"SET seccional = '000' "+
					"FROM(SELECT nit_entidad, SUM(Cantidad) AS cantidad FROM( "+
					"			SELECT *, " +
					"				ROW_NUMBER() OVER( "+ 
					"					PARTITION BY nit_entidad "+
					"					ORDER BY "+
					"					nit_entidad "+
					"				) Cantidad "+
					"			FROM temp_reg1 "+
					"			) tmp "+
					"			group by(nit_entidad) "+
					") B "+
					"WHERE A.nit_entidad = B.nit_entidad AND B.cantidad = 1; ";
					dataTable = postgreSql.ConsultarDatos(queryPostgrSql);

					//Guarda la fecha de creacion /sgp/ iss de la entidad central para asignarle a las subdivisiones en caso de que no la tengan definida
					queryPostgrSql = "SELECT SUBSTRING(replace(replace(replace(hv.feccreaua, ' ', ''), ':', ''), '-', ''),1,8) AS fecha_creacion, " +
                    "replace(replace(hv.feciss, ' ', ''), '-', '') AS fecha_afiliacion_iss, " +
                    "replace(replace(hv.fecsgp, ' ', ''), '-', '') AS fecha_vigencia_sgp, " +
                    "substring(replace(replace(replace(hv.fecmodreg, ' ', ''), ':', ''), '-', '') from 1 for 12) AS fecha_actualizacion, " +
                    "CASE WHEN LENGTH(hv.nitua) > 9 THEN SUBSTRING(hv.nitua,1,9) ELSE hv.nitua END AS nit_entidad " +
                    "FROM pruebas.tblhojavidaunidadesadministrativas hv WHERE hv.codet = '"+ codEt+"' AND hv.codua ='"+ codUa+"'; ";
                    dataTable = postgreSql.ConsultarDatos(queryPostgrSql);

					if(dataTable.Rows.Count == 1)
                    {
						fechacreacion_central = dataTable.Rows[0][0].ToString();
						fechaiss_central = dataTable.Rows[0][1].ToString();
						fechasgp_central = dataTable.Rows[0][2].ToString();
						fechaactualizacion_central = dataTable.Rows[0][3].ToString();
						nit = dataTable.Rows[0][4].ToString();
					}

					queryPostgrSql = "SELECT COUNT(*) FROM temp_reg1;";
					cantidad = postgreSql.ConsultarDato(queryPostgrSql);
					cantidad_lineas = cantidad_lineas + Int64.Parse(cantidad);

					Console.WriteLine("Cantidad de registros persona R1 (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + cantidad);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo R1. {0}", ex.Message));
            }
        }

        public void InformacionR2(String pc_datos_ultenvio, PostgreSql postgreSql)
        {
            try
            {
                String queryPostgrSql = string.Empty;
                DataTable dataTable = new DataTable();

                try
                {
                    queryPostgrSql = "DROP TABLE IF EXISTS temp_reg2; ";
                    postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE TEMP TABLE temp_reg2 AS " +
					"SELECT * FROM( " +
					"SELECT *, ROW_NUMBER () OVER (PARTITION BY tipo_documento,documento ORDER BY fecha_ultima_modificacion DESC) AS Cantidad " +
					"FROM (" +
						"SELECT 2 AS tipo_registro, ea_tip_doc AS tipo_documento, " + //ACTIVOS
						"ea_nro_doc AS documento, " +
						"CASE WHEN split_part(TRIM(REPLACE(ea_nombre,'  ',' ')),' ',3) <> '' " +
							"THEN split_part(TRIM(REPLACE(ea_nombre,'  ',' ')),' ',3) " +
							"WHEN split_part(TRIM(REPLACE(ea_nombre,'  ',' ')),' ',2) <> '' " +
							"THEN split_part(TRIM(REPLACE(ea_nombre,'  ',' ')),' ',2) " +
							"ELSE '' END AS primer_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(ea_nombre, '  ', ' ')), ' ', 4) <> '' " +
							"THEN split_part(TRIM(REPLACE(ea_nombre,'  ',' ')),' ',4) " +
							"ELSE '' END AS segundo_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(ea_nombre, '  ', ' ')), ' ', 1) <> '' " +
							"THEN split_part(TRIM(REPLACE(ea_nombre,'  ',' ')),' ',1) " +
							"ELSE '' END AS primer_apellido, " +
						"CASE WHEN split_part(TRIM(REPLACE(ea_nombre, '  ', ' ')), ' ', 2) <> '' " +
							"THEN split_part(TRIM(REPLACE(ea_nombre,'  ',' ')),' ',2) " +
							"ELSE '' END AS segundo_apellido, " +
						"CASE WHEN \"CODIGO_VIGENCIA\" LIKE '0' THEN '00' " +
							 "WHEN \"CODIGO_VIGENCIA\" LIKE '1' THEN '01' " +
							 "ELSE \"CODIGO_VIGENCIA\" END AS vigencia_documento, " +
						ultima_fecha_rnec + " AS fecha_actualizacion_rnec,  " +
						"'' AS fecha_fallecimiento_minsalud, " +
						"CASE WHEN ea_fec_iss IS NULL THEN to_char(CAST(ea_fec_afp as date),'YYYYMMDD') " +
							 "WHEN ea_fec_afp IS NULL THEN to_char(cast(ea_fec_iss as date),'YYYYMMDD') " +
							 "WHEN ea_fec_afp<ea_fec_iss THEN to_char(cast(ea_fec_afp as date),'YYYYMMDD') " +
							 "ELSE to_char(cast(ea_fec_iss as date),'YYYYMMDD') END AS fecha_primera_afiliacion_sgp, " +
						"CASE WHEN ea_fec_iss IS NULL THEN 'RAIS' " +
							 "WHEN ea_fec_afp IS NULL THEN 'COLP' " +
							 "WHEN ea_fec_afp<ea_fec_iss THEN 'RAIS' " +
							 "ELSE 'COLP' END AS codigo_regimen_primera_afiliacion, " +
						"'' AS tasa_bono, " +
						"'' AS tipo_depuracion, " +
						"'' AS numero_resolucion_fallecimiento, " +
						"'' AS anio_resolucion_fallecimiento, " +
						"'' AS documento_homonimo, " +
						"'' AS descripcion_depuracion, " +
						"'' AS valor_bono, " +
						"'' AS fecha_pago_bono,  " +
						"'' AS numero_acto_admin_pago_bono, " +
						"'' AS anio_acto_admin_pago_bono, " +
						"'' AS numero_contrato_ops, " +
						"'' AS numero_resolucion_nombramiento_docente, " +
						"'' AS anio_resolucion_nombramiento_docente, " +
						"'' AS fecha_depuracion,  " +
						"'NO' AS depurado_historia_laboral, " +
						"'NO' AS depurado_pensionado, " +
						"'NO' AS depurado_sustitutos, " +
						"'' AS numero_fallo_judicial_demanda, " +
						"'' AS anio_fallo_judicial,  " +
						"'' AS valor_conmitado_pagado_cuota_parte_futura, " +
						"'' AS nit_entidad_receptora_conmut_pago_cuota_parte_futura,  " +
						"'' AS acto_administrativo_depuracion,  " +
						"'' AS anio_administrativo_depuracion,  " +
						"'' AS archivo,  " +
						"'' AS numero_documento_usuario_registra, " +
						"CASE WHEN to_char(f_modifica ,'YYYYMMDDHH24MI')<> '' " +
							 "THEN to_char(f_modifica,'YYYYMMDDHH24MI') " +
							 "ELSE to_char(now(),'YYYYMMDDHH24MI') END AS fecha_ultima_modificacion, " +
						"CASE WHEN temp_salariobase.\"SALARIOBASEBONO\" IS NULL THEN ea_sal_92 ELSE temp_salariobase.\"SALARIOBASEBONO\" END AS salario_base_bono_pensional, " +
						"CASE WHEN temp_salariobase.\"FECHA_SALARIOBASEBONO\" IS NULL THEN " +
								"CASE WHEN ea_fec_sal_92 IS NOT NULL THEN to_char(cast(ea_fec_sal_92 as date), 'YYYYMMDD') ELSE '' END " +
							  "ELSE to_char(cast(temp_salariobase.\"FECHA_SALARIOBASEBONO\" as date), 'YYYYMMDD') END AS fecha_salario_base_bono_pensional, " +
						"'' AS salario_base_bono_pensional_calculado, " +
						"'ACTIVO' AS grupoA, " +
						"ea_co_dane AS codet, " +
						"ea_nro_ord AS codua, " +
						"ea_id AS regid, " +
						"ea_sexo AS genero, " +
						"to_char(ea_fec_nac, 'YYYYMMDD') AS fecha_nacimiento " +
						"FROM temp_activos " +
						"LEFT JOIN temp_registraduria ON ea_tip_doc = temp_registraduria.\"TIPO_DOCUMENTO_PK2\" AND ea_nro_doc = temp_registraduria.\"NUMERO_DOCUMENTO_PK1\" " +
						"LEFT JOIN temp_salariobase ON ea_tip_doc = temp_salariobase.\"TIPO_DOCUMENTO_PK2\" AND ea_nro_doc = temp_salariobase.\"NUMERO_DOCUMENTO_PK1\" " +
						"UNION " +
						"SELECT 2 AS tipo_registro, rs_tip_doc AS tipo_documento, " + //RETIRADOS
						"rs_nro_doc AS documento,  " +
						"CASE WHEN split_part(TRIM(REPLACE(rs_nombre,'  ',' ')),' ',3) <> '' " +
							"THEN split_part(TRIM(REPLACE(rs_nombre,'  ',' ')),' ',3) " +
							"WHEN split_part(TRIM(REPLACE(rs_nombre,'  ',' ')),' ',2) <> '' " +
							"THEN split_part(TRIM(REPLACE(rs_nombre,'  ',' ')),' ',2) " +
							"ELSE '' END AS primer_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(rs_nombre, '  ', ' ')), ' ', 4) <> '' " +
							"THEN split_part(TRIM(REPLACE(rs_nombre,'  ',' ')),' ',4) " +
							"ELSE '' END AS segundo_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(rs_nombre, '  ', ' ')), ' ', 1) <> '' " +
							"THEN split_part(TRIM(REPLACE(rs_nombre,'  ',' ')),' ',1) " +
							"ELSE '' END AS primer_apellido, " +
						"CASE WHEN split_part(TRIM(REPLACE(rs_nombre, '  ', ' ')), ' ', 2) <> '' " +
							"THEN split_part(TRIM(REPLACE(rs_nombre,'  ',' ')),' ',2) " +
							"ELSE '' END AS segundo_apellido, " +
						"CASE WHEN \"CODIGO_VIGENCIA\" LIKE '0' THEN '00' " +
							 "WHEN \"CODIGO_VIGENCIA\" LIKE '1' THEN '01' " +
							 "ELSE \"CODIGO_VIGENCIA\" END AS vigencia_documento, " +
						ultima_fecha_rnec + " AS fecha_actualizacion_rnec, " +
						"'' AS fecha_fallecimiento_minsalud, " +
						"CASE WHEN rs_fec_iss IS NULL THEN to_char(CAST(rs_fec_afp as date),'YYYYMMDD') " +
							 "WHEN rs_fec_afp IS NULL THEN to_char(cast(rs_fec_iss as date),'YYYYMMDD') " +
							 "WHEN rs_fec_afp<rs_fec_iss THEN to_char(cast(rs_fec_afp as date),'YYYYMMDD') " +
							 "ELSE to_char(cast(rs_fec_iss as date),'YYYYMMDD') END AS fecha_primera_afiliacion_sgp, " +
						"CASE WHEN rs_fec_iss IS NULL THEN 'RAIS' " +
							 "WHEN rs_fec_afp IS NULL THEN 'COLP' " +
							 "WHEN rs_fec_afp<rs_fec_iss THEN 'RAIS' " +
							 "ELSE 'COLP' END AS codigo_regimen_primera_afiliacion, " +
						"'' AS tasa_bono, " +
						"CASE WHEN rs_eliminado IS TRUE THEN 'DEM' ELSE '' END AS tipo_depuracion, " + // --EL TIPO DE DEPURACIÓN SE DEBE ACTUALIZAR CUANDO ESTA INFORMACIÓN SE MONTE A LAS BD DE POSTGRES. LUIS YA SABE DE ESTE PROCESO. POR DEFECTO SE ENVÍA DEM
						"'' AS numero_resolucion_fallecimiento, " +
						"'' AS anio_resolucion_fallecimiento, " +
						"'' AS documento_homonimo, " +
						"'' AS descripcion_depuracion, " +
						"'' AS valor_bono, " +
						"'' AS fecha_pago_bono, " +
						"'' AS numero_acto_admin_pago_bono, " +
						"'' AS anio_acto_admin_pago_bono, " +
						"'' AS numero_contrato_ops, " +
						"'' AS numero_resolucion_nombramiento_docente, " +
						"'' AS anio_resolucion_nombramiento_docente, " +
						"CASE WHEN rs_eliminado IS TRUE THEN to_char( '" + fechadef_depuracion + "':: date ,'YYYYMMDDHH24MI') ELSE '' END AS fecha_depuracion, " + // --LA FECHA DE DEPURACIÓN SE DEBE ACTUALIZAR CUANDO ESTA INFORMACIÓN SE MONTE A LAS BD DE POSTGRES. LUIS YA SABE DE ESTE PROCESO. POR DEFECTO SE ENVÍA fechadef_depuracion "+
						"CASE WHEN rs_eliminado IS TRUE THEN 'SI' ELSE 'NO' END AS depurado_historia_laboral, " +
						"'NO' AS depurado_pensionado, " +
						"'NO' AS depurado_sustitutos, " +
						"'' AS numero_fallo_judicial_demanda,  " +
						"'' AS anio_fallo_judicial,  " +
						"'' AS valor_conmitado_pagado_cuota_parte_futura,  " +
						"'' AS nit_entidad_receptora_conmut_pago_cuota_parte_futura,  " +
						"'' AS acto_administrativo_depuracion, " +
						"'' AS anio_administrativo_depuracion, " +
						"'' AS archivo, " +
						"'' AS numero_documento_usuario_registra, " +
						"CASE WHEN to_char(f_modifica,'YYYYMMDDHH24MI')<> '' " +
							 "THEN to_char(f_modifica,'YYYYMMDDHH24MI') " +
							 "ELSE to_char(now(),'YYYYMMDDHH24MI') END AS fecha_ultima_modificacion, " +
						"CASE WHEN temp_salariobase.\"SALARIOBASEBONO\" IS NULL THEN rs_sal_92 ELSE temp_salariobase.\"SALARIOBASEBONO\" END AS salario_base_bono_pensional, " +
						"CASE WHEN temp_salariobase.\"FECHA_SALARIOBASEBONO\" IS NULL THEN " +
							  "CASE WHEN rs_fec_sal_92 IS NOT NULL THEN to_char(cast(rs_fec_sal_92 as date), 'YYYYMMDD') ELSE '' END " +
							  "ELSE to_char(cast(temp_salariobase.\"FECHA_SALARIOBASEBONO\" as date), 'YYYYMMDD') END AS fecha_salario_base_bono_pensional, " +
						"'' AS salario_base_bono_pensional_calculado, " +
						"'RETIRADO' AS grupoA, " +
						"rs_co_dane AS codet, " +
						"rs_nro_ord AS codua, " +
						"rs_id AS regid, " +
						"rs_sexo AS genero, " +
						"to_char(rs_fec_nac, 'YYYYMMDD') AS fecha_nacimiento " +
						"FROM temp_retirados " +
						"LEFT JOIN temp_registraduria ON rs_tip_doc = temp_registraduria.\"TIPO_DOCUMENTO_PK2\" AND rs_nro_doc = temp_registraduria.\"NUMERO_DOCUMENTO_PK1\" " +
						"LEFT JOIN temp_salariobase ON rs_tip_doc = temp_salariobase.\"TIPO_DOCUMENTO_PK2\" AND rs_nro_doc = temp_salariobase.\"NUMERO_DOCUMENTO_PK1\" " +
						"UNION " +
						"SELECT 2 AS tipo_registro, pe_tip_doc AS tipo_documento, " + //PENSIONADOS
						"pe_nro_doc AS documento, " +
						"CASE WHEN split_part(TRIM(REPLACE(pe_nombre,'  ',' ')),' ',3) <> '' " +
							"THEN split_part(TRIM(REPLACE(pe_nombre,'  ',' ')),' ',3) " +
							"WHEN split_part(TRIM(REPLACE(pe_nombre,'  ',' ')),' ',2) <> '' " +
							"THEN split_part(TRIM(REPLACE(pe_nombre,'  ',' ')),' ',2) " +
							"ELSE '' END AS primer_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(pe_nombre, '  ', ' ')), ' ', 4) <> '' " +
							"THEN split_part(TRIM(REPLACE(pe_nombre,'  ',' ')),' ',4) " +
							"ELSE '' END AS segundo_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(pe_nombre, '  ', ' ')), ' ', 1) <> '' " +
							"THEN split_part(TRIM(REPLACE(pe_nombre,'  ',' ')),' ',1) " +
							"ELSE '' END AS primer_apellido, " +
						"CASE WHEN split_part(TRIM(REPLACE(pe_nombre, '  ', ' ')), ' ', 2) <> '' " +
							"THEN split_part(TRIM(REPLACE(pe_nombre,'  ',' ')),' ',2) " +
							"ELSE '' END AS segundo_apellido, " +
						"CASE WHEN \"CODIGO_VIGENCIA\" LIKE '0' THEN '00' " +
							 "WHEN \"CODIGO_VIGENCIA\" LIKE '1' THEN '01' " +
							 "ELSE \"CODIGO_VIGENCIA\" END AS vigencia_documento, " +
						ultima_fecha_rnec + " AS fecha_actualizacion_rnec, " +
						"'' AS fecha_fallecimiento_minsalud, " +
						"'' AS fecha_primera_afiliacion_sgp, " +
						"CASE WHEN pe_fec_iss IS NOT NULL THEN 'COLP' ELSE 'OTRO' END AS codigo_regimen_primera_afiliacion, " +
						"'' AS tasa_bono, " +
						"CASE WHEN pe_eliminado IS TRUE THEN 'DEM' ELSE '' END AS tipo_depuracion, " +
						"'' AS numero_resolucion_fallecimiento, " +
						"'' AS anio_resolucion_fallecimiento, " +
						"'' AS documento_homonimo, " +
						"'' AS descripcion_depuracion, " +
						"'' AS valor_bono, " +
						"'' AS fecha_pago_bono, " +
						"'' AS numero_acto_admin_pago_bono, " +
						"'' AS anio_acto_admin_pago_bono, " +
						"'' AS numero_contrato_ops, " +
						"'' AS numero_resolucion_nombramiento_docente, " +
						"'' AS anio_resolucion_nombramiento_docente, " +
						"CASE WHEN pe_eliminado IS TRUE THEN to_char( '" + fechadef_depuracion + "':: date , 'YYYYMMDDHH24MI') ELSE '' END AS fecha_depuracion,  " +//LA FECHA DE DEPURACIÓN SE DEBE ACTUALIZAR CUANDO ESTA INFORMACIÓN SE MONTE A LAS BD DE POSTGRES. LUIS YA SABE DE ESTE PROCESO. POR DEFECTO SE ENVÍA fechadef_depuracion
						"'NO' AS depurado_historia_laboral, " +
						"CASE WHEN pe_eliminado IS TRUE THEN 'SI' ELSE 'NO' END AS depurado_pensionado, " +
						"'NO' AS depurado_sustitutos, " +
						"'' AS numero_fallo_judicial_demanda, " +
						"'' AS anio_fallo_judicial, " +
						"'' AS valor_conmitado_pagado_cuota_parte_futura, " +
						"'' AS nit_entidad_receptora_conmut_pago_cuota_parte_futura, " +
						"'' AS acto_administrativo_depuracion, " +
						"'' AS anio_administrativo_depuracion, " +
						"'' AS archivo, " +
						"'' AS numero_documento_usuario_registra, " +
						"CASE WHEN to_char(f_modifica ,'YYYYMMDDHH24MI')<> '' " +
							 "THEN to_char(f_modifica,'YYYYMMDDHH24MI') " +
							 "ELSE to_char(now(),'YYYYMMDDHH24MI') END AS fecha_ultima_modificacion, " +
						"null AS salario_base_bono_pensional, " +
						"null AS fecha_salario_base_bono_pensional, " +
						"'' AS salario_base_bono_pensional_calculado, " +
						"'PENSIONADO' AS grupoA, " +
						"pe_co_dane AS codet, " +
						"pe_nro_ord AS codua, " +
						"pe_id AS regid, " +
						"pe_sexo AS genero, " +
						"to_char(pe_fec_nac, 'YYYYMMDD') AS fecha_nacimiento " +
						"FROM temp_pensionados " +
						"LEFT JOIN temp_registraduria ON pe_tip_doc = \"TIPO_DOCUMENTO_PK2\" AND pe_nro_doc = \"NUMERO_DOCUMENTO_PK1\" " +
						"UNION " + //SUSTITUTOS
						"SELECT 2 AS tipo_registro, be_tip_doc AS tipo_documento, " +
						"be_nro_doc AS documento, " +
						"CASE WHEN split_part(TRIM(REPLACE(be_nombre,'  ',' ')),' ',3) <> '' " +
							"THEN split_part(TRIM(REPLACE(be_nombre,'  ',' ')),' ',3) " +
							"WHEN split_part(TRIM(REPLACE(be_nombre,'  ',' ')),' ',2) <> '' " +
							"THEN split_part(TRIM(REPLACE(be_nombre,'  ',' ')),' ',2) " +
							"ELSE '' END AS primer_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(be_nombre, '  ', ' ')), ' ', 4) <> '' " +
							"THEN split_part(TRIM(REPLACE(be_nombre,'  ',' ')),' ',4) " +
							"ELSE '' END AS segundo_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(be_nombre, '  ', ' ')), ' ', 1) <> '' " +
							"THEN split_part(TRIM(REPLACE(be_nombre,'  ',' ')),' ',1) " +
							"ELSE '' END AS primer_apellido, " +
						"CASE WHEN split_part(TRIM(REPLACE(be_nombre, '  ', ' ')), ' ', 2) <> '' " +
							"THEN split_part(TRIM(REPLACE(be_nombre,'  ',' ')),' ',2) " +
							"ELSE '' END AS segundo_apellido, " +
						"CASE WHEN \"CODIGO_VIGENCIA\" LIKE '0' THEN '00' " +
							 "WHEN \"CODIGO_VIGENCIA\" LIKE '1' THEN '01' " +
							 "ELSE \"CODIGO_VIGENCIA\" END AS vigencia_documento, " +
						ultima_fecha_rnec + " AS fecha_actualizacion_rnec, " +
						"'' AS fecha_fallecimiento_minsalud, " +
						"null AS fecha_primera_afiliacion_sgp, " +
						"'OTRO' AS codigo_regimen_primera_afiliacion, " +
						"'' AS tasa_bono, " +
						"CASE WHEN be_eliminado IS TRUE THEN 'DEM' ELSE '' END AS tipo_depuracion, " +
						"'' AS numero_resolucion_fallecimiento, " +
						"'' AS anio_resolucion_fallecimiento, " +
						"'' AS documento_homonimo, " +
						"'' AS descripcion_depuracion, " +
						"'' AS valor_bono, " +
						"'' AS fecha_pago_bono, " +
						"'' AS numero_acto_admin_pago_bono, " +
						"'' AS anio_acto_admin_pago_bono, " +
						"'' AS numero_contrato_ops, " +
						"'' AS numero_resolucion_nombramiento_docente, " +
						"'' AS anio_resolucion_nombramiento_docente, " +
						"CASE WHEN be_eliminado IS TRUE THEN to_char( '" + fechadef_depuracion + "':: date  ,'YYYYMMDDHH24MI') ELSE '' END AS fecha_depuracion,  " + //LA FECHA DE DEPURACIÓN SE DEBE ACTUALIZAR CUANDO ESTA INFORMACIÓN SE MONTE A LAS BD DE POSTGRES. LUIS YA SABE DE ESTE PROCESO. POR DEFECTO SE ENVÍA fechadef_depuracion
						"'NO' AS depurado_historia_laboral, " +
						"'NO' AS depurado_pensionado, " +
						"CASE WHEN be_eliminado IS TRUE THEN 'SI' ELSE 'NO' END AS depurado_sustitutos, " +
						"'' AS numero_fallo_judicial_demanda, " +
						"'' AS anio_fallo_judicial, " +
						"'' AS valor_conmitado_pagado_cuota_parte_futura, " +
						"'' AS nit_entidad_receptora_conmut_pago_cuota_parte_futura, " +
						"'' AS acto_administrativo_depuracion, " +
						"'' AS anio_administrativo_depuracion, " +
						"'' AS archivo, " +
						"'' AS numero_documento_usuario_registra, " +
						"CASE WHEN to_char(f_modifica,'YYYYMMDDHH24MI')<> '' " +
							 "THEN to_char(f_modifica,'YYYYMMDDHH24MI') " +
							 "ELSE to_char(now(),'YYYYMMDDHH24MI') END AS fecha_ultima_modificacion, " +
						"null AS salario_base_bono_pensional, " +
						"null AS fecha_salario_base_bono_pensional, " +
						"'' AS salario_base_bono_pensional_calculado, " +
						"'SUSTITUTO' AS grupoA, " +
						"be_co_dane AS codet, " +
						"be_nro_ord AS codua, " +
						"be_id AS regid, " +
						"be_sexo AS genero, " +
						"to_char(be_fec_nac, 'YYYYMMDD') AS fecha_nacimiento " +
						"FROM temp_sustitutos " +
						"LEFT JOIN temp_registraduria ON be_tip_doc = \"TIPO_DOCUMENTO_PK2\" AND be_nro_doc = \"NUMERO_DOCUMENTO_PK1\" " +
						"UNION " +
						"SELECT 2 AS tipo_registro, pf_tip_mue AS tipo_documento, " + //PENSIONADOS FALLECIDOS
						"pf_doc_mue AS documento, " +
						"CASE WHEN split_part(TRIM(REPLACE(pf_nom_mue,'  ',' ')),' ',3) <> '' " +
							"THEN split_part(TRIM(REPLACE(pf_nom_mue,'  ',' ')),' ',3) " +
							"WHEN split_part(TRIM(REPLACE(pf_nom_mue,'  ',' ')),' ',2) <> '' " +
							"THEN split_part(TRIM(REPLACE(pf_nom_mue,'  ',' ')),' ',2) " +
							"ELSE '' END AS primer_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(pf_nom_mue, '  ', ' ')), ' ', 4) <> '' " +
							"THEN split_part(TRIM(REPLACE(pf_nom_mue,'  ',' ')),' ',4) " +
							"ELSE '' END AS segundo_nombre, " +
						"CASE WHEN split_part(TRIM(REPLACE(pf_nom_mue, '  ', ' ')), ' ', 1) <> '' " +
							"THEN split_part(TRIM(REPLACE(pf_nom_mue,'  ',' ')),' ',1) " +
							"ELSE '' END AS primer_apellido, " +
						"CASE WHEN split_part(TRIM(REPLACE(pf_nom_mue, '  ', ' ')), ' ', 2) <> '' " +
							"THEN split_part(TRIM(REPLACE(pf_nom_mue,'  ',' ')),' ',2) " +
							"ELSE '' END AS segundo_apellido, " +
						"CASE WHEN \"CODIGO_VIGENCIA\" LIKE '0' THEN '00' " +
							 "WHEN \"CODIGO_VIGENCIA\" LIKE '1' THEN '01' " +
							 "ELSE \"CODIGO_VIGENCIA\" END AS vigencia_documento, " +
						ultima_fecha_rnec + " AS fecha_actualizacion_rnec, " +
						"'' AS fecha_fallecimiento_minsalud, " +
						"'' AS fecha_primera_afiliacion_sgp, " +
						"CASE WHEN pf_fec_iss IS NOT NULL THEN 'COLP' ELSE 'OTRO' END AS codigo_regimen_primera_afiliacion, " +
						"'' AS tasa_bono, " +
						"CASE WHEN pf_eliminado IS TRUE THEN 'DEM' ELSE '' END AS tipo_depuracion, " +
						"TRIM(to_char(pf_nro_res_fallecimiento, '9999999999')) AS numero_resolucion_fallecimiento, " +
						 "CASE WHEN pf_ano_res_fallecimiento IS NULL THEN '' " +
							 "WHEN pf_ano_res_fallecimiento BETWEEN 10 AND 99 THEN TO_CHAR(1900 + pf_ano_res_fallecimiento,'9999') " +
							 "WHEN pf_ano_res_fallecimiento BETWEEN 10000000 AND 99999999 THEN SUBSTRING(TO_CHAR(pf_ano_res_fallecimiento, '99999999'),6,8) " +
							 "WHEN pf_ano_res_fallecimiento BETWEEN 1900 AND " + anio_corte + " THEN TO_CHAR(pf_ano_res_fallecimiento,'9999') " +
							 "ELSE '' END AS anio_resolucion_fallecimiento, " +
						"'' AS documento_homonimo, " +
						"'' AS descripcion_depuracion, " +
						"'' AS valor_bono, " +
						"'' AS fecha_pago_bono, " +
						"'' AS numero_acto_admin_pago_bono,  " +
						"'' AS anio_acto_admin_pago_bono, " +
						"'' AS numero_contrato_ops, " +
						"'' AS numero_resolucion_nombramiento_docente, " +
						"'' AS anio_resolucion_nombramiento_docente, " +
					   "CASE WHEN pf_eliminado IS TRUE THEN to_char( '" + fechadef_depuracion + "':: date ,'YYYYMMDDHH24MI') ELSE '' END AS fecha_depuracion, " +
						"'NO' AS depurado_historia_laboral, " +
						"CASE WHEN pf_eliminado IS TRUE THEN 'SI' ELSE 'NO' END AS depurado_pensionado, " +
						"'NO' AS depurado_sustitutos, " +
						"'' AS numero_fallo_judicial_demanda, " +
						"'' AS anio_fallo_judicial, " +
						"'' AS valor_conmitado_pagado_cuota_parte_futura, " +
						"'' AS nit_entidad_receptora_conmut_pago_cuota_parte_futura, " +
						"'' AS acto_administrativo_depuracion, " +
						"'' AS anio_administrativo_depuracion, " +
						"'' AS archivo, " +
						"'' AS numero_documento_usuario_registra, " +
						"CASE WHEN to_char(f_modifica,'YYYYMMDDHH24MI')<> '' " +
							 "THEN to_char(f_modifica,'YYYYMMDDHH24MI') " +
							 "ELSE to_char(now(),'YYYYMMDDHH24MI') END AS fecha_ultima_modificacion, " +
						"null AS salario_base_bono_pensional, " +
						"null AS fecha_salario_base_bono_pensional, " +
						"'' AS salario_base_bono_pensional_calculado, " +
						"'PENSIONADO FALLECIDO' AS grupoA, " +
						"pf_co_dane AS codet, " +
						"pf_nro_ord AS codua, " +
						"pf_id AS regid, " +
						"pf_sexo AS genero, " +
						"to_char(pf_fec_nac, 'YYYYMMDD') AS fecha_nacimiento " +
						"FROM temp_pensionadosfallecidos " +
						"LEFT JOIN temp_registraduria ON pf_tip_mue = \"TIPO_DOCUMENTO_PK2\" AND pf_doc_mue = \"NUMERO_DOCUMENTO_PK1\" " +
						")AS info_personas " +
					") tmp WHERE tmp.Cantidad = 1;";
                    postgreSql.EjecutarQuery(queryPostgrSql);

                    queryPostgrSql = "SELECT COUNT(*) FROM temp_reg2;";
					cantidad_registros2 = postgreSql.ConsultarDato(queryPostgrSql);
					cantidad = cantidad_registros2;
					cantidad_lineas = cantidad_lineas + Int64.Parse(cantidad);
					Console.WriteLine("Cantidad de registros R2 (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + cantidad_registros2);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error construyendo informacion R2. {0}", ex.Message));
            }
        }

		public void InformacionTemporal(String pc_datos_ultenvio, PostgreSql postgreSql)
		{
			try
			{
				String queryPostgrSql = string.Empty;
				DataTable dataTable = new DataTable();

				try
				{
					queryPostgrSql = "DROP TABLE IF EXISTS temporal_total; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE TEMP TABLE temporal_total AS " +
					"SELECT* FROM( " +
						"SELECT " +
						"tipo_documento, documento, primer_nombre, segundo_nombre, primer_apellido, segundo_apellido, " +
						"fec_nac, fec_ingreso, fec_retiro, COALESCE(empleador,'000000000') AS empleador, entidad, mismaua, grupoa, sec_pub, otr_pub, sec_pri, fuente, " +
						"ROW_NUMBER() OVER( " +
							"PARTITION BY tipo_documento, documento, empleador, entidad, mismaua " +
							"ORDER BY documento " +
						") " +
						"from( " +
							"select tipo_documento, documento, primer_nombre, " +
							"segundo_nombre, primer_apellido, segundo_apellido, " +
							"salario_base_bono_pensional, grupoa, codet, TMP_UNION.codua, " +
							"ea_nro_doc, ea_nombre, cast(ea_fec_nac as text) as fec_nac, " +
							"cast(ea_fec_ing as text) as fec_ingreso, fec_retiro, " +
							"cast(f1.nit_entidad as text) as empleador, " +
							"f1.nombre_entidad as entidad, " +
							"'true' as mismaUA, " +
							"'true' as sec_pub, " +
							"'false' as otr_pub, " +
							"'false' as sec_pri, " +
							"'actual' as fuente " +
							"FROM( " +
								"SELECT f3.tipo_documento, f3.documento, f3.primer_nombre, " +
										"f3.segundo_nombre, f3.primer_apellido, f3.segundo_apellido, " +
										"f3.salario_base_bono_pensional, f3.grupoa, f3.codet, f3.codua, a.ea_nro_doc, " +
										"a.ea_nombre, a.ea_fec_nac, a.ea_fec_ing, '' fec_retiro " +
								"from temp_reg2 AS f3 " +
								"inner join temp_activos AS a on a.ea_tip_doc = f3.tipo_documento  and a.ea_nro_doc = f3.documento " +
								"union all " +
								"SELECT f3.tipo_documento, f3.documento, f3.primer_nombre, " +
										"f3.segundo_nombre, f3.primer_apellido, f3.segundo_apellido, " +
										"f3.salario_base_bono_pensional, f3.grupoa, f3.codet, f3.codua, r.rs_nro_doc, " +
										"r.rs_nombre, r.rs_fec_nac, r.rs_fec_ing, cast(r.rs_fec_ret as text) " +
								"from temp_reg2 AS f3 " +
								"inner join temp_retirados AS R on r.rs_tip_doc = f3.tipo_documento  and r.rs_nro_doc = f3.documento) AS TMP_UNION " +
							"INNER JOIN temp_reg1 AS f1 ON TMP_UNION.codet = f1.codigo_dane AND TMP_UNION.codua = f1.codua " +
							"WHERE grupoA IN('ACTIVO', 'RETIRADO')  " +
							"union all " +
							"SELECT tipo_documento, documento, primer_nombre, " +
							"segundo_nombre, primer_apellido, segundo_apellido, " +
							"salario_base_bono_pensional, grupoa, codet, TMP_UNION.codua, " +
							"ha_nro_doc, '' as nombre, '' as fec_nac, cast(ha_fec_ing as text) as fec_ingreso, " +
							"cast(ha_fec_ret as text) as fec_retiro, " +
							"CASE " +
								"WHEN ha_misma_ua = true THEN CAST(f1.nit_entidad AS TEXT) " +
								"WHEN ha_misma_ua = false then cast(ha_nit as text) " +
							"END as empleador, " +
							"CASE " +
								"WHEN ha_misma_ua = true THEN f1.nombre_entidad " +
								"WHEN ha_misma_ua = false then ha_empresa " +
							"END as empresa, " +
							"CASE " +
								"WHEN ha_misma_ua = true THEN 'true' " +
								"WHEN ha_misma_ua = false then 'false' " +
							"END as mismaUA, " +
							"CASE " +
								"WHEN ha_sec_pub = true THEN 'true' " +
								"WHEN ha_sec_pub = false then 'false' " +
							"END as sec_pub, " +
							"CASE " +
								"WHEN ha_otr_pub = true THEN 'true' " +
								"WHEN ha_otr_pub = false then 'false' " +
							"END as otr_pub, " +
							"CASE " +
								"WHEN ha_sec_pri = true THEN 'true' " +
								"WHEN ha_sec_pri = false then 'false' " +
							"END as sec_pri, " +
							"'hl' as fuente " +
							"FROM( " +
								"SELECT f3.tipo_documento, f3.documento, f3.primer_nombre, " +
										"f3.segundo_nombre, f3.primer_apellido, f3.segundo_apellido, " +
										"f3.salario_base_bono_pensional, f3.grupoa, f3.codet, f3.codua, hlA.ha_misma_ua, hlA.ha_nro_doc, " +
										"hlA.ha_fec_ing, hlA.ha_fec_ret, hlA.ha_nit, hlA.ha_empresa, hlA.ha_sec_pub, hlA.ha_otr_pub, hlA.ha_sec_pri " +
								"FROM temp_reg2 AS f3 " +
								"INNER JOIN temp_historialaboral_activos AS hlA ON hlA.ha_tip_doc = f3.tipo_documento " +
									"AND hlA.ha_nro_doc = f3.documento and hlA.ha_misma_ua = true AND grupoA = 'ACTIVO' " +
								"UNION ALL " +
								"SELECT f3.tipo_documento, f3.documento, f3.primer_nombre, " +
										"f3.segundo_nombre, f3.primer_apellido, f3.segundo_apellido, " +
										"f3.salario_base_bono_pensional, f3.grupoa, f3.codet, f3.codua, hlA1.ha_misma_ua, hlA1.ha_nro_doc, " +
										"hlA1.ha_fec_ing, hlA1.ha_fec_ret, hlA1.ha_nit, hlA1.ha_empresa, hlA1.ha_sec_pub, hlA1.ha_otr_pub, hlA1.ha_sec_pri " +
								"FROM temp_reg2 AS f3 " +
								"INNER JOIN temp_historialaboral_activos AS hlA1 ON hlA1.ha_tip_doc = f3.tipo_documento " +
									"AND hlA1.ha_nro_doc = f3.documento and hlA1.ha_misma_ua = false AND grupoA = 'ACTIVO' " +
								"UNION ALL " +
								"SELECT f3.tipo_documento, f3.documento, f3.primer_nombre, " +
										"f3.segundo_nombre, f3.primer_apellido, f3.segundo_apellido, " +
										"f3.salario_base_bono_pensional, f3.grupoa, f3.codet, f3.codua, hlR.hs_misma_ua, hlR.hs_nro_doc, " +
										"hlR.hs_fec_ing, hlR.hs_fec_ret, hlR.hs_nit, hlR.hs_empresa, hlR.hs_sec_pub, hlR.hs_otr_pub, hlR.hs_sec_pri " +
								"FROM temp_reg2 AS f3 " +
								"INNER JOIN temp_historialaboral_retirados AS hlR ON hlR.hs_tip_doc = f3.tipo_documento " +
									"AND hlR.hs_nro_doc = f3.documento and hlR.hS_misma_ua = true AND grupoA = 'RETIRADO' " +
								"UNION ALL " +
								"SELECT f3.tipo_documento, f3.documento, f3.primer_nombre, " +
										"f3.segundo_nombre, f3.primer_apellido, f3.segundo_apellido, " +
										"f3.salario_base_bono_pensional, f3.grupoa, f3.codet, f3.codua, hlR1.hs_misma_ua, hlR1.hs_nro_doc, " +
										"hlR1.hs_fec_ing, hlR1.hs_fec_ret, hlR1.hs_nit, hlR1.hs_empresa, hlR1.hs_sec_pub, hlR1.hs_otr_pub, hlR1.hs_sec_pri " +
								"FROM temp_reg2 AS f3 " +
								"INNER JOIN temp_historialaboral_retirados AS hlR1 ON hlR1.hs_tip_doc = f3.tipo_documento " +
									"AND hlR1.hs_nro_doc = f3.documento and hlR1.hS_misma_ua = false AND grupoA = 'RETIRADO') AS TMP_UNION " +
							"INNER JOIN temp_reg1 AS f1 ON TMP_UNION.codet = f1.codigo_dane AND TMP_UNION.codua = f1.codua " +
							"WHERE grupoA IN('ACTIVO', 'RETIRADO') " +
						") AS tmp " +
					") TMP2; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE INDEX idx_doc_nit ON temporal_total (tipo_documento,documento,empleador);";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "DROP TABLE IF EXISTS temporal;";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE TEMP TABLE temporal AS "+
					"SELECT * FROM temporal_total WHERE mismaua = 'true'; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "SELECT COUNT(*) FROM temporal;";
					Console.WriteLine("Cantidad de registros temporal (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + postgreSql.ConsultarDato(queryPostgrSql));
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error construyendo temporal. {0}", ex.Message));
			}
		}

		public void InformacionR4(String pc_datos_ultenvio, PostgreSql postgreSql)
		{
			try
			{
				String queryPostgrSql = string.Empty;
				DataTable dataTable = new DataTable();
				String nombreEntidadCentral = string.Empty;

				queryPostgrSql = "SELECT r1.nombre_entidad FROM temp_reg1 AS r1 WHERE seccional = '000';";
				nombreEntidadCentral = postgreSql.ConsultarDato(queryPostgrSql);					

				try
				{
					queryPostgrSql = "DROP TABLE IF EXISTS temp_reg4; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE TEMP TABLE temp_reg4 AS "+
					"SELECT 4 AS tipo_registro, " +
					nit +" AS nit_certificadora,  " +
					"'000' AS seccional_certificadora, " +
					"'"+nombreEntidadCentral +"' AS nombre_certificadora, "+
					"CASE " +
						"WHEN f3.grupoA = 'ACTIVO' THEN " +
							"(LPAD(f3.codet, 5, '0') || '-' || LPAD(f3.codua, 5, '0') || '-' || 'EA' || '-' || SUBSTRING(LPAD(md5(random() :: text), 5, '0') from 1 for 5)) " +
						"ELSE " +
							"(LPAD(f3.codet, 5, '0') || '-' || LPAD(f3.codua, 5, '0') || '-' || 'RS' || '-' || SUBSTRING(LPAD(md5(random() :: text), 5, '0') from 1 for 5))  " +
					"END AS numero_certificacion,  " +
					"'I' AS tipo_informacion, " +
					"TMP.empleador AS nit_empleador, " +
					"f1.seccional AS seccional_empleador, " +
					"TMP.entidad AS nombre_empleador, " +
					"CASE " +
						"WHEN TMP.mismaua = 'false' THEN 'OS' " +
						"ELSE " +
							"CASE " +
								"WHEN f1.tipo_empleador = 'HSP' THEN 'SA' ELSE 'OS' " +
							"END " +
					"END AS sector_seccional_empleador, " +
					"CASE " +
						"WHEN TMP.sec_pub = 'true' THEN " +
							"CASE " +
								"WHEN SUBSTRING(f1.codigo_dane,3,3) = '000' THEN 'PUBLICO DEPARTAMENTAL' " +
								"WHEN f1.codigo_dane IN('11001', '76001', '13001') THEN 'PUBLICO DISTRITAL' " +
								"ELSE 'PUBLICO MUNICIPAL' " +
							"END " +
						"WHEN TMP.sec_pri = 'true' THEN 'PRIVADO' " +
						"ELSE 'PUBLICO MUNICIPAL' " +
				   "END AS naturaleza_empleador, " +
				   "CAST('O_OTRO' AS TEXT) AS sub_sector_entidad_territorial, " +
				   "CAST('' AS TEXT) AS fecha_certificacion, " +
				   "f3.tipo_documento AS tipo_documento_beneficiario, " +
					"f3.documento AS documento_beneficiario, " +
					"f3.primer_nombre AS primer_nombre_beneficiario, " +
					"f3.segundo_nombre AS segundo_nombre_beneficiario, " +
					"f3.primer_apellido AS primer_apellido_beneficiario, " +
					"f3.segundo_apellido AS segundo_apellido_beneficiario, " +
					"f3.genero AS genero, " +
					"f3.fecha_nacimiento AS fecha_nacimiento, " +
					"CAST('' AS TEXT) AS tipo_documento_alterno_beneficiario, " +
					"CAST('' AS TEXT) AS documento_alterno_beneficiario, " +
					"CAST('' AS TEXT) AS primer_nombre_alterno, " +
					"CAST('' AS TEXT) AS segundo_nombre_alterno, " +
					"CAST('' AS TEXT) AS primer_apellido_alterno, " +
					"CAST('' AS TEXT) AS segundo_apellido_alterno, " +
					"2 AS tipo_indicio, " +
					"CAST('' AS TEXT) AS tipo_documento_funcionario, " +
					"CAST('' AS TEXT) AS documento_funcionario, " +
					"CAST('' AS TEXT) AS primer_nombre_funcionario, " +
					"CAST('' AS TEXT) AS segundo_nombre_funcionario, " +
					"CAST('' AS TEXT) AS primer_apellido_funcionario, " +
					"CAST('' AS TEXT) AS segundo_apellido_funcionario, " +
					"CAST('' AS TEXT) AS cargo_funcionario, " +
					"CAST('' AS TEXT) AS tipo_documento_usuario, " +
					"CAST('' AS TEXT) AS numero_documento_usuario, " +
					"f3.fecha_ultima_modificacion, " +
					"TMP.mismaua " +
					"FROM temp_reg2 AS f3 " +
				   "INNER JOIN temporal AS TMP ON f3.tipo_documento = TMP.tipo_documento AND f3.documento = TMP.documento " +
						"AND ROW_NUMBER = 1 " +
					"INNER JOIN temp_reg1 AS f1 ON f3.codet = f1.codigo_dane AND f3.codua = f1.codua " +
					"WHERE f3.grupoA IN('ACTIVO', 'RETIRADO'); ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "SELECT COUNT(*) FROM temp_reg4;";
					cantidad = postgreSql.ConsultarDato(queryPostgrSql);
					cantidad_lineas = cantidad_lineas + Int64.Parse(cantidad);
					Console.WriteLine("Cantidad de registros R4 (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + cantidad);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error construyendo R4. {0}", ex.Message));
			}
		}

		public void InformacionR5(String pc_datos_ultenvio, PostgreSql postgreSql)
		{
			try
			{
				String queryPostgrSql = string.Empty;
				DataTable dataTable = new DataTable();

				try
				{
					queryPostgrSql = "DROP TABLE IF EXISTS temp_reg5; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE TEMP TABLE temp_reg5 AS "+
					"SELECT* FROM( " +
						"SELECT 5 AS tipo_registro, " + //VINCULACIÓN ACTUAL ACTIVOS
						"r4.numero_certificacion, " +
						"'I' AS tipo_informacion, " +
						"'LABORAL' AS tipo_novedad, " +
						"CASE WHEN TMP.fec_ingreso <> '' THEN to_char(cast(TMP.fec_ingreso as date),'YYYYMMDD') ELSE '' END AS fecha_inicial, " +
						"'' AS fecha_final, " +
						"'130' AS cargo, " +
						"CASE WHEN f3.ea_fec_iss IS NOT NULL OR f3.ea_fec_afp IS NOT NULL OR f3.ea_fec_otr IS NOT NULL THEN 'S' ELSE 'N' END AS realizo_aportes, " +
						"CASE WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_iss > f3.ea_fec_afp AND f3.ea_fec_iss > f3.ea_fec_otr THEN "+ nitiss  +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_afp > f3.ea_fec_iss AND f3.ea_fec_afp > f3.ea_fec_otr THEN 14 " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_otr > f3.ea_fec_iss AND f3.ea_fec_otr > f3.ea_fec_afp THEN 6 " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NULL AND f3.ea_fec_iss > f3.ea_fec_afp THEN " + nitiss +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NULL AND f3.ea_fec_afp > f3.ea_fec_iss THEN 14 " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_afp IS NULL AND f3.ea_fec_iss > f3.ea_fec_otr THEN " + nitiss +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_afp IS NULL AND f3.ea_fec_otr > f3.ea_fec_iss THEN 6 " +
							 "WHEN f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_iss IS NULL AND f3.ea_fec_afp > f3.ea_fec_otr THEN 14 " +
							 "WHEN f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_iss IS NULL AND f3.ea_fec_otr > f3.ea_fec_afp THEN 6 " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NULL AND f3.ea_fec_otr IS NULL THEN " + nitiss +
							 "WHEN f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_iss IS NULL AND f3.ea_fec_otr IS NULL THEN 14 " +
							 "WHEN f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_iss IS NULL AND f3.ea_fec_afp IS NULL THEN 6 " +
							 "ELSE 0 END AS nit_fondo_aportes, " +
						"CASE WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_iss > f3.ea_fec_afp AND f3.ea_fec_iss > f3.ea_fec_otr THEN 'ISS' " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_afp > f3.ea_fec_iss AND f3.ea_fec_afp > f3.ea_fec_otr THEN 'AFP' " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_otr > f3.ea_fec_iss AND f3.ea_fec_otr > f3.ea_fec_afp THEN 'Otro' " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NULL AND f3.ea_fec_iss > f3.ea_fec_afp THEN 'ISS' " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NULL AND f3.ea_fec_afp > f3.ea_fec_iss THEN 'AFP' " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_afp IS NULL AND f3.ea_fec_iss > f3.ea_fec_otr THEN 'ISS' " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_afp IS NULL AND f3.ea_fec_otr > f3.ea_fec_iss THEN 'Otro' " +
							 "WHEN f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_iss IS NULL AND f3.ea_fec_afp > f3.ea_fec_otr THEN 'AFP' " +
							 "WHEN f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_iss IS NULL AND f3.ea_fec_otr > f3.ea_fec_afp THEN 'Otro' " +
							 "WHEN f3.ea_fec_iss IS NOT NULL AND f3.ea_fec_afp IS NULL AND f3.ea_fec_otr IS NULL THEN 'ISS' " +
							 "WHEN f3.ea_fec_afp IS NOT NULL AND f3.ea_fec_iss IS NULL AND f3.ea_fec_otr IS NULL THEN 'AFP' " +
							 "WHEN f3.ea_fec_otr IS NOT NULL AND f3.ea_fec_iss IS NULL AND f3.ea_fec_afp IS NULL THEN 'Otro' " +
							 "ELSE '' END AS nombre_fondo_aportes, " +
						"8 AS horas_laboradas, " +
						"0 AS dias_interrupcion, " +
						"'' AS fuente_recursos, " +
						"'' AS nit_establecimiento, " +
						"'' AS nombre_establecimiento, " +
						"'' AS nivel_establecimiento, " +
						"'' AS municipio, " +
						"'' AS factor_aportes, " +
						"'' AS sesiones_asistidas, " +
						"'' AS sesiones_no_asistidas, " +
						"'' AS total_sesiones " +
						"FROM temp_activos AS f3 " +
						"INNER JOIN temporal AS TMP ON f3.ea_tip_doc = TMP.tipo_documento AND f3.ea_nro_doc = TMP.documento " +
							"AND TMP.fuente <> 'hl' " +
						"INNER JOIN temp_reg4 AS r4 ON r4.tipo_documento_beneficiario = TMP.tipo_documento AND r4.documento_beneficiario = TMP.documento " +
							"AND r4.nit_empleador = TMP.empleador  AND TMP.mismaua = r4.mismaua " +
						"INNER JOIN temp_reg1 AS f1 ON f1.codua = f3.ea_nro_ord AND f1.codigo_dane = f3.ea_co_dane " +
						"UNION " +
						"SELECT 5 AS tipo_registro, " + //HL ACTIVOS
						"r4.numero_certificacion, " +
						"'I' AS tipo_informacion, " +
						"'LABORAL' AS tipo_novedad, " +
						"CASE WHEN TMP.fec_ingreso <> '' THEN to_char(cast(TMP.fec_ingreso as date),'YYYYMMDD') ELSE '' END AS fecha_inicial, " +
						"CASE WHEN TMP.fec_retiro <> '' THEN to_char(cast(TMP.fec_retiro as date),'YYYYMMDD') ELSE '' END AS fecha_final, " +
						"'130' AS cargo, " +
						"CASE WHEN hl.ha_cot_ent <> 0 AND hl.ha_cot_ent <> 15 AND hl.ha_labor_a + hl.ha_labor_m > 0 AND hl.ha_cot_sem > 0 THEN 'S' " +
							 "ELSE 'N' END AS realizo_aportes, " +
						"CASE WHEN fondos.nit IS NOT NULL THEN fondos.nit " +
							 "WHEN fondos.id IS NOT NULL THEN fondos.id ELSE '0' END AS nit_fondo_aportes, " +
						"CASE WHEN fondos.id IS NOT NULL THEN fondos.nombre ELSE '' END AS nombre_fondo_aportes, " +
						"8 AS horas_laboradas, " +
						"0 AS dias_interrupcion, " +
						"'' AS fuente_recursos, " +
						"'' AS nit_establecimiento, " +
						"'' AS nombre_establecimiento, " +
						"'' AS nivel_establecimiento, " +
						"'' AS municipio, " +
						"'' AS factor_aportes, " +
						"'' AS sesiones_asistidas, " +
						"'' AS sesiones_no_asistidas, " +
						"'' AS total_sesiones " +
						"FROM temp_historialaboral_activos AS hl " +
						"INNER JOIN temporal AS TMP ON hl.ha_tip_doc = TMP.tipo_documento AND hl.ha_nro_doc = TMP.documento " +
							"AND cast(hl.ha_nit as text) = TMP.empleador AND TMP.fuente = 'hl' AND TMP.mismaua = cast(hl.ha_misma_ua as text) " +
						"INNER JOIN temp_reg4 AS r4 ON r4.tipo_documento_beneficiario = TMP.tipo_documento AND r4.documento_beneficiario = TMP.documento " +
							"AND r4.nit_empleador = TMP.empleador AND TMP.mismaua = r4.mismaua " +
						"LEFT OUTER JOIN pruebas.fondos_aportes AS fondos ON fondos.id = hl.ha_cot_ent " +
						"UNION " +
						"SELECT 5 AS tipo_registro,  " + //VINCULACION ACTUAL RETIRADOS
						"r4.numero_certificacion, " +
						"'I' AS tipo_informacion, " +
						"'LABORAL' AS tipo_novedad, " +
						"CASE WHEN TMP.fec_ingreso <> '' THEN to_char(cast(TMP.fec_ingreso as date),'YYYYMMDD') ELSE '' END AS fecha_inicial, " +
						"CASE WHEN TMP.fec_retiro <> '' THEN to_char(cast(TMP.fec_retiro as date),'YYYYMMDD') ELSE '' END AS fecha_final, " +
						"'130' AS cargo, " +
						"CASE WHEN(f9.rs_fec_iss IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_iss AS DATE), 'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101'  ELSE f1.fecha_vigencia_sgp END) OR " +
								  "(f9.rs_fec_afp IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_afp AS DATE), 'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101' ELSE f1.fecha_vigencia_sgp END) OR " +
								   "(f9.rs_fec_otr IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_otr AS DATE), 'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101' ELSE f1.fecha_vigencia_sgp END) THEN 'S'  " +
								  "ELSE 'N' END AS realizo_aportes, " +
						"CASE WHEN(f9.rs_fec_iss IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_iss AS DATE),'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101' ELSE f1.fecha_vigencia_sgp END) THEN 0 " +
							 "WHEN(f9.rs_fec_afp IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_afp AS DATE), 'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101' ELSE f1.fecha_vigencia_sgp END) THEN 14 " +
							 "WHEN(f9.rs_fec_otr IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_otr AS DATE), 'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101' ELSE f1.fecha_vigencia_sgp END) THEN 6 " +
							 "ELSE 0 END AS nit_fondo_aportes, " +
						"CASE WHEN(f9.rs_fec_iss IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_iss AS DATE),'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101' ELSE f1.fecha_vigencia_sgp END) THEN 'ISS' " +
							 "WHEN(f9.rs_fec_afp IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_afp AS DATE), 'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101' ELSE f1.fecha_vigencia_sgp END) THEN 'AFP' " +
							 "WHEN(f9.rs_fec_otr IS NOT NULL AND TO_CHAR(CAST(f9.rs_fec_otr AS DATE), 'YYYYMMDD') < CASE WHEN f1.fecha_vigencia_sgp IS NULL THEN '20210101' ELSE f1.fecha_vigencia_sgp END) THEN 'Otro' " +
							 "ELSE '' END AS nombre_fondo_aportes, " +
						"8 AS horas_laboradas, " +
						"0 AS dias_interrupcion, " +
						"'' AS fuente_recursos, " +
						"'' AS nit_establecimiento, " +
						"'' AS nombre_establecimiento, " +
						"'' AS nivel_establecimiento, " +
						"'' AS municipio, " +
						"'' AS factor_aportes, " +
						"'' AS sesiones_asistidas, " +
						"'' AS sesiones_no_asistidas, " +
						"'' AS total_sesiones " +
						"FROM temp_retirados AS f9 " +
						"INNER JOIN temporal AS TMP ON f9.rs_tip_doc = TMP.tipo_documento AND f9.rs_nro_doc = TMP.documento " +
							"AND TMP.fuente <> 'hl' " +
						"INNER JOIN temp_reg4 AS r4 ON r4.tipo_documento_beneficiario = TMP.tipo_documento AND r4.documento_beneficiario = TMP.documento " +
							"AND r4.nit_empleador = TMP.empleador AND TMP.mismaua = r4.mismaua " +
						"INNER JOIN temp_reg1 AS f1 ON f1.codua = f9.rs_nro_ord AND f1.codigo_dane = f9.rs_co_dane " +
						"UNION " +
						"SELECT 5 AS tipo_registro, "+
						"r4.numero_certificacion, " +
						"'I' AS tipo_informacion, " +
						"'LABORAL' AS tipo_novedad, " +
						"CASE WHEN TMP.fec_ingreso <> '' THEN to_char(cast(TMP.fec_ingreso as date),'YYYYMMDD') ELSE '' END AS fecha_inicial, " +
						"CASE WHEN TMP.fec_retiro <> '' THEN to_char(cast(TMP.fec_retiro as date),'YYYYMMDD') ELSE '' END AS fecha_final, " +
						"'130' AS cargo, " +
						"CASE WHEN hl.hs_cot_ent <> 0 AND hl.hs_cot_ent <> 15 AND hl.hs_labor_a + hl.hs_labor_m > 0 AND hl.hs_cot_sem > 0 THEN 'S' " +
							"ELSE 'N' END AS realizo_aportes, " +
						"CASE WHEN fondos.nit IS NOT NULL THEN fondos.nit " +
							 "WHEN fondos.id IS NOT NULL THEN fondos.id ELSE '0' END AS nit_fondo_aportes, " +
						"CASE WHEN fondos.id IS NOT NULL THEN fondos.nombre ELSE '' END AS nombre_fondo_aportes, " +
						"8 AS horas_laboradas, " +
						"0 AS dias_interrupcion, " +
						"'' AS fuente_recursos, " +
						"'' AS nit_establecimiento, " +
						"'' AS nombre_establecimiento, " +
						"'' AS nivel_establecimiento, " +
						"'' AS municipio, " +
						"'' AS factor_aportes, " +
						"'' AS sesiones_asistidas, " +
						"'' AS sesiones_no_asistidas, " +
						"'' AS total_sesiones " +
						"fROM temp_historialaboral_retirados AS hl " +
						"INNER JOIN temporal AS TMP ON hl.hs_tip_doc = TMP.tipo_documento AND hl.hs_nro_doc = TMP.documento " +
							"AND cast(hl.hs_nit as text) = TMP.empleador AND TMP.fuente = 'hl' AND TMP.mismaua = cast(hl.hs_misma_ua as text) " +
						"INNER JOIN temp_reg4 AS r4 ON r4.tipo_documento_beneficiario = TMP.tipo_documento AND r4.documento_beneficiario = TMP.documento " +
							"AND r4.nit_empleador = TMP.empleador AND TMP.mismaua = r4.mismaua " +
						"LEFT JOIN pruebas.fondos_aportes AS fondos ON fondos.id = hl.hs_cot_ent " +		
					") AS historia_laboral; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "SELECT COUNT(*) FROM temp_reg5;";
					cantidad = postgreSql.ConsultarDato(queryPostgrSql);
					cantidad_lineas = cantidad_lineas + Int64.Parse(cantidad);
					Console.WriteLine("Cantidad de registros R5 (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + cantidad);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error construyendo R5. {0}", ex.Message));
			}
		}

		public void InformacionR6(String pc_datos_ultenvio, PostgreSql postgreSql)
		{
			try
			{
				String queryPostgrSql = string.Empty;
				DataTable dataTable = new DataTable();

				try
				{
					queryPostgrSql = "DROP TABLE IF EXISTS temp_reg6; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE TEMP TABLE temp_reg6 AS "+
					"SELECT* FROM( " +
					   "SELECT 6 AS tipo_registro, " +
					   "r4.numero_certificacion, " +
						"'I' AS tipo_informacion, " +
						"(EXTRACT(YEAR FROM f3.f_corte) || '12') AS periodo, " +
						"'N' AS salario_integral, " +
						"9999 AS factor_salarial, " +
						"f3.ea_salario AS valor_devengado, " +
						"CASE WHEN f3.ea_fec_iss IS NOT NULL OR f3.ea_fec_afp IS NOT NULL OR f3.ea_fec_otr IS NOT NULL THEN 'S' ELSE 'N' END AS realizo_aportes, " +
						"'MENSUAL' AS periodicidad_factor, " +
 						"(EXTRACT(YEAR FROM f3.f_corte) || '0101') AS fecha_inicial_causacion, " +
						"(EXTRACT(YEAR FROM f3.f_corte) || '1231') AS fecha_final_causacion " +
						"FROM temp_activos AS f3 " +
						"INNER JOIN temp_reg4 AS r4 ON r4.tipo_documento_beneficiario = f3.ea_tip_doc AND r4.documento_beneficiario = f3.ea_nro_doc " +
						"UNION " +
						"SELECT 6 AS tipo_registro, " +
						"r4.numero_certificacion, " +
						"'I' AS tipo_informacion, " +
						"(EXTRACT(YEAR FROM rs_fec_ret) || '12') AS periodo, " +
						"'N' AS salario_integral, " +
						"9999 AS factor_salarial, " +
						"f9.rs_salario AS valor_devengado, " +
						"CASE WHEN f9.rs_fec_iss IS NOT NULL OR f9.rs_fec_afp IS NOT NULL OR f9.rs_fec_otr IS NOT NULL THEN 'S' ELSE 'N' END AS realizo_aportes, " +
						"'MENSUAL' AS periodicidad_factor, " +
 						"(EXTRACT(YEAR FROM rs_fec_ret) || '0101') AS fecha_inicial_causacion, " +
						 "(EXTRACT(YEAR FROM rs_fec_ret) || '1231') AS fecha_final_causacion " +
						"FROM temp_retirados AS f9 " +
						"INNER JOIN temp_reg4 AS r4 ON r4.tipo_documento_beneficiario = f9.rs_tip_doc AND r4.documento_beneficiario = f9.rs_nro_doc " +
					") AS salarios; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "SELECT COUNT(*) FROM temp_reg6;";
					cantidad = postgreSql.ConsultarDato(queryPostgrSql);
					cantidad_lineas = cantidad_lineas + Int64.Parse(cantidad);
					Console.WriteLine("Cantidad de registros R6 (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + cantidad);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error construyendo R6. {0}", ex.Message));
			}
		}

		public void InformacionR7(String pc_datos_ultenvio, PostgreSql postgreSql)
		{
			try
			{
				String queryPostgrSql = string.Empty;
				DataTable dataTable = new DataTable();

				try
				{
					queryPostgrSql = "DROP TABLE IF EXISTS temp_reg7; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE TEMP TABLE temp_reg7 AS "+
					"SELECT* FROM( " + //						--PENSIONADOS
						"SELECT 7 AS tipo_registro, " +
						"LPAD(f5.pe_co_dane, 5, '0') || '-' || LPAD(f5.pe_nro_ord, 5, '0') || '-' || 'PV' || '-' || SUBSTRING(LPAD(CAST(f5.pe_id AS TEXT), 5, '0') from 1 for 5) AS numero_certificacion, " +
						"'I' AS tipo_informacion, " +
						"f1.nit_entidad AS nit_empleador, " +
						"f1.seccional AS seccional, " +
						"f1.nombre_entidad AS nombre_empleador, " +
						"f5.pe_tip_doc AS tipo_documento, " +
						"f5.pe_nro_doc AS numero_documento, " +
						"CASE WHEN f5.pe_sexo <> '' THEN f5.pe_sexo ELSE 'M' END AS genero, " +
						"CASE WHEN f5.pe_est_civ <> '' THEN UPPER(f5.pe_est_civ) ELSE 'L' END AS estado_civil, " +
						"CASE WHEN to_char(cast(f5.pe_fec_nac as date), 'YYYYMMDD') <> '' THEN to_char(cast(f5.pe_fec_nac as date),'YYYYMMDD') ELSE '19500101' END AS fecha_nacimiento, " +
						"CASE WHEN to_char(cast(f5.pe_f_nac_c as date), 'YYYYMMDD') <> '' THEN to_char(cast(f5.pe_f_nac_c as date),'YYYYMMDD') ELSE '19500101' END AS fecha_nacimiento_conyuge, " +
						"CASE WHEN f5.pe_sexo = 'M' THEN 'F' WHEN f5.pe_sexo = 'F' THEN 'M' ELSE 'F' END AS genero_conyuge, " +
						"CASE WHEN f5.pe_est_pen IS TRUE THEN 'S' ELSE 'N' END AS estado_invalidez, " +
						"CASE WHEN f5.pe_directo <> '' THEN f5.pe_directo ELSE 'D' END AS pension_directa, " +
						"f5.pe_pension AS valor_mesada, "+
						"CASE WHEN f5.pe_com_iss IS TRUE THEN 'S' ELSE 'N' END AS pension_compartida_iss, " +
						"CASE WHEN f5.pe_fut_iss IS TRUE THEN 'S' ELSE 'N' END AS pension_futura_compartida_iss, " +
						"COALESCE(ROUND(CAST(f5.pe_por_pen AS numeric), 2), '100') AS porcentaje_pension_cargo, " +
						  "CASE WHEN f5.pe_fec_iss IS NOT NULL THEN to_char(cast(f5.pe_fec_iss as date),'YYYYMMDD') ELSE '' END AS fecha_afiliacion_iss, " +
						"COALESCE(to_char(cast(f5.pe_fec_pen as date), 'YYYYMMDD'), '19910101') AS fecha_pension, " +
						  "COALESCE(to_char(cast(f5.f_corte as date), 'YYYYMMDD'), to_char(CAST('"+ fechaCorte+ "' AS date), 'YYYYMMDD')) AS fecha_corte_informacion, " +
							 "CASE WHEN f5.pe_mesadas_junio IS NOT NULL AND f5.pe_mesadas_junio >= 1 AND f5.pe_mesadas_dic IS NOT NULL AND f5.pe_mesadas_dic = 0 THEN 0 " +
							  "WHEN f5.pe_mesadas_junio IS NOT NULL AND f5.pe_mesadas_junio >= 1 AND(f5.pe_mesadas_dic IS NULL OR f5.pe_mesadas_dic >= 1) THEN 1 " +
							 "ELSE 0 END AS numero_mesadas_junio,  " +
						"CASE WHEN f5.pe_mesadas_dic IS NOT NULL AND f5.pe_mesadas_dic >= 1 THEN 1 ELSE 0 END AS numero_mesadas_diciembre, " +
						"CASE WHEN to_char(cast(f5.pe_fec_res as date), 'YYYYMMDD') <> '' THEN to_char(cast(f5.pe_fec_res as date),'YYYYMMDD') " +
							 "ELSE to_char(cast('1991-01-01' as date),'YYYYMMDD') END AS fecha_resolucion_pension, " +
						"COALESCE(CAST(f5.pe_nro_res AS VARCHAR), '000000000000001') AS numero_resolucion_pension, " +
						"'' AS adjunto_resolucion_pension, " +
						"COALESCE(f5.pe_tipo_pension, 'V') AS tipo_pension, " +
						 "COALESCE(f5.pe_pension_resolucion, 0) AS valor_mesada_resolucion, " +
						  "COALESCE(f5.pe_pago_salud_razon, 'NOAP') AS razon_aportes_salud, " +
						   "COALESCE(ROUND(CAST(f5.pe_pago_salud_porcentaje AS numeric), 2), 0) AS porcentaje_aportes_salud, " +
							 "COALESCE(f5.pe_pago_salud_monto, 0) AS valor_aportes_salud, " +
							 " COALESCE(f5.pe_pago_salud_desc_razon, 'NOAP') AS razon_aporte_salud_desc, " +
						"'' AS fecha_fallecimiento, " +
						"'' AS tipo_documento_fallecimiento, " +
						"'' AS numero_resolucion_fallecimiento, " +
						"'' AS anio_resolucion_fallecimiento, " +
						"0 AS valor_ultima_mesada, " +
						"'' AS fecha_ultima_mesada, " +
						"'' AS tipo_documento_registra, " +
						"'' AS numero_documento_registra, " +
						"COALESCE(to_char(f5.f_modifica, 'YYYYMMDDHH24MI'), to_char(NOW(), 'YYYYMMDDHH24MI')) AS fecha_ultima_modificacion " +
						"FROM temp_pensionados AS f5 " +
						"JOIN temp_reg1 AS f1 ON f5.pe_co_dane = f1.codigo_dane AND f5.pe_nro_ord = f1.codua " +
						"UNION " +//						-- PENSIONADOS FALLECIDOS
						"SELECT 7 AS tipo_registro, " +
						"LPAD(f5.pf_co_dane, 5, '0') || '-' || LPAD(f5.pf_nro_ord, 5, '0') || '-' || 'PF' || '-' || SUBSTRING(LPAD(CAST(f5.pf_id AS TEXT), 5, '0') from 1 for 5) AS numero_certificacion, " +
						"'I' AS tipo_informacion, " +
						"f1.nit_entidad AS nit_empleador, " +
						"f1.seccional AS seccional, " +
						"f1.nombre_entidad AS nombre_empleador, " +
						"f5.pf_tip_mue AS tipo_documento, " +
						"f5.pf_doc_mue AS numero_documento, " +
						"CASE WHEN f5.pf_sexo <> '' THEN f5.pf_sexo ELSE 'M' END AS genero, " +
						"CASE WHEN f5.pf_est_civ <> '' THEN UPPER(f5.pf_est_civ) ELSE 'L' END AS estado_civil, " +
						"CASE WHEN to_char(cast(f5.pf_fec_nac as date), 'YYYYMMDD') <> '' THEN to_char(cast(f5.pf_fec_nac as date),'YYYYMMDD') ELSE '19500101' END AS fecha_nacimiento, " +
						"CASE WHEN to_char(cast(f5.pf_f_nac_c as date), 'YYYYMMDD') <> '' THEN to_char(cast(f5.pf_f_nac_c as date),'YYYYMMDD') ELSE '19500101' END AS fecha_nacimiento_conyuge, " +
						"CASE WHEN f5.pf_sexo = 'M' THEN 'F' WHEN f5.pf_sexo = 'F' THEN 'M' ELSE 'F' END AS genero_conyuge, " +
						"CASE WHEN f5.pf_est_pen IS TRUE THEN 'S' ELSE 'N' END AS estado_invalidez, " +
						"CASE WHEN f5.pf_directo <> '' THEN f5.pf_directo ELSE 'D' END AS pension_directa, " +
						"f5.pf_ultima_pension AS valor_mesada, " +
						"CASE WHEN f5.pf_com_iss IS TRUE THEN 'S' ELSE 'N' END AS pension_compartida_iss, " +
						"CASE WHEN f5.pf_fut_iss IS TRUE THEN 'S' ELSE 'N' END AS pension_futura_compartida_iss, " +
						"COALESCE(ROUND(CAST(f5.pf_por_pen AS numeric), 2), '100') AS porcentaje_pension_cargo, " +
						  "CASE WHEN f5.pf_fec_iss IS NOT NULL THEN to_char(cast(f5.pf_fec_iss as date),'YYYYMMDD') ELSE '' END AS fecha_afiliacion_iss, " +
						"COALESCE(to_char(cast(f5.pf_fec_pen as date), 'YYYYMMDD'), '19910101') AS fecha_pension, " +
						  "COALESCE(to_char(cast(f5.f_corte as date), 'YYYYMMDD'), to_char(CAST('"+ fechaCorte + "' AS date), 'YYYYMMDD')) AS fecha_corte_informacion, " +
							 "CASE WHEN f5.pf_mesadas_junio IS NOT NULL AND f5.pf_mesadas_junio >= 1 AND f5.pf_mesadas_dic IS NOT NULL AND f5.pf_mesadas_dic = 0 THEN 0 " +
							  "WHEN f5.pf_mesadas_junio IS NOT NULL AND f5.pf_mesadas_junio >= 1 AND(f5.pf_mesadas_dic IS NULL OR f5.pf_mesadas_dic >= 1) THEN 1 " +
							 "ELSE 0 END AS numero_mesadas_junio, " +
						"CASE WHEN f5.pf_mesadas_dic IS NOT NULL AND f5.pf_mesadas_dic >= 1 THEN 1 ELSE 0 END AS numero_mesadas_diciembre, " +
						"CASE WHEN to_char(cast(f5.pf_fec_res as date), 'YYYYMMDD') <> '' THEN to_char(cast(f5.pf_fec_res as date),'YYYYMMDD') " +
							 "ELSE to_char(cast('1991-01-01' as date),'YYYYMMDD') END AS fecha_resolucion_pension, " +
						"COALESCE(CAST(f5.pf_nro_res AS VARCHAR), '000000000000001') AS numero_resolucion_pension, " +
						"'' AS adjunto_resolucion_pension, " +
						"COALESCE(f5.pf_tipo_pension, 'V') AS tipo_pension, " +
						"COALESCE(f5.pf_pension_resolucion, 0) AS valor_mesada_resolucion, " +
						"COALESCE(f5.pf_pago_salud_razon, 'NOAP') AS razon_aportes_salud, " +
						"COALESCE(ROUND(CAST(f5.pf_pago_salud_porcentaje AS numeric), 2), 0) AS porcentaje_aportes_salud, " +
						"COALESCE(f5.pf_pago_salud_monto, 0) AS valor_aportes_salud, " +
						"COALESCE(f5.pf_pago_salud_desc_razon, 'NOAP') AS razon_aporte_salud_desc, " +
						"to_char(cast(f5.pf_fec_fallecimiento as date), 'YYYYMMDD') AS fecha_fallecimiento, " +
						"COALESCE(f5.pf_tip_doc_fallecimiento, 'RC') AS tipo_documento_fallecimiento, " +
						"CASE WHEN LENGTH(CAST(f5.pf_nro_res_fallecimiento AS VARCHAR)) > 4 THEN " +
							"SUBSTR(CAST(f5.pf_nro_res_fallecimiento AS VARCHAR), LENGTH(CAST(f5.pf_nro_res_fallecimiento AS VARCHAR)) - 3, LENGTH(CAST(f5.pf_nro_res_fallecimiento AS VARCHAR))) " +
							"ELSE COALESCE(CAST(f5.pf_nro_res_fallecimiento AS VARCHAR),'0') END AS numero_resolucion_fallecimiento, " +
						"CASE WHEN LENGTH(CAST(f5.pf_ano_res_fallecimiento AS VARCHAR)) > 4 THEN " +
									"SUBSTR(CAST(f5.pf_ano_res_fallecimiento AS VARCHAR), LENGTH(CAST(f5.pf_ano_res_fallecimiento AS VARCHAR)) - 3, LENGTH(CAST(f5.pf_ano_res_fallecimiento AS VARCHAR))) " +
									"ELSE COALESCE(CAST(f5.pf_ano_res_fallecimiento AS VARCHAR),'0') END AS anio_resolucion_fallecimiento, " +
						"f5.pf_ultima_pension AS valor_ultima_mesada, " +
						"to_char(cast(f5.pf_fec_fallecimiento as date), 'YYYYMMDD') AS fecha_ultima_mesada, " +
						"'' AS tipo_documento_registra, " +
						"'' AS numero_documento_registra, " +
						"COALESCE(to_char(f5.f_modifica, 'YYYYMMDDHH24MI'), to_char(NOW(), 'YYYYMMDDHH24MI')) AS fecha_ultima_modificacion " +
						"FROM temp_pensionadosfallecidos AS f5 " +
						"JOIN temp_reg1 AS f1 ON f5.pf_co_dane = f1.codigo_dane AND f5.pf_nro_ord = f1.codua " +
					") AS pensionados; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "SELECT COUNT(*) FROM temp_reg7;";
					cantidad = postgreSql.ConsultarDato(queryPostgrSql);
					cantidad_lineas = cantidad_lineas + Int64.Parse(cantidad);
					Console.WriteLine("Cantidad de registros R7 (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + cantidad);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error construyendo R7. {0}", ex.Message));
			}
		}

		public void InformacionR8(String pc_datos_ultenvio, PostgreSql postgreSql)
		{
			try
			{
				String queryPostgrSql = string.Empty;
				DataTable dataTable = new DataTable();

				try
				{
					queryPostgrSql = "DROP TABLE IF EXISTS temp_reg8; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "CREATE TEMP TABLE temp_reg8 AS "+
					"SELECT 8 AS tipo_registro, "+
					"LPAD(COALESCE(F6.be_co_dane, 'DANE'), 5, '0') || '-' || LPAD(COALESCE(F6.be_nro_ord, '01'), 5, '0') || '-' || 'BE' || '-' || SUBSTRING(LPAD(CAST(COALESCE(F6.be_id, '01') AS TEXT), 5, '0') from 1 for 5) AS numero_certificacion, " +
					"'I' AS tipo_informacion, " +
					"f1.nit_entidad AS nit_empleador, " +
					"f1.seccional AS seccional, " +
					"f1.nombre_entidad AS nombre_empleador, " +
					"f6.be_tip_doc AS tipo_documento, " +
					"f6.be_nro_doc AS numero_documento, " +
					"f6.be_sexo AS genero, " +
					"COALESCE(to_char(cast(f6.be_fec_nac as date), 'YYYYMMDD'), '19500101') AS fecha_nacimiento, " +
					  "f6.be_tip_ben AS tipo_beneficiario, " +
					"CASE WHEN f6.be_est_ben IS TRUE THEN 'S' ELSE 'N' END AS estado_invalidez, " +
					"'' AS fecha_resolucion_sustitucion_pension, " +
					"'' AS numero_resolucion_sustitucion_pension, " +
					"'' AS adjunto_resolucion_sustitucion_pension, " +
					"f6.be_pension_resolucion AS valor_mesada_resolucion, " +
					"f6.be_directo AS pension_directa, " +
					"f6.be_pension AS valor_mesada, " +
					"ROUND(CAST(f6.be_por_pen AS numeric), 4) AS porcentaje_pension_cargo, " +
					 "f6.be_tip_mue AS tipo_documento_causante, " +
					"f6.be_doc_mue AS numero_documento_causante, " +
					"COALESCE(to_char(cast(f6.f_corte as date), 'YYYYMMDD'), to_char(CAST('"+ fechaCorte+"' AS date), 'YYYYMMDD')) AS fecha_corte_informacion, " +
					   "CASE WHEN f6.be_mesadas_junio IS NOT NULL AND f6.be_mesadas_junio >= 1 AND f6.be_mesadas_dic IS NOT NULL AND f6.be_mesadas_dic = 0 THEN 0 " +
						  "WHEN f6.be_mesadas_junio IS NOT NULL AND f6.be_mesadas_junio >= 1 AND(f6.be_mesadas_dic IS NULL OR f6.be_mesadas_dic >= 1) THEN 1 " +
						 "ELSE 0 END AS numero_mesadas_junio, " +
					"CASE WHEN f6.be_mesadas_dic IS NOT NULL AND f6.be_mesadas_dic >= 1 THEN 1 ELSE 0 END AS numero_mesadas_diciembre, " +
					"CASE WHEN f6.be_parentesco IN('C','H','M','P','I') THEN f6.be_parentesco ELSE 'I' END AS parentesco, " +
					"ROUND(CAST(f6.be_porc_benef AS numeric), 2) AS porcentaje_pension_sustituto, " +
					"COALESCE(f6.be_pago_salud_razon, 'NOAP') AS razon_aporte_salud, " +
					"COALESCE(ROUND(CAST(f6.be_pago_salud_porcentaje AS numeric), 2), 0) AS porcentaje_aportes_salud, " +
					  "COALESCE(f6.be_pago_salud_monto, 0) AS valor_aportes_salud, " +
					   "f6.be_pago_salud_desc_razon AS razon_aporte_salud_desc, " +
					"CASE WHEN f6.be_com_iss IS TRUE THEN 'S' ELSE 'N' END AS pension_compartida_iss, " +
					"CASE WHEN f6.be_hijos_fallecido IS TRUE THEN 'S' ELSE 'N' END AS hijos_con_fallecido, " +
					"'' AS tipo_documento_registra, " +
					"'' AS numero_documento_registra, " +
					"COALESCE(to_char(f6.f_modifica, 'YYYYMMDDHH24MI'), to_char(NOW(), 'YYYYMMDDHH24MI')) AS fecha_ultima_modificacion " +
					"FROM temp_sustitutos AS f6 " +
					"JOIN temp_reg1 AS f1 ON f6.be_co_dane = f1.codigo_dane AND f6.be_nro_ord = f1.codua; ";
					postgreSql.EjecutarQuery(queryPostgrSql);

					queryPostgrSql = "SELECT COUNT(*) FROM temp_reg8;";
					cantidad = postgreSql.ConsultarDato(queryPostgrSql);
					cantidad_lineas = cantidad_lineas + Int64.Parse(cantidad);
					Console.WriteLine("Cantidad de registros R8 (PostgreSql - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + cantidad);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error construyendo R8. {0}", ex.Message));
			}
		}

		public Boolean EscribirArchivo(String pathArchivo, PostgreSql postgreSql)
		{
			try
			{
				Boolean Generado = false;
				String queryPostgrSql = string.Empty;
				DataTable dataTable = new DataTable();

				try
				{
					queryPostgrSql = "SELECT REPLACE(concat(tipo_registro,'_;_',nit_entidad,'_;_',REGEXP_REPLACE(nombre_entidad,'^\\s+|\\s+$','','g'),'_;_',REGEXP_REPLACE(seccional,'^\\s+|\\s+$','','g'),'_;_',codigoPasivocol,'_;_',REGEXP_REPLACE(tipo_entidad,'^\\s+|\\s+$','','g'),'_;_', " +
					"REGEXP_REPLACE(tipo_empleador, '^\\s+|\\s+$', '', 'g'),'_;_',to_char(CAST('" + fechaCorte + "' AS date), 'YYYYMMDD'),'_;_'," + cantidad_registros2 + ",'_;_'," + cantidad_lineas + ",'_;_', CASE WHEN fecha_creacion IS NULL THEN '" + fechacreacion_central + "' ELSE fecha_creacion END,'_;_',CASE WHEN fecha_afiliacion_iss IS NULL THEN '" + fechaiss_central + "' ELSE fecha_afiliacion_iss END,'_;_', " +
					"CASE WHEN fecha_vigencia_sgp IS NULL THEN '" + fechasgp_central + "' ELSE fecha_vigencia_sgp END,'_;_',codigo_dane,'_;_',REGEXP_REPLACE(nombre_representante_legal, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(cargo_representante_legal, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(email_representante_legal, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(nombre_coordinador_pasivocol, '^\\s+|\\s+$', '', 'g'),'_;_', " +
					"cargo_coordinador_pasivocol,'_;_',REGEXP_REPLACE(telefono_coordinador_pasivocol, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(email_coordinador_pasivocol, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(direccion_entidad, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(nombre_ciudad, '^\\s+|\\s+$', '', 'g'),'_;_', " +
					"REGEXP_REPLACE(telefonos, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(pagina_web, '^\\s+|\\s+$', '', 'g'),'_;_',CASE WHEN fecha_creacion IS NULL THEN '" + fechacreacion_central + "' ELSE fecha_creacion END,'_;_',tipo_numero_acto_administrativo,'_;_',estado_entidad,'_;_',fecha_liquidacion_entidad,'_;_',numero_liquidacion_entidad,'_;_', " +
					"codigo_contaduria,'_;_',CASE WHEN fecha_actualizacion IS NULL THEN '" + fechaactualizacion_central + "' ELSE fecha_actualizacion END,'_;_',estado_revision),'\"','') from temp_reg1 " +
                    "UNION ALL " +//*R2*
                    "SELECT CONCAT(tipo_registro,'_;_',tipo_documento,'_;_',documento,'_;_',REGEXP_REPLACE(primer_nombre, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(segundo_nombre, '^\\s+|\\s+$', '', 'g'),'_;_', " +
                    "REGEXP_REPLACE(primer_apellido, '^\\s+|\\s+$', '', 'g'),'_;_',REGEXP_REPLACE(segundo_apellido, '^\\s+|\\s+$', '', 'g'),'_;_',vigencia_documento,'_;_',fecha_actualizacion_rnec,'_;_',fecha_fallecimiento_minsalud,'_;_', " +
                    "fecha_primera_afiliacion_sgp,'_;_',codigo_regimen_primera_afiliacion,'_;_',tasa_bono,'_;_',tipo_depuracion,'_;_',numero_resolucion_fallecimiento,'_;_', " +
                    "TRIM(anio_resolucion_fallecimiento),'_;_',documento_homonimo,'_;_',descripcion_depuracion,'_;_',valor_bono,'_;_',fecha_pago_bono,'_;_', " +
                    "numero_acto_admin_pago_bono,'_;_',anio_acto_admin_pago_bono,'_;_',numero_contrato_ops,'_;_',numero_resolucion_nombramiento_docente,'_;_', " +
                    "anio_resolucion_nombramiento_docente,'_;_',fecha_depuracion,'_;_',depurado_historia_laboral,'_;_',depurado_pensionado,'_;_',depurado_sustitutos,'_;_', " +
                    "numero_fallo_judicial_demanda,'_;_',anio_fallo_judicial,'_;_',valor_conmitado_pagado_cuota_parte_futura,'_;_', " +
                    "nit_entidad_receptora_conmut_pago_cuota_parte_futura,'_;_',acto_administrativo_depuracion,'_;_',anio_administrativo_depuracion,'_;_', " +
                    "archivo,'_;_',numero_documento_usuario_registra,'_;_',fecha_ultima_modificacion,'_;_',salario_base_bono_pensional,'_;_', " +
                    "CASE WHEN SUBSTRING(CAST(fecha_salario_base_bono_pensional AS TEXT) from 1 for 1) = '0' " +
                    "THEN regexp_replace(CAST(fecha_salario_base_bono_pensional AS TEXT),'0','1', 'i') " +
                    "ELSE fecha_salario_base_bono_pensional END ,'_;_',salario_base_bono_pensional_calculado) from temp_reg2 " +
                    "UNION ALL " +//R4*/
                    "SELECT concat(tipo_registro, '_;_', nit_certificadora, '_;_', seccional_certificadora, '_;_', nombre_certificadora, '_;_', numero_certificacion, '_;_', " +
                    "tipo_informacion, '_;_', nit_empleador, '_;_', seccional_empleador, '_;_', REGEXP_REPLACE(nombre_empleador, '^\\s+|\\s+$', '', 'g'), '_;_', sector_seccional_empleador, '_;_', naturaleza_empleador, '_;_', " +
                    "sub_sector_entidad_territorial, '_;_', fecha_certificacion, '_;_', tipo_documento_beneficiario, '_;_', documento_beneficiario, '_;_', REGEXP_REPLACE(primer_nombre_beneficiario, '^\\s+|\\s+$', '', 'g'), '_;_', " +
                    "REGEXP_REPLACE(segundo_nombre_beneficiario, '^\\s+|\\s+$', '', 'g'), '_;_', REGEXP_REPLACE(primer_apellido_beneficiario, '^\\s+|\\s+$', '', 'g'), '_;_', REGEXP_REPLACE(segundo_apellido_beneficiario, '^\\s+|\\s+$', '', 'g'), '_;_', genero, '_;_', fecha_nacimiento, '_;_', " +
                    "tipo_documento_alterno_beneficiario, '_;_', documento_alterno_beneficiario, '_;_', primer_nombre_alterno, '_;_', segundo_nombre_alterno, '_;_', primer_apellido_alterno, '_;_', " +
                    "segundo_apellido_alterno, '_;_', tipo_indicio, '_;_', tipo_documento_funcionario, '_;_', documento_funcionario, '_;_', primer_nombre_funcionario, '_;_', " +
                    "segundo_nombre_funcionario, '_;_', primer_apellido_funcionario, '_;_', segundo_apellido_funcionario, '_;_', cargo_funcionario, '_;_', tipo_documento_usuario, '_;_', numero_documento_usuario, '_;_', " +
                    "fecha_ultima_modificacion) from temp_reg4 " +
                    "UNION ALL " +//*R5*/
                    "SELECT concat(tipo_registro, '_;_', numero_certificacion, '_;_', tipo_informacion, '_;_', tipo_novedad, '_;_', fecha_inicial, '_;_', " +
                    "fecha_final, '_;_', cargo, '_;_', realizo_aportes, '_;_', nit_fondo_aportes, '_;_', nombre_fondo_aportes, '_;_', horas_laboradas, '_;_', dias_interrupcion, '_;_', " +
                    "fuente_recursos, '_;_', nit_establecimiento, '_;_', nombre_establecimiento, '_;_', nivel_establecimiento, '_;_', municipio, '_;_', factor_aportes, '_;_', sesiones_asistidas, '_;_', sesiones_no_asistidas, '_;_', " +
                    "total_sesiones) from temp_reg5 " +
                    "UNION ALL " +//*R6*/
                    "select concat(tipo_registro, '_;_', numero_certificacion, '_;_', tipo_informacion, '_;_', periodo, '_;_', salario_integral, '_;_', factor_salarial, '_;_', " +
                    "valor_devengado, '_;_', realizo_aportes, '_;_', periodicidad_factor, '_;_', fecha_inicial_causacion, '_;_', fecha_final_causacion) from temp_reg6 " +
                    "UNION ALL " +//*R7*/
                    "SELECT concat(tipo_registro, '_;_', numero_certificacion, '_;_', tipo_informacion, '_;_', nit_empleador, '_;_', seccional, '_;_', nombre_empleador, '_;_', tipo_documento, '_;_', numero_documento, '_;_', " +
                    "genero, '_;_', estado_civil, '_;_', " +
                    "CASE WHEN CAST(SUBSTRING(CAST(fecha_nacimiento AS TEXT) from 1 for 4) AS INT) < 1900 THEN CONCAT('1901',SUBSTRING(CAST(fecha_nacimiento AS TEXT) from 5 for 4)) ELSE fecha_nacimiento END,'_;_', " +
                    "CASE WHEN CAST(SUBSTRING(CAST(fecha_nacimiento_conyuge AS TEXT) from 1 for 4) AS INT) < 1900 THEN CONCAT('1901',SUBSTRING(CAST(fecha_nacimiento_conyuge AS TEXT) from 5 for 4)) ELSE CAST(fecha_nacimiento_conyuge AS TEXT) END,'_;_', " +
                    "genero_conyuge,'_;_',estado_invalidez,'_;_',pension_directa,'_;_',valor_mesada,'_;_',pension_compartida_iss,'_;_',pension_futura_compartida_iss,'_;_', " +
                    "porcentaje_pension_cargo,'_;_',fecha_afiliacion_iss,'_;_',fecha_pension,'_;_',fecha_corte_informacion,'_;_',numero_mesadas_junio,'_;_',numero_mesadas_diciembre,'_;_', " +
                    "fecha_resolucion_pension,'_;_',numero_resolucion_pension,'_;_',adjunto_resolucion_pension,'_;_',tipo_pension,'_;_',valor_mesada_resolucion,'_;_',razon_aportes_salud,'_;_', " +
                    "porcentaje_aportes_salud,'_;_',valor_aportes_salud,'_;_',razon_aporte_salud_desc,'_;_', " +
                    "CASE WHEN fecha_fallecimiento <> '' THEN " +
                    "CASE WHEN CAST(SUBSTRING(CAST(fecha_fallecimiento AS TEXT) from 1 for 4) AS INT) < 1950 THEN CAST(fecha_pension AS TEXT) ELSE CAST(fecha_fallecimiento AS TEXT) END " +
                    "ELSE fecha_fallecimiento END,'_;_', " +
                    "tipo_documento_fallecimiento,'_;_',numero_resolucion_fallecimiento,'_;_',anio_resolucion_fallecimiento,'_;_',valor_ultima_mesada,'_;_', " +
                    "CASE WHEN fecha_ultima_mesada <> '' THEN " +
                    "CASE WHEN CAST(SUBSTRING(CAST(fecha_ultima_mesada AS TEXT) from 1 for 4) AS INT) < 1950 THEN CAST(fecha_pension AS TEXT) ELSE CAST(fecha_ultima_mesada AS TEXT) END " +
                    "ELSE fecha_ultima_mesada END,'_;_', " +
                    "tipo_documento_registra,'_;_',numero_documento_registra,'_;_',fecha_ultima_modificacion) from temp_reg7 " +
                    "UNION ALL " +//*R8*/
                    "SELECT concat(tipo_registro, '_;_', numero_certificacion, '_;_', tipo_informacion, '_;_', nit_empleador, '_;_', seccional, '_;_', nombre_empleador, '_;_', tipo_documento, '_;_', numero_documento, '_;_', " +
                    "genero, '_;_', fecha_nacimiento, '_;_', tipo_beneficiario, '_;_', estado_invalidez, '_;_', fecha_resolucion_sustitucion_pension, '_;_', numero_resolucion_sustitucion_pension, '_;_', " +
                    "adjunto_resolucion_sustitucion_pension, '_;_', valor_mesada_resolucion, '_;_', pension_directa, '_;_', valor_mesada, '_;_', porcentaje_pension_cargo, '_;_', " +
                    "tipo_documento_causante, '_;_', numero_documento_causante, '_;_', fecha_corte_informacion, '_;_', numero_mesadas_junio, '_;_', numero_mesadas_diciembre, '_;_', " +
                    "parentesco, '_;_', porcentaje_pension_sustituto, '_;_', razon_aporte_salud, '_;_', porcentaje_aportes_salud, '_;_', valor_aportes_salud, '_;_', " +
                    "razon_aporte_salud_desc, '_;_', pension_compartida_iss, '_;_', hijos_con_fallecido, '_;_', tipo_documento_registra, '_;_', numero_documento_registra, '_;_', fecha_ultima_modificacion) from temp_reg8; ";
                    dataTable = postgreSql.ConsultarDatos(queryPostgrSql);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}

				if (dataTable.Rows.Count > 0)
				{
					using (FileStream fs = new FileStream(pathArchivo, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
					{
						using (StreamWriter sw = new StreamWriter(fs))
						{
							foreach (DataRow row in dataTable.Rows)
							{
								sw.WriteLine(row[0]);
							}
						}
					}
					Console.WriteLine("Archivo generado (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "): " + pathArchivo);
					Generado = true;
				}

				return Generado;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error generando archivo. {0}", ex.Message));
			}
		}

		private DateTime StringToDatetime(string strFecha)
		{
			string[] formatos = new[] { "dd-MM-yyyy", "dd/MM/yyyy", "yyyyMMdd"};
			DateTime fechaValida;
			DateTime.TryParseExact(strFecha, formatos, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fechaValida);
			return fechaValida;
		}
	}
}
