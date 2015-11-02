namespace nunit.integration.tests.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Build.Utilities;

    internal class EnvironmentManager
    {
        private static readonly string[] NUnitFiles = { Const.NUnitConsoleFileName, "nunit.engine.api.dll", "nunit.engine.dll", "Mono.Cecil.dll", "nunit-agent.exe", "nunit-agent.exe.config", "nunit-agent-x86.exe", "nunit-agent-x86.exe.config", "nunit.engine.addins", "addins" };

        public void CopyNUnitFrameworkAssemblies(string directoryName, string originNUnitPath, TargetDotNetFrameworkVersion frameworkVersion)
        {
            foreach (var assemblyPath in EnumerateNUnitAssemblies(originNUnitPath, frameworkVersion))
            {
                var targetFileName = Path.GetFileName(assemblyPath);
                if (targetFileName == null)
                {
                    continue;
                }

                File.Copy(assemblyPath, Path.Combine(directoryName, targetFileName), true);
            }
        }

        public void CopyReference(string directoryName, string referenceFileName)
        {
            var targetFileName = Path.GetFileName(referenceFileName);
            if (targetFileName == null)
            {
                return;
            }

            File.Copy(referenceFileName, Path.Combine(directoryName, targetFileName), true);
        }

        public IEnumerable<string> EnumerateNUnitAssemblies(string nunitBasePath, TargetDotNetFrameworkVersion frameworkVersion)
        {
            var nunitPath = Path.Combine(nunitBasePath, PathUtilities.GetNUnitAssembliesPath(frameworkVersion));
            yield return Path.Combine(nunitPath, "nunit.framework.dll");
        }

        public void CreateDirectory(string directoryName)
        {
            if (directoryName == null)
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            if (Directory.Exists(directoryName))
            {
                Directory.Delete(directoryName, true);                
            }

            Directory.CreateDirectory(directoryName);
        }

        public string PrepareNUnitClonsoleAndGetPath(string sandboxPath, string nunitPath)
        {
            var nunitBasePath = Path.GetFullPath(Path.Combine(sandboxPath, "nunit"));
            CreateDirectory(nunitBasePath);
            foreach (var nunitFile in NUnitFiles)
            {
                var src = Path.Combine(nunitPath, nunitFile);
                var dts = Path.Combine(nunitBasePath, nunitFile);
                if (File.Exists(src))
                {
                    File.Copy(src, dts, true);                
                }

                if (Directory.Exists(src))
                {
                    DeepCopy(new DirectoryInfo(src), new DirectoryInfo(dts));                    
                }                
            }

            return nunitBasePath;
        }
       
        public void RemoveFileOrDirectoryFromNUnitDirectory(string fileToRemove, string nunitConsolePath)
        {
            var path = Path.Combine(nunitConsolePath, fileToRemove);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static void DeepCopy(DirectoryInfo source, DirectoryInfo target)
        {
            if (!target.Exists)
            {
                target.Create();                
            }

            foreach (var dir in source.GetDirectories())
            {
                var subDir = target.CreateSubdirectory(dir.Name);
                DeepCopy(dir, subDir);
            }

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name));
            }            
        }

    }
}
