using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AvaloniaProject.Services;

internal static class SatelliteAssemblyResolver
{
    private const string LocalesFolderName = "locales";
    private static bool _registered;

    public static void Register()
    {
        if (_registered)
            return;

        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        _registered = true;
    }

    private static Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var requestedAssembly = new AssemblyName(args.Name);
        var assemblyName = requestedAssembly.Name;

        if (string.IsNullOrWhiteSpace(assemblyName) ||
            string.IsNullOrWhiteSpace(requestedAssembly.CultureName) ||
            requestedAssembly.CultureName.Equals("neutral", StringComparison.OrdinalIgnoreCase) ||
            !assemblyName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var localesDirectory = Path.Combine(AppContext.BaseDirectory, LocalesFolderName);
        if (!Directory.Exists(localesDirectory))
            return null;

        var availableCultureDirectories = Directory.EnumerateDirectories(localesDirectory)
            .ToDictionary(path => Path.GetFileName(path), path => path, StringComparer.OrdinalIgnoreCase);

        foreach (var cultureCandidate in GetCultureFallbackChain(requestedAssembly.CultureName))
        {
            if (!availableCultureDirectories.TryGetValue(cultureCandidate, out var cultureDirectory))
                continue;

            var assemblyPath = Path.Combine(cultureDirectory, $"{assemblyName}.dll");
            if (File.Exists(assemblyPath))
                return Assembly.LoadFrom(assemblyPath);
        }

        return null;
    }

    private static IEnumerable<string> GetCultureFallbackChain(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            yield break;

        var current = cultureName;
        while (!string.IsNullOrWhiteSpace(current) && !current.Equals("neutral", StringComparison.OrdinalIgnoreCase))
        {
            yield return current;

            var separatorIndex = current.LastIndexOf('-');
            current = separatorIndex > 0 ? current[..separatorIndex] : string.Empty;
        }
    }
}



