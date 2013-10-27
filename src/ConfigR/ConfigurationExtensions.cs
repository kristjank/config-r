﻿// <copyright file="ConfigurationExtensions.cs" company="ConfigR contributors">
//  Copyright (c) ConfigR contributors. (configr.net@gmail.com)
// </copyright>

namespace ConfigR
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CSharp.RuntimeBinder;

    public static class ConfigurationExtensions
    {
        public static T Get<T>(this IConfiguration values)
        {
            T value;
            if (!values.TryGet<T>(out value))
            {
                throw new ConfigurationErrorsException("Type not found.");
            }

            return value;
        }

        public static T GetOrDefault<T>(this IConfiguration values)
        {
            T value;
            return values.TryGet<T>(out value) ? value : default(T);
        }

        public static bool TryGet<T>(this IConfiguration values, out T value)
        {
            Guard.AgainstNullArgument("configurator", values);

            value = default(T);
            foreach (var candidate in values.Items.Select(pair => pair.Value).Where(candidate => candidate != null))
            {
                if (typeof(T).IsAssignableFrom(candidate.GetType()))
                {
                    value = candidate;
                    return true;
                }
            }

            return false;
        }

        public static dynamic Get(this IConfiguration values, string key)
        {
            return values.Get<dynamic>(key);
        }

        public static dynamic GetOrDefault(this IConfiguration values, string key)
        {
            return values.GetOrDefault<dynamic>(key);
        }

        public static bool TryGet(this IConfiguration values, string key, out dynamic value)
        {
            return values.TryGet<dynamic>(key, out value);
        }

        public static T Get<T>(this IConfiguration values, string key)
        {
            Guard.AgainstNullArgument("configurator", values);

            return Get<T>(key, values[key]);
        }

        public static T GetOrDefault<T>(this IConfiguration values, string key)
        {
            Guard.AgainstNullArgument("configurator", values);

            dynamic value;
            return values.TryGet(key, out value) ? Get<T>(key, value) : default(T);
        }

        public static bool TryGet<T>(this IConfiguration values, string key, out T value)
        {
            Guard.AgainstNullArgument("configurator", values);

            value = default(T);
            dynamic dynamicValue;
            if (!values.TryGet(key, out dynamicValue))
            {
                return false;
            }

            value = Get<T>(key, dynamicValue);
            return true;
        }

        private static T Get<T>(string key, dynamic value)
        {
            try
            {
                return (T)value;
            }
            catch (RuntimeBinderException ex)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Error getting '{0}'.", key), ex);
            }
        }
    }
}