using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMGUIFramework.Interfaces

namespace NMGUIFramework
{
    public static class SettingsService
    {

        public static IReadOnlySettingsService AsReadOnly(this IReadOnlySettingsService service)
        {
            return !(service is ISettingsService) ? service : new ReadOnlySettingsService(service);
        }

        public static async Task<T> GetSettingsAsync<T>(this IReadOnlySettingsService service, string key)
        {
            return (T)await service.GetSettingsAsync(key, typeof(T));
        }

        public static Task SetSettingAsync<T>(this ISettingsService service, string key, object value)
        {
            return service.SetSettingsAsync(key, value, value.GetType());
        }

        public static Task SetSettingAsync(this ISettingsService service, string key, object value)
        {
            return service.SetSettingAsync(key, value, value.GetType());
        }

        public static Task RegisterSettingAsync<T>(this ISettingsService service, string key)
        {
            var type = typeof(T);
            var defaultValue = GetDefaultValue(type);

            return service.RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        public static Task RegisterSettingAsync<T>(this ISettingsService service, string key, T defaultValue)
        {
            return RegisterSettingAsync(key, defaultValue, defaultValue, typeof(T));    
        }

        public static Task RegisterSettingAsync

    }
}
