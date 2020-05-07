---
uid: addinreplacementintheframework
---

NUnit 2.6 supports six types of addins, all of which are being removed from NUnit 3.0:
* SuiteBuilders
* TestCaseBuilders
* TestDecorators
* TestCaseProviders
* DataPointProviders
* EventListeners

### General Approach

The addin design for NUnit 2.6 was intended to extend to the console and gui runners in addition to the framework. However, this was never implemented and all six of the existing addin types apply to the framework only.

In NUnit 3.0, the functions provided by these addins are being taken over by the use of custom attributes. NUnit 3.0 attributes are generally active. That is, they contain code to perform the function they call for rather than simply serving as markers to be interpreted by the runner.

#### Advantages

In general, all the same capabilities will be present in NUnit 3.0 and will be much more easily accessible to those who create extensions. Currently, creating an extension is complex and error prone. Use of active attributes generally involves one of two approaches:

1. Derive the new attribute class from an existing NUnit base that provides the needed facilities. For example, a new data-providing attribute might derive from `DataAttribute`.

2. Derive directly from `NUnitAttribute`, the base of the NUnit attribute hierarchy, and implement one or more interfaces that perform the desired function.

#### Limitations

All existing addins will need to be re-implemented as custom attributes. They will not work in NUnit 3.0.

Addins not based on custom attributes are no longer possible. In NUnit 2.6, for example, it was possible to write an addin that defined tests based on the name of a method, e.g.: methods beginning with "Test". This sort of extension will no longer be possible in the NUnit 3.0 framework. However, this does not seem to be a big problem, since virtually all addins that we know about have been based on attributes.

### Implementation

Because parts of NUnit are implemented as *internal addins*, it's not possible to simply remove all addin support at once. Many things would stop working if we did this. Therefore, we will refactor code for each of the internal types to conform to the new design, only removing the overall addin framework when this is complete.

The remaining sections of this spec deal with how each of the addin types is being replaced. The order of the sections reflects the order in which we are implementing the changes. The implementation status of each of the types is shown in parentheses.

> This spec refers to a number of interfaces that form part of the NUnit framework. Until a technical note
> covering these interfaces is published, please rely on the source code for documentation.

> Some of the sections that follow have not had all their design work completed, so the degree of detail
> varies among them. More information will be added as work progresses.

#### TestDecorators 
> Status: REMOVED

TestDecorators in NUnit 2.6 could do one of three things:

1. Modify the properties of the test or the execution context
2. Drop the test by returning null.
3. Replace the test, possibly wrapping the old test within the new one.

In NUnit 3.0, each of these is implemented differently.

To modify the properties of the test, create an attribute that implements `IApplyToTest`.

To modify the execution context, implement `IApplyToContext`. This is a new capability.

It is no longer possible - but not necessary either - to replace the test. In NUnit 3.0, the test object does not execute itself. Instead, a series of commands is created, which carry out the necessary operations to run the test. An attribute that implements `ICommandDecorator` is able to contribute to add additional commands to be executed when running the test.

It is no longer possible to simply eliminate the test. Once created, a test will always appear in the UI, for example. However, by use of a command decorator, it is possible to prevent the test from executing.

#### DataPointProviders
> Status: Removed

NUnit 2.6 has two built-in providers of data for individual parameters of test methods:
* `ParameterDataProvider` gets data from attributes that appear directly on the parameter.
* `DatapointProvider` gets data from `DataPoint` and `DataPointSource` attributes, which appear on the data source rather than on the parameter.

In the current 3.0 code, `ParameterDataProvider` actually delegates its work to the attributes, which must implement `IParameterDataSource`. In NUnit 3.0, users will be able to create additional attributes implementing this interface and they will be used as data sources.

On the other hand, `DataPointProvider` contains the code for accessing the data specified by the `DataPoint` and `DataPointSource` attributes, which are simply used as markers. This is because the attributes do not have a reference to the member on which they are placed. It would be necessary to introduce a method to provide them with this reference in order to do the processing within the attribute and this would appear to introduce otherwise unneeded code with no particular benefit. Consequently, for the initial implementation of NUnit 3.0, it will not be possible to create custom attributes that work in similar fashion to `DataPoint` or `DataPointSource`. If the need arises, some interface may be introduced.

Since `DataPointProvider`s are only called from within `TestCaseProvider`s, implementation of these changes may need to be interleaved with changes related to `TestCaseProvider`s. See the next section for details.

#### TestCaseProviders 
> Status: REMOVED

NUnit 2.6 currently has two built-in TestCaseProviders:
* `DataAttributeTestCaseProvider` gets test case data from any DataAttribute, such as `TestCaseAttribute`.
* `CombinatorialTestCaseProvider` creates test cases by combining parameter data from a `DataPointProvider`.

Currently, `DataAttributeTestCaseProvider` delegates all the work to the `DataAttribute`. This is the desired approach. Users implementing a new custom data attribute may inherit from `DataAttribute` or implement an interface. 

`CombinatorialTestCaseProvider` works differently. It instantiates one of three available `CombiningStrategy `types, based on attributes appearing on the method. The `CombiningStrategy` is used to generate test cases from the available parameter data.

For NUnit 3.0, the work of combining parameter data into cases will be moved into the `CombiningStrategy` attributes. An interface will be defined and users will be able to create new combining strategies by defining a custom attribute that implements the interface.

#### EventListeners
> Status: REMOVED

EventListeners implement the `ITestListener` interface and are notified when important events in the life of a test occur. Almost all the functions of an `EventListener` can already be emulated in NUnit 2.6 by use of an `ActionAttribute`. `ActionAttribute`s are not yet implemented in NUnit 3.0.

Consequently, the first step in removing EventListeners is to implement `ActionAttribute`s. Some changes may be made in the process and a separate spec will be written to describe them.

The only function of EventListeners that cannot be duplicated by ActionAttributes at this time is the capture of text output from the test. A new approach will be designed for this purpose.

#### TestCaseBuilders
> Status: REMOVED

NUnit 2.6 has one built-in TestCaseBuilder, `NUnitTestCaseBuilder`, which implements `ITestCaseBuilder2`. All TestCaseBuilders must implement either `ITestCaseBuilder` or `ITestCaseBuilder2`.

In NUnit 3.0, the interface and its implementation will be moved into the attributes that designate a test. For example, `TestAttribute` will actually build a test case. This may be done using `ITestCaseBuilder2` or a new interface may be designed. Users wishing to create a new kind of test case will need to define a new attribute, which implements the interface.

A few issues need to be resolved:

1. Some attributes, such as `TestCaseAttribute` or `TestCaseSourceAttribute` serve dual functions: they both mark a test case and provide data for the test case. Some combination of the code for generating test cases with that for creating tests is likely to be required.
2. For backward compatibility, an extra `TestAttribute` accompanying one or more `TestCaseAttribute`s should not generate an additional `TestCase`.

#### SuiteBuilders
> Status: REMOVED

NUnit 2.6 has one built-in SuiteBuilder, which implements the required `ISuiteBuilder` interface.

Similarly to what is being done for test cases, the building of a test fixture will be moved into the `TestFixture` attribute. The supporting interface may need to be redefined. Users may create new types of test fixtures by defining a new attribute, which implements the interface.

Similar issues as with TestCaseBuilder must be resolved, in addition to a few others:

1. `TestFixtureAttribute` both marks a test case and provides arguments used to construct the fixture.
2. *Extra* `TestFixtureAttribute`s in the hierarchy must be ignored.
3. We want to add similar data-generation capabilities to `TestFixture` as exist for methods using `TestCaseSource`.
4. We want to allow "TheoryFixtures", which combine multiple related Theories into a single fixture.
5. We want to support an `AbstractTestFixtureAttribute`, which marks a class as the base of a fixture hierarchy but not a fixture itself.