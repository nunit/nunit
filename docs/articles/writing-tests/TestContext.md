Each NUnit test runs in an execution context, which includes information about the environment as well as the test itself. The `TestContext` class allows tests to access certain information about the execution context.

> [!NOTE]
> It's important to remember that "test" in NUnit may refer to an individual test cases or a suite such as a `TestFixture`. Within a test method, `SetUp` method or `TearDown` method, the context is that of the individual test case. Within a `OneTimeSetUp` or `OneTimeTearDown` method, the context refers to the fixture as a whole. This can lead to confusion, since most runners display information about fixtures differently from test cases. In fact, some runners may not display information about fixtures at all!

### Static Properties

#### CurrentContext

Gets the context of the currently executing test. This context
is created separately for each test before it begins execution.
See below for properties of the current context.

#### Out

Gets a TextWriter used for sending output to the current test result.

#### Error

Gets a TextWriter used for sending error output intended for immediate display.

#### Progress

Gets a TextWriter used for  sending normal (non-error) output intended for immediate display.

#### TestParameters

Test parameters may be supplied to a run in various ways, depending on the runner used. For example, the console runner provides a command-line argument and v3.4 of the NUnit 3 VS Adapter will supports specifying them in a .runsettings file. The static TestParameters property returns an object representing those passed-in parameters.

The TestParameters object supports the following properties:

 * `Count` - The number of parameters.
 * `Names` - A collection of the names of the supplied parameters.
 * `this[string name]` - The value of a parameter. In Vb, use `Item`.

The TestParameters object supports the following methods:

 * `Exists(string name)` - Returns true if a parameter of that name exists.
 * `Get(string name)`- Returns the same value as the indexer.
 * `Get<T>(string name, T defaultValue)` - Returns the value of the parameter converted from a string to type T or the specified default if the parameter doesn't exist. Throws an exception if conversion fails.

**Note** that all parameter values are strings. You may convert them to other Types using the generic `Get` method listed above or using your own code. An exception may be thrown if the supplied value cannot be converted correctly.

### Static Methods

#### Write

Writes text to the current test result.

```csharp
    Write(bool value)
    Write(char value)
    Write(char[] value)
    Write(double value)
    Write(Int32 value)
    Write(Int64 value)
    Write(decimal value)
    Write(object value)
    Write(Single value)
    Write(string value)
    Write(UInt32 value)
    Write(UInt64 value)
    Write(string format, object arg1)
    Write(string format, object arg1, object arg2)
    Write(string format, object arg1, object arg2, object arg3)
    Write(string format, params object[] args)
```
#### WriteLine

Writes text to the current test result, followed by a newline.

```csharp
    WriteLine()
    WriteLine(bool value)
    WriteLine(char value)
    WriteLine(char[] value)
    WriteLine(double value)
    WriteLine(Int32 value)
    WriteLine(Int64 value)
    WriteLine(decimal value)
    WriteLine(object value)
    WriteLine(Single value)
    WriteLine(string value)
    WriteLine(UInt32 value)
    WriteLine(UInt64 value)
    WriteLine(string format, object arg1)
    WriteLine(string format, object arg1, object arg2)
    WriteLine(string format, object arg1, object arg2, object arg3)
    WriteLine(string format, params object[] args)
```
#### AddFormatter (3.2+)

Adds a formatter for values based on some criterion, such as the Type of the value. The provided formatter will be used when an expected or actual value needs to be displayed as part of a message from a constraint.

```csharp
    AddFormatter(ValueFormatter formatter);
    AddFormatter(ValueFormatterFactory formatterFactory);
```

Both `ValueFormatter` and `ValueFormatterFactory` are delegates. `ValueFormatter` takes a single object as an argument and returns its string representation. The `AddFormatter` overload that takes a ValueFormatter is intended for use in most cases that arise.

#### AddTestAttachment (3.7+)

Attaches a file, with optional description, to the current test.

```csharp
    AddTestAttachment(string filePath, string description = null);
```

The file will be attached to the test result in the xml report. Test runners, such as the NUnit 3 VS Adapter, may also present the file to the user.

Notes:
1. The file must exist at the time of attachment.
1. File paths will be resolved as fully rooted paths, relative to `TestContext.CurrentContext.WorkDirectory`, which can be set by the user.

### Properties of the CurrentContext

#### Test

Gets a representation of the current test, with the following properties:

 * **ID** - The unique Id of the test
 * **Name** - The name of the test, whether set by the user or generated automatically
 * **FullName** - The fully qualified name of the test
 * **MethodName** - The name of the method representing the test, if any
 * **Properties** - An `IPropertyBag` of the test properties

#### Result

Gets a representation of the test result, with the following properties:

 * **Outcome** - A `ResultState` representing the outcome of the test. `ResultState` has the following properties:
   * **Status** - A `TestStatus` with four possible values:
     * Inconclusive
     * Skipped
     * Passed
     * Failed
   * **Label** - An optional string value, which can provide sub-categories for each Status. See below for a list of common outcomes supported internally by NUnit.
   * **Site** - A `FailureSite` value, indicating the stage of execution in which the test generated its result. Possible values are
     * Test
     * SetUp
     * TearDown
     * Parent
     * Child

Although the outcome of the test may be accessed during setup or test execution, it only has a useful value in the teardown stage.

##### Common Outcomes

The following is a list of outcomes currently produced by NUnit. Others may be added in the future.
   * Success: the test passed. (Status=Passed)
   * Inconclusive: the test was inconclusive. (Status=Inconclusive)
   * Failure: a test assertion failed. (Status=Failed, Label=empty)
   * Error: an unexpected exception occurred. (Status=Failed, Label=Error)
   * NotRunnable: the test was invalid and could not be run. (Status=Failed, Label=Invalid)
   * Cancelled: the user cancelled while this test was running. (Status=Failed, Label=Cancelled)
   * Ignored: the test was ignored. (Status=Skipped, Label=Ignored)
   * Explicit: the test was not run because it is marked Explicit. (Status=Skipped, Label=Explicit)
   * Skipped: the test was skipped for some other reason. (Status=Skipped, Label=empty)

#### TestDirectory

Gets the full path of the directory containing the current test assembly.

#### WorkDirectory

Gets the full path of the directory to be used for output from this test run. The XML result file and any redirected output files are located under this directory. This is normally the directory that was current when execution of
NUnit began but may be changed by use of the **--work** option of nunit-console.

#### Random

Returns a `Randomizer` object, which may be used in the test code to generate random values. These values are repeatable on reruns of the tests so long as (a) the test assembly is not changed and (b) the same seed is used. The initial random seed used in any test run may be found in the XML result file and may be provided to a subsequent run on the command line.

See [[Randomizer Methods]] for details about each available random data type.
