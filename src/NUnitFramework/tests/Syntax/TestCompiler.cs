// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

#if !(NETCOREAPP1_1 || NETCOREAPP2_0)
using System;
using System.CodeDom.Compiler;

namespace NUnit.Framework.Syntax
{
    internal class TestCompiler
    {
        private readonly Microsoft.CSharp.CSharpCodeProvider provider;
        CompilerParameters options;

        public TestCompiler() : this( null, null ) { }

        public TestCompiler( string[] assemblyNames ) : this( assemblyNames, null ) { }

        public TestCompiler( string[] assemblyNames, string outputName )
        {
            this.provider = new Microsoft.CSharp.CSharpCodeProvider();
            this.options = new CompilerParameters();

            if ( assemblyNames != null && assemblyNames.Length > 0 )
                options.ReferencedAssemblies.AddRange( assemblyNames );
            if ( outputName != null )
                options.OutputAssembly = outputName;

            options.IncludeDebugInformation = false;
            options.TempFiles = new TempFileCollection( ".", false );
            options.GenerateInMemory = false;
        }

        public CompilerParameters Options
        {
            get { return options; }
        }

        public CompilerResults CompileCode( string code )
        {
            return provider.CompileAssemblyFromSource( options, code );
        }
    }
}
#endif
