**Assert.LessOrEqual** tests whether one object is less than or equal to another.
Contrary to the normal order of Asserts, these methods are designed to be
read in the "natural" English-language or mathematical order. Thus
**Assert.LessOrEqual(x, y)** asserts that x is less than or equal to y (x <= y).

```csharp
Assert.LessOrEqual(int arg1, int arg2);
Assert.LessOrEqual(int arg1, int arg2,
                   string message, params object[] parms);

Assert.LessOrEqual(uint arg1, uint arg2);
Assert.LessOrEqual(uint arg1, uint arg2,
                   string message, params object[] parms);

Assert.LessOrEqual(long arg1, long arg2);
Assert.LessOrEqual(long arg1, long arg2,
                   string message, params object[] parms);

Assert.LessOrEqual(ulong arg1, ulong arg2);
Assert.LessOrEqual(ulong arg1, ulong arg2,
                   string message, params object[] parms);

Assert.LessOrEqual(decimal arg1, decimal arg2);
Assert.LessOrEqual(decimal arg1, decimal arg2,
                   string message, params object[] parms);

Assert.LessOrEqual(double arg1, double arg2);
Assert.LessOrEqual(double arg1, double arg2,
                   string message, params object[] parms);

Assert.LessOrEqual(float arg1, float arg2);
Assert.LessOrEqual(float arg1, float arg2,
                   string message, params object[] parms);

Assert.LessOrEqual(IComparable arg1, IComparable arg2);
Assert.LessOrEqual(IComparable arg1, IComparable arg2,
                   string message, params object[] parms);
```

#### See also...
 * [Assert.Greater](Assert.Greater.md)
 * [Assert.GreaterOrEqual](Assert.GreaterOrEqual.md)
 * [Assert.Less](Assert.Less.md)
 * [Comparison Constraints](xref:constraints#comparison-constraints)
