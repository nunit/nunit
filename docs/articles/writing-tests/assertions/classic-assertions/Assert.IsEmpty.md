**Assert.IsEmpty** may be used to test either a string or a collection or IEnumerable.
When used with a string, it succeeds if the string is the empty string.
When used with a collection, it succeeds if the collection is empty.

```csharp
Assert.IsEmpty(string aString);
Assert.IsEmpty(string aString, string message, params object[] args);

Assert.IsEmpty(IEnumerable collection);
Assert.IsEmpty(IEnumerable collection, string message,
               params object[] args);
```

> [!NOTE]
> When used with an IEnumerable that is not also an ICollection, **Assert.IsEmpty** attempts to enumerate the contents. It should not be used in cases where this results in an unwanted side effect.

#### See also...
 * [Condition Constraints](xref:constraints#condition-constraints)
