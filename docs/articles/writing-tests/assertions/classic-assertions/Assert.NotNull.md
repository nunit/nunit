**Assert.NotNull** and **Assert.IsNotNull** test that the specified object is non-null.
The two forms are provided for compatibility with past versions of NUnit and
NUnitLite.

```csharp
Assert.NotNull(object anObject);
Assert.NotNull(object anObject, string message, params object[] parms);

Assert.IsNotNull(object anObject);
Assert.IsNotNull(object anObject, string message, params object[] parms);
```

#### See also...
 * [Condition Constraints](constraints#condition-constraints)
