using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMGUIFramework.Interfaces;

namespace NMGUIFramework.Settings
{
    public static class SettingsService
    {
        /// <summary>
        ///     Wraps an <see cref="IReadOnlySettingsService" /> in an read only instance.
        /// </summary>
        /// <param name="service">The service to wrap.</param>
        /// <returns>A pure read only settings service.</returns>
        public static IReadOnlySettingsService AsReadOnly(this IReadOnlySettingsService service)
        {
            return !(service is ISettingsService) ? service : new ReadOnlySettingsService(service);
        }

        /// <summary>
        ///     Gets the value of setting with a specific key.
        /// </summary>
        /// <typeparam name="T">The type of the setting to retrieve</typeparam>
        /// <param name="service">The service to retrive the setting from.</param>
        /// <param name="key">The key of the setting</param>
        /// <returns>The value of the setting, if its present in this service</returns>
        public static async Task<T> GetSettingAsync<T>(this IReadOnlySettingsService service, string key)
        {
            return (T)await service.GetSettingAsync(key, typeof(T));
        }

        /// <summary>
        ///     Sets a setting to a new value.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="service">The service to write the setting to.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value to set.</param>
        public static Task SetSettingAsync<T>(this ISettingsService service, string key, T value)
        {
            return service.SetSettingAsync(key, value, typeof(T));
        }

        /// <summary>
        ///     Sets a setting to a new value.
        /// </summary>
        /// <param name="service">The service to write the setting to.</param>
        /// <param name="key">The key of the setting</param>
        /// <param name="value">The value of the setting</param>
        /// <remarks>
        ///     The settings type will be determined automatically. This can lead to unexpected results.
        /// </remarks>
        public static Task SetSettingAsync(this ISettingsService service, string key, object value)
        {
            return service.SetSettingAsync(key, value, value.GetType());
        }

        /// <summary>
        ///     Registers a setting in this service
        /// </summary>
        /// <typeparam name="T">The type of the setting</typeparam>
        /// <param name="service">The service to register the setting within</param>
        /// <param name="key">The key of the setting</param>
        public static Task RegisterSettingAsync<T>(this ISettingsService service, string key)
        {
            var type = typeof(T);
            var defaultValue = GetDefaultValue(type);

            return service.RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <summary>
        ///     Registers a setting in this service
        /// </summary>
        /// <param name="service">The service to register the setting within</param>
        /// <param name="key">The key of the setting</param>
        /// <param name="type">The type of the setting</param>
        public static Task RegisterSettingAsync(this ISettingsService service, string key, Type type)
        {
            var defaultValue = GetDefaultValue(type);
            return service.RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <summary>
        ///     Registers a setting with a default value in this service .
        /// </summary>
        /// <typeparam name="T">The type of the setting</typeparam>
        /// <param name="service">The service to register the setting within</param>
        /// <param name="key">The key of this setting</param>
        /// <param name="defaultValue">The value of this setting</param>
        public static Task RegisterSettingAsync<T>(this ISettingsService service, string key, T defaultValue)
        {
            return service.RegisterSettingAsync(key, defaultValue, defaultValue, typeof(T));
        }

        /// <summary>
        ///     Registers a setting with a default value in this service
        /// </summary>
        /// <param name="service">The service to register the setting within</param>
        /// <param name="key">The key of this setting</param>
        /// <param name="defaultValue">The default value of this setting</param>
        /// <remarks>
        ///     The settings type will be determined automatically. This can lead to unexpected results.
        /// </remarks>
        public static Task RegisterSettingAsync(this ISettingsService service, string key, object defaultValue)
        {
            return service.RegisterSettingAsync(key, defaultValue, defaultValue, defaultValue.GetType());
        }

        /// <summary>
        ///     Registers a setting with a default value in this service
        /// </summary>
        /// <param name="service">The service to register the setting within</param>
        /// <param name="key">The key of this setting</param>
        /// <param name="defaultValue">The default value of this setting</param>
        /// <param name="type">The type of the setting.</param>
        public static Task RegisterSettingAsync(this ISettingsService service, string key, object defaultValue, Type type)
        {
            return service.RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <summary>
        ///     Registers a setting with a default and an initial value in this service.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="key">The key of this setting.</param>
        /// <param name="service">The service to register the setting within</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="initialValue">The initial value of this setting.</param>
        public static Task RegisterSettingAsync<T>(this ISettingsService service, string key, T defaultValue, T initialValue)
        {
            return service.RegisterSettingAsync(key, defaultValue, initialValue, typeof(T));
        }

        /// <summary>
        ///     Registers a setting with a default and an initial value in this service.
        /// </summary>
        /// <param name="service">The service to register the setting within</param>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="initialValue">The initial value of this setting.</param>
        /// <remarks>
        ///     The settings type will be determined automatically. This can lead to unexpected results.
        /// </remarks>
        public static Task RegisterSettingAsync(this ISettingsService service, string key, object defaultValue, object initialValue)
        {
            var defaultType = defaultValue.GetType().GetTypeInfo();
            var initialType = initialValue.GetType().GetTypeInfo();

            if (!AreAssignable(defaultType, initialType, out var type) &&
                // The types are not derived from each other, so we have to find a common base type
                // We doen't have to check for a common interface, because we can't deserialize an interface.
                // Maybe we can allow this for collection like interfaces, but that is a topic to cover later.
                !HaveCommonBaseType(initialType, defaultType, out type))
                throw new ArgumentException("defaultValue and intitialValue does not share a common base type");

            return service.RegisterSettingAsync(key, defaultValue, initialValue, type);
        }
    }
}
