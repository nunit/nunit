The **ValuesAttribute** is used to specify a set of values to be provided
for an individual parameter of a parameterized test method. Since
NUnit combines the data provided for each parameter into a set of
test cases, data must be provided for all parameters if it is
provided for any of them.

By default, NUnit creates test cases from all possible combinations
of the data values provided on parameters - the combinatorial approach.
This default may be modified by use of specific attributes on the
test method itself.

#### Example

```csharp
[Test]
public void MyTest([Values(1, 2, 3)] int x, [Values("A", "B")] string s)
{
    ...
}
```

The above test will be executed six times, as follows:

```csharp
MyTest(1, "A")
MyTest(1, "B")
MyTest(2, "A")
MyTest(2, "B")
MyTest(3, "A")
MyTest(3, "B")
```

#### Values with Enum or Boolean

The values attribute works in a special way with Enums and Boolean parameters.

When used without any arguments, the **[Values]** attribute on an enum parameter 
will automatically include all possible values of the enumeration.

```csharp
[Test]
public void MyEnumTest([Values]MyEnumType myEnumArgument)
{
    //...
}
```

There is the same support for Boolean values. Add the **[Values]** attribute to a bool
and the method will be run with true and false.

```csharp
[Test]
public void MyBoolTest([Values]bool value)
{
    //...
}
```

#### See also...
 * [[Range Attribute]]
 * [[Random Attribute]]
 * [[Sequential Attribute]]
 * [[Combinatorial Attribute]]
 * [[Pairwise Attribute]]
