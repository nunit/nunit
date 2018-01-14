// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.IO;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// AssemblyHelper provides static methods for working
    /// with assemblies.
    /// </summary>
    public static class AssemblyHelper
    {
#if NETSTANDARD1_6
        const string UriSchemeFile = "file";
        const string SchemeDelimiter = "://";
#else
        static readonly string UriSchemeFile = Uri.UriSchemeFile;
        static readonly string SchemeDelimiter = Uri.SchemeDelimiter;
#endif

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
            string codeBase = assembly.CodeBase;

            if (IsFileUri(codeBase))
                return GetAssemblyPathFromCodeBase(codeBase);

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
            return Path.GetDirectoryName(GetAssemblyPath(assembly));
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

#if NETSTANDARD1_6 || NETSTANDARD2_0
        private sealed class ReflectionAssemblyLoader
        {
            private static ReflectionAssemblyLoader instance;
            private static bool isInitialized;

            private readonly Func<string, Assembly> loadFromAssemblyPath;

            public Assembly LoadFromAssemblyPath(string assemblyPath) => loadFromAssemblyPath.Invoke(assemblyPath);

            private ReflectionAssemblyLoader(Func<string, Assembly> loadFromAssemblyPath)
            {
                this.loadFromAssemblyPath = loadFromAssemblyPath;
            }

            public static ReflectionAssemblyLoader TryGet()
            {
                if (isInitialized) return instance;
                instance = TryInitialize();
                isInitialized = true;
                return instance;
            }

            private static ReflectionAssemblyLoader TryInitialize()
            {
                var assemblyLoadContextType = Type.GetType("System.Runtime.Loader.AssemblyLoadContext", throwOnError: false);
                if (assemblyLoadContextType == null) return null;

                var defaultContext = assemblyLoadContextType.GetRuntimeProperty("Default").GetValue(null);

                var loadFromAssemblyPath = (Func<string, Assembly>)assemblyLoadContextType
                        .GetRuntimeMethod("LoadFromAssemblyPath", new[] { typeof(string) })
                        .CreateDelegate(typeof(Func<string, Assembly>), defaultContext);

                assemblyLoadContextType.GetRuntimeEvent("Resolving").AddEventHandler(defaultContext,
                    new Func<object, AssemblyName, Assembly>((context, assemblyName) =>
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
                if (fromLoader != null) return fromLoader;

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
            var ext = Path.GetExtension(nameOrPath).ToLower();

            // Handle case where this is the path to an assembly
            if (ext == ".dll" || ext == ".exe")
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
            return uri.ToLower().StartsWith(UriSchemeFile);
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
