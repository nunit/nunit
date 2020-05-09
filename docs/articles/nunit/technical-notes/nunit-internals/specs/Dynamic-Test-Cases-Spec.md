### DRAFT - Not Yet Implemented
This specification describes changes in how test cases will be generated in NUNit 3.0

### Rationale

NUnit currently supports parameterized tests using the TestCase, TestCaseSource,
Values, ValueSource, Random and Range attributes. All these data sources provide
data that is used to generate test cases as the tests are loaded. We refer to these
as "pre-generated" test cases.

There is also a need for what we are calling "dynamically generated" test cases,
which are not created until the test is about to run and which may be re-created
each time the test is run.

For example, it may be desirable to perform some initialization of the data in
a OneTimeFixtureSetUp method. This is not possible using the pre-generated model,
because the tests are generated before any execution.
As another example, the data may consist of the names of files in a certain 
directory. If the directory content is changing dynamically, we may want our
tests to reflect those changes, which is only possible using the dynamic model.

For all these reasons - and because users have requested it - we are introducing
the notion of "dynamically generated" test cases in NUnit 3.0. Some existing
attributes will be re-purposed and new attributes created to support it.


> **NOTE** Dynamic test case generation is also required for the proper implementation of **Theories**, which will be described in a separate specification.

### Design

#### Key Tradeoffs

The two types of test cases have important differences in how they may 
be used by NUnit and each has certain advantages and limitations as well.

|  Pre-Generated  |  Dynamically Generated  |
|-----------------|-------------------------|
| Data must be known at the time of loading in order to construct the test. | Data is not needed until the point of running the test. |
| Data may not change between test runs unless the tests are reloaded. | Data may change between test runs. |
| Test cases are available to show in the Gui as soon as the test is loaded. | Test cases can only be displayed after the test has been run. |
| Certain types of errors may be displayed as soon as the test is loaded. | Errors can only be displayed after the test has been run. |
| Data may not be initialized in OneTimeSetUp because the individual test cases have already been created by that time. | Data may be initialized in OneTimeSetUp if desired. |
| External changes to a data source have no effect on subsequent test runs. | Each test run reflects the current state of the data. |
| Static methods, properties and fields must be used to hold test data for two forms of the TestCaseSource attribute. | Instance methods, properties and fields may be used in all cases. |
| Test runs are automatically repeatable within a session at least until the test is reloaded. | Repeatability within a session will require special support within NUnit. |
| Data-generating code is only executed once - as the test is loaded. | Data-generating code is executed for each run. |

#### General Approach

  * Introduce dynamic test case generation, as described above.

  * For ease of use, each data attribute should use either the pre-generated or dynamically generated model exclusively.

  * Parameterized test methods may mix both types of attribute, allowing both pre-generated and dynamically generated test cases.

  * Provided that external data sources such as files and databases are not changed, we will support repeatable test runs across different program sessions and within the same session.

  * The Gui will display new test cases as they are generated and will provide a visual cue to distinguish the two types of test cases.

#### Syntax

As in NUnit 2.5, Attributes applied to test methods will provide or point to the data to be used as test arguments. Except as noted below, attributes will not change syntactically and will continue to operate as before even though many of them will now generate test cases dynamically. The following paragraphs describe the impact of the change on each attribute.

**TestCaseAttribute** will continue to work as it now does, producing pre-generated test cases. 

**TestCaseSourceAttribute** and **ValueSourceAttribute** will continue to create pre-generated test cases, but will be more limited in their usage than before. The following data sources will be supported:
  * Static fields, properties or methods of the test fixture.
  * Static or instance fields properties or methods of an external or nested class.

> It's likely that there will be ways to 'trick' NUnit into accepting an instance member for one of these attributes, leading to instantiation of the test class at load time. If that occurs, the results are undefined. Obviously, such trickery is not recommended and we'll try hard to detect and reject such attempts.

A new **DynamicDataSourceAttribute** (the name is a placeholder) will supplant both **TestCaseSourceAttribute** and **ValueSourceAttribute** for dynamic generation of test cases and parameter values respectively. Suggestions are welcome for the final name of this attribute.

**ValuesAttribute**, **RangeAttribute**, **RandomAttribute**, **CombinatorialAttribute**, **PairwiseAttribute** and **SequentialAttribute** (that is, all other data attributes) will be used to produce test cases dynamically.

**DatapointAttribute** and **DatapointsAttribute** are only used for Theories, but for the sake of completeness it may be noted that they will also operate dynamically.

Where necessary, additional properties may be added to some attributes in order to provide for repeatability of tests. At the moment, the only known requirement is for specification of a random seed for **RandomAttribute** but others may arise in implementation.

#### Gui Design Notes

Since we don't have an NUnit 3.0 Gui yet, these notes are for the future...

When tests are run in batch mode using the console runner, the distinction
between fixed and dynamic data is moot. In that environment, tests are loaded
and executed in rapid sequence, after which the process is terminated.

But when using the Gui, the affect of using dynamic data is quite visible.
Dynamic tests are initially loaded without showing any subordinate test cases.
After each run, test cases are repopulated and may even be different.
This has several important consequences for the Gui...

  * It must be able to deal with test cases added during test execution.
  * It must be able to trigger regeneration of test cases for a rerun.
  * It must also provide some way to re-run the same tests in a repeatable fashion.
  * It must give the user control over whether to run the tests repeatably or not.

### Implementation Notes

Interface changes will be required to identify the presence of either pre-generated or dynamic data for a method. NUnitTestCaseBuilder will accept a test method with arguments so long as either type is present but will only create pre-generated test cases. Dynamic test case generation will take place in the Run method of ParameterizedMethodSuite class using registered extensions that support dynamic data.

The result will be a new addition to the standard sequence of test execution, as follows:

  - Create test fixture instance
  - Run TestFixtureSetUp
  - **Generate dynamic test cases**
  - For each test case
    - Run SetUp method
    - Run Test method
    - Run TearDown method
  - Run TestFixtureTearDown
  - Dispose and destroy the test fixture instance

### Unresolved Issues

  * None known, implementation issues aside.
