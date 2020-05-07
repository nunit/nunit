NUnit 3 implements a great deal of its functionality in its attributes. This functionality is accessed through a number of standard interfaces, which are implemented by the attributes. Users may create their own attributes by implementing these interfaces. 

For ease of understanding, the interfaces are grouped according to the stage in the life-cycle of a test at which they are used. The two primary stages in the life of a test are Load-Time and Execution-Time.

### Load-Time Interfaces

_Loading_ tests means loading the assembly into memory and examining its content to discover the classes and fixtures that represent tests. The internal structures that represent tests are built at this time. If requested by the application, information about the tests may be returned for display, as is done in the NUnit GUI runner.

The following interfaces are called at load time.

| Interface              | Used By |
|------------------------|---------|
| [[IFixtureBuilder\|IFixtureBuilder-Interface]]       | Attributes that know how to build a fixture from a test class
| [[ITestBuilder\|ITestBuilder-Interface]]              | Attributes that know how to build one or more parameterized test cases for a method
| [[ISimpleTestBuilder\|ISimpleTestBuilder-Interface]] | Attributes that know how to build a single non-parameterized test case for a method
| [[IParameterDataSource\|IParameterDataSource-Interface]] | Attributes that supply values for a single parameter for use in generating test cases
| [[IImplyFixture\|IImplyFixture-Interface]]           | Attributes used on a method to signal that the defining class should be treated as a fixture
| [[IApplyToTest\|IApplyToTest-Interface]]             | Attributes that make modifications to a test immediately after it is constructed

### Execution-Time Interfaces

At execution-time, some or all of the tests that were previously loaded are actually run. Their results are returned and made available to the application.

The following interfaces are called at execution time.

| Interface              | Used By |
|------------------------|---------|
| [[IApplyToContext\|IApplyToContext-Interface]] | Attributes that set up the context prior to execution
| [[ICommandWrapper\|ICommandWrapper-Interface]] | Attributes that can wrap a `TestCommand` with another command 
