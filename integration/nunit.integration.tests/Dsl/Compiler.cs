namespace nunit.integration.tests.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Microsoft.Build.Utilities;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal class Compiler
    {
        private static readonly SyntaxTree TestClassSyntaxTree;

        private const string AssemblyInfoResourceName = "nunit.integration.tests.Templates.AssemblyInfo.cs";
        private const string TestFileResourceName = "nunit.integration.tests.Templates.UnitTest.cs";
        private readonly static ResourceManager ResourceManager = new ResourceManager();

        static Compiler()
        {
            TestClassSyntaxTree = CSharpSyntaxTree.ParseText(ResourceManager.GetContentFromResource(TestFileResourceName));
        }

        public void Compile(TestAssembly testAssembly, string assemblyFileName, TargetDotNetFrameworkVersion dotNetFrameworkVersion)
        {
            DotNetFrameworkArchitecture architecture = DotNetFrameworkArchitecture.Current;
            switch (testAssembly.Platform)
            {                    
                case Platform.X86:
                    architecture = DotNetFrameworkArchitecture.Bitness32;
                    break;

                case Platform.X64:
                    architecture = DotNetFrameworkArchitecture.Bitness32;
                    break;
            }

            var assemblyInfoSyntaxTree = CSharpSyntaxTree.ParseText(ResourceManager.GetContentFromResource(AssemblyInfoResourceName) + Environment.NewLine + string.Join(Environment.NewLine, testAssembly.Attributes));
            var compilation =
                CSharpCompilation.Create(Path.GetFileName(assemblyFileName))
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithPlatform(testAssembly.Platform)
                    .WithOptimizationLevel(OptimizationLevel.Debug))
                    .AddReferences(GetDotNetFrameworkReferences(dotNetFrameworkVersion, architecture))
                    .AddReferences(testAssembly.References.Select(assembly => MetadataReference.CreateFromFile(assembly)))
                    .AddSyntaxTrees(assemblyInfoSyntaxTree)
                    .AddSyntaxTrees(testAssembly.Classes.Select(CreateClassSyntaxTree));

            using (var file = new FileStream(assemblyFileName, FileMode.Create))
            {
                var result = compilation.Emit(file);
                if (!result.Success)
                {
                    var code = string.Join(Environment.NewLine + Environment.NewLine, compilation.SyntaxTrees.Select(i => i.ToString()));
                    throw new Exception($"Errors:\n{string.Join(Environment.NewLine, result.Diagnostics.Select(i => i.GetMessage(CultureInfo.InvariantCulture)))}\n\nCode:\n{code}");
                }

                file.Flush(true);                
            }            
        }        

        private static SyntaxTree CreateClassSyntaxTree(TestClass testClass)
        {
            var attributes = testClass.Attributes.Select(i => CSharpSyntaxTree.ParseText(i).GetRoot().DescendantNodes().OfType<AttributeSyntax>().Single()).ToList();
            return
                SyntaxFactory.CompilationUnit()
                    .AddUsings(GetUsingDirectives().ToArray())
                    .AddMembers(
                        SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(testClass.NamespaceName))
                            .AddMembers(
                                SyntaxFactory.ClassDeclaration(testClass.ClassName)
                                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                    .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("TestFixtureAttribute")))))
                                    .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(attributes)))
                                    .AddMembers(testClass.Methods.Select(CreateMethod).ToArray()))).SyntaxTree;            
        }

        private static MemberDeclarationSyntax CreateMethod(Method method)
        {
            var methodTemplate = TestClassSyntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single(i => string.Equals(i.Identifier.ValueText, method.Template, StringComparison.InvariantCultureIgnoreCase));
            return methodTemplate.WithIdentifier(SyntaxFactory.IdentifierName(method.Name).Identifier);
        }

        private static IEnumerable<UsingDirectiveSyntax> GetUsingDirectives()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System"));
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("NUnit"), SyntaxFactory.IdentifierName("Framework")));
        }

        private IEnumerable<PortableExecutableReference> GetDotNetFrameworkReferences(TargetDotNetFrameworkVersion dotNetFrameworkVersion, DotNetFrameworkArchitecture architecture)
        {
            yield return MetadataReference.CreateFromFile(ToolLocationHelper.GetPathToDotNetFrameworkFile("mscorlib.dll", dotNetFrameworkVersion, architecture));
            yield return MetadataReference.CreateFromFile(ToolLocationHelper.GetPathToDotNetFrameworkFile("System.dll", dotNetFrameworkVersion, architecture));
            if(dotNetFrameworkVersion != TargetDotNetFrameworkVersion.Version11 && dotNetFrameworkVersion != TargetDotNetFrameworkVersion.Version20)
            {
                yield return MetadataReference.CreateFromFile(ToolLocationHelper.GetPathToDotNetFrameworkFile("System.Core.dll", dotNetFrameworkVersion, architecture));
            }

            yield return MetadataReference.CreateFromFile(ToolLocationHelper.GetPathToDotNetFrameworkFile("System.Configuration.dll", dotNetFrameworkVersion, architecture));
        }
    }
}
