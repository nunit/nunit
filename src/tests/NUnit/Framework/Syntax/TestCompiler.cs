// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.CodeDom.Compiler;

namespace NUnit.Framework.Syntax
{
    class TestCompiler
    {
        Microsoft.CSharp.CSharpCodeProvider provider;
#if !CLR_2_0
		ICodeCompiler compiler;
#endif
		CompilerParameters options;

		public TestCompiler() : this( null, null ) { }

		public TestCompiler( string[] assemblyNames ) : this( assemblyNames, null ) { }

		public TestCompiler( string[] assemblyNames, string outputName )
		{
			this.provider = new Microsoft.CSharp.CSharpCodeProvider();
#if !CLR_2_0
			this.compiler = provider.CreateCompiler();
#endif
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
#if CLR_2_0
			return provider.CompileAssemblyFromSource( options, code );
#else
            return compiler.CompileAssemblyFromSource(options, code);
#endif
        }
    }
}
