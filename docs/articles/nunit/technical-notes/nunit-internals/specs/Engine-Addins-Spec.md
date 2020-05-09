> **NOTE:** This page is a specification that was used as a starting point for creating the feature in NUnit. It needs to be reviewed and revised in order to accurately reflect what was actually built. If you take it with a grain of salt, it may still be helpful to you as documentation. This notice will be removed when the page is brought up to date.

This spec covers a proposed new approach to Engine addins, replacing the use of Mono.Addins.

## Background
We originally planned to use Mono.Addins for the engine and have done so in the first betas. However, Mono.Addins no longer supports .NET 2.0. We are using a modified version that we created and which we will have to maintain in the future. Mono.Addins has many more features than we expect to use and has a rather large memory and disk footprint.

In sum, Mono.Addins is not really carrying its weight for our usage. We will try to replace it with a simple plugin architecture of our own design, using some pieces of our old NUnit 2.x addin feature as well as other features inspired by Mono.Addins.

At a minimum, we need to support existing addins that are supported by Mono.Addins. Once we have that support, we can replace Mono.Addins. Further features could be added later.

Three addin types are currently supported:

1. Project Loaders - used to load NUnit and VS projects
2. Result Writers - used to write out results in NUnit 2 format
3. Driver Factories - used to create the driver for running NUnit 2 tests under NUnit 3

## Design

### Extension Points

In both Mono and NUnit (2.x) addins, extensibility centers around `ExtensionPoints`. An `ExtensionPoint` is a place in the application where add-ins can register themselves in order to provide added functionality. extension nodes to provide extra functionality. NUnit 3.0 will continue to use this concept. 

In our initial implementation, all extension points must be known in advance and are contained in the engine. At a future point, we will probably want to add a way to dynamically create new extension points so that addins can themselves host extensions.

`ExtensionPoints` in the engine will be identified by use of `ExtensionPointAttribute` at the assembly level. Each attribute identifies one extension point, specifying an identifying string (the Path) and the required Type of any extension objects to be registered with it.

###### Example:
```C#
[assembly: ExtensionPoint(Path="/NUnit/Engine/DriverService"
                          Type="NUnit.Engine.Extensibility.IDriverFactory")]
```

In this example, the Path identifying the extension point is "/NUnit/Engine/DriverService." Any type to be plugged into this extension point must implement `IDriverFactory`. Note that even though each extension point is typically implemented by some class in the system, the identity of that class is an implementation detail, which is not revealed in the `ExtensionPoint`.

### Extensions

An `Extension` is a single object of the required type, which is registered with an `ExtensionPoint`. Extensions are identified by the `ExtensionAttribute` which is applied to the class. Extensions identified in this way must have a default constructor. See the `Addins` section below for dealing with more complex situations.

###### Example:
```C#
    [Extension(Path = "/NUnit/Engine/DriverService")]
    public class NUnit2DriverFactory : IDriverFactory
    {
        ...
    }
```

The example above shows an extension Type being used with the `ExtensionPoint` defined in the previous example. An instance of NUnit2DriverFactory will be supplied to the DriverService when needed. The Path property must match the Path for the intended `ExtensionPoint`.

The Path is actually optional so long as NUnit is able to deduce the correct ExtensionPoint based on the Type.
In fact, that's the case in this example, which can be rewritten more simply as...

```C#
    [Extension]
    public class NUnit2DriverFactory : IDriverFactory
    {
        ...
    }
```

The Path may be omitted provided that no other extension point is able to accept an object of the same class as the extension.

#### Locating Addins

Assemblies containing Addins and Extensions are stored in one or more locations indicated in files of type `.addins`. Each line of the file contains the path of an addin assembly or a directory containing assemblies. Wildcards may be used for assembly entries and relative paths are interpreted based on the location of the `.addins` file. The default `nunit.engine.addins` is located in the engine directory and lists addins we build with NUnit, which are contained in the addins directory.

Any assemblies specified in a `.addins` file will be scanned fully, looking for addins and extensions. Any directories specified will be browsed, first looking for any `.addins` files. If one or more files are found, the content of the files will direct all further browsing. If no such file is found, then all `.dll` files in the directory will be scanned, just as if a `.addins` file contained "*.dll."

Assemblies are be examined using Cecil. Any assembly that cannot be opened is be ignored, with a log message generated. This be a normal occurrence in cases where the assembly targets a higher level runtime than that which is in use. Info is saved for actual instantiation of extensions on a just-in-time basis.

We hope that the combination of specifically indicating which assemblies to scan and the use of Cecil to do the scanning will make this process quite efficient. If that turns out not to be the case, we can use an assembly-level attribute to identify assemblies containing extensions.

## Future Enhancements

### Addins

An `Addin` is a Type that provides `Extensions`. As indicated in the previous section, simple extensions providing a single instance of the object through a default constructor do not require an Addin. For more complex situations, an Addin object could be allowed to create and register one or more extensions. This is the approach that we took in NUnit 2.x.

Addins would be identified by the `AddinAttribute` and implement the IAddin interface. They actively participate in the installation of extensions and may be used to create objects that require parameters, to install multiple extensions or to select among different extensions.

###### Example:
```C#
    [Addin]
    public class MyAddin : IAddin
    {
        public bool Install(IExtensionHost host)
        {
            var ep = host.GetExtensionPoint("/NUnit/Engine/DriverFactory");
            ep.Install(new NUnit2DriverFactory());
        }
    }

    public class NUnit2DriverFactory : IDriverFactory
    {
        ...
    }
```

The above example does the same thing as the previous example in a more complicated way. Obviously, you would use this approach only in more complex situations. Note that the factory class does not have an `ExtensionAttribute` as this would lead to its being installed twice.

**Notes:** 
1. This design feature is not required initially and will be omitted from the implementation until we actually require it. 

2. The interfaces used in this section are notionally based on NUnit 2.6.4.

### Addins on Addins

Initially, we only need one level of addins to allow for everything we now do. It may be convenient at some point to support addins on top of other addins. For example, a given framework driver might be enhanced with some special feature through an addin.

An `ExtensionPoint` in an extension assembly could be identified in the same way as an engine extension point is known, using the `ExtensionPointAttribute` at assembly level. However, this is not a feature we need for the 3.0 release, only one to keep in mind for the future.