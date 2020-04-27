**Assert.IsNotInstanceOf** succeeds if the object provided as an actual value is not an instance of the expected type.

```csharp
Assert.IsNotInstanceOf(Type expected, object actual);
Assert.IsNotInstanceOf(Type expected, object actual,
                       string message, params object[] parms);
Assert.IsNotInstanceOf<T>(object actual);
Assert.IsNotInstanceOf<T>(object actual,
                          string message, params object[] parms);
```

#### See also...
 * [Type Constraints](constraints#type-constraints)
