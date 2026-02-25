// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NUnit.Framework.Tests.TestUtilities
{
    public static class TestAssemblyHelper
    {
        public static Assembly GenerateInMemoryAssembly(string code, string[] referencedAssemblies)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var references = referencedAssemblies
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToList();

            // Add default runtime references
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            if (assemblyPath is not null)
            {
                references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")));
            }


            var assemblyName = $"InMemoryAssembly_{Guid.NewGuid():N}";

            var compilation = CSharpCompilation.Create(
                assemblyName,
                [syntaxTree],
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }

                var errors = string.Join(", ", result.Diagnostics
                    .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                    .Select(diagnostic => diagnostic.ToString()));

                throw new CompileErrorException($"Failed to compile embedded source code:\n{errors}\nCode: {code}");
            }
        }
    }

    public class CompileErrorException : Exception
    {
        public CompileErrorException()
        {
        }
        public CompileErrorException(string message) : base(message)
        {
        }
        public CompileErrorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
