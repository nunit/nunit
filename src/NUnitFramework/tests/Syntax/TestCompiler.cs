// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if !NETCOREAPP
using System.CodeDom.Compiler;

namespace NUnit.Framework.Syntax
{
    internal class TestCompiler
    {
        private readonly Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

        public CompilerParameters Options { get; } = new CompilerParameters();

        public TestCompiler() : this( null, null ) { }

        public TestCompiler( string[] assemblyNames ) : this( assemblyNames, null ) { }

        public TestCompiler( string[] assemblyNames, string outputName )
        {
            if ( assemblyNames != null && assemblyNames.Length > 0 )
                Options.ReferencedAssemblies.AddRange( assemblyNames );
            if ( outputName != null )
                Options.OutputAssembly = outputName;

            Options.IncludeDebugInformation = false;
            Options.TempFiles = new TempFileCollection( ".", false );
            Options.GenerateInMemory = false;
        }

        public CompilerResults CompileCode( string code )
        {
            return provider.CompileAssemblyFromSource( Options, code );
        }
    }
}
#endif
