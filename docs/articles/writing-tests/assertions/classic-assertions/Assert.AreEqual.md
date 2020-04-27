**Assert.AreEqual** tests whether the two arguments are equal. 

```csharp
Assert.AreEqual(double expected, double actual, double tolerance);
Assert.AreEqual(double expected, double actual, double tolerance,
                string message, params object[] parms);

Assert.AreEqual(object expected, object actual);
Assert.AreEqual(object expected, object actual,
                string message, params object[] parms);
```

#### Comparing Numerics of Different Types

The method overloads that compare two objects make special provision so that numeric
values of different types compare as expected. This assert succeeds:

```csharp
Assert.AreEqual(5, 5.0);
```
#### Comparing Floating Point Values

Values of type float and double are compared using an additional
argument that indicates a tolerance within which they will be considered
as equal.

Special values are handled so that the following Asserts succeed:

```csharp
Assert.AreEqual(double.PositiveInfinity, double.PositiveInfinity);
Assert.AreEqual(double.NegativeInfinity, double.NegativeInfinity);
Assert.AreEqual(double.NaN, double.NaN);
```

#### Comparing Arrays and Collections

NUnit is able to compare single-dimensioned arrays, multi-dimensioned arrays, 
nested arrays (arrays of arrays) and collections. Two arrays or collections are considered equal
if they have the same dimensions and if each pair of corresponding elements is equal. 

NUnit 3.0 adds the ability to compare generic collections and dictionaries.

#### See also...
 * [Equal Constraint](EqualConstraint)
 * [[DefaultFloatingPointTolerance Attribute]]
