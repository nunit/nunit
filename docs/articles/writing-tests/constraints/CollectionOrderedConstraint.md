**CollectionOrderedConstraint** tests that an `IEnumerable` is ordered. If the actual value passed does not implement `IEnumerable`, an exception is thrown.

The constraint supports both simple and property-based ordering (Ordered.By).

### Simple Ordering

Simple ordering is based on the values of the items themselves. It is implied when the `By` modifier is not used.

```C#
int[] iarray = new int[] { 1, 2, 3 };
Assert.That(iarray, Is.Ordered);

string[] sarray = new string[] { "c", "b", "a" };
Assert.That(sarray, Is.Ordered.Descending);
```
The following modifiers are supported:

```C#
...Ascending
...Descending
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

By default, the order is expected to be ascending.

### Property-Based Ordering

Property-based ordering uses one or more properties that are common to every item in the enumeration. It is used when one or more instances of the `By` modifier appears in the ordering expression.

```C#
string[] sarray = new string[] ("a", "aa", "aaa");
Assert.That(sarray, Is.Ordered.By("Length"));

string[] sarray2 = new string[] ("aaa", "aa", "a");
Assert.That(sarray2, Is.Ordered.Descending.By("Length"));
```

The following Modifiers are supported:

```C#
...Then
...Ascending
...Descending
...By(string propertyName)
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

### Ordering on Multiple Properties

An ordering expression may use multiple `By` modifiers, each referring to a different property. The following examples assume a collection of items with properties named A and B.

```C#
Assert.That(collection, Is.Ordered.By("A").Then.By("B"));
Assert.That(collection, Is.Ordered.By("A").Then.By("B").Descending);
Assert.That(collection, Is.Ordered.Ascending.By("A").Then.Descending.By("B"));
Assert.That(collection, Is.Ordered.Ascending.By("A").By("B").Descending);
Assert.That(collection, Is.Ordered.Ascending.By("A").Descending.By("B")); // Illegal!
```

#### Notes:
1. The `Then` modifier divides the expression into ordering steps. Each step may optionally contain one `Ascending` or `Descending` modifier and one `Using` modifier.
2. If `Then` is not used, each new `By` modifier marks the beginning of a step. The last example statement is illegal because the first group contains both Ascending and Descending. Use of `Then` is recommended for clarity.
