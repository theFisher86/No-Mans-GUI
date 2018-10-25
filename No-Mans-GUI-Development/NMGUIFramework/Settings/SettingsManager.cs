using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace NMGUIFramework.Settings
{
    /// <summary>
    /// This class handles setting across the application. Allowing the Tool Plugins to access any settings. 
    /// It's very VERY basic and only handles string atm (although given knowledge of the type, it could be used to store anything.)
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        private readonly string _settingsFile = "settings.json";
        private Dictionary<string, string> _settings;

        private Dictionary<string, string> Settings
        {
            get
            {
                if(_settings == null)
                {
                    Load();
                }
                return _settings;
            }
        }

        public void AddSetting(string setting, string value)
        {
            if(Settings.ContainsKey(setting))
            {
                Settings[setting] = value;
            }
            else
            {
                Settings.Add(setting, value);
            }
        }

        public string GetSetting(string setting)
        {
            if(Settings.ContainsKey(setting))
            {
                return Settings[setting];
            }
            return null;
        }

        public void Load()
        {
            if(!File.Exists(_settingsFile))
            {
                File.Create(_settingsFile);
                _settings = new Dictionary<string, string>();
            }

            string json = File.ReadAllText(_settingsFile);

            _settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public void Save()
        {
            if(!File.Exists(_settingsFile))
            {
                File.Create(_settingsFile);
            }

            if(_settings != null)
            {
                string json = JsonConvert.SerializeObject(_settings);
                File.WriteAllText(_settingsFile, json);
            }
        }
    }
}
