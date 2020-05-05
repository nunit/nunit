The **Assert.Inconclusive** method indicates that the test could not be
completed with the data available. It should be used in situations where
another run with different data might run to completion, with either a
success or failure outcome.

```csharp
Assert.Inconclusive();
Assert.Inconclusive(string message, params object[] parms);
```

#### See also...
 * [[Assert.Pass]]
 * [[Assert.Fail]]
 * [[Assert.Ignore]]
