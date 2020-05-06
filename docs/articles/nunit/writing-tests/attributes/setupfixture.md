---
uid: setupfixture-attribute
---

This is the attribute that marks a class that contains the one-time
setup or teardown methods for all the test fixtures under a given
namespace. The class may contain at most one method marked with the
OneTimeSetUpAttribute and one method marked with the OneTimeTearDownAttribute.
	
There are a few restrictions on a class that is used as a setup fixture.

 * It must be a publicly exported type or NUnit will not see it.

 * It must have a default constructor or NUnit will not be able to construct it.

The OneTimeSetUp method in a SetUpFixture is executed once before any of the fixtures
contained in its namespace. The OneTimeTearDown method is executed once after all the 
fixtures have completed execution. In the examples below, the method RunBeforeAnyTests()
is called before any tests or setup methods in the NUnit.Tests namespace. The method
RunAfterAnyTests() is called after all the tests in the namespace as well as their
individual or fixture teardowns have completed execution.

Multiple SetUpFixtures may be created in a given namespace. The order of execution
of such fixtures is indeterminate.

A SetUpFixture outside of any namespace provides SetUp and TearDown for the entire assembly.

#### Example:

```csharp
using System;
using NUnit.Framework;

namespace NUnit.Tests
{
  [SetUpFixture]
  public class MySetUpClass
  {
	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
	  // ...
	}

	[OneTimeTearDown]
	public void RunAfterAnyTests()
	{
	  // ...
	}
  }
}
```

> [!NOTE]
> Prior to NUnit 3.0, SetUpFixture used the SetUp and TearDown attributes rather than OneTimeSetUp and OneTimeTearDown.
The older attributes are no longer supported in SetUpFixtures in NUnit 3.0 and later.

#### See also...

 * [SetUp Attribute](SetUp.md)
 * [TearDown Attribute](TearDown.md)
 * [OneTimeSetUp Attribute](OneTimeSetUp.md)
 * [OneTimeTearDown Attribute](OneTimeTearDown.md)

