using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMGUIFramework.Settings
{
    public interface ISettingsManager
    {
        void AddSetting(string setting, string value);
        string GetSetting(string setting);
        void Save();
        void Load();
    }
}
