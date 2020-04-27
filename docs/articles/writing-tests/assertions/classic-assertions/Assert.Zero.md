**Assert.Zero** tests that a value is zero.

```csharp
Assert.Zero(int actual);
Assert.Zero(int actual, string message, params object[] args);

Assert.Zero(uint actual);
Assert.Zero(uint actual, string message, params object[] args);

Assert.Zero(long actual);
Assert.Zero(long actual, string message, params object[] args);

Assert.Zero(ulong actual);
Assert.Zero(ulong actual, string message, params object[] args);

Assert.Zero(decimal actual);
Assert.Zero(decimal actual, string message, params object[] args);

Assert.Zero(double actual);
Assert.Zero(double actual, string message, params object[] args);

Assert.Zero(float actual);
Assert.Zero(float actual, string message, params object[] args);
```

You may also use **Assert.That** with a Is.Zero constraint to achieve the
same result.
