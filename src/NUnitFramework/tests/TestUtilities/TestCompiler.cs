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
        private readonly List<MetadataReference> _references;

        public TestCompiler()
            : this(Enumerable.Empty<Assembly>())
        {
        }

        public TestCompiler(params IEnumerable<Type> referencedTypes)
            : this(referencedTypes.Select(t => t.Assembly))
        {
        }

        public TestCompiler(params IEnumerable<Assembly> assemblies)
        {
            Dictionary<string, Assembly> alreadyReferencedAssemblies = [];

            // Always reference the NUnit framework assembly
            AddAssemblyReference(alreadyReferencedAssemblies, typeof(Assert).Assembly);

            // Add user-specified references
            foreach (var assembly in assemblies)
            {
                AddAssemblyReference(alreadyReferencedAssemblies, assembly);
            }

            // Convert the loaded assemblies to metadata references
            _references = new List<MetadataReference>(alreadyReferencedAssemblies.Count);
            foreach (var assembly in alreadyReferencedAssemblies.Values)
            {
                _references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        private static void AddAssemblyReference(Dictionary<string, Assembly> alreadyReferencedAssemblies, Assembly assembly)
        {
            AddAssemblyReference(alreadyReferencedAssemblies, assembly, assembly.GetName());
        }

        private static void AddAssemblyReference(Dictionary<string, Assembly> alreadyReferencedAssemblies, Assembly assembly, AssemblyName assemblyName)
        {
            if (alreadyReferencedAssemblies.ContainsKey(assemblyName.Name!))
            {
                // This assembly is already referenced
                return;
            }

            alreadyReferencedAssemblies.Add(assemblyName.Name!, assembly);

            // Add the new assemblies references recursively
            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                if (!alreadyReferencedAssemblies.ContainsKey(referencedAssembly.Name!))
                {
                    try
                    {
                        var loadedAssembly = Assembly.Load(referencedAssembly);
                        AddAssemblyReference(alreadyReferencedAssemblies, loadedAssembly, referencedAssembly);
                    }
                    catch (Exception ex)
                    {
                        throw new CompileErrorException($"Failed to load assembly '{referencedAssembly.FullName}' referenced by '{assembly.FullName}'.", ex);
                    }
                }
            }
        }

        private EmitResult CompileCode(string code, Stream stream, string? assemblyName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: [syntaxTree],
                references: _references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation.Emit(stream);
        }

        private string DefaultAssemblyName => $"InMemoryAssembly_{Guid.NewGuid():N}";

        public EmitResult CompileCode(string code)
        {
            using var ms = new MemoryStream();
            return CompileCode(code, ms, DefaultAssemblyName);
        }

        public Assembly GenerateInMemoryAssembly(string code)
            => GenerateInMemoryAssembly(code, DefaultAssemblyName);

        public Assembly GenerateInMemoryAssembly(string code, string? assemblyName)
        {
            using var ms = new MemoryStream();
            var result = CompileCode(code, ms, assemblyName);

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
