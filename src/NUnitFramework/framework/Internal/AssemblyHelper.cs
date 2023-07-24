// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Reflection;
#if !NETFRAMEWORK
using System.Runtime.Loader;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// AssemblyHelper provides static methods for working
    /// with assemblies.
    /// </summary>
    public static class AssemblyHelper
    {
        private static readonly string UriSchemeFile = Uri.UriSchemeFile;
        private static readonly string SchemeDelimiter = Uri.SchemeDelimiter;

        #region GetAssemblyPath

        /// <summary>
        /// Gets the path from which an assembly was loaded.
        /// For builds where this is not possible, returns
        /// the name of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The path.</returns>
        public static string GetAssemblyPath(Assembly assembly)
        {
#if NETFRAMEWORK
            // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.location 
            // .NET Framework only: 
            // If the loaded file was shadow-copied, the location is that of the file after being shadow-copied.
            // To get the location before the file has been shadow-copied, use the CodeBase property.
            string? codeBase = assembly.CodeBase;

            if (codeBase is not null && IsFileUri(codeBase))
                return GetAssemblyPathFromCodeBase(codeBase);
#endif

            return assembly.Location;
        }

        #endregion

        #region GetDirectoryName

        /// <summary>
        /// Gets the path to the directory from which an assembly was loaded.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The path.</returns>
        public static string GetDirectoryName(Assembly assembly)
        {
            return Path.GetDirectoryName(GetAssemblyPath(assembly))!;
        }

        #endregion

        #region GetAssemblyName

        /// <summary>
        /// Gets the AssemblyName of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>An AssemblyName</returns>
        public static AssemblyName GetAssemblyName(Assembly assembly)
        {
            return assembly.GetName();
        }

        #endregion

        #region Load

#if !NETFRAMEWORK
        private sealed class ReflectionAssemblyLoader
        {
            private static ReflectionAssemblyLoader? _instance;
            private static bool _isInitialized;

            private readonly Func<string, Assembly> _loadFromAssemblyPath;

            public Assembly LoadFromAssemblyPath(string assemblyPath) => _loadFromAssemblyPath.Invoke(assemblyPath);

            private ReflectionAssemblyLoader(Func<string, Assembly> loadFromAssemblyPath)
            {
                _loadFromAssemblyPath = loadFromAssemblyPath;
            }

            public static ReflectionAssemblyLoader? TryGet()
            {
                if (_isInitialized) return _instance;
                _instance = TryInitialize();
                _isInitialized = true;
                return _instance;
            }

            private static ReflectionAssemblyLoader? TryInitialize()
            {
                var assemblyLoadContextType = Type.GetType("System.Runtime.Loader.AssemblyLoadContext", throwOnError: false);
                if (assemblyLoadContextType is null) return null;

                var defaultContext = assemblyLoadContextType.GetRuntimeProperty("Default")!.GetValue(null);

                var loadFromAssemblyPath = (Func<string, Assembly>)assemblyLoadContextType
                        .GetRuntimeMethod("LoadFromAssemblyPath", new[] { typeof(string) })!
                        .CreateDelegate(typeof(Func<string, Assembly>), defaultContext);

                assemblyLoadContextType.GetRuntimeEvent("Resolving")!.AddEventHandler(defaultContext,
                    new Func<AssemblyLoadContext, AssemblyName, Assembly?>((context, assemblyName) =>
                    {
                        var dllPath = Path.Combine(AppContext.BaseDirectory, assemblyName.Name + ".dll");
                        if (File.Exists(dllPath)) return loadFromAssemblyPath.Invoke(dllPath);

                        var exePath = Path.Combine(AppContext.BaseDirectory, assemblyName.Name + ".exe");
                        if (File.Exists(exePath)) return loadFromAssemblyPath.Invoke(exePath);

                        return null;
                    }));

                return new ReflectionAssemblyLoader(loadFromAssemblyPath);
            }
        }

        /// <summary>
        /// Loads an assembly given a string, which is the AssemblyName
        /// </summary>
        public static Assembly Load(string name)
        {
            var ext = Path.GetExtension(name);
            if (ext.Equals(".dll", StringComparison.OrdinalIgnoreCase)
                || ext.Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                var fromLoader = ReflectionAssemblyLoader.TryGet()?.LoadFromAssemblyPath(Path.GetFullPath(name));
                if (fromLoader is not null) return fromLoader;

                name = Path.GetFileNameWithoutExtension(name);
            }

            return Assembly.Load(new AssemblyName { Name = name });
        }
#else
        /// <summary>
        /// Loads an assembly given a string, which may be the
        /// path to the assembly or the AssemblyName
        /// </summary>
        public static Assembly Load(string nameOrPath)
        {
            var ext = Path.GetExtension(nameOrPath);

            // Handle case where this is the path to an assembly
            if (ext.Equals(".dll", StringComparison.OrdinalIgnoreCase) || ext.Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return Assembly.Load(AssemblyName.GetAssemblyName(nameOrPath));
            }

            // Assume it's the string representation of an AssemblyName
            return Assembly.Load(nameOrPath);
        }
#endif

        #endregion

        #region Helper Methods

        private static bool IsFileUri(string uri)
        {
            return uri.StartsWith(UriSchemeFile, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the assembly path from code base.
        /// </summary>
        /// <remarks>Public for testing purposes</remarks>
        /// <param name="codeBase">The code base.</param>
        /// <returns></returns>
        public static string GetAssemblyPathFromCodeBase(string codeBase)
        {
            // Skip over the file:// part
            int start = UriSchemeFile.Length + SchemeDelimiter.Length;

            if (codeBase[start] == '/') // third slash means a local path
            {
                // Handle Windows Drive specifications
                if (codeBase[start + 2] == ':')
                    ++start;
                // else leave the last slash so path is absolute
            }
            else // It's either a Windows Drive spec or a share
            {
                if (codeBase[start + 1] != ':')
                    start -= 2; // Back up to include two slashes
            }

            return codeBase.Substring(start);
        }

        #endregion
    }
}
