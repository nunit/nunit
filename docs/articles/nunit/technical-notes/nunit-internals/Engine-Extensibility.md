---
uid: engineextensibility
---

The NUnit Test Engine uses a plugin architecture to allow new functionality to be added by third parties. We originally planned to use `Mono.Addins` for this purpose and did so in the first betas. Because `Mono.Addins` no longer supports .NET 2.0, we were using a modified version that we created ourselves and which we would have to maintain in the future. Since `Mono.Addins` has many more features than we expect to use we decided to return to a custom plugin architecture.

The NUnit 3.0 Engine Extensibility model is essentially based on the NUnit V2 addin design with a number of improvements, primarily inspired by `Mono.Addins`. On this page, we describe that model as a guide for folks working on NUnit or otherwise needing to understand it. See [[Writing Engine Extensions]] for user-focused information about how to create an extension.

## Extension Points

NUnit 3.0 Extensibility centers around `ExtensionPoints`. An `ExtensionPoint` is a place in the application where add-ins can register themselves in order to provide added functionality. Each extension point is identified by a string, called the `Path`, is associated with a particular `Type` to be used by extensions and may also have an optional `Description`.

How an extension is used depends on the particular extension point. For example, some extension points may only allow one extension to be registered while others may support multiple extensions. Where multiple extensions are supported, they may be used as alternatives or they may all operate, depending on the precise purpose of the extension point. In other words, read the docs and understand the exact semantics of an extension point before trying to create an extension for it.

In our initial implementation, all extension points are contained in the engine. In a future release, we will allow addins to define extension points and host extensions as well.

`ExtensionPoints` are identified by use of either the `TypeExtensionPointAttribute` or the `ExtensionPointAttribute`.

#### TypeExtensionPointAttribute

This is the most common way we identify `ExtensionPoints` in NUnit. The `TypeExtensionPointAttribute` is applied to an interface or class, exposed to the user in the nunit.engine.api assembly. This indicates that extensions must implement the specified interface or derive from the class.

For example, here is the code used to define the extension point for driver factories - classes that know how to create an appropriate driver for a test assembly.

```csharp
    /// <summary>
    /// Interface implemented by a Type that knows how to create a driver for a test assembly.
    /// </summary>
    [TypeExtensionPoint(
        Description = "Supplies a driver to run tests that use a specific test framework.")]
    public interface IDriverFactory
    {
        /// <summary>
        /// Gets a flag indicating whether a given assembly name and version
        /// represent a test framework supported by this factory.
        /// </summary>
        bool IsSupportedTestFramework(string assemblyName, Version version);

        /// <summary>
        /// Gets a driver for a given test assembly and a framework
        /// which the assembly is already known to reference.
        /// </summary>
        /// <param name="domain">The domain in which the assembly will be loaded</param>
        /// <param name="assemblyName">The Name of the test framework reference</param>
        /// <param name="version">The version of the test framework reference</param>
        /// <returns></returns>
        IFrameworkDriver GetDriver(AppDomain domain, string assemblyName, Version version);
    }
```

In this case, we used the default constructor. An alternate constructor allows specifying the `Path` but is usually not needed. In this case, NUnit will assign a default `Path` of "/NUnit/Engine/TypeExtensions/IDriverFactory". Extensions that implement `IDriverFactory` will be automatically associated with this extension point.

`Description` is the only named property for this attribute.

#### ExtensionPointAttribute

Extensions may also defined by use of the `ExtensionPointAttribute` at the assembly level. The `Path` and the `Type` must be specified in the attribute constructor. Each attribute identifies one extension point supported by that assembly, specifying an identifying string (the Path) and the required Type of any extension objects to be registered with it.

The following example shows an alternative way we might have identified the same driver factory extension point shown above. _This is not actual NUnit code, but only a hypothetical example._

```csharp
[assembly: ExtensionPoint(
               "/NUnit/Engine/TypeExtensions/IDriverFactory",
               typeof(IDriverFactory),
               Description="Supplies a driver to run tests that use a specific test framework.")]
```

This example defines **exactly** the same extension point as in the `TypeExtensionPointAttribute` example, albeit in a more roundabout way.

Again, `Description` is the only named property for this attribute.

#### Supported Extension Points

The following extension types are supported by the engine:

* [Project Loaders](xref:ProjectLoaders)
* [Result Writers](xref:ResultWriters)
* [Framework Drivers](xref:FrameworkDrivers)
* [Event Listeners](xref:EventListeners) (NUnit 3.4 and later only)

## Extensions

An `Extension` is a single object of the required `Type`, which is registered with an `ExtensionPoint`. Extensions are identified by the `ExtensionAttribute` and additional information may be provided by use of the `ExtensionPropertyAttribute`, both of which are applied to the class that implements the extension.

All `Extensions` must have a default constructor, which is used by NUnit to create the object when it is needed.

#### ExtensionAttribute

The `ExtensionAttribute` has only a default constructor, as well as two named properties, `Path` and `Description`. If the path is not provided, NUnit will try to find the appropriate extension point based on what Types are inherited or implemented by the class on which the attribute is placed.

Assuming the extension point definition used above, any of the following would identify the classes as driver factories.

```csharp
    [Extension(Path = "/NUnit/Engine/TypeExtensions/IDriverFactory")]
    public class DriverFactory1 : IDriverFactory
    {
        ...
    }

    [Extension]
    public class DriverFactory2 : IDriverFactory
    {
        ...
    }
```

Generally, the `Path` will be omitted and the default value used. It may be needed in some cases, where classes implement multiple interfaces or inherit other classes that do so. Usually, this is not necessary if you follow the Single Responsibility principle.

#### ExtensionPropertyAttribute

Using only the `ExtensionAttribute`, NUnit would have to create instances of every extension in order to query it (for example) about its capabilities. Since extensions are generally in separate assemblies, this means that many potentially unneeded assemblies would be loaded.

The `ExtensionPropertyAttribute` avoids this problem by allowing the extension to specify information about what it is able to do. NUnit scans the attributes using `Mono.Cecil` without actually loading the assembly, so that resources are not taken up by unused assemblies.

To illustrate this, we will use the example of the engine's project loader `ExtensionPoint`. You can read about this extension point in detail at [[Project-Loaders]] but the essential thing for this example is that the extension point is passed a file path and must determine whether that file is in a format that it can interpret and load. We do that by loading the extension and calling its `CanLoadFrom(string path)` method.

If we only knew what file extensions were used by the particular format, we could avoid loading the extension unnecessarily. That's where `ExtensionPropertyAttribute` comes in. The following is an example taken from NUnit's own extension for loading NUnit projects.

```csharp
    [Extension]
    [ExtensionProperty("FileExtension", ".nunit")]
    public class NUnitProjectLoader : IProjectLoader
    {
        ...
    }
```
By use of the `ExtensionPropertyAttribute` the assembly containing this extension will never be loaded unless the user asks NUnit to run tests in a file of type `.nunit`. If this attribute were not present, then the engine would have to load the assembly, construct the object and call its `CanLoadFrom` method.

Of course, this means that the extension author must know a great deal about how each extension point works. That's why we provide a page for each supported extension points with details of how to use it.

#### Locating Addins

Assemblies containing Addins and Extensions are stored in one or more locations indicated in files of type `.addins`. Each line of the file contains the path of an addin assembly or a directory containing assemblies. Wildcards may be used for assembly entries and relative paths are interpreted based on the location of the `.addins` file. The default `nunit.engine.addins` is located in the engine directory and lists addins we build with NUnit, which are contained in the addins directory.

The following is an example of a possible `.addins` file, with comments indicating what each line does:
```
# This line is a comment and is ignored. The next (blank) line is ignored as well.

*.dll                   # include all dlls in the same directory
addins/*.dll            # include all dlls in the addins directory too
special/myassembly.dll  # include a specific dll in a special directory
/some/other/directory/  # process another directory, which may contain its own addins file
                        # note that an absolute path is allowed, but is probably not a good idea
                        # in most cases 
```

Any assemblies specified in a `.addins` file will be scanned fully, looking for addins and extensions. Any directories specified will be browsed, first looking for any `.addins` files. If one or more files are found, the content of the files will direct all further browsing. If no such file is found, then all `.dll` files in the directory will be scanned, just as if a `.addins` file contained "*.dll."

Assemblies are examined using Cecil, without actually loading them. Info is saved for actual instantiation of extensions on a just-in-time basis.
