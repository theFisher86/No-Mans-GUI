using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMGUIFramework.Interfaces
{
    public interface IReadOnlySettingsService
    {
        Task<object> GetSettingAsync(string key, Type type);

        Task<bool> IsRegisteredAsync(string key);
    }
}
