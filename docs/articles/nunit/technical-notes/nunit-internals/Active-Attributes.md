### Out Of Date - Needs Rewrite
NUnit identifies tests and fixtures of custom attributes. Additional attributes
are used to identify test data, modify tests and control how they execute.
NUnit 3.0 is changing how attributes are used and recognized internally, while
maintaining the same API for the test writer. This specification describes how
the new version will recognize and use attributes, the interfaces implemented
by attributes and the class hierarchy of NUnit's custom attributes.

### Rationale

In the NUnit 2.x releases, attributes are recognized by the text of their name
rather than using the attribute Type. This means that there is no need or the
runner to have an actual reference to the nunit.framework assembly, where 
attributes are defined. This approach was taken in order to support multiple
versions of the framework. Each new release of NUnit was able to run tests
built against older frameworks, so long as the names remained the same.

However, this approach has some drawbacks:

  * As new attributes were added to the framework, it became necessary to also add the name of each attribute to the nunit core, which contained the code to load and run tests. The growing number of attributes in the framework now presents and extremely large surface area for the core to be aware of.
  * Inheritance from attributes does not work as users would expect with NUnit failing to automatically recognize those attributes as equivalent to their base classes. In a few cases, NUnit contains special code to scan the base classes of any attributes found but it is not easy for users to discover which attributes allow this.
  * NUnit must examine all attributes of a given class or method, comparing their names with the names it understands. This is quite inefficient and can slow down loading of assemblies containing large numbers of classes or methods.

By locating the code to load and run tests in the framework assembly, NUnit 3.0 eliminates the first problem above. Test runners now need only be aware of a much narrower API. The design section below describes how we are handling the second and third issues.

### Design

With NUnit 3.0, attributes are now only used within the framework assembly. Since all references are within that assembly, we can now handle them as Types rather than strings. All existing code that uses attribute names will be modified to use Type references, improving efficiency and allowing use of standard object-oriented programming techniques. The following sections provide an outline of the planned design. As implementation progresses, the code itself should be examined for updated information and further details.

### Active Attributes

NUnit has a relatively small number of test types (e.g. TestFixture, TestMethod, Theory) and a relatively larger number of attributes. The current (2.x) design uses attributes almost exclusively as passive containers for their arguments and properties. All active code around attributes is located in the code around loading and running tests.

In almost all cases, it is possible to reverse this logic and have the attributes play an active part in modifying the operation of tests. For example, the NUnit 2.6 test building logic understands that a test description may be found as a property of a DescriptionAttribute, TestAttribute or TestFixtureAttribute. In NUnit 3.0, each of those attributes will simply be called upon to update the test under construction with whatever information they can supply using the **IApplyToTest** interface.

### Attribute Hierarchy

With just a few exceptions, NUnit's current set of attributes inherit from System.Attribute directly. Since we are now switching to use of the standard .NET reflection mechanism via
attribute Types, it is useful to categorize attributes according to how and when they are used.

For example, at a certain point in the construction of tests, NUnit must scan the attributes on a method to apply all those that provide modifications to the test being built. This is currently done by looking at **all** attributes of the method but it will be much more convenient to only retrieve TestModificationAttributes.

The top levels of the Attribute hierarchy are listed here to the extent that they have been thought out. This will change as we implement it and will not necessarily remain stable in future releases because the hierarchy is considered to be an internal implementation detail.

  * NUnitAttribute
    * TestIdentificationAttribute
    * TestModificationAttribute
      * PropertyAttribute
    * TestActionAttribute
    * DataAttribute
      * TestCaseDataAttribute
      * ParameterDataAttribute 

#### Interface Usage

While class inheritance of attributes is useful for retrieving groups of them and for sharing implementation, public access to capabilities of an attribute is better provided through an interface. For example, most TestModificationAttributes will implement the **IApplyToTest** interface, allowing them to be called upon to modify the test under construction.

```csharp
public interface IApplyToTest
{
    void ApplyToTest(ITest test);
}
```

Interfaces are defined in the NUnit.Framework.Api namespace.

#### Properties Versus Separate Attributes

In some cases, there is a design choice to be made between use of a separate attribute or a property of the primary attribute for holding a particular of information. For example, the description of a test may be given as a property of TestAttribute or using a separate DescriptionAttribute. Recent versions of NUnit have deprecated use of properties in favor of separate attributes. Separate attributes provide a more orthogonal design and have been preferred for that reason.

However, the introduction of parameterized tests has necessitated adding properties to the TestCaseAttribute and other attributes, representing functionality that was provided by a separate attribute on non-parameterized tests. For example, a test is ignored using the IgnoreAttribute while a TestCase is ignored by setting the Ignore property of TestCaseAttribute to true.

For NUnit 3.0, we will try to provide some consistency of use for cases like this. This may result in "undeprecating" certain existing properties and creating new ones.

#### Attribute Targets

NUnit will continue to rely on the AttributeUsageAttribute being properly defined on each of its attributes. This allows us to use one attribute type for both methods and classes while still counting on a given attribute only appearing in the proper context.

### Unresolved Issues

One problem that is created by the new approach is that NUnit will no longer "just work" with
tests compiled against framework versions prior to 3.0. This requires the NUnit test execution engine to use the old approach when dealing with tests compiled against a pre-3.0 framework version. This specification does not cover the execution engine, so the problem is not discussed further here.

