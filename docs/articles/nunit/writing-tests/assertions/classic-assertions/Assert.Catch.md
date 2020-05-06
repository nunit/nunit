**Assert.Catch** is similar to **Assert.Throws** but will pass for an exception
that is derived from the one specified.

```csharp
Exception Assert.Catch(TestDelegate code);
Exception Assert.Catch(TestDelegate code,
                       string message, params object[] parms);

Exception Assert.Catch(Type expectedExceptionType, TestDelegate code);
Exception Assert.Catch(Type expectedExceptionType, TestDelegate code,
                       string message, params object[] parms);

T Assert.Catch<T>(TestDelegate code);
T Assert.Catch<T>(TestDelegate code,
                  string message, params object[] parms);
```

#### See also...>
 * [Assert.CatchAsync](Assert.CatchAsync.md)
 * [Assert.Throws](Assert.Throws.md)
 * [Assert.ThrowsAsync](Assert.ThrowsAsync.md)
 * [ThrowsConstraint](xref:ThrowsConstraint)
