Asserts that a number is negative.

```csharp
//true
Assert.Negative(-1);
//false
Assert.Negative(1);
```

All the overloads of the method are
```csharp
Assert.Negative(int actual);
Assert.Negative(int actual, string message, params object[] args);

Assert.Negative(uint actual);
Assert.Negative(uint actual, string message, params object[] args);

Assert.Negative(long actual);
Assert.Negative(long actual, string message, params object[] args);

Assert.Negative(ulong actual);
Assert.Negative(ulong actual, string message, params object[] args);

Assert.Negative(decimal actual);
Assert.Negative(decimal actual, string message, params object[] args);

Assert.Negative(double actual);
Assert.Negative(double actual, string message, params object[] args);

Assert.Negative(float actual);
Assert.Negative(float actual, string message, params object[] args);
```
You may also use **Assert.That** with a **Is.Negative** constraint to achieve the same result.

#### See also...
 * [[Assert.Positive]]
 * [[Assert.Zero]]
 * [[Assert.NotZero]]
 * [[Assert.IsNaN]]
