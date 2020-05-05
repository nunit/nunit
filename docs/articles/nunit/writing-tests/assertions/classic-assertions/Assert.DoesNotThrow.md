**Assert.DoesNotThrow** verifies that the delegate provided as an argument 
does not throw an exception. See [[Assert.DoesNotThrowAsync]] for asynchronous code.

```csharp
void Assert.DoesNotThrow(TestDelegate code);
void Assert.DoesNotThrow(TestDelegate code,
                         string message, params object[] parms);
```

#### See also...
 * [[Assert.Throws]]
 * [[ThrowsConstraint]]
