﻿namespace Gu.Localization
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>Utility class for finding resources.</summary>
    internal static class ResourceCultures
    {
        private static readonly IReadOnlyList<CultureInfo> EmptyCultures = new CultureInfo[0];

        /// <summary>
        /// Gets all cultures found in the directory. The search is donde by:
        /// 1) Enumerate all folders named with valid culture names
        /// 2) Check that the folder contains resource files
        /// The result is not cached
        /// </summary>
        /// <param name="executingDirectory">The directory to chek</param>
        /// <returns>The cultures found. If none an empty list is returned.</returns>
        internal static IReadOnlyList<CultureInfo> GetAllCultures(DirectoryInfo executingDirectory)
        {
            if (!executingDirectory.Exists)
            {
                return EmptyCultures;
            }

            List<CultureInfo> cultures = null;
            foreach (var directory in executingDirectory.EnumerateDirectories())
            {
                var cultureName = directory.Name;
                if (!Culture.Exists(directory.Name))
                {
                    continue;
                }

                if (directory.EnumerateFiles("*.resources.dll", SearchOption.TopDirectoryOnly).Any())
                {
                    if (cultures == null)
                    {
                        cultures = new List<CultureInfo>();
                    }

                    cultures.Add(CultureInfo.GetCultureInfo(cultureName));
                }
            }

            return cultures ?? EmptyCultures;
        }
    }
}
