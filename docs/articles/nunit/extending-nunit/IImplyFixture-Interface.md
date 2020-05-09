The `IImplyFixture` interface is an empty interface, used solely as a marker:

```csharp
public interface IImplyFixture
{
}
```

If a class contains any method with an attribute that implements this interface, that class is treated as an NUnit TestFixture without any `TestFixture` attribute being specified. The following NUnit attributes currently implement this interface:
* `TestAttribute`
* `TestCaseAttribute`
* `TestCaseSourceAttribute`
* `TheoryAttribute`
