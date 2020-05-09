### DRAFT IN PROCESS

This technical note describes the inheritance hierarchy used for attributes in NUnit. It applies to the built-in attributes and may also be used as a guide for where to place custom attributes in the hierarchy. However, use of these attributes as base classes is entirely optional for custom attributes, as all functionality is accessed through the implemented interfaces.

Essentially, we use attribute inheritance to define the "what" of an attribute... that is, what it primarily **is** and what it primarily is used for. The actual functionality of an attribute is defined by the use of interfaces.

### Abstract Attribute Classes

The base of the hierarchy is formed by a number of abstract classes. They generally incorporate no code.

#### NUnitAttribute

All NUnit attributes inherit directly or indirectly from `NUnitAttribute`. Its only purpose is to allow easy selection of all NUnit attributes on a member. If a custom attribute does not fit under any of the other attribute classes, you may derive it directly from `NUnitAttribute`.

#### TestFixtureBuilderAttribute

`TestFixtureBuilderAttribute` is the base class for any attribute that knows how to build a test fixture of some kind from a provided class. In this context, a test fixture means any test that is based on a user class. 

NUnit includes the following attributes derived from `TestFixtureBuilderAttribute`:
* `TestFixtureAttribute`
* `SetUpFixtureAttribute`.

Derived classes that build fixtures should implement the `IFixtureBuilder` interface. `TestFixtureBuilderAttribute` does not implement this interface itself since future versions of NUnit may introduce additional interfaces that build fixtures. Therefore, the choice of interface is left to the derived class.

#### TestCaseBuilderAttribute

`TestCaseBuilderAttribute` is the base class for any attribute that knows how to build a test case from a given method. Test cases may be simple (without arguments) or parameterized (taking arguments) and are always based on a `MethodInfo`.

NUnit includes the following attributes derived from `TestCaseBuilderAttribute`:`
* `CombiningStrategyAttribute`
  * `CombinatorialAttribute`
  * `PairwiseAttribute`
  * `SequentialAttribute`
* `TestAttribute`
* `TestCaseAttribute`
* `TestCaseSourceAttribute`
* `TheoryAttribute`

As with `TestFixtureBuilderAttribute`, the choice of a specific interface to implement is left to the derived class. Custom classes that build test cases should implement one of the interfaces designed for the purpose: `ISimpleTestCaseBuilder` or `ITestCaseBuilder`. Further interfaces may be defined in the future. In addition, any builder that is intended to cause a non-attributed class to be used as an NUnit TestFixture should implement `IImplyFixture`.

#### IncludeExcludeAttribute

`IncludeExcludeAttribute` is the base class for any attributes used to decide whether to include a test in the current run or exclude it based on the string properties `Include`, `Exclude` and `Reason`. The abstract base simply makes these properties available to the derived class, which is responsible for taking action on them.

NUnit currently defines two attributes that derive from `IncludeExcludeAttribute`:
* `CultureAttribute`
* `PlatformAttribute`

These two attributes implement `IApplyToTest` and set the `RunState` of the test based on interpreting the arguments and the current environment. Custom classes derived from `IncludeExcludeAttribute` should do the same thing.

