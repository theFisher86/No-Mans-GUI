﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NoMansGUI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("X:\\Modding\\NMS\\UnPakd\\")]
        public string pathUnpakdFiles {
            get {
                return ((string)(this["pathUnpakdFiles"]));
            }
            set {
                this["pathUnpakdFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Games\\SteamLibrary\\steamapps\\common\\No Man\'s Sky\\GAMEDATA\\PCBANKS")]
        public string pathPCBanks {
            get {
                return ((string)(this["pathPCBanks"]));
            }
            set {
                this["pathPCBanks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string pathModelViewer {
            get {
                return ((string)(this["pathModelViewer"]));
            }
            set {
                this["pathModelViewer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("default")]
        public string currentTheme {
            get {
                return ((string)(this["currentTheme"]));
            }
            set {
                this["currentTheme"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>Default</string>
  <string>Gek</string>
  <string>Korvax</string>
  <string>Vykeen</string>
  <string>Fourthrace</string>
  <string>Light</string>
  <string>Dark</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection allThemes {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["allThemes"]));
            }
            set {
                this["allThemes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string RecentFolder {
            get {
                return ((string)(this["RecentFolder"]));
            }
            set {
                this["RecentFolder"] = value;
            }
        }
    }
}
