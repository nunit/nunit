This page describes significant changes in SetUp and TearDown in NUnit 3.0

Existing NUnit 2.6.4 attributes used for SetUp and TearDown were

  * SetUpAttribute
  * TearDownAttribute
  * TestFixtureSetUpAttribute
  * TestFixtureTearDownAttribute
  * SetUpFixtureAttribute

Taken together, these attributes provided per-test setup and teardown at the fixture level and one-time setup and teardown at the fixture, namespace and assembly levels. 

These features were somewhat confusing:

  * SetUpFixture seems not very well understood by users in general. 
  * TestFixtureSetUp and TestFixtureTearDown could do with better names.
  * SetUp and TearDown designate per-test setup/teardown within a test fixture, one-time setup/teardown within a setup fixture

For NUnit 3.0 we standardized the use of attributes for setup and teardown and renamed some of them to make their function clearer. 

#### Attribute Usage

  * [SetUpAttribute](xref:setup-attribute) is now used exclusively for per-test setup.

  * [TearDownAttribute](xref:teardown-attribute) is now used exclusively for per-test teardown. 

  * [OneTimeSetUpAttribute](xref:onetimesetup-attribute) is used for one-time setup per test-run. If you run _n_ tests, this event will only occur once.

  * [OneTimeTearDownAttribute](xref:onetime-attribute) is used for one-time teardown per test-run. If you run _n_ tests, this event will only occur once

  * [SetUpFixtureAttribute](xref:setupfixture-attribute) continues to be used as at before, but with changed method attributes.

#### Attribute Usage by Fixture Type

|                     | TestFixture  | SetUpFixture |
|---------------------|--------------|--------------|
| OneTimeSetUp        |  Supported   |  Supported   |
| OneTimeTearDown     |  Supported   |  Supported   |
| TestFixtureSetUp    |  Deprecated  | Not Allowed  |
| TestFixtureTearDown |  Deprecated  | Not Allowed  |
| SetUp               |  Supported   | Not Allowed  |
| TearDown            |  Supported   | Not Allowed  |

#### Backward Compatibility

**TestFixtureSetUpAttribute** and **TestFixtureTearDownAttribute** continue to be supported as synonyms for **OneTimeSetUpAttribute** and **OneTimeTearDownAttribute** in test fixtures, but are deprecated.

Since **SetUpAttribute** and **TearDownAttribute** are used in two different ways, it's not possible to simply deprecate their usage in SetUpFixture. They have been disallowed in that context, which is a [[breaking change|breaking-changes]].

#### How Setup and TearDown Methods Are Called

Multiple SetUp, OneTimeSetUp, TearDown and OneTimeTearDown methods may exist within a class. The rules for how the setup methods are called will be the same in NUnit 3.0 as in NUnit 2.6. However, there is a change in the calling of the teardown methods.

Setup methods (both types) are called on base classes first, then on derived classes. If any setup method throws an exception, no further setups are called. This is the same as in NUnit 2.6.

Teardown methods (again, both types) are called on derived classes first, then on the base class. In NUnit 2.6, all teardown methods were called so long as **any** setup method was called. It was entirely up to the teardown method to determine how much of the initialization took place.

In NUnit 3.0, the teardown methods at any level in the inheritance hierarchy will be called only if a setup method **at the same level** was called. The following example is illustrates the difference.

```csharp
public class BaseClass
{
   [SetUp]
   public void BaseSetUp() { ... } // Exception thrown!

   [TearDown]
   public void BaseTearDown() { ... }
}

[TestFixture]
public class DerivedClass : BaseClass
{
   [SetUp]
   public void DerivedSetUp() { ... }

   [TearDown]
   public void DerivedTearDown() { ... }

   [Test]
   public void TestMethod() { ... }
}
```
Assume that an exception is thrown in BaseSetUp. In NUnit 2.6, methods would be executed as follows:
* BaseSetUp
* DerivedTearDown
* BaseTearDown

In NUnit 3.0, execution will proceed as follows:
* BaseSetUp
* BaseTearDown

This is potentially a [[breaking change|breaking-changes]] for some users.

#### Unresolved Issues

  * We need to define how setup and teardown methods are ordered with respect to the newly introduced [[Action Attributes]] and how they interact.
