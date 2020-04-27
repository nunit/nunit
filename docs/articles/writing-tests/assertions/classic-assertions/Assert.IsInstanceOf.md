**Assert.IsInstanceOf** succeeds if the object provided as an actual value is an instance of the expected type.

```csharp
Assert.IsInstanceOf(Type expected, object actual);
Assert.IsInstanceOf(Type expected, object actual,
                    string message, params object[] parms);
Assert.IsInstanceOf<T>(object actual);
Assert.IsInstanceOf<T>(object actual,
                       string message, params object[] parms);
```

#### See also...
 * [Type Constraints](constraints#type-constraints)

