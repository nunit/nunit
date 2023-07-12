// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK

using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;

namespace NUnit.Framework.Tests.TestUtilities
{
    public static class TestAssemblyHelper
    {
        public static Assembly GenerateInMemoryAssembly(string code, string[] referencedAssemblies)
        {
            var options = new CompilerParameters()
            {
                GenerateInMemory = true,
                
            };
            options.ReferencedAssemblies.AddRange(referencedAssemblies);
            
            var codeProvider = CodeDomProvider.CreateProvider("CSharp");
            var result = codeProvider.CompileAssemblyFromSource(options, code);

            if (!result.Errors.HasErrors)
                return result.CompiledAssembly;

            var errors = string.Join(", ", result.Errors
                                                 .Cast<CompilerError>()
                                                 .Select(err => err.ToString()).ToArray());

            throw new CompileErrorException($"Failed to compile embedded source code: {errors}\nCode: {code}");
        }
    }

    public class CompileErrorException : Exception
    {
        public CompileErrorException() { }
        public CompileErrorException(string message) : base(message) { }
        public CompileErrorException(string message, Exception inner) : base(message, inner) { }
    }



}
#endif
