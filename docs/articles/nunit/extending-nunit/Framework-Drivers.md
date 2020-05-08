**Framework Drivers** are extensions that know how to create a driver for a particular framework. The NUnit engine provides drivers for both the NUnit 3.x and NUnit 2.x frameworks. Third parties may provide drivers for other frameworks by creating extensions.

The `ExtensionPoint` for framework drivers uses the Path "NUnit.Engine.DriverService" and accepts an extension of Type `NUnit.Engine.Extensibility.IDriverFactory`. The definition of a driver factory might look like this:

```csharp
[Extension]
public class MyOwnFrameworkDriverFactory : IDriverFactory
{
    ...
}
```

The `IDriverFactory` interface is defined as follows:

```csharp
public interface IDriverFactory
{
    // Gets a flag indicating whether the provided assembly name and version
    // represent a test framework supported by this factory
    bool IsSupportedTestFramework(string assemblyName, Version version);

    // Gets a driver for use with the specified framework assembly name and version
    IFrameworkDriver GetDriver(AppDomain domain, string assemblyName, Version version);
}
```

The `IFrameworkDriver` interface is defined as follows:
```csharp
public interface IFrameworkDriver
{
    // Gets and sets the unique identifer for this driver
    string ID { get; set; }

    // Loads the tests in an assembly
    string Load(string testAssemblyPath, IDictionary<string, object> settings);

    // Counts the test cases that would be executed
    int CountTestCases(string filter);

    // Executes the tests in an assembly
    string Run(ITestEventListener listener, string filter);

    // Returns information about the tests in an assembly
    string Explore(string filter);

    // Cancels an ongoing test run.
    void StopRun(bool force);
```

The strings returned by Run and Explore are XML representations and the filter is also in XML format. See the source code for NUnit3FrameworkDriver and NUnit2FrameworkDriver for details.