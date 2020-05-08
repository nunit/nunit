---
uid: ProjectLoaders
---

**Project Loaders** are extensions that know how to load a project in a particular format and create a package suitable for running tests under NUnit. NUnit itself provides two of them:
  * NUnitProjectLoader
  * VisualStudioProjectLoader

The extension point for project loaders accepts extensions that implement the `NUnit.Engine.Extensibility.IProjectLoader` interface. The definition of a project loader extension might look something like the following...

```csharp
[Extension]
[ExtensionProperty("FileExtension", ".xxx")]
[ExtensionProperty("FileExtension", ".yyy")]
public class SomeProjectLoader : IProjectLoader
{
    ...
}
```

The engine will only load the extension if it encounters a potential project file using the indicated file extensions.

The `IProjectLoader` interface is defined as follows:

```csharp
/// <summary>
/// The IProjectLoader interface is implemented by any class
/// that knows how to load projects in a specific format.
/// </summary>
[TypeExtensionPoint(
    Description = "Recognizes and loads assemblies from various types of project formats.")]
public interface IProjectLoader
{
    /// <summary>
    /// Returns true if the file indicated is one that this
    /// loader knows how to load.
    /// </summary>
    /// <param name="path">The path of the project file</param>
    /// <returns>True if the loader knows how to load this file, otherwise false</returns>
    bool CanLoadFrom(string path);

    /// <summary>
    /// Loads a project of a known format.
    /// </summary>
    /// <param name="path">The path of the project file</param>
    /// <returns>An IProject interface to the loaded project or null if the project cannot be loaded</returns>
    IProject LoadFrom(string path);
}
```

An `IProject`, which is returned by `LoadFrom` is defined as:
```csharp
/// <summary>
/// Interface for the various project types that the engine can load.
/// </summary>
public interface IProject
{
    /// <summary>
    /// Gets the path to the file storing this project, if any.
    /// If the project has not been saved, this is null.
    /// </summary>
    string ProjectPath { get; }

    /// <summary>
    /// Gets the active configuration, as defined
    /// by the particular project.
    /// </summary>
    string ActiveConfigName { get; }

    /// <summary>
    /// Gets a list of the configs for this project
    /// </summary>
    IList<string> ConfigNames { get; }

    /// <summary>
    /// Gets a test package for the primary or active
    /// configuration within the project. The package 
    /// includes all the assemblies and any settings
    /// specified in the project format.
    /// </summary>
    /// <returns>A TestPackage</returns>
    TestPackage GetTestPackage();

    /// <summary>
    /// Gets a TestPackage for a specific configuration
    /// within the project. The package includes all the
    /// assemblies and any settings specified in the 
    /// project format.
    /// </summary>
    /// <param name="configName">The name of the config to use</param>
    /// <returns>A TestPackage for the named configuration.</returns>
    TestPackage GetTestPackage(string configName);
}
```

`TestPackage` is defined in the `nunit.engine.api` assembly and includes a list of assemblies together with a dictionary of settings used by the package.
