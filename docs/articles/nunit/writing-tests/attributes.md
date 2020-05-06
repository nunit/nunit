NUnit uses custom attributes to identify tests. All NUnit attributes are contained in the NUnit.Framework namespace. Each source file that contains tests must include a using statement for that namespace and the project must reference the framework assembly, `nunit.framework.dll`.

This table lists all the attributes supported by NUnit. 

|   Attribute                       |    Usage    |
|-----------------------------------|-------------|
| [Apartment Attribute](attributes/apartment.md)           | Indicates that the test should run in a particular apartment. |
| [Author Attribute](attributes/author.md)              | Provides the name of the test author. |
| [Category Attribute](attributes/category.md)            | Specifies one or more categories for the test. |
| [Combinatorial Attribute](attributes/combinatorial.md)       | Generates test cases for all possible combinations of the values provided. |
| [Culture Attribute](attributes/culture.md)             | Specifies cultures for which a test or fixture should be run. |
| [Datapoint Attribute](attributes/datapoint.md)           | Provides data for [Theories](xref:Theory-Attribute). |
| [DatapointSource Attribute](attributes/datapointsource.md)     | Provides data for [Theories](xref:Theory-Attribute). |
| [DefaultFloatingPointTolerance Attribute](attributes/defaultfloatingpointtolerance.md) | Indicates that the test should use the specified tolerance as default for float and double comparisons. |
| [Description Attribute](attributes/description.md)         | Applies descriptive text to a Test, TestFixture or Assembly. |
| [Explicit Attribute](attributes/explicit.md)            | Indicates that a test should be skipped unless explicitly run. |
| [Ignore Attribute](attributes/ignore.md)              | Indicates that a test shouldn't be run for some reason. |
| [LevelOfParallelism Attribute](attributes/levelofparallelism.md)  | Specifies the level of parallelism at assembly level. |
| [MaxTime Attribute](attributes/maxtime.md)             | Specifies the maximum time in milliseconds for a test case to succeed. |
| [NonParallelizable Attribute](attributes/nonparallelizable.md)   | Specifies that the test and its descendants may not be run in parallel. |
| [NonTestAssembly Attribute](attributes/nontestassembly.md)     | Specifies that the assembly references the NUnit framework, but that it does not contain tests. |
| [OneTimeSetUp Attribute](attributes/onetimesetup.md)        | Identifies methods to be called once prior to any child tests. |
| [OneTimeTearDown Attribute](attributes/onetimeteardown.md)     | Identifies methods to be called once after all child tests. |
| [Order Attribute](attributes/order.md)               | Specifies the order in which decorated test should be run (against others). |
| [Pairwise Attribute](attributes/pairwise.md)            | Generate test cases for all possible pairs of the values provided. |
| [Parallelizable Attribute](attributes/parallelizable.md)      | Indicates whether test and/or its descendants can be run in parallel. |
| [Platform Attribute](attributes/platform.md)            | Specifies platforms for which a test or fixture should be run. |
| [Property Attribute](attributes/property.md)            | Allows setting named properties on any test case or fixture. |
| [Random Attribute](attributes/random.md)              | Specifies generation of random values as arguments to a parameterized test. |
| [Range Attribute](attributes/range.md)               | Specifies a range of values as arguments to a parameterized test. |
| [Repeat Attribute](attributes/repeat.md)              | Specifies that the decorated method should be executed multiple times. |
| [RequiresThread Attribute](attributes/requiresthread.md)      | Indicates that a test method, class or assembly should be run on a separate thread. |
| [Retry Attribute](attributes/retry.md)               | Causes a test to be rerun if it fails, up to a maximum number of times. |
| [Sequential Attribute](attributes/sequential.md)          | Generates test cases using values in the order provided, without additional combinations. |
| [SetCulture Attribute](attributes/setculture.md)          | Sets the current Culture for the duration of a test. |
| [SetUICulture Attribute](attributes/setuiculture.md)        | Sets the current UI Culture for the duration of a test. |
| [SetUp Attribute](attributes/setup.md)               | Indicates a method of a TestFixture called just before each test method. |
| [SetUpFixture Attribute](attributes/setupfixture.md)        | Marks a class with one-time setup or teardown methods for all the test fixtures in a namespace. |
| [SingleThreaded Attribute](attributes/singlethreaded.md)      | Marks a fixture that requires all its tests to run on the same thread. |
| [TearDown Attribute](attributes/teardown.md)            | Indicates a method of a TestFixture called just after each test method. |
| [Test Attribute](attributes/test.md)                | Marks a method of a TestFixture that represents a test. |
| [TestCase Attribute](attributes/testcase.md)            | Marks a method with parameters as a test and provides inline arguments. |
| [TestCaseSource Attribute](attributes/testcasesource.md)      | Marks a method with parameters as a test and provides a source of arguments. |
| [TestFixture Attribute](attributes/testfixture.md)         | Marks a class as a test fixture and may provide inline constructor arguments. |
| [TestFixtureSetup Attribute](attributes/testfixturesetup.md)    | Deprecated synonym for [OneTimeSetUp Attribute](attributes/onetimesetup.md). |
| [TestFixtureSource Attribute](attributes/testfixturesource.md)   | Marks a class as a test fixture and provides a source for constructor arguments. |
| [TestFixtureTeardown Attribute](attributes/testfixtureteardown.md) | Deprecated synonym for [OneTimeTearDown Attribute](attributes/onetimeteardown.md). |
| [TestOf Attribute](attributes/testof.md)              | Indicates the name or Type of the class being tested. |
| [Theory Attribute](attributes/theory.md)              | Marks a test method as a Theory, a special kind of test in NUnit. |
| [Timeout Attribute](attributes/timeout.md)             | Provides a timeout value in milliseconds for test cases. |
| [Values Attribute](attributes/values.md)              | Provides a set of inline values for a parameter of a test method. |
| [ValueSource Attribute](attributes/valuesource.md)         | Provides a source of values for a parameter of a test method. |
