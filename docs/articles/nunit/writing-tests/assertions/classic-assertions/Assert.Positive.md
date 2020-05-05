Asserts that a number is positive.

```csharp
//false
Assert.Positive(-1);
//true
Assert.Positive(1);
```

All the overloads of the method are
```csharp
Assert.Positive(int actual);
Assert.Positive(int actual, string message, params object[] args);

Assert.Positive(uint actual);
Assert.Positive(uint actual, string message, params object[] args);

Assert.Positive(long actual);
Assert.Positive(long actual, string message, params object[] args);

Assert.Positive(ulong actual);
Assert.Positive(ulong actual, string message, params object[] args);

Assert.Positive(decimal actual);
Assert.Positive(decimal actual, string message, params object[] args);

Assert.Positive(double actual);
Assert.Positive(double actual, string message, params object[] args);

Assert.Positive(float actual);
Assert.Positive(float actual, string message, params object[] args);
```
You may also use **Assert.That** with a **Is.Positive** constraint to achieve the same result.

#### See also...
*  [[Assert.Negative]]
 * [[Assert.Zero]]
 * [[Assert.NotZero]]
 * [[Assert.IsNaN]]
