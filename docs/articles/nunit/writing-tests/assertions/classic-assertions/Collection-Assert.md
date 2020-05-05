The CollectionAssert class provides a number of methods that are useful when examining collections and their contents or for comparing two collections. These methods may be used with any object implementing `IEnumerable`.

The **AreEqual** overloads succeed if the corresponding elements of the two
collections are equal. **AreEquivalent** tests whether the collection contents
are equal, but without regard to order. In both cases, elements are compared using
NUnit's default equality comparison.

```csharp
CollectionAssert.AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType);
CollectionAssert.AllItemsAreInstancesOfType(
    IEnumerable collection, Type expectedType, string message, params object[] args);

CollectionAssert.AllItemsAreNotNull(IEnumerable collection);
CollectionAssert.AllItemsAreNotNull(
    IEnumerable collection, string message, params object[] args);

CollectionAssert.AllItemsAreUnique(IEnumerable collection);
CollectionAssert.AllItemsAreUnique(
    IEnumerable collection, string message, params object[] args);

CollectionAssert.AreEqual(IEnumerable expected, IEnumerable actual);
CollectionAssert.AreEqual(
    IEnumerable expected, IEnumerable actual, string message, params object[] args);

CollectionAssert.AreEquivalent(IEnumerable expected, IEnumerable actual);
CollectionAssert.AreEquivalent(
    IEnumerable expected, IEnumerable actual, string message, params object[] args);

CollectionAssert.AreNotEqual(IEnumerable expected, IEnumerable actual);
CollectionAssert.AreNotEqual(
    IEnumerable expected, IEnumerable actual, string message, params object[] args);

CollectionAssert.AreNotEquivalent(IEnumerable expected, IEnumerable actual);
CollectionAssert.AreNotEquivalent(
    IEnumerable expected, IEnumerable actual, string message, params object[] args);

CollectionAssert.Contains(IEnumerable expected, object actual);
CollectionAssert.Contains(
    IEnumerable expected, object actual, string message, params object[] args);

CollectionAssert.DoesNotContain(IEnumerable expected, object actual);
CollectionAssert.DoesNotContain(
    IEnumerable expected, object actual, string message, params object[] args);

CollectionAssert.IsSubsetOf(IEnumerable subset, IEnumerable superset);
CollectionAssert.IsSubsetOf(
    IEnumerable subset, IEnumerable superset, string message, params object[] args);

CollectionAssert.IsNotSubsetOf(IEnumerable subset, IEnumerable superset);
CollectionAssert.IsNotSubsetOf(
    IEnumerable subset, IEnumerable superset, string message, params object[] args);

CollectionAssert.IsEmpty(IEnumerable collection);
CollectionAssert.IsEmpty(
    IEnumerable collection, string message, params object[] args);

CollectionAssert.IsNotEmpty(IEnumerable collection);
CollectionAssert.IsNotEmpty(
    IEnumerable collection, string message, params object[] args);

CollectionAssert.IsOrdered(IEnumerable collection);
CollectionAssert.IsOrdered(
    IEnumerable collection, string message, params object[] args);

CollectionAssert.IsOrdered(IEnumerable collection, IComparer comparer);
CollectionAssert.IsOrdered(IEnumerable collection,
    IComparer comparer, string message, params object[] args);
```

#### See also...
 * [Collection Constraints](https://github.com/nunit/docs/wiki/Constraints#collection-constraints)
