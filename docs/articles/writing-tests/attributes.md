NUnit uses custom attributes to identify tests. All NUnit attributes are contained in the NUnit.Framework namespace. Each source file that contains tests must include a using statement for that namespace and the project must reference the framework assembly, `nunit.framework.dll`.

This table lists all the attributes supported by NUnit. 

|   Attribute                       |    Usage    |
|-----------------------------------|-------------|
| [[Apartment Attribute]]           | Indicates that the test should run in a particular apartment. |
| [[Author Attribute]]              | Provides the name of the test author. |
| [[Category Attribute]]            | Specifies one or more categories for the test. |
| [[Combinatorial Attribute]]       | Generates test cases for all possible combinations of the values provided. |
| [[Culture Attribute]]             | Specifies cultures for which a test or fixture should be run. |
| [[Datapoint Attribute]]           | Provides data for [Theories](Theory-Attribute). |
| [[DatapointSource Attribute]]     | Provides data for [Theories](Theory-Attribute). |
| [[DefaultFloatingPointTolerance Attribute]] | Indicates that the test should use the specified tolerance as default for float and double comparisons. |
| [[Description Attribute]]         | Applies descriptive text to a Test, TestFixture or Assembly. |
| [[Explicit Attribute]]            | Indicates that a test should be skipped unless explicitly run. |
| [[Ignore Attribute]]              | Indicates that a test shouldn't be run for some reason. |
| [[LevelOfParallelism Attribute]]  | Specifies the level of parallelism at assembly level. |
| [[MaxTime Attribute]]             | Specifies the maximum time in milliseconds for a test case to succeed. |
| [[NonParallelizable Attribute]]   | Specifies that the test and its descendants may not be run in parallel. |
| [[NonTestAssembly Attribute]]     | Specifies that the assembly references the NUnit framework, but that it does not contain tests. |
| [[OneTimeSetUp Attribute]]        | Identifies methods to be called once prior to any child tests. |
| [[OneTimeTearDown Attribute]]     | Identifies methods to be called once after all child tests. |
| [[Order Attribute]]               | Specifies the order in which decorated test should be run (against others). |
| [[Pairwise Attribute]]            | Generate test cases for all possible pairs of the values provided. |
| [[Parallelizable Attribute]]      | Indicates whether test and/or its descendants can be run in parallel. |
| [[Platform Attribute]]            | Specifies platforms for which a test or fixture should be run. |
| [[Property Attribute]]            | Allows setting named properties on any test case or fixture. |
| [[Random Attribute]]              | Specifies generation of random values as arguments to a parameterized test. |
| [[Range Attribute]]               | Specifies a range of values as arguments to a parameterized test. |
| [[Repeat Attribute]]              | Specifies that the decorated method should be executed multiple times. |
| [[RequiresThread Attribute]]      | Indicates that a test method, class or assembly should be run on a separate thread. |
| [[Retry Attribute]]               | Causes a test to be rerun if it fails, up to a maximum number of times. |
| [[Sequential Attribute]]          | Generates test cases using values in the order provided, without additional combinations. |
| [[SetCulture Attribute]]          | Sets the current Culture for the duration of a test. |
| [[SetUICulture Attribute]]        | Sets the current UI Culture for the duration of a test. |
| [[SetUp Attribute]]               | Indicates a method of a TestFixture called just before each test method. |
| [[SetUpFixture Attribute]]        | Marks a class with one-time setup or teardown methods for all the test fixtures in a namespace. |
| [[SingleThreaded Attribute]]      | Marks a fixture that requires all its tests to run on the same thread. |
| [[TearDown Attribute]]            | Indicates a method of a TestFixture called just after each test method. |
| [[Test Attribute]]                | Marks a method of a TestFixture that represents a test. |
| [[TestCase Attribute]]            | Marks a method with parameters as a test and provides inline arguments. |
| [[TestCaseSource Attribute]]      | Marks a method with parameters as a test and provides a source of arguments. |
| [[TestFixture Attribute]]         | Marks a class as a test fixture and may provide inline constructor arguments. |
| [[TestFixtureSetup Attribute]]    | Deprecated synonym for [[OneTimeSetUp Attribute]]. |
| [[TestFixtureSource Attribute]]   | Marks a class as a test fixture and provides a source for constructor arguments. |
| [[TestFixtureTeardown Attribute]] | Deprecated synonym for [[OneTimeTearDown Attribute]]. |
| [[TestOf Attribute]]              | Indicates the name or Type of the class being tested. |
| [[Theory Attribute]]              | Marks a test method as a Theory, a special kind of test in NUnit. |
| [[Timeout Attribute]]             | Provides a timeout value in milliseconds for test cases. |
| [[Values Attribute]]              | Provides a set of inline values for a parameter of a test method. |
| [[ValueSource Attribute]]         | Provides a source of values for a parameter of a test method. |
