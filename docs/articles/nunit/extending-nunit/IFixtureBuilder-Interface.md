This interface is used by attributes that know how to build a fixture from a user class. `IFixtureBuilder` is defined as follows:

```csharp
public interface IFixtureBuilder
{
    TestSuite BuildFrom(ITypeInfo type);
}
```

Custom fixture builders should examine the provided `ITypeInfo` and return an appropriate type of fixture based on it. If the fixture is intended to be an NUnit `TestFixture`, then the helper class `NUnitTestFixtureBuilder` may be used to create it.

The following NUnit attributes currently implement this interface:
* `TestFixtureAttribute`
* `TestFixtureSourceAttribute`
* `SetUpFixtureAttribute`

**Notes:**

1. `ITypeInfo` is an internal interface used by NUnit to wrap a Type. See [source code](https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Interfaces/ITypeInfo.cs) for details.

2. It would make more sense for this interface method to return `TestFixture` rather than `TestSuite`. We use `TestSuite` because it is the common base for both `TestFixture` and `SetupFixture`. In a future version, we will try to adjust the hierarchy so that all suites based on a class are derived from `TestFixture`.

