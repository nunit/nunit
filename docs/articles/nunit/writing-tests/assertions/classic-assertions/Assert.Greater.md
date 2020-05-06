**Assert.Greater** tests whether one object is greater than another.
Contrary to the normal order of Asserts, these methods are designed to be
read in the "natural" English-language or mathematical order. Thus
**Assert.Greater(x, y)** asserts that x is greater than y (x > y).

```csharp
Assert.Greater(int arg1, int arg2);
Assert.Greater(int arg1, int arg2,
               string message, params object[] parms);

Assert.Greater(uint arg1, uint arg2);
Assert.Greater(uint arg1, uint arg2,
               string message, params object[] parms);

Assert.Greater(long arg1, long arg2);
Assert.Greater(long arg1, long arg2,
               string message, params object[] parms);

Assert.Greater(ulong arg1, ulong arg2);
Assert.Greater(ulong arg1, ulong arg2,
               string message, params object[] parms);

Assert.Greater(decimal arg1, decimal arg2);
Assert.Greater(decimal arg1, decimal arg2,
               string message, params object[] parms);

Assert.Greater(double arg1, double arg2);
Assert.Greater(double arg1, double arg2,
               string message, params object[] parms);

Assert.Greater(float arg1, float arg2);
Assert.Greater(float arg1, float arg2,
               string message, params object[] parms);

Assert.Greater(IComparable arg1, IComparable arg2);
Assert.Greater(IComparable arg1, IComparable arg2,
               string message, params object[] parms);
```

#### See also...
 * [Assert.GreaterOrEqual](Assert.GreaterOrEqual.md)
 * [Assert.Less](Assert.Less.md)
 * [Assert.LessOrEqual](Assert.LessOrEqual.md)
 * [Comparison Constraints](xref:constraints#comparison-constraints)
