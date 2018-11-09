using NMGUIFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMGUIFramework.Settings
{
    public class ReadOnlySettingsService : IReadOnlySettingsService
    {

        private readonly IReadOnlySettingsService _service;

        public ReadOnlySettingsService(IReadOnlySettingsService service)
        {
            _service = service;
        }

        public Task<object> GetSettingsAsync(string key, Type type)
        {
            return _service.GetSettingsAsync(key, type);
        }

        public Task<T> GetSettingAsync<T>(string key)
        {
            return _service.GetSettingAsync<T>(key);
        }

        public Task<bool> IsRegisterdAsync(string key)
        {
            return _service.IsRegisterdAsync(key);
        }
    }
}
