> **Note:** `Action Attributes` are a feature of NUnit V2, designed to better enable composability of test logic. They are carried over to NUnit 3, but are not the recommended approach for most new work. Instead, for most problems of extensibility, NUnit 3 [Custom Attributes](Custom-Attributes.md)  are a better approach and are more likely to be supported in future releases.

> However, at this time, `Action Attributes` provide the only approach that allows placing an attribute on a higher-level element and having it affect the behavior of lower-level elements. The rest of this page describes `Action Attribute` usage in NUnit 3.

Often when writing unit tests we have logic that we want to run upon certain events in the test cycle (e.g. SetUp, TearDown, FixtureSetUp, FixtureTearDown, etc.). NUnit has had the ability to execute code upon these events by decorating fixture classes and methods with the appropriate NUnit- provided attributes.

`Action Attributes` allow the user to create custom attributes to encapsulate specific actions for use before or after any test is run.

### The Problem of Composability

Suppose we have some tests in multiple fixtures that need the same in-memory test database to be created and destroyed on each test run. We could create a base fixture class and derive each fixture that depends on the test from that class. Alternatively, we could create a `SetUpFixture` class at the level of a common namespace shared by each fixture. 

This works fine, until we need some other reusable functionality, say the ability to configure or reset a 
ServiceLocator. We could put that functionality in the base fixture class or setup fixture, but now we're mixing two different responsibilities into the base class. In the case of a setup fixture, this only works if all classes requiring both features are located in a common namespace. In some cases we may *not* want the test database, but we do want ServiceLocator configuration; and sometimes we want the opposite. Still other times we'll want both - so we'd have to make the base class configurable.

If we now discover a third piece of functionality we need to reuse, like configuring the Thread's CurrentPrincipal in arbitrary ways, the complexity of the solution very quickly. We've violated the Single Responsibility Principle and are suffering for it. What we really want is the ability to separate the different pieces of reusable test logic and compose them together as our tests need them.

### Resolving the Problem

`Action Attributes` get us out of our bind. Consider this example:

```C#
[TestFixture, ResetServiceLocator]
public class MyTests
{
    [Test, CreateTestDatabase]
    public void Test1() { /* ... */ }

    [Test, CreateTestDatabase, AsAdministratorPrincipal]
    public void Test2() { /* ... */ }

    [Test, CreateTestDatabase, AsNamedPrincipal("charlie.poole")]
    public void Test3() { /* ... */ }

    [Test, AsGuestPrincipal]
    public void Test4() { /* ... */ }
}
```

Here we have used user-defined attributes to identify five different actions
that we want to compose together in different ways for different tests:
  * ResetServiceLocator
  * CreateTestDatabase
  * AsAdministratorPrincipal
  * AsNamedPrincipal
  * AsGuestPrincipal

We can reuse these actions in other test fixtures, simply by decorating
them with the appropriate attributes.without having to inherit from a base class.
We can even develop and distribute a library of common test actions.

### Implementing an Action Attribute

Action attributes are defined by the programmer. They implement the `ITestAction`
interface, which is defined as follows:

```C#
public interface ITestAction
{
    void BeforeTest(ITest test);

    void AfterTest(ITest test);

    ActionTargets Targets { get; }
}
```

For convenience, you may derive your own action attribute from NUnit's `TestActionAttribute`,
an abstract class with virtual implementations of each member of the interface. Alternatively, you
may derive from `System.Attribute` and implement the interface directly.

#### Action Targets

The value returned from the `Targets` property determines when the `BeforeTest` and
`AfterTest` methods will be called. The ActionTargets enum is defined as follows:

```C#
[Flags]
public enum ActionTargets
{
    Default = 0,

    Test = 1,

    Suite = 2
}
```

When an attribute that returns `ActionTargets.Suite` is applied to either a class or a parameterized 
method, NUnit will execute the attribute's `BeforeTest` method prior to executing the test suite
and then execute the `AfterTest` method after the test suite has finished executing. This is similar 
to how the `OneTimeSetUp` and `OneTimeTearDown` attributes work.

On the other hand, when an attribute that returns `ActionTargets.Test` is used in the same
situations, NUnit will execute the attribute's `BeforeTest` method prior to each contained
test case and the `AfterTest` method after each test case. This is similar to how the `SetUp`
and `TearDown` attributes work.

Action attributes that return `ActionTargets.Default` target the particular code item to
which they are attached. When attached to a method, they behave as if `ActionTargets.Test` had been
specified. When attached to a class or assembly, they behave as if `ActionTargets.Suite` was returned.

#### ITest Interface

The `BeforeTest` and `AfterTest` methods are provided with information
about the test that is about to run (before) or has just run (after). The `ITest`
interface is an internal NUnit interface to the representation of a test, which may be
either a test case or a suite. The before and after methods may use the interface 
to decide what actions to take or retrieve information about the test.

```C#
public interface ITest : IXmlNodeBuilder
{
    // Gets the id of the test
    int Id { get; }
    
    // Gets the name of the test
    string Name { get; }
    
    // Gets the fully qualified name of the test
    string FullName { get; }
    
    // Gets the Type of the test fixture, if applicable, or
    // null if no fixture type is associated with this test.
    Type FixtureType { get; }
    
    // Gets a MethodInfo for the method implementing this test.
    // Returns null if the test is not implemented as a method.
    MethodInfo Method { get; }
    
    // Gets the RunState of the test, indicating whether it can be run.
    RunState RunState { get; }
    
    // Count of the test cases (1 if this is a test case)
    int TestCaseCount { get; }
    
    // Gets the properties of the test
    IPropertyBag Properties { get; }
    
    // Gets the parent test, if any.
    ITest Parent { get; }
    
    // Returns true if this is a test suite
    bool IsSuite { get; }
    
    // Gets a bool indicating whether the current test
    // has any descendant tests.
    bool HasChildren { get; }
    
    // Gets this test's child tests
    System.Collections.Generic.IList<ITest> Tests { get; }
}
```

### Examples

The examples that follow all use the following sample Action Attribute:

```C#
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class |
                AttributeTargets.Interface | AttributeTargets.Assembly,
                AllowMultiple = true)]
public class ConsoleActionAttribute : Attribute, ITestAction
{
    private string _Message;

    public ConsoleActionAttribute(string message) { _Message = message; }

    public void BeforeTest(ITest test)
    {
        WriteToConsole("Before", test);
    }

    public void AfterTest(ITest test)
    {
        WriteToConsole("After", test);
    }

    public ActionTargets Targets
    {
        get { return ActionTargets.Test | ActionTargets.Suite; }
    }

    private void WriteToConsole(string eventMessage, ITest details)
    {
        Console.WriteLine("{0} {1}: {2}, from {3}.{4}.",
            eventMessage,
            details.IsSuite ? "Suite" : "Case",
            _Message,
            details.FixtureType != null ? details.FixtureType.Name : "{no fixture}",
            details.Method != null ? details.Method.Name : "{no method}");
    }
}
```

Note that the above Action Attribute returns the union of ActionTargets.Test and ActionTargets.Suite. This is permitted, but will probably not be the normal case. It is done here so we can reuse the attribute in multiple examples. The attribute takes a single constructor argument, a message, that will be used to write output to the console. All of the Before and After methods write output via the WriteToConsole method.
   
### Method Attached Actions

#### Example 1 (applied to simple test method):

```C#
[TestFixture]
public class ActionAttributeSampleTests
{
    [Test][ConsoleAction("Hello")]
    public void SimpleTest()
    {
        Console.WriteLine("Test ran.");
    }
}
```

##### Console Output:
```
  Before Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  Test ran.
  After Case: Hello, from ActionAttributeSampleTests.SimpleTest.
```

#### Example 2 (applied action twice to test method):

```C#
[TestFixture]
public class ActionAttributeSampleTests
{
    [Test] [ConsoleAction("Hello")]
    [ConsoleAction("Greetings")]
    public void SimpleTest()
    {
        Console.WriteLine("Test run.");
    }
}
```

##### Console Output:
```
  Before Case: Greetings, from ActionAttributeSampleTests.SimpleTest.
  Before Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  Test run.
  After Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  After Case: Greetings, from ActionAttributeSampleTests.SimpleTest.
```

##### Remarks
You are permitted to apply the same attribute multiple times. Note
that the order in which attributes are applied is indeterminate, although
it will generally be stable for a single release of .NET.

#### Example 3 (applied to a test method with test cases):

```C#
[TestFixture]
public class ActionAttributeSampleTests
{
    [Test] [ConsoleAction("Hello")]
    [TestCase("02")]
    [TestCase("01")]
    public void SimpleTest(string number)
    {
        Console.WriteLine("Test run {0}.", number);
    }
}
```

##### Console Output:
```
  Before Suite: Hello, from ActionAttributeSampleTests.SimpleTest.
  Before Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  Test run 01.
  After Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  Before Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  Test run 02.
  After Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  After Suite: Hello, from ActionAttributeSampleTests.SimpleTest.
```

##### Remarks
When one or more [TestCase] attributes are applied to a method, NUnit treats the method as a test suite.  
You'll notice that BeforeTest was run once before the suite and AfterTest was run once after it.
In addition, BeforeTest and AfterTest are run again for each individual test case.
Note that the order in which test cases are executed is indeterminate.

### Type Attached Actions

#### Example 1:

```C#
[TestFixture] [ConsoleAction("Hello")]
public class ActionAttributeSampleTests
{
    [Test]
    public void SimpleTestOne()
    {
        Console.WriteLine("Test One.");
    }
    
    [Test]
    public void SimpleTestTwo()
    {
        Console.WriteLine("Test Two.");
    }
}
```

##### Console Output:
```
  Before Suite: Hello, from ActionAttributeSampleTests.{no method}.
  Before Case: Hello, from ActionAttributeSampleTests.SimpleTestOne.
  Test ran.
  After Case: Hello, from ActionAttributeSampleTests.SimpleTestOne.
  Before Case: Hello, from ActionAttributeSampleTests.SimpleTestTwo.
  Test ran.
  After Case: Hello, from ActionAttributeSampleTests.SimpleTestTwo.
  After Suite: Hello, from ActionAttributeSampleTests.{no method}.
```

##### Remarks
In this case, the class is the test suite. BeforeTest and AfterTest are run once each for this class and then again for each test.

#### Example 2 (attached to interface):

```C#
[ConsoleAction("Hello")]
public interface IHaveAnAction
{
}

[TestFixture]
public class ActionAttributeSampleTests : IHaveAnAction
{
    [Test] 
    public void SimpleTest()
    {
        Console.WriteLine("Test run.");
    }
}
```

##### Console Output:
```
  Before Suite: Hello, from ActionAttributeSampleTests.{no method}.
  Before Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  Test run.
  After Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  After Suite: Hello, from ActionAttributeSampleTests.{no method}.
```

##### Remarks
Action attributes can be applied to an interface.  If a class marked with [TestFixture] implements an interface that has an action attribute applied to the interface, the class inherits the action attribute from the interface.  It behaves as if you applied the action attribute to the class itself.

#### Example 3 (action attribute is applied to interface and attribute uses interface to provide data to tests):

```C#
[AttributeUsage(AttributeTargets.Interface)]
public class InterfaceAwareActionAttribute : TestActionAttribute
{
    private readonly string _Message;

    public InterfaceAwareActionAttribute(string message) { _Message = message; }

    public override void BeforeTest(ITest details)
    {
        IHaveAnAction obj = details.Fixture as IHaveAnAction;
        if(obj != null)
            obj.Message = _Message;
    }

    public override ActionTargets Targets
    {
        get { return ActionTargets.Test; }
    }
}

[InterfaceAwareAction("Hello")]
public interface IHaveAnAction { string Message { get; set; } }

[TestFixture]
public class ActionAttributeSampleTests : IHaveAnAction
{
    [Test] 
    public void SimpleTest()
    {
        Console.WriteLine("{0}, World!", Message);
    }

    public string Message { get; set; }
}
```

##### Console Output:
```C#
  Hello, World!
```

##### Remarks</h5>
Here we see a new action attribute, `InterfaceAwareAction`.  This attribute uses the Fixture property of the TestDetails passed into BeforeTest and casts it to an interface, IHaveAnAction.  If the fixture implements the IHaveAnAction interface, the attribute sets the Message property to the string passed into the constructor of the attribute.  Since the attribute is applied to the interface, any class that implements this interface gets its Message property set to the string provided to the constructor of the attribute.  This is useful when the action attribute provides some data or service to the tests.

Note that this attribute inherits from `TestActionAttribute`. It uses the default (do-nothing) implementation
of `AfterTest` and overrides both `BeforeTest` and `Target`.

### Assembly Attached Action

#### Example 1:

```C#
[assembly: ConsoleAction("Hello")]

[TestFixture]
public class ActionAttributeSampleTests
{
    [Test] 
    public void SimpleTest()
    {
        Console.WriteLine("Test run.");
    }
}
```

##### Console Output:
```C#
  Before Suite: Hello, from {no fixture}.{no method}.
  Before Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  Test run.
  After Case: Hello, from ActionAttributeSampleTests.SimpleTest.
  After Suite: Hello, from {no fixture}.{no method}.
```

##### Remarks
The `ConsoleAction` attribute in this example is applied to the entire assembly.  NUnit treats an assembly as a test suite (in fact, a suite of suites).  Since the `ConsoleAction` attribute implements both ITestSuiteAction and ITestCaseAction, NUnit will run BeforeTest once before any tests are run in the assembly, and AfterTest after all tests are run in the assembly.  Additionally, BeforeTest and AfterTest will be run for every test case in the assembly.  It is unlikely that action attributes are applied to assemblies often.  However, it is useful to build action attributes that ensure state gets cleaned up before and after each tests to prevent individual tests from affecting the outcome of other test.  For example, if you have any static or cached data or services, an action attribute can be used to clean them up for each test.