The **OrderAttribute** may be placed on a test method or fixture to specify the order in which tests are run. Ordering is given by the required `order` argument to the attribute, an `int`.

#### Example

The following tests will be run in the order:
 * TestA
 * TestB
 * TestC

```csharp
public class MyFixture
{
    [Test, Order(1)]
    public void TestA() { ... }


    [Test, Order(2)]
    public void TestB() { ... }

    [Test]
    public void TestC() { ... }
}
```

#### Notes:

1. Tests with an `OrderAttribute` argument are started before any tests without the attribute.

2. Ordered tests are started in ascending order of the `order` argument.

3. Among tests with the same `order` value or without the attribute, execution order is indeterminate.

4. Tests do not wait for prior tests to finish. If multiple threads are in use, a test may be started while some earlier tests are still being run.
