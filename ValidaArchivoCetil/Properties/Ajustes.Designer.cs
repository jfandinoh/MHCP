﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MHCP.DGT.SUPPT.ValidaArchivoCetil.Program.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Ajustes : global::System.Configuration.ApplicationSettingsBase {
        
        private static Ajustes defaultInstance = ((Ajustes)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Ajustes())));
        
        public static Ajustes Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("F:\\\\")]
        public string PATH_FILE_LOCAL {
            get {
                return ((string)(this["PATH_FILE_LOCAL"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Borrador")]
        public string DIRECTORIO_EXTRACCION {
            get {
                return ((string)(this["DIRECTORIO_EXTRACCION"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ProcesadoExito")]
        public string DIRECTORIO_ENVIAR_A_SUPPT {
            get {
                return ((string)(this["DIRECTORIO_ENVIAR_A_SUPPT"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ProcesadoError")]
        public string DIRECTORIO_ERROR {
            get {
                return ((string)(this["DIRECTORIO_ERROR"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("s-cetil02")]
        public string USERNAME_FILESERVER {
            get {
                return ((string)(this["USERNAME_FILESERVER"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("t1o6FFjNuu8NTQ")]
        public string PASSWORD_FILESERVER {
            get {
                return ((string)(this["PASSWORD_FILESERVER"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\MH-EXPRFSSA01.MHEXT.RED\\GestionDocumental\\MHCP_01GESTION\\11_DGRESS\\SUPPTPRUEBA\\" +
            "DOCUMENTOS\\SUPPT_EXTERNO\\Cetil\\HistoriaLaboral")]
        public string PATH_FILE_SERVER {
            get {
                return ((string)(this["PATH_FILE_SERVER"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.249.6")]
        public string URL_SERVER_POSTGRES {
            get {
                return ((string)(this["URL_SERVER_POSTGRES"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("supptuser")]
        public string USER_SERVER_POSTGRES {
            get {
                return ((string)(this["USER_SERVER_POSTGRES"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("S2U0p1pT9B*")]
        public string PASS_SERVER_POSTGRES {
            get {
                return ((string)(this["PASS_SERVER_POSTGRES"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("BDSuppt")]
        public string DB_SERVER_POSTGRES {
            get {
                return ((string)(this["DB_SERVER_POSTGRES"]));
            }
        }
    }
}
