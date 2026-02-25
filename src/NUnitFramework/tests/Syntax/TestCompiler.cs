// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Generic;
using System.IO;

namespace NUnit.Framework.Tests.Syntax
{
    internal class TestCompiler
    {
        private readonly List<string> _referencedAssemblies = new();
        private readonly string? _outputAssembly;

        public TestCompiler() : this(null, null)
        {
        }

        public TestCompiler(string[]? assemblyNames) : this(assemblyNames, null)
        {
        }

        public TestCompiler(string[]? assemblyNames, string? outputName)
        {
            if (assemblyNames is not null && assemblyNames.Length > 0)
                _referencedAssemblies.AddRange(assemblyNames);

            _outputAssembly = outputName;
        }

        public EmitResult CompileCode(string code)
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

            var assemblyName = _outputAssembly is not null
                ? Path.GetFileNameWithoutExtension(_outputAssembly)
                : Path.GetRandomFileName();

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: [syntaxTree],
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var outputPath = _outputAssembly ?? Path.Combine(Path.GetTempPath(), assemblyName + ".dll");

            using var stream = File.Create(outputPath);
            return compilation.Emit(stream);
        }
    }
}
