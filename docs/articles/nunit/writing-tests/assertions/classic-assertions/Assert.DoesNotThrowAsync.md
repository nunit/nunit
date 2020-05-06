**Assert.DoesNotThrowAsync** verifies that the delegate provided as an argument 
does not throw an exception. See [Assert.DoesNotThrow](Assert.DoesNotThrow.md) for synchronous code.

```csharp
void Assert.DoesNotThrowAsync(AsyncTestDelegate code);
void Assert.DoesNotThrowAsync(AsyncTestDelegate code,
                              string message, params object[] parms);
```

#### See also...
 * [Assert.ThrowsAsync](Assert.ThrowsAsync.md)
 * [ThrowsConstraint](xref:ThrowsConstraint)
