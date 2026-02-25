// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace NUnit.Framework.Tests.TestUtilities
{
    internal class TestCompiler
    {
        private readonly List<string> _referencedAssemblies = new();

        public TestCompiler() : this(null)
        {
        }

        public TestCompiler(string[]? assemblyNames)
        {
            if (assemblyNames is not null && assemblyNames.Length > 0)
                _referencedAssemblies.AddRange(assemblyNames);
        }

        private EmitResult CompileCode(string code, Stream stream)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var references = new List<MetadataReference>();

            // Add user-specified references
            foreach (var assembly in _referencedAssemblies)
            {
                references.Add(MetadataReference.CreateFromFile(assembly));
            }

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
                syntaxTrees: [syntaxTree],
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation.Emit(stream);
        }

        public EmitResult CompileCode(string code)
        {
            using var ms = new MemoryStream();
            return CompileCode(code, ms);
        }

        public Assembly GenerateInMemoryAssembly(string code)
        {
            using var ms = new MemoryStream();
            var result = CompileCode(code, ms);

            if (result.Success)
            {
                return Assembly.Load(ms.ToArray());
            }

            var errors = string.Join("\n",
                result.Diagnostics
                      .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                      .Select(diagnostic => diagnostic.ToString()));

            throw new CompileErrorException($"Failed to compile:\n{errors}\nCode: {code}");
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
}
