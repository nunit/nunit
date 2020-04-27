**IgnoreAttribute** is used to indicate that a test should not be executed for
some reason. Note that with NUnit 3, the reason must be specified. Ignored 
tests are displayed by the runners as warnings in order to provide a reminder
that the test needs to be corrected or otherwise changed and re-instated.

Note that the **IgnoreAttribute** is attached to a method. If you have multiple test cases using the same method, adding  it will ignore all the cases. To ignore individual test cases see [Ignoring Individual Test Cases](#ignoring-individual-test-cases) below.

#### Test Fixture Syntax

```csharp
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  [Ignore("Ignore a fixture")]
  public class SuccessTests
  {
    // ...
  }
}
```

#### Test Syntax

```csharp
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  public class SuccessTests
  {
    [Test]
    [Ignore("Ignore a test")]
    public void IgnoredTest()
    { /* ... */ }
}
```

#### Ignore Until

The `Until` named parameter allows you to ignore a test for a specific period of time,
after which the test will run normally. The until date must be a string
that can be parsed to a date.

```csharp
[TestFixture]
[Ignore("Waiting for Joe to fix his bugs", Until = "2014-07-31 12:00:00Z"]
public class MyTests
{
 [Test]
 public void Test1() { /* ... */ }
}
```

In the above example, it's assumed that the test would fail if run. With the
IgnoreAttribute, it will give a warning until the specified date. After that
time, it will run normally and either pass or fail.

#### Ignoring Individual Test Cases

The **IgnoreAttribute** causes all the test cases using the method on which it is placed to be ignored. Ignoring individual test cases is possible, depending on how they are specified.

   Attribute        |   How to ignore a case
--------------------|------------------------------------------------------------------
**TestCase**        | Use the **Ignore** named parameter of the **TestCaseAttribute.**
**TestCaseSource**  | Use **TestCaseData** for the source and set the **Ignore** property.

