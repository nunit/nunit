The `IApplyToTest` interface is used to make modifications to a test immediately after it is constructed. It is defined as follows:

```csharp
public interface IApplyToTest
{
    void ApplyToTest(Test test);
}
```

The `Test` Type is quite general and the argument may represent a suite or an individual test case. If the distinction is important, then you must code the attribute to examine the argument and react accordingly.

The interface may appear on the same attribute that is used to construct the test or on a separate attribute. In either case, it will only be called after the test is built. 

The order in which `ApplyToTest` is called on multiple attributes is indeterminate. If two attributes make completely independent changes to a test, then the order is not relevant. But if they both change the same property, or related properties, then it may necessary to make tests in the attribute code to ensure that the correct value 'wins'.

The most common example of this is for attributes that change the RunState of a test. If one attribute is trying to set it to `RunState.Ignore`, while the other wants it to be `RunState.NotRunnable`, we would normally expect the 'worst' value to win and for the test to be non-runnable. We can achieve that by code like the following:

```csharp
// In the attribute setting NotRunnable
test.RunState = RunState.NotRunnable;
...

// In the attribute setting Ignore
if (test.RunState != RunState.NotRunnable)
    test.RunState = RunState.Ignore;
```

The following NUnit attributes implement `IApplyToTest`:
* `CategoryAttribute`
* `CombiningStrategyAttribute`
* `CultureAttribute`
* `ExplicitAttribute`
* `IgnoreAttribute`
* `OrderAttribute`
* `PlatformAttribute`
* `PropertyAttribute` (and, through it, a large number of derived attributes)
* `RequiresThreadAttribute`
* `TestAttribute`
* `TestFixtureAttribute`

