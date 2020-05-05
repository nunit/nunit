**Assert.NotZero** tests that a value is not zero.

```csharp
Assert.NotZero(int actual);
Assert.NotZero(int actual, string message, params object[] args);

Assert.NotZero(uint actual);
Assert.NotZero(uint actual, string message, params object[] args);

Assert.NotZero(long actual);
Assert.NotZero(long actual, string message, params object[] args);

Assert.NotZero(ulong actual);
Assert.NotZero(ulong actual, string message, params object[] args);

Assert.NotZero(decimal actual);
Assert.NotZero(decimal actual, string message, params object[] args);

Assert.NotZero(double actual);
Assert.NotZero(double actual, string message, params object[] args);

Assert.NotZero(float actual);
Assert.NotZero(float actual, string message, params object[] args);
```

You may also use **Assert.That** with a `Is.Not.Zero` constraint to achieve the
same result.
