// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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

using Mono.Cecil;

namespace NUnit.Engine.Services
{
    public class ExtensionAssembly
    {
        public ExtensionAssembly(string filePath, bool fromWildCard)
        {
            FilePath = filePath;
            FromWildCard = fromWildCard;
        }

        public string FilePath { get; private set; }
        public bool FromWildCard { get; private set; }

        private AssemblyDefinition _assemblyDefinition;
        public AssemblyDefinition Assembly
        {
            get
            {
                if (_assemblyDefinition == null)
                {
                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(FilePath));
                    resolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
                    var parameters = new ReaderParameters() { AssemblyResolver = resolver };

                    _assemblyDefinition = AssemblyDefinition.ReadAssembly(FilePath, parameters);
                }

                return _assemblyDefinition;
            }
        }

        public ModuleDefinition MainModule
        {
            get { return Assembly.MainModule; }
        }

        public AssemblyNameDefinition AssemblyName
        {
            get { return Assembly.Name; }
        }

        /// <summary>
        /// IsDuplicateOf returns true if two assemblies have the same name.
        /// </summary>
        public bool IsDuplicateOf(ExtensionAssembly other)
        {
            return AssemblyName.Name == other.AssemblyName.Name;
        }
        
        /// <summary>
        /// IsBetterVersion determines whether another assembly is
        /// a better (higher) version than the current assembly.
        /// It is only intended to be called if IsDuplicateOf
        /// has already returned true.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsBetterVersionOf(ExtensionAssembly other)
        {
#if DEBUG
            if (!IsDuplicateOf(other))
                throw new NUnitEngineException("IsBetterVersonOf should only be called on duplicate assemblies");
#endif

            var version = AssemblyName.Version;
            var otherVersion = other.AssemblyName.Version;

            if (version > otherVersion)
                return true;

            if (version < otherVersion)
                return false;

            // Versions are equal, override only if this one was specified exactly while the other wasn't
            return !FromWildCard && other.FromWildCard;
        }
    }
}
