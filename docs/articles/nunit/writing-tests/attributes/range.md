The **RangeAttribute** is used to specify a range of values to be provided
for an individual parameter of a parameterized test method. Since
NUnit combines the data provided for each parameter into a set of
test cases, data must be provided for all parameters if it is
provided for any of them.

By default, NUnit creates test cases from all possible combinations
of the datapoints provided on parameters - the combinatorial approach.
This default may be modified by use of specific attributes on the
test method itself.

RangeAttribute supports the following constructors:

```csharp
public RangeAttribute(int from, int to);
public RangeAttribute(int from, int to, int step);
public RangeAttribute(long from, long to, long step);
public RangeAttribute(float from, float to, float step);
public RangeAttribute(double from, double to, double step);
```

#### Example

The following test will be executed nine times.

```csharp
[Test]
public void MyTest(
    [Values(1, 2, 3)] int x,
    [Range(0.2, 0.6, 0.2)] double d)
{
    ...
}
```

The MyTest method is called nine times, as follows:

```csharp
MyTest(1, 0.2)
MyTest(1, 0.4)
MyTest(1, 0.6)
MyTest(2, 0.2)
MyTest(2, 0.4)
MyTest(2, 0.6)
MyTest(3, 0.2)
MyTest(3, 0.4)
MyTest(3, 0.6)
```

#### See also...
 * [Values Attribute](Values.md)
 * [Random Attribute](Random.md)
 * [Sequential Attribute](Sequential.md)
 * [Combinatorial Attribute](Combinatorial.md)
 * [Pairwise Attribute](Pairwise.md)
