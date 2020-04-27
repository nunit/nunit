Assumptions are intended to express the state a test must be in to provide a meaningful result. They are functionally similar to assertions, however a unmet assumption will produce an Inconclusive test result, as opposed to a Failure.

Assumptions make use of the `Assume` static class.

### Syntax

```csharp
Assume.That(myString, Is.EqualTo("Hello"));
```

`Assume.That()` has the same set of overloads as `Assert.That()`. For further details there, see the [[Constraint Model]] documentation.

**Note:** Failing assumptions indicate that running tests is invalid,  while [[Multiple Asserts]] allow testing to continue after a failure. For that reason, the two features are incompatible and assumptions may not be used within a [[multiple assert|Multiple Asserts]] block.