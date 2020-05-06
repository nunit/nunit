#### Attribute Usage

  * [SetUpAttribute](xref:setup-attribute) is now used exclusively for per-test setup.

  * [TearDownAttribute](xref:teardown-attribute) is now used exclusively for per-test teardown. 

  * [OneTimeSetUpAttribute](xref:onetimesetup-attribute) is used for one-time setup per test-run. If you run _n_ tests, this event will only occur once.

  * [OneTimeTearDownAttribute](xref:onetimeteardown-attribute) is used for one-time teardown per test-run. If you run _n_ tests, this event will only occur once

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

#### How Setup and TearDown Methods Are Called

Multiple SetUp, OneTimeSetUp, TearDown and OneTimeTearDown methods may exist within a class.

Setup methods (both types) are called on base classes first, then on derived classes. If any setup method throws an exception, no further setups are called.

Teardown methods (again, both types) are called on derived classes first, then on the base class. The teardown methods at any level in the inheritance hierarchy will be called only if a setup method **at the same level** was called. The following example is illustrates the difference.

```C#
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
Execution will proceed as follows:
* BaseSetUp
* BaseTearDown

rather than

* BaseSetUp
* DerivedTearDown
* BaseTearDown

See also: [SetUp and TearDown Changes](xref:setupteardownchanges)