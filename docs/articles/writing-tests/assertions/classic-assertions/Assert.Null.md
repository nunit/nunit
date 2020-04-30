**Assert.Null** and **Assert.IsNull** test that the specified object is null.
The two forms are provided for compatibility with past versions of NUnit and
NUnitLite.

```csharp
Assert.Null(object anObject);
Assert.Null(object anObject, string message, params object[] parms);

Assert.IsNull(object anObject);
Assert.IsNull(object anObject, string message, params object[] parms);
```

#### See also...
 * [Condition Constraints](xref:constraints#condition-constraints)
