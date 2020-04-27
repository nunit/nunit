**Assert.IsAssignableFrom** succeeds if the object provided may be assigned a value of the expected type.

```csharp
Assert.IsAssignableFrom(Type expected, object actual);
Assert.IsAssignableFrom(Type expected, object actual,
                        string message, params object[] parms);
Assert.IsAssignableFrom<T>(object actual);
Assert.IsAssignableFrom<T>(object actual,
                           string message, params object[] parms);
```

#### See also...
 * [Type Constraints](constraints#type-constraints)
