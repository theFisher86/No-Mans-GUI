using System;
using System.Threading.Tasks;

namespace NMGUIFramework.Interfaces
{
    public interface ISettingsService : IReadOnlySettingsService
    {
        Task SetSettingsAsync(string key, object value, Type type);

        Task RegisterSettingAsync(string key, object defaultValue, object initalValue, Type type);

        Task UnregisterSettingAsync(string key);

        Task StoreAsync();

        Task DiscardAsync();
    }
}
