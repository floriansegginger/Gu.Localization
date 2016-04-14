﻿namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Threading;

    /// <summary> Class for translating resources </summary>
    public static class Translator
    {
        private static CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;
        private static IReadOnlyList<CultureInfo> allCultures;

        /// <summary>
        /// Notifies when the current language changes.
        /// </summary>
        public static event EventHandler<CultureInfo> CurrentCultureChanged;

        /// <summary>
        /// Gets or sets the culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture ??
                       AllCultures.FirstOrDefault() ??
                       CultureInfo.InvariantCulture;
            }

            set
            {
                if (CultureInfoComparer.Equals(currentCulture, value))
                {
                    return;
                }

                currentCulture = value;
                OnCurrentCultureChanged(value);
            }
        }

        /// <summary> Gets a list with all cultures found for the application </summary>
        public static IReadOnlyList<CultureInfo> AllCultures => allCultures ?? (allCultures = GetAllCultures());

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="resourceManager">
        /// The <see cref="ResourceManager"/> containing translations
        /// </param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static string Translate(ResourceManager resourceManager, string key)
        {
            if (resourceManager == null)
            {
                return string.Format(Properties.Resources.NullManagerFormat, key);
            }

            if (string.IsNullOrEmpty(key))
            {
                return "null";
            }

            var translated = resourceManager.GetString(key, CurrentCulture);
            if (translated == null)
            {
                return string.Format(Properties.Resources.MissingKeyFormat, key);
            }

            if (translated == string.Empty)
            {
                if (!AllCultures.Contains(CurrentCulture, CultureInfoComparer.Default))
                {
                    return string.Format(Properties.Resources.MissingCultureFormat, key);
                }

                if (resourceManager.GetResourceSet(CurrentCulture, false, false)
                                   .OfType<DictionaryEntry>()
                                   .All(x => !Equals(x.Key, key)))
                {
                    return string.Format(Properties.Resources.MissingTranslationFormat, key);
                }
            }

            return translated;
        }

        /// <summary>
        /// Check if the <paramref name="resourceManager"/> has a translation for <paramref name="key"/>
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/></param>
        /// <param name="key">The key</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>True if a translation exists</returns>
        internal static bool HasKey(ResourceManager resourceManager, string key, CultureInfo culture)
        {
            return resourceManager.GetString(key, culture) != null;
        }

        private static void OnCurrentCultureChanged(CultureInfo e)
        {
            CurrentCultureChanged?.Invoke(null, e);
        }

        private static IReadOnlyList<CultureInfo> GetAllCultures()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            Debug.WriteLine(currentDirectory);
            return ResourceCultures.GetAllCultures(currentDirectory);
        }
    }
}
