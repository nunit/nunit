**Assert.IsNotEmpty** may be used to test either a string or a collection or IEnumerable.
When used with a string, it succeeds if the string is not the empty string.
When used with a collection, it succeeds if the collection is not empty.

```csharp
Assert.IsNotEmpty(string aString);
Assert.IsNotEmpty(string aString, string message, params object[] args);

Assert.IsNotEmpty(IEnumerable collection);
Assert.IsNotEmpty(IEnumerable collection, string message,
                  params object[] args);
```

**Note:** When used with an IEnumerable that is not also an ICollection, **Assert.IsEmpty** attempts to enumerate the contents. It should not be used in cases where this results in an unwanted side effect.

#### See also...
 * [Condition Constraints](constraints#condition-constraints)
