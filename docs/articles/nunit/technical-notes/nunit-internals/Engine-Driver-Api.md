The TestEngine uses drivers to interface with frameworks. This isolates framework-specific code from the more general code used to load and run tests. Each test framework that is used with the engine needs a driver to support it. While a driver could theoretically support multiple frameworks, we expect the normal case to be a one-to-one mapping of drivers to frameworks.

Because they are quite different internally, NUnit itself treats its own 2.x and 3.x framework versions as separate frameworks, with each using a different driver. The NUnit 3 driver is the only one built into the engine. All other drivers must be written and installed as extensions.

Designing an API for drivers involved compromise. The API had to be simple enough to be easily implemented by those wanting to use NUnit with a particular framework. On the other hand, if it was too simple, some advanced capabilities of a given framework might not be easily accessible.

### Implementation Details

The driver API is encapsulated in the `IDriverFactory` and `IFrameworkDriver` interfaces, which must be implemented by all framework driver extensions to the engine.

#### IDriverFactory

The IDriverFactory interface is called by the engine to determine if a particular extension is able to create a driver for a particular framework assembly. The engine passes the AssemblyName of each assembly referenced by the test assembly to the factory to see if it is a supported framework. If it finds one, then it uses that driver. If not, it goes on to check the next driver extension.

```csharp
namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// Interface implemented by a Type that knows how to create a driver for a test assembly.
    /// </summary>
    [TypeExtensionPoint(Description = "Supplies a driver to run tests that use a specific test framework.")]
    public interface IDriverFactory
    {
        /// <summary>
        /// Gets a flag indicating whether a given AssemblyName
        /// represents a test framework supported by this factory.
        /// </summary>
        /// <param name="reference">An AssemblyName referring to the possible test framework.</param>
        bool IsSupportedTestFramework(AssemblyName reference);

        /// <summary>
        /// Gets a driver for a given test assembly and a framework
        /// which the assembly is already known to reference.
        /// </summary>
        /// <param name="domain">The domain in which the assembly will be loaded</param>
        /// <param name="reference">An AssemblyName referring to the test framework.</param>
        /// <returns></returns>
        IFrameworkDriver GetDriver(AppDomain domain, AssemblyName reference);
    }
}
```

#### IFrameworkDriver

The `IFrameworkDriver` interface is returned from `IDriverFactory` and is the key interface for actually loading, exploring and running the tests in the test assembly. In theory, a single driver factory could return different drivers in different situations, but we expect a one-to-one mapping of factories to drivers to be most commonly used.

```csharp
namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// The IFrameworkDriver interface is implemented by a class that
    /// is able to use an external framework to explore or run tests
    /// under the engine.
    /// </summary>
    public interface IFrameworkDriver
    {
        /// <summary>
        /// Gets and sets the unique identifier for this driver,
        /// used to ensure that test ids are unique across drivers.
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Loads the tests in an assembly.
        /// </summary>
        /// <returns>An Xml string representing the loaded test</returns>
        string Load(string testAssemblyPath, IDictionary<string, object> settings);

        /// <summary>
        /// Count the test cases that would be executed.
        /// </summary>
        /// <param name="filter">An XML string representing the TestFilter to use in counting the tests</param>
        /// <returns>The number of test cases counted</returns>
        int CountTestCases(string filter);

        /// <summary>
        /// Executes the tests in an assembly.
        /// </summary>
        /// <param name="listener">An ITestEventHandler that receives progress notices</param>
        /// <param name="filter">A XML string representing the filter that controls which tests are executed</param>
        /// <returns>An Xml string representing the result</returns>
        string Run(ITestEventListener listener, string filter);

        /// <summary>
        /// Returns information about the tests in an assembly.
        /// </summary>
        /// <param name="filter">An XML string representing the filter that controls which tests are included</param>
        /// <returns>An Xml string representing the tests</returns>
        string Explore(string filter);

        /// <summary>
        /// Cancel the ongoing test run. If no test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        void StopRun(bool force);
    }
}
```

As designed, the `IFrameworkDriver` interface maps most directly to the requirements of the NUnit 3 framework. Drivers for other frameworks need to function as an adapter to run tests and return understandable results to the engine.

The filter argument passed to several of the interface methods is an XML string representing the filter. See [Test Filters](../usage/Test-Filters.md) for a description of the format, which is directly understood by the NUnit 3 framework, but which must be converted by the driver to something that is understood by other frameworks.

#### ITestEventListener

The `ITestEventListener` interface is implemented by the Engine and used by each driver to report significant events during the execution of tests.

```csharp
namespace NUnit.Engine
{
    /// <summary>
    /// The ITestEventListener interface is used to receive notices of significant
    /// events while a test is running. Its single method accepts an Xml string, 
    /// which may represent any event generated by the test framework, the driver
    /// or any of the runners internal to the engine. Use of Xml means that
    /// any driver and framework may add additional events and the engine will
    /// simply pass them on through this interface.
    /// </summary>
    [TypeExtensionPoint(Description = "Allows an extension to process progress reports and other events from the test.")]
    public interface ITestEventListener
    {
        /// <summary>
        /// Handle a progress report or other event.
        /// </summary>
        /// <param name="report">An XML progress report.</param>
        void OnTestEvent(string report);
    }
}
```

