The NUnit Test Engine uses a plugin architecture that allows users and third parties to add new functionality to the engine. The extensibility model defines a number of Extension Points to which Extensions may be added. This page gives general information that applies to all types of extensions you may want to write. The individual pages for each type of extension give specific details. For a detailed description of the engine extensibility architecture, see [[Engine Extensibility]].

## Extension Attribute

Every extension is implemented by a class with specific characteristics:
 * Has a default constructor so that NUnit can create an instance.
 * Implements some interface that varies according to the particular extension.
 * Is marked with the `ExtensionAttribute` so that NUnit can recognize it as an extension.

The code for a typical extension might look like this.

```csharp
    [Extension]
    public class MyExtension : ISomeInterface // Depending on the extension point
    {
        // Your code here
    }
```

The attribute has four named properties, all optional:

* **Path** This is a string that uniquely identifies the extension point to which the extension applies. It is only rarely needed, since NUnit can usually deduce the type of extension based on what interface is implemented by the extension class.

* **Description** An optional description of what the extension does.

* **Enabled** A boolean flag indicating whether the extension is enabled. This defaults to true. The setting is used by advanced extensions with functionality that is turned on and off depending on user input.

* **EngineVersion** The minimum engine version supported by the extension. Although optional, you should use this property if your extension will not work with all versions of the engine. If you don't use it and your extension requires engine services that are not present, then it might throw an exception or cause other errors.

> [!NOTE]
> Only engine versions 3.4 or later check this property. The only way to avoid errors in the case of lower engine versions is to not install such extensions.

## ExtensionPropertyAttribute

Using only the `ExtensionAttribute`, NUnit would have to create instances of every extension in order to ask questions like "What file extensions do you support." This would mean loading many potentially unneeded assemblies.

The `ExtensionPropertyAttribute` avoids the problem. NUnit's own extension for loading NUnit projects is a good example:

```csharp
    [Extension]
    [ExtensionProperty("FileExtension", ".nunit")]
    public class NUnitProjectLoader : IProjectLoader
    {
        ...
    }
```

By use of the `ExtensionPropertyAttribute` NUnit is able to postpone loading the extension until the user actually uses a file of type `.nunit`. If the extension is never needed, then it won't be loaded at all. For information about what properties are used by each extension point, see the individual pages for each type of extension.

> [!NOTE]
> Extensions are usually created each in their own assembly for efficiency. It's possible to have several related extensions in the same assembly, but they will all be loaded into memory as soon as one is used.

## Kinds of Extensions

As of version 3.4, the NUnit Engine supports four types of extensions. The individual pages for each type give specific details on implementing each of them.

* [Project Loaders](Project-Loaders.md)
* [Result Writers](Result-Writers.md)
* [Framework Drivers](Framework-Drivers.md)
* [Event Listeners](Event-Listeners.md)

## Installing Extensions

Once an extension is written and compiled, it has to be placed somewhere such that NUnit will find it.

 > This section is under construction. For now see https://github.com/nunit/docs/wiki/Engine-Extensibility#locating-addins
