// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK

namespace NUnit.TestUtilities
{
    public static class TestAssemblyHelper
    {
        public static Assembly GenerateInMemoryAssembly(string code, string[] referencedAssemblies)
        {
            var options = new CompilerParameters() { GenerateInMemory = true };
            options.ReferencedAssemblies.AddRange(referencedAssemblies);

            var codeProvider = CodeDomProvider.CreateProvider("CSharp");
            var result = codeProvider.CompileAssemblyFromSource(options, code);

            if (!result.Errors.HasErrors)
                return result.CompiledAssembly;

            var errors = string.Join(", ", result.Errors
                                                 .Cast<CompilerError>()
                                                 .Select(err => err.ToString()).ToArray());

            throw new InvalidOperationException($"Failed to create assembly: {errors}");
        }
    }
}
#endif
