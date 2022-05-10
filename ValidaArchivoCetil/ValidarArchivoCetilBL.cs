namespace MHCP.DGT.SUPPT.ValidaArchivoCetil.Program
{
    using Npgsql;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Proceso que valida estructura del archivo generado antes de disponerlo en SUPPT para su procesamiento
    /// </summary>
    public  class ValidarArchivoCetilBL
    {
        #region OBJETOS PRIVADOS
        private static string pathFileServer;
        private static string userFileServer;
        private static string passFileServer;
        private static string pathFileLocal;
        private static string pathBorrador;
        private static string pathExito;
        private static string pathError;

        private static string IpServerPostgres;
        private static string DbServerPostgres;
        private static string UserServerPostgres;
        private static string PassServerPostgres;
        

        private static string[] NoObligaTipo1 = { "24", "28", "29", "30", "32" };
        private static string[] NoObligaTipo2 = { "5", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17",
                                                    "18", "19", "20", "21", "22", "23", "24", "25", "26", "30", "31", "32",
                                                    "33", "34", "35", "36", "37", "39", "40", "41" };
        private static string[] NoObligaTipo3 = { "6" };
        private static string[] NoObligaTipo4 = { "10", "11", "12", "17", "19", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33",
                                                    "34", "35", "36", "37"};
        private static string[] NoObligaTipo5 = { "6", "7", "8", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21" };
        private static string[] NobligaTipo6 = { "8", "9", "10", "11" };
        private static string[] NobligaTipo7 = { "27", "34", "35", "36", "37", "38", "39", "40", "41" };
        private static string[] NobligaTipo8 = { "13", "14", "15", "33", "34" };
        private static string[] NobligaTipo9 = { "7", "8" };
        private static string[] NoObligaTipo10 = { "4", "5", "7", "8", "9", "10", "11", "12", "13" };

        private static string[] obliganTipo2 = { "7", "8" };

        private static int[] fechasTipo1 = { 6, 9, 10, 11, 24, 27 };
        private static int[] fechasHorasTipo1 = { 30 };
        private static int[] fechasTipo2 = { 8, 9, 10, 19, 39 };
        private static int[] fechasHorasTipo2 = { 25, 37 };
        private static int[] fechasTipo3 = { 5, 6 };
        private static int[] fechasHorasTipo3 = { };
        private static int[] fechasTipo4 = { 12, 20 };
        private static int[] fechasHorasTipo4 = { 37 };
        private static int[] fechasTipo5 = { 4, 5 };
        private static int[] fechasHorasTipo5 = { };
        private static int[] fechasTipo6 = { 9, 10 };//el index o columna 3 para este tipo es YYYYMM (Periodo)
        private static int[] fechasHorasTipo6 = { };
        private static int[] fechasTipo7 = { 10, 11, 19, 20, 21, 24, 33, 38 };
        private static int[] fechasHorasTipo7 = { 41 };
        private static int[] fechasTipo8 = { 9, 12, 21 };
        private static int[] fechasHorasTipo8 = { 34 };
        private static int[] fechasTipo9 = { };
        private static int[] fechasHorasTipo9 = { 7 };
        private static int[] fechasTipo10 = { };
        private static int[] fechasHorasTipo10 = { 12 };

        private static List<string[]> _tipo2 = new List<string[]>();
        private static List<string[]> _tipo3 = new List<string[]>();
        private static List<string[]> _tipo4 = new List<string[]>();
        private static List<string[]> _tipo5 = new List<string[]>();
        private static List<string[]> _tipo7 = new List<string[]>();
        private static List<string[]> _tipo8 = new List<string[]>();

        private static Dictionary<string, string[]> obliga;
        private static Dictionary<string, int[]> fechas;
        private static Dictionary<string, int[]> fechashoras;
        private static Dictionary<string, int> estructura;
        private static Dictionary<string, Dictionary<int, string[]>> parametricas;

        private string[] lines = new string[] { "descripcion;linea;columna;tipo" };
        private List<string> listLines = new List<string>();
        private List<string> listLinesCalculos = new List<string>();

        private int ContadorActivos;
        private int ContadorRetirados;
        private int ContadorPensionados;
        private int ContadorPensionadosFallecidos;
        private int ContadorSustitutos;
        private int ContadorSustitutosDepurados;
        private int ContadorPensionadosDepurados;
        private int ContadorSustitutosSupuestos;

        private string nitTipo1 = "";
        private string cedulasActivos = "";
        private string cedulasRetirados = "";

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidarArchivoCetilBL()
        {
            NetworkShare.ConnectToShare(pathFileServer, userFileServer, passFileServer); //Conecta a FileServer SUPPT

            pathFileServer = Ajustes.Default.PATH_FILE_SERVER;
            pathFileLocal = Ajustes.Default.PATH_FILE_LOCAL;
            pathBorrador = string.Concat(pathFileLocal, Ajustes.Default.DIRECTORIO_EXTRACCION);
            pathExito = string.Concat(pathFileLocal, Ajustes.Default.DIRECTORIO_ENVIAR_A_SUPPT);
            pathError = string.Concat(pathFileLocal, Ajustes.Default.DIRECTORIO_ERROR);
            userFileServer = Ajustes.Default.USERNAME_FILESERVER;
            passFileServer = Ajustes.Default.PASSWORD_FILESERVER;

            IpServerPostgres = Ajustes.Default.URL_SERVER_POSTGRES;
            DbServerPostgres = Ajustes.Default.DB_SERVER_POSTGRES;
            UserServerPostgres = Ajustes.Default.USER_SERVER_POSTGRES;
            PassServerPostgres = Ajustes.Default.PASS_SERVER_POSTGRES;

            obliga = new Dictionary<string, string[]>();
            obliga.Add("obligaTipo1", NoObligaTipo1);
            obliga.Add("obligaTipo2", NoObligaTipo2);
            obliga.Add("obligaTipo3", NoObligaTipo3);
            obliga.Add("obligaTipo4", NoObligaTipo4);
            obliga.Add("obligaTipo5", NoObligaTipo5);
            obliga.Add("obligaTipo6", NobligaTipo6);
            obliga.Add("obligaTipo7", NobligaTipo7);
            obliga.Add("obligaTipo8", NobligaTipo8);
            obliga.Add("obligaTipo9", NobligaTipo9);
            obliga.Add("obligaTipo10", NoObligaTipo10);

            estructura = new Dictionary<string, int>();
            estructura.Add("1", 32);
            estructura.Add("2", 41);
            estructura.Add("3", 7);
            estructura.Add("4", 38);
            estructura.Add("5", 21);
            estructura.Add("6", 11);
            estructura.Add("7", 42);
            estructura.Add("8", 35);
            estructura.Add("9", 8);
            estructura.Add("10", 12);

            fechas = new Dictionary<string, int[]>();
            fechas.Add("fechasTipo1", fechasTipo1);
            fechas.Add("fechasTipo2", fechasTipo2);
            fechas.Add("fechasTipo3", fechasTipo3);
            fechas.Add("fechasTipo4", fechasTipo4);
            fechas.Add("fechasTipo5", fechasTipo5);
            fechas.Add("fechasTipo6", fechasTipo6);
            fechas.Add("fechasTipo7", fechasTipo7);
            fechas.Add("fechasTipo8", fechasTipo8);
            fechas.Add("fechasTipo9", fechasTipo9);
            fechas.Add("fechasTipo10", fechasTipo10);

            fechashoras = new Dictionary<string, int[]>();
            fechashoras.Add("fechasHorasTipo1", fechasHorasTipo1);
            fechashoras.Add("fechasHorasTipo2", fechasHorasTipo2);
            fechashoras.Add("fechasHorasTipo3", fechasHorasTipo3);
            fechashoras.Add("fechasHorasTipo4", fechasHorasTipo4);
            fechashoras.Add("fechasHorasTipo5", fechasHorasTipo5);
            fechashoras.Add("fechasHorasTipo6", fechasHorasTipo6);
            fechashoras.Add("fechasHorasTipo7", fechasHorasTipo7);
            fechashoras.Add("fechasHorasTipo8", fechasHorasTipo8);
            fechashoras.Add("fechasHorasTipo9", fechasHorasTipo9);
            fechashoras.Add("fechasHorasTipo10", fechasHorasTipo10);

            parametricas = new Dictionary<string, Dictionary<int, string[]>>();
            //Parametricas R1
            Dictionary<int, string[]> pTipo1 = new Dictionary<int, string[]>();
            pTipo1.Add(5, new string[] { "C", "D" });
            pTipo1.Add(6, new string[] { "CEN", "HSP", "SPB", "OTR" });
            pTipo1.Add(15, new string[] { "ALC", "GOB", "GTE" });
            pTipo1.Add(27, new string[] { "ACT", "ELQ", "LIQ" });
            pTipo1.Add(32, new string[] { "R", "A" });
            parametricas.Add("1", pTipo1);
            //Parametricas R2
            Dictionary<int, string[]> pTipo2 = new Dictionary<int, string[]>();
            pTipo2.Add(2, new string[] { "C", "E", "P", "T", "R", "O" });
            pTipo2.Add(8, new string[] { "00", "01", "12", "14", "21", "22", "23", "24", "25", "26", "27", "28", "51",
                "52", "53", "54", "55", "56", "88" });
            pTipo2.Add(12, new string[] { "COLP", "RAIS", "OTRO" });
            pTipo2.Add(14, new string[] { "BON", "CNM", "CPF", "DEM", "DOC", "FAL", "FIN", "HOM", "IND", "NOC", "OPS",
                "SBF", "POE", "CND" });
            pTipo2.Add(27, new string[] { "SI", "NO" });
            pTipo2.Add(28, new string[] { "SI", "NO" });
            pTipo2.Add(29, new string[] { "SI", "NO" });
            parametricas.Add("2", pTipo2);
            //Parametricas R3
            Dictionary<int, string[]> pTipo3 = new Dictionary<int, string[]>();
            pTipo3.Add(2, new string[] { "C", "E", "P", "T", "R", "O" });
            parametricas.Add("3", pTipo3);
            //Parametricas R4
            Dictionary<int, string[]> pTipo4 = new Dictionary<int, string[]>();
            pTipo4.Add(6, new string[] { "C", "I" });
            pTipo4.Add(10, new string[] { "SA", "ED", "OS" });
            pTipo4.Add(11, new string[] { "PUBLICO NACIONAL", "PUBLICO DEPARTAMENTAL", "PUBLICO DISTRITAL", "PUBLICO MUNICIPAL",
                "PRIVADO", "MIXTO", "SIN INFORMACION" });
            pTipo4.Add(12, new string[] { "ED_AMAG", "ED_ENTI", "SA_CONC", "SA_ENT", "O_OTRO" });
            pTipo4.Add(14, new string[] { "C", "E", "P", "T", "R", "O" });
            pTipo4.Add(20, new string[] { "F", "M" });
            pTipo4.Add(22, new string[] { "C", "E", "P", "T", "R" });
            pTipo4.Add(28, new string[] { "1", "2", "3", "4", "5" });
            pTipo4.Add(29, new string[] { "C", "E", "P", "T", "R" });
            pTipo4.Add(36, new string[] { "C", "E", "P", "T", "R" });
            parametricas.Add("4", pTipo4);
            //Parametricas R5
            Dictionary<int, string[]> pTipo5 = new Dictionary<int, string[]>();
            pTipo5.Add(3, new string[] { "C", "I" });
            pTipo5.Add(4, new string[] { "LABORAL", "LICENCIA" });
            pTipo5.Add(7, new string[] { "100", "110", "120", "130", "140", "150", "160", "170", "180", "190", "200", "210", "220",
                "400", "410", "420", "430", "440", "450", "460", "470", "480", "490", "500", "510", "520", "530", "540", "550",
                "560", "570", "580", "590", "600", "610", "620", "630", "640", "650", "660", "670", "680", "690", "700", "710", "720" });
            pTipo5.Add(8, new string[] { "S", "N" });
            pTipo5.Add(11, new string[] { "1", "2", "3", "4", "5", "6", "7", "8" });
            pTipo5.Add(13, new string[] { "10", "20", "30" });
            pTipo5.Add(16, new string[] { "10", "20", "30", "40", "50" });
            pTipo5.Add(18, new string[] { "10", "20", "30", "40" });
            pTipo5.Add(19, new string[] { "700", "710", "720" });
            pTipo5.Add(20, new string[] { "700", "710", "720" });
            pTipo5.Add(21, new string[] { "700", "710", "720" });
            parametricas.Add("5", pTipo5);
            //Parametricas R6
            Dictionary<int, string[]> pTipo6 = new Dictionary<int, string[]>();
            pTipo6.Add(3, new string[] { "C", "I" });
            pTipo6.Add(5, new string[] { "S", "N" });
            pTipo6.Add(6, new string[] { "10", "20", "30", "40", "50", "60", "70", "80", "90", "100", "110", "120", "130", "140", "150", "160", "170", "180", "190", "9999",
                "1010", "1020", "1030", "1040", "1050", "1060", "1070", "1080", "1090", "1100", "1110", "1120", "1130", "1140", "1150", "1160", "1170", "1180", "1190", "1200",
                "2000", "2010", "2020", "2030", "2040", "2050", "2060", "2070", "2080", "2090", "2100", "2110", "2120", "2130", "2140", "2150", "2160", "2170", "2180", "2190",
                "2200", "2210", "2220", "2230", "2240", "2250", "2260", "2270" });
            pTipo6.Add(8, new string[] { "S", "N" });
            pTipo6.Add(9, new string[] { "MENSUAL", "BIMENSUAL", "TRIMESTRAL", "SEMESTRAL", "ANUAL", "QUINQUENAL" });
            parametricas.Add("6", pTipo6);
            //Parametricas R7
            Dictionary<int, string[]> pTipo7 = new Dictionary<int, string[]>();
            pTipo7.Add(3, new string[] { "C", "I" });
            pTipo7.Add(7, new string[] { "C", "E", "P", "T", "R", "O" });
            pTipo7.Add(9, new string[] { "F", "M" });
            pTipo7.Add(10, new string[] { "C", "S", "V", "L", "D" });
            pTipo7.Add(13, new string[] { "F", "M" });
            pTipo7.Add(14, new string[] { "S", "N" });
            pTipo7.Add(15, new string[] { "D", "T" });
            pTipo7.Add(17, new string[] { "S", "N" });
            pTipo7.Add(18, new string[] { "S", "N" });
            pTipo7.Add(23, new string[] { "1", "0" });
            pTipo7.Add(24, new string[] { "1", "0" });
            pTipo7.Add(28, new string[] { "I", "V" });
            pTipo7.Add(30, new string[] { "L100", "OTRO", "NOAP" });
            pTipo7.Add(35, new string[] { "PW", "RC", "MS" });
            pTipo7.Add(40, new string[] { "C", "E", "P", "T", "R" });
            parametricas.Add("7", pTipo7);
            //Parametricas R8
            Dictionary<int, string[]> pTipo8 = new Dictionary<int, string[]>();
            pTipo8.Add(3, new string[] { "C", "I" });
            pTipo8.Add(7, new string[] { "C", "E", "P", "T", "R", "O" });
            pTipo8.Add(9, new string[] { "F", "M" });
            pTipo8.Add(11, new string[] { "V", "T" });
            pTipo8.Add(12, new string[] { "S", "N" });
            pTipo8.Add(17, new string[] { "D", "T" });
            pTipo8.Add(20, new string[] { "C", "E", "T" });
            pTipo8.Add(23, new string[] { "1", "0" });
            pTipo8.Add(24, new string[] { "1", "0" });
            pTipo8.Add(25, new string[] { "C", "H", "M", "P", "I" });
            pTipo8.Add(27, new string[] { "L100", "OTRO", "NOAP" });
            pTipo8.Add(31, new string[] { "S", "N" });
            pTipo8.Add(32, new string[] { "S", "N" });
            pTipo8.Add(33, new string[] { "C", "E", "P", "T" });
            parametricas.Add("8", pTipo8);
            //Parametricas R9
            Dictionary<int, string[]> pTipo9 = new Dictionary<int, string[]>();
            pTipo9.Add(2, new string[] { "C", "E", "P", "T", "R", "O" });
            pTipo9.Add(4, new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09" });
            parametricas.Add("9", pTipo9);
            //Parametricas R10
            Dictionary<int, string[]> pTipo10 = new Dictionary<int, string[]>();
            pTipo10.Add(6, new string[] { "C", "I" });
            pTipo10.Add(10, new string[] { "C", "E", "P", "T" });
            parametricas.Add("10", pTipo10);
        }

        /// <summary>
        /// Valida un archivo
        /// </summary>
        public void Validar()
        {
            try
            {
                // 1. OBTIENE NOMBRE DEL ARCHIVO A PROCESAR DESDE CARPETA LOCAL
                string _NombreArchivo = ObtenerNombreArchivo();

                if (!string.IsNullOrEmpty(_NombreArchivo))
                {
                    List<string[]>  _InfoArchivo = ObtenerInfoArchivo(_NombreArchivo);

                    if (_InfoArchivo != null && _InfoArchivo.Count > 0)
                    {
                        ValidarArchivo(_InfoArchivo, _NombreArchivo);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene nombre del archivo a procesar
        /// </summary>
        /// <returns></returns>
        private string ObtenerNombreArchivo()
        {
            string resultado = string.Empty;

            try
            {
                DirectoryInfo directorySelected = new DirectoryInfo(pathBorrador);
                FileInfo[] files = directorySelected.GetFiles().OrderBy(p => p.CreationTime).ToArray();

                if (files != null && files.Length > 0)
                {
                    resultado = files[0].Name;
                }

                //foreach (FileInfo file in files)
                //{
                //    // DO Something...
                //}

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene lista de arreglos con información de cada linea del achivo
        /// </summary>
        /// <param name="pNombreArchivo"></param>
        /// <returns></returns>
        private List<string[]> ObtenerInfoArchivo(string pNombreArchivo)
        {
            List<string[]> resultado = new List<string[]>();
            try
            {
                FileInfo file = new FileInfo(string.Concat(pathBorrador, "\\", pNombreArchivo));
                string[] seperators = { "_;_" };

                using (StreamReader sr = file.OpenText())
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] items = s.Split(seperators, StringSplitOptions.None);
                        resultado.Add(items);
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Valida archivo
        /// </summary>
        /// <param name="pInfoArchivo"></param>
        private void ValidarArchivo(List<string[]> pInfoArchivo, string nombreArchivo)
        {
            try
            {
                #region VALIDACIONES

                int cont = 0;
                bool exitoso = false;

                _tipo2 = new List<string[]>();
                _tipo2.AddRange(pInfoArchivo.Where(x => x[0] == "2").ToList());
                _tipo3 = new List<string[]>();
                _tipo3.AddRange(pInfoArchivo.Where(x => x[0] == "3").ToList());
                _tipo4 = new List<string[]>();
                _tipo4.AddRange(pInfoArchivo.Where(x => x[0] == "4").ToList());
                _tipo5 = new List<string[]>();
                _tipo5.AddRange(pInfoArchivo.Where(x => x[0] == "5").ToList());
                _tipo7 = new List<string[]>();
                _tipo7.AddRange(pInfoArchivo.Where(x => x[0] == "7").ToList());
                _tipo8 = new List<string[]>();
                _tipo8.AddRange(pInfoArchivo.Where(x => x[0] == "8").ToList());

                nitTipo1 = pInfoArchivo[0][1];

                //Valida estructura, obligatoriedad de campos y campos parametrizados
                foreach (string[] itemList in pInfoArchivo)
                {
                    cont++;
                    string _obligaKey = string.Concat("obligaTipo", itemList[0]);
                    string _fechaKey = string.Concat("fechasTipo", itemList[0]);
                    string _fechahoraKey = string.Concat("fechasHorasTipo", itemList[0]);

                    int valEstructura = 0;
                    estructura.TryGetValue(itemList[0], out valEstructura);

                    if (valEstructura > 0) //Si encuentra estructura definida para el tipo de archivo
                    {
                        if (itemList.Length != valEstructura) //Si el registro no tiene la estructura adecuada
                        {
                            listLines.Add(string.Concat("Estructura Incorrecta. Número de campos no coinciden.;"
                                                , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                                                , string.Format("R{0}", itemList[0])));
                        }
                        else //Si el registro tiene la estructura adecuada
                        {
                            //Valida datos parametricos
                            Dictionary<int, string[]> itemParametrica;
                            parametricas.TryGetValue(itemList[0], out itemParametrica);

                            if (itemParametrica != null)
                            {
                                foreach (var item in itemParametrica)
                                {
                                    string valIni = itemList[item.Key - 1].Trim();

                                    if (valIni != "" && valIni != "0" && !item.Value.Contains(valIni))
                                    {
                                        listLines.Add(string.Concat(string.Format("Dato incorrecto: {0}, se esperaba cualquiera de estos: {1} ", valIni, string.Join(",", item.Value)), ";"
                                                , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";", item.Key.ToString(), ";"
                                                , string.Format("R{0}", itemList[0])));
                                    }
                                }
                            }

                            //Recorre los campos de un registro
                            for (int i = 1; i <= itemList.Length; i++)
                            {

                                bool _campoObliga = true;
                                string val = itemList[i - 1];

                                //Valida campos obligatorios
                                if (!obliga[_obligaKey].Contains(i.ToString()))
                                {
                                    if (val.Trim() == "")
                                    {
                                        #region CONDICIONES DE CAMPOS OBLIGATORIOS
                                        //R4 - Campo 13 (Fecha de la certificación) es obligatorio si 
                                        //Campo 6 (Tipo de información) es diferente a "I" (Indicio)
                                        if (itemList[0] == "4" && i.ToString() == "13" && itemList[5] == "I")
                                        {
                                            _campoObliga = false;
                                        }

                                        //R5 - Campos 9 y 10 (Fondo aportes) son obligatorios si 
                                        //campo 8 (Realizó aportes) es "S" (SI)
                                        if (itemList[0] == "5" && itemList[7] != "S")
                                        {
                                            if (i.ToString() == "9" || i.ToString() == "10")
                                            {
                                                _campoObliga = false;
                                            }
                                        }

                                        //R7 - Campo 12 (Tipo Doc beneficiario) y 13 (Documento beneficiario) son obligatorios si 
                                        //campo 10 (Estado Civil) es "C" (Casado)
                                        if (itemList[0] == "7" && itemList[10] != "C")
                                        {
                                            if (i.ToString() == "12" || i.ToString() == "13")
                                            {
                                                _campoObliga = false;
                                            }
                                        }

                                        //R7 - Campo 33 (Razón aporte salud SINO Ley 100) es obligatorio si 
                                        //campo 30 (Razón aporte salud) es igual a "OTRO"
                                        if (itemList[0] == "7" && i.ToString() == "33" && itemList[29] != "OTRO")
                                        {
                                            _campoObliga = false;
                                        }

                                        //R7 - Campo 20 (Fecha afiliación ISS) es obligatorio si 
                                        //campo 17 (Pension Compartida) es "S" (SI)
                                        if (itemList[0] == "7" && i.ToString() == "20" && itemList[16] != "S")
                                        {
                                            _campoObliga = false;
                                        }

                                        //R8 - Campo 30 (Razón aporte salud SINO Ley 100) es obligatorio si 
                                        //campo 27 (Razón aporte salud) es igual a "OTRO"
                                        if (itemList[0] == "8" && i.ToString() == "30" && itemList[29] != "OTRO")
                                        {
                                            _campoObliga = false;
                                        }

                                        //R8 - Campo 32 (Hijos con fallecido) es obligatorio si 
                                        //la diferencia entre campo 22 (fecha corte información)
                                        //y campo 10 (fecha de nacimiento) es menor o igual a 30 años
                                        //Y si campo 25 (Parentesco) es "C" Conyugue
                                        if (itemList[0] == "8" && i.ToString() == "32" && itemList[24] == "C")
                                        {
                                            //INICIO - Calcular edad
                                            DateTime fechaNac = DateTime.ParseExact(
                                                s: itemList[9],
                                                format: "yyyyMMdd",
                                                provider: CultureInfo.GetCultureInfo("tr-TR"));

                                            DateTime fechaCorteInfo = DateTime.ParseExact(
                                                s: itemList[21],
                                                format: "yyyyMMdd",
                                                provider: CultureInfo.GetCultureInfo("tr-TR"));

                                            int edad = fechaCorteInfo.Year - fechaNac.Year;

                                            if (fechaNac > fechaCorteInfo.AddYears(-edad)) edad--;
                                            //FIN - Calcular edad

                                            //Si edad es mayor a 30 no obliga al campo 32 (Hijos con fallecido)
                                            if (edad > 30)
                                            {
                                                _campoObliga = false;
                                            }
                                        }
                                        else if (itemList[0] == "8" && i.ToString() == "32" && itemList[24] != "C")
                                        {
                                            _campoObliga = false;
                                        }

                                        #endregion CONDICIONES DE CAMPOS OBLIGATORIOS

                                        if (_campoObliga)
                                        {
                                            string strVal = string.Concat("Campo Obligatorio.;", (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";", i.ToString(), ";", string.Format("R{0}", itemList[0]));
                                            listLines.Add(strVal);
                                        }
                                    }
                                }

                                //Verifica espacios al inicio o final del valor de un campo
                                if (val.Trim().Length != itemList[i - 1].Length)
                                {
                                    listLines.Add(string.Concat("Valor del campo contiene espacios al inicio o final de la cadena.;"
                                        , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";", i.ToString(), ";"
                                        , string.Format("R{0}", itemList[0])));
                                }

                                //Valida valor de campos fecha
                                if (fechas[_fechaKey].Contains(i - 1))
                                {

                                    if (val.Trim() != "")
                                    {
                                        //Verifica longitud de campo fecha
                                        if (val.Trim().Length != 8)
                                        {
                                            listLines.Add(string.Concat("Longitud incorrecta de campo fecha.;"
                                                    , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";", i.ToString(), ";"
                                                    , string.Format("R{0}", itemList[0])));
                                        }
                                        else
                                        {
                                            //Verifica fecha sea correcta
                                            bool esCorrecta = ValidarFecha(val, 8);
                                            if (!esCorrecta)
                                            {
                                                listLines.Add(string.Concat("Valor incorrecto en campo fecha.;"
                                                    , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";", i.ToString(), ";"
                                                    , string.Format("R{0}", itemList[0])));
                                            }
                                        }
                                    }
                                }
                                else if (fechashoras[_fechahoraKey].Contains(i - 1))
                                {
                                    //Valida valir de campos fecha hora
                                    if (val.Trim() != "")
                                    {
                                        //Verifica longitud de campo fecha hora
                                        if (val.Trim().Length != 12)
                                        {
                                            listLines.Add(string.Concat("Longitud incorrecta de campo fecha hora.;"
                                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";", i.ToString(), ";"
                                            , string.Format("R{0}", itemList[0])));
                                        }
                                        else
                                        {
                                            //Verifica fecha sea correcta
                                            bool esCorrecta = ValidarFecha(itemList[i - 1].ToString(), 12);
                                            if (!esCorrecta)
                                            {
                                                listLines.Add(string.Concat("Valor incorrecto en campo fecha hora.;"
                                                    , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";", i.ToString(), ";"
                                                    , string.Format("R{0}", itemList[0])));
                                            }
                                        }
                                    }
                                }

                                //Valida formato y valor del campo PERIODO del registro tipo 6 (yyyyMM)
                                if (itemList[0].ToString() == "6" && (i - 1) == 3)
                                {
                                    if (val.Trim() != "")
                                    {
                                        string _paramValor = string.Concat(val.Trim(), "01");

                                        //Verifica fecha sea correcta
                                        bool esCorrecta = ValidarFecha(_paramValor, 8);
                                        if (!esCorrecta)
                                        {
                                            listLines.Add(string.Concat("Valor incorrecto en campo fecha.;"
                                                , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";", i.ToString(), ";"
                                                , string.Format("R{0}", itemList[0])));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        listLines.Add(string.Concat("Estructura Incorrecta. Revisar separadores.;", cont.ToString(), ";;"));
                    }
                }


                //1. Valida al menos un regitro tipo 5 por cada tipo 4
                //2. Valida un regitro tipo 4 tenga su respectivo tipo 2 
                //3. Valida existencia de registro tipo 1 según el nit seccional empleador y codigo seccional empleador
                //4. Valida registros repetidos por número de certificación y número de cédula
                foreach (string[] itemList in _tipo4)
                {
                    bool encontre = false;
                    //bool encontre2 = false;

                    //1.
                    encontre = pInfoArchivo.Any(x => x[0] == "5" && x[1] == itemList[4]);

                    if (encontre)
                    {
                        continue;
                    }
                    else
                    {
                        listLines.Add(string.Concat("No existe un Registro tipo 5 asociado a este tipo 4.;"
                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                            , "R4"));
                    }

                    //2.
                    encontre = pInfoArchivo.Any(x => x[0] == "2" && x[2] == itemList[14]);

                    if (!encontre)
                    {
                        listLines.Add(string.Concat("No existe un registro tipo 2 asociado a este tipo " + itemList[0].ToString() + ".;"
                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                            , "R" + itemList[0].ToString()));
                    }

                    //3.
                    encontre = pInfoArchivo.Any(x => x[0] == "1" && x[1] == itemList[6] && Convert.ToInt32(x[3]) == Convert.ToInt32(itemList[7]));

                    if (!encontre)
                    {
                        listLines.Add(string.Concat("No existe un registro tipo 1 con el mismo Nit y Código seccional empleador, según lo relacionado en este tipo " + itemList[0].ToString() + ".;"
                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                            , "R" + itemList[0].ToString()));
                    }

                    //4.
                    int cuantos = pInfoArchivo.Where(x => x[0] == "4" && x[4] == itemList[4]).ToList().Count;

                    if (cuantos > 1)
                    {
                        listLines.Add(string.Concat("Registro tipo " + itemList[0].ToString() + " repetido con mismo número de ceritificación o indicio.;"
                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                            , "R" + itemList[0].ToString()));
                    }

                }

                //1. Valida tipo 2 Depuración CND tenga un registro tipo 9
                //2. Valida un regitro tipo 2 tenga un tipo 4, ó tipo 7 ó tipo 8
                foreach (string[] itemList in _tipo2)
                {
                    bool encontre = false;
                    //bool encontre2 = false;
                    string[] depende = { "7", "8" };

                    //1.
                    if (itemList[13] == "CND")
                    {

                        encontre = pInfoArchivo.Any(x => x[0] == "9" && x[2] == itemList[2]);

                        if (encontre)
                        {
                            continue;
                        }
                        else
                        {
                            listLines.Add(string.Concat("No existe un Registro tipo 9 asociado por depuración CND a este tipo 2.;"
                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                            , "R2"));
                        }
                    }


                    //2
                    encontre = pInfoArchivo.Any(x => depende.Contains(x[0]) && x[7] == itemList[2]);

                    if (encontre)
                    {
                        continue;
                    }
                    else
                    {
                        encontre = pInfoArchivo.Any(x => x[0] == "4" && x[14] == itemList[2]);

                        if (encontre)
                        {
                            continue;
                        }
                        else
                        {
                            listLines.Add(string.Concat("No existe un registro tipo 4, o 7, o 8 asociado a este tipo 2.;"
                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                            , "R2"));
                        }
                    }
                }

                //1. Valida existencia de registro tipo 1 según el nit seccional empleador y codigo seccional empleador
                foreach (string[] itemList in _tipo7)
                {
                    bool encontre = false;

                    encontre = pInfoArchivo.Any(x => x[0] == "1" && x[1] == itemList[3] && Convert.ToInt32(x[3]) == Convert.ToInt32(itemList[4]));


                    if (!encontre)
                    {
                        listLines.Add(string.Concat("No existe un registro tipo 1 con el mismo Nit y Código seccional empleador, según lo relacionado en este tipo " + itemList[0].ToString() + ".;"

                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                            , "R" + itemList[0].ToString()));
                    }
                }

                //1. Valida existencia de registro tipo 1 según el nit seccional empleador y codigo seccional empleador
                foreach (string[] itemList in _tipo8)
                {
                    bool encontre = false;

                    encontre = pInfoArchivo.Any(x => x[0] == "1" && x[1] == itemList[3] && Convert.ToInt32(x[3]) == Convert.ToInt32(itemList[4]));

                    if (!encontre)
                    {
                        listLines.Add(string.Concat("No existe un registro tipo 1 con el mismo Nit y Código seccional empleador, según lo relacionado en este tipo " + itemList[0].ToString() + ".;"
                            , (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;"
                            , "R" + itemList[0].ToString()));
                    }
                }

                //Valida un regitro tipo 7 ó tipo 8 tengan su respectivo tipo 2 
                foreach (string[] itemList in pInfoArchivo.Where(x => obliganTipo2.Contains(x[0])).ToList())
                {
                    bool encontre = false;

                    encontre = pInfoArchivo.Any(x => x[0] == "2" && x[2] == itemList[7]);

                    if (!encontre)
                    {
                        listLines.Add(string.Concat("No existe al menos un registro tipo 2 asociado a este tipo " + itemList[0].ToString() + ".;", (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;", "R" + itemList[0].ToString()));
                    }

                }

                List<string> validados = new List<string>();
                foreach (string[] itemList in _tipo2)
                {
                    string valida = string.Concat(itemList[1], itemList[2]);

                    if (!validados.Contains(valida))
                    {
                        if (pInfoArchivo.Where(x => x == itemList).Count() > 1)
                        {
                            listLines.Add("Registros tipo 2 repetidos exactamente iguales.;" + (pInfoArchivo.IndexOf(itemList) + 1).ToString() + ";;R2");
                        }

                        if (pInfoArchivo.Where(x => x[0] == "2" && x[1] == itemList[1] && x[2] == itemList[2]).Count() > 1)
                        {
                            listLines.Add("Registro tipo 2 repetido. Corregir archivo y dejar el registro con campo FECHA DE ÚLTIMA MODIFICACIÓN más reciente.;" + (pInfoArchivo.IndexOf(itemList) + 1).ToString() + ";;R2");
                        }
                    }

                    validados.Add(valida);
                }

                List<string> validatres = new List<string>();
                foreach (string[] itemList in _tipo3)
                {
                    string valida = string.Concat(itemList[1], itemList[2]);

                    if (!validatres.Contains(valida))
                    {

                        if (pInfoArchivo.Where(x => x == itemList).Count() > 1)
                        {
                            listLines.Add("Registros tipo 3 repetidos exactamente iguales.;" + (pInfoArchivo.IndexOf(itemList) + 1).ToString() + ";;R3");
                        }

                        if (pInfoArchivo.Where(x => x[0] == "3" && x[1] == itemList[1] && x[2] == itemList[2]).Count() > 1)
                        {
                            listLines.Add("Registro tipo 3 repetido. Corregir archivo y dejar el registro con campo FECHA ULTIMO REPORTE OBP más reciente;" + (pInfoArchivo.IndexOf(itemList) + 1).ToString() + ";;R3");
                        }
                    }

                    validatres.Add(valida);

                    //Valida que exista un registro tipo 2 para este tipo 3
                    bool encontre = false;

                    encontre = pInfoArchivo.Any(x => x[0] == "2" && x[1] == itemList[1] && x[2] == itemList[2]);

                    if (!encontre)
                    {
                        listLines.Add(string.Concat("No existe un registro tipo 2 asociado a este tipo " + itemList[0].ToString() + ".;", (pInfoArchivo.IndexOf(itemList) + 1).ToString(), ";;", "R" + itemList[0].ToString()));
                    }
                }

                #endregion


                #region CALCULOS


                ///XSD-> validar archivos
                ///
                /*1. Calcula el número de personas activas o retiradas
                 * - Para este conteo se toman los registros tipo 2 estos se
                 *      cruzan con los regitros tipo 4 por el campo DocumentoBeneficiario y que estos registros tipo 4
                 *      tengan el mismo valor en el campo NitSeccionalEmpleador que el que tiene el registro tipo 1 en el campo Nit Entidad Territorial
                 * - Luego estos registros tipo 4 se buscan su correspondiente registro tipo 5 los cuales cruzan por el campo Numero de Certificación o Indicio
                 * - Estos registros tipo 5 se ordenan por el campo fecha inicial y se toma el mas reciente
                 * - A este último registro se le verifica el campo fecha final, si esta vació es activo, de lo contrario si esta lleno es retirado
                 *
                 *2. Calcula el número de personas pensionadas y pensionadas fallecidas
                 * Se toman los registros tipo 2 y se cruzan con los registros tipo 7 por los campos tipo documento beneficiario y documento beneficiario
                 * Para estos registros tipo 7 filtrar el campo nit seccional empleador por el que tiene el registro tipo 1 en el campo Nit Entidad Territorial
                 * Se toma un solo registros segun los resultados (por que campo se pueden ordenar para saber cual es el mas reciente?)
                 * De este registro se valida el campo fecha de fallecimiento, si es vació es Pensionado, de lo contrario es Pensionado Fallecido
                 * 
                 *3. Para los sustitutos lo mismo que el anterior, pero cruzando los tipo 2 con tipo 8
                 * Y se verifica la existencia del registro tipo 8 para el número de documento*/
                foreach (string[] itemList in _tipo2)
                {
                    string nitTipo2 = itemList[2];
                    string tipodocTipo2 = itemList[1];
                    string depuraTipo2 = itemList[13];
                    bool encontre = false;

                    //1
                    List<string[]> _localtipo4 = new List<string[]>();
                    _localtipo4.AddRange(_tipo4.Where(x => x[6] == nitTipo1 && x[14] == nitTipo2).ToList());

                    if (_localtipo4.Count > 0)
                    {
                        List<string[]> _localtipo5 = new List<string[]>();
                        _localtipo5.AddRange(_tipo5.Where(x => _localtipo4.Select(y => y[4]).ToList().Contains(x[1])).ToList());

                        if (_localtipo5.Count > 0)
                        {
                            string[] recienteTipo5 = _localtipo5.OrderByDescending(x => x[4]).FirstOrDefault();

                            if (recienteTipo5[5].ToString() == string.Empty)
                            {
                                ContadorActivos++;
                                cedulasActivos += string.Concat(nitTipo2, ",");
                            }
                            else
                            {
                                ContadorRetirados++;
                                cedulasRetirados += string.Concat(nitTipo2, ",");
                            }
                        }
                    }

                    //2
                    List<string[]> _localtipo7 = new List<string[]>();
                    _localtipo7.AddRange(_tipo7.Where(x => x[3] == nitTipo1 && x[6] == tipodocTipo2 && x[7] == nitTipo2).ToList());

                    if (_localtipo7.Count > 0)
                    {
                        string[] _registro7 = _localtipo7.FirstOrDefault();

                        if (_registro7[33].ToString() == string.Empty)
                        {
                            ContadorPensionados++;
                            if (depuraTipo2 != string.Empty)
                                ContadorPensionadosDepurados++;
                        }
                        else
                        {
                            ContadorPensionadosFallecidos++;

                            int anioFall = Convert.ToInt32(_registro7[33].ToString().Substring(0, 4));

                            if (anioFall >= 2015)
                            {
                                bool existeOcho = _tipo8.Any(x => x[20] == _registro7[7]);

                                if (!existeOcho)
                                    ContadorSustitutosSupuestos++;
                            }
                        }
                    }


                    //3
                    encontre = _tipo8.Any(x => x[3] == nitTipo1 && x[6] == tipodocTipo2 && x[7] == nitTipo2);

                    if (encontre)
                    {
                        ContadorSustitutos++;

                        if (depuraTipo2 != string.Empty)
                            ContadorSustitutosDepurados++;
                    }
                }

                /*Totales
                     * 1. Total pensionados vivos
                     * 2. Total pensionados fallecidos
                     * 3. Total sustitutos supuestos
                     */
                //1
                int ContadorTotalPensionadosVivos = _tipo7.Count(x => x[33] == "");
                int ContadorTotalPensionadosFallecidos = _tipo7.Count(x => x[33] != "");
                int ContadorTotalSustitutosSupuestos = _tipo8
                    .Count(y => _tipo7
                        .Where(x => x[33] != "" && Convert.ToInt32(x[33].ToString().Substring(0, 4)) >= 2015)
                        .Select(x => x[7])
                        .Contains(y[20])
                    );

                #endregion


                #region PREPARA Y GENERA RESULTADOS

                string sourcePath = string.Concat(pathBorrador,"\\",nombreArchivo);

                //Genera archivo de inconsistencias
                if (listLines.Count == 0)
                {
                    exitoso = true;
                    //listLines.Add("Sin Inconsistencias");

                    //Crea el directorio destino de resultados si este no existe
                    Directory.CreateDirectory(pathExito);
                    string targetPath = string.Concat(pathExito,"\\",nombreArchivo);

                    //Genera archivo de datos calculados
                    listLinesCalculos.Add("******************** Conteos cruzando con registros de personas, tipo 2 ********************");
                    listLinesCalculos.Add("");
                    listLinesCalculos.Add(string.Concat("Número de Activos: ", ContadorActivos.ToString(), ". (", cedulasActivos, ")"));
                    listLinesCalculos.Add(string.Concat("Número de Retirados: ", ContadorRetirados.ToString(), ". (", cedulasRetirados, ")"));
                    listLinesCalculos.Add("Número de Pensionados: " + ContadorPensionados.ToString());
                    listLinesCalculos.Add("Número de Pensionados Fallecidos: " + ContadorPensionadosFallecidos.ToString());
                    listLinesCalculos.Add("Número de Sustitutos: " + ContadorSustitutos.ToString());
                    listLinesCalculos.Add("Número de Sustitutos Depurados: " + ContadorSustitutosDepurados.ToString());
                    listLinesCalculos.Add("Número de Pensionados Depurados (Vivos): " + ContadorPensionadosDepurados.ToString());
                    listLinesCalculos.Add("Número de Sustitutos Supuestos (Fallecidos del 2015 en adelante): " + ContadorSustitutosSupuestos.ToString());
                    listLinesCalculos.Add("");
                    listLinesCalculos.Add("******************** Totales sin cruzar con registros de personas, tipo 2 ********************");
                    listLinesCalculos.Add("");
                    listLinesCalculos.Add("Total pensionados vivos: " + ContadorTotalPensionadosVivos.ToString());
                    listLinesCalculos.Add("Total pensionados fallecidos: " + ContadorTotalPensionadosFallecidos.ToString());
                    listLinesCalculos.Add("Total Sustitutos Supuestos (Fallecidos del 2015 en adelante): " + ContadorTotalSustitutosSupuestos.ToString());

                    // Guarda archivo de cálculos en carpeta exitoso
                    string nombreCalculos = string.Concat(pathExito, "\\" , nombreArchivo.Split('.')[0], "_CALCULOS.txt");

                    if (File.Exists(nombreCalculos))
                    {
                        File.Delete(nombreCalculos);
                    }

                    System.IO.File.WriteAllLines(@nombreCalculos, listLinesCalculos.ToArray());

                    if (File.Exists(targetPath))
                    {
                        File.Delete(targetPath);
                    }

                    // Mueve el archivo localmente a carpeta exitoso
                    File.Move(sourcePath, targetPath);

                    // Mueve archivo a file server
                    NetworkShare.ConnectToShare(pathFileServer, userFileServer, passFileServer); //Connect with the new credentials

                    File.Copy(targetPath, string.Concat(pathFileServer, "\\",nombreArchivo));

                    //NetworkShare.DisconnectFromShare(pathFileServer, false); //Remove this line also

                }
                else
                {
                    exitoso = false;
                    //Crea el directorio destino de resultados si este no existe
                    Directory.CreateDirectory(pathError);
                    string targetPath = string.Concat(pathError, "\\", nombreArchivo);

                    // Guarda archivo de inconsistencias en carpeta error
                    string nombre = string.Concat(pathError, "\\", nombreArchivo.Split('.')[0], "_INCONSISTE.txt");

                    if (File.Exists(nombre))
                    {
                        File.Delete(nombre);
                    }

                    System.IO.File.WriteAllLines(@nombre, listLines.ToArray());

                    if (File.Exists(targetPath))
                    {
                        File.Delete(targetPath);
                    }

                    // Mueve el archivo localmente a carpeta error
                    File.Move(sourcePath, targetPath);
                    //NetworkShare.DisconnectFromShare(pathFileServer, false); //Remove this line also
                }

                // Guardar auditoria en BDSuppt Postgres
                GuardarAuditoriaPostgres(exitoso, nombreArchivo);

                #endregion

                // NetworkShare.DisconnectFromShare(pathFileServer, false); //Remove this line also
            }
            catch (Exception ex)
            {
               // NetworkShare.DisconnectFromShare(pathFileServer, false); //Remove this line also
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Validá el formato de un campo fecha y que sea una fecha correcta
        /// </summary>
        /// <param name="valor"></param>
        private bool ValidarFecha(string valor, int longitud)
        {
            bool _correcta = false;

            DateTime valorFecha;

            if (longitud == 8)
            {

                string[] format = { "yyyyMMdd" };

                _correcta = DateTime.TryParseExact(valor,
                                           format,
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None,
                                           out valorFecha) &&
                                           // Valida que la fecha sea posterior a 1577
                                           string.Compare(valor, "15771001") == 1;


            }
            else if (longitud == 12)
            {
                valor = string.Concat(valor, "01");
                valor = valor.Insert(8, " ");
                string[] format = { "yyyyMMdd HHmmss" };

                _correcta = DateTime.TryParseExact(valor,
                                           format,
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None,
                                           out valorFecha) &&
                                           // Valida que la fecha sea posterior a 1577
                                           string.Compare(valor, "15771001 000000") == 1;

            }

            return _correcta;
        }

        /// <summary>
        /// Se conecta a BDSuppt (postgres) en PASIVOCOL para realizar actualización a registro  de aufitoria
        /// </summary>
        /// <param name="pExitoso"></param>
        /// <param name="pNombreArchivo"></param>
        public void GuardarAuditoriaPostgres(bool pExitoso, string pNombreArchivo )
        {
            try
            {
                string connstring = String.Format("Server={0};Port=5432;User Id={1};Password={2};Database={3};",
                IpServerPostgres,
                UserServerPostgres,
                PassServerPostgres,
                DbServerPostgres
                );

                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                string sql = "UPDATE \"Suppt\".\"Archivo\" SET \"fechaValidacion\"=:fecha, \"validacionExitosa\"=:valida, \"idEstado\"=:estado WHERE nombre=:archivo;";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add(new NpgsqlParameter("fecha", DateTime.Now));
                cmd.Parameters.Add(new NpgsqlParameter("valida", pExitoso));
                cmd.Parameters.Add(new NpgsqlParameter("archivo", pNombreArchivo));
                cmd.Parameters.Add(new NpgsqlParameter("estado", 4));
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }   
        }
    }
}
