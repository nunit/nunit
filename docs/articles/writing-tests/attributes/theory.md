---
uid: Theory-Attribute
---

A Theory is a special type of test, used to verify a general
statement about the system under development. Normal tests are
_example-based_. That is, the developer supplies one or
more examples of inputs and expected outputs either within the
code of the test or - in the case of [[Parameterized Tests]] -
as arguments to the test method. A theory, on the other hand,
makes a general statement that all of its assertions will pass
for all arguments satisfying certain assumptions.
   
Theories are implemented in NUnit as non-generic
methods within a **TestFixture**, which are annotated
with the **TheoryAttribute**. Theory methods must always have 
arguments and therefore appears quite similar to [[Parameterized Tests]]
at first glance. However, a Theory incorporates additional data sources 
for its arguments and allows special processing for assumptions
about that data. The key difference, though, is that theories
make general statements and are more than just a set of examples.
   
#### Data for Theories

The primary source of data for a **Theory** is the
[[Datapoint Attribute]] or [[DatapointSource Attribute]]. 
NUnit will use any class members of the required types, which are annotated
with one of these attributes, to provide data for each parameter
of the Theory. NUnit assembles the values for individual arguments 
combinatorially to provide test cases for the theory.
   
In addition to the Datapoint and Datapoints attributes, it
is possible to use any of the approaches for supplying data
that are recognized on normal parameterized tests. We suggest
that this capability not be overused, since it runs counter
to the distinction between a test based on examples and a
theory. However, it may be useful in order to guarantee that
a specific test case is included.

For **boolean** and **enum** arguments, NUnit can supply the 
data without any action by the user. All possible values are supplied
to the argument. This feature is disabled if the user supplies any 
values for the argument.

> [!NOTE]
> Because NUnit searches for datapoints based on the type of the argument, generic methods may not currently be used as theories. This limitation may be removed in a future release. See below for a workaround using a generic fixture.
   
#### Assumptions

The theory itself is responsible for ensuring that all data supplied
meets its assumptions. It does this by use of the
**Assume.That(...)** construct, which works just like
**Assert.That(...)** but does not cause a failure. If
the assumption is not satisfied for a particular test case, that case
returns an Inconclusive result, rather than a Success or Failure. 
   
The overall result of executing a Theory over a set of test cases is 
determined as follows:
   
 * If the assumptions are violated for **all** test cases, then the Theory itself is marked as a failure.
   
 * If any Assertion fails, the Theory itself fails.
   
 * If at least **some** cases pass the stated assumptions, and there are **no** assertion failures or exceptions, then the Theory passes.

Since the user does not generally care about inconclusive cases under
a theory, they are not normally displayed in the Gui. For situations
where they are needed - such as debugging - the context menu for the
theory provides an option to display them.
   
#### Example:

In the following example, the Theory SquareRootDefinition
verifies that the implementation of square root satisfies
the following definition:
   
> Given a non-negative number, the square root of that number
> is always non-negative and, when multiplied by itself, gives 
> the original number.

```csharp
public class SqrtTests
{
    [DatapointSource]
    public double[] values = new double[] { 0.0, 1.0, -1.0, 42.0 };

    [Theory]
    public void SquareRootDefinition(double num)
    {
        Assume.That(num >= 0.0);

        double sqrt = Math.Sqrt(num);

        Assert.That(sqrt >= 0.0);
        Assert.That(sqrt * sqrt, Is.EqualTo(num).Within(0.000001));
    }
}
```

#### Theories in Generic Fixtures

In a generic fixture with Type parameter `T` individual methods using `T` as
a parameter type or not generic, since `T` has been resolved to an actual
Type in instantiating the fixture instance. You may use such methods as
theories and any data of the appropriate type will be used.

```csharp
[TestFixture(typeof(int))]
[TestFixture(typeof(double))]
public class TheorySampleTestsGeneric<T>
{
    [Datapoint]
    public double[] ArrayDouble1 = { 1.2, 3.4 };
    [Datapoint]
    public double[] ArrayDouble2 = { 5.6, 7.8 };
    [Datapoint]
    public int[] ArrayInt = { 0, 1, 2, 3 };

    [Theory]
    public void TestGenericForArbitraryArray(T[] array)
    {
        Assert.That(array.Length, Is.EqualTo(4));
    }
}
```
   
#### See also...

 * [[Datapoint Attribute]]
 * [[DatapointSource Attribute]]
 * [[Parameterized Tests]]
