This is the attribute that marks a class that contains tests and, optionally, 
setup or teardown methods.
	
Most restrictions on a class that is used as a test fixture have now been
eliminated. A test fixture class:

 * May be public, protected, private or internal.
 * May be a static class.
 * May be generic, so long as any type parameters are provided or
   can be inferred from the actual arguments.
 * May not be abstract - although the attribute may be applied to an
   abstract class intended to serve as a base class for test fixtures.
 * If no arguments are provided with the TestFixtureAttribute, the class
    must have a default constructor. 
 * If arguments are provided, they must match one of the constructors.

If any of these restrictions are violated, the class is not runnable
as a test and will display as an error.

It is advisable that the constructor not have any side effects, 
since NUnit may construct the object multiple times in the course of a session.

Beginning with NUnit 2.5, the **TestFixture** attribute is optional
for non-parameterized, non-generic fixtures. So long as the class contains
at least one method marked with the **Test**, **TestCase** or 
**TestCaseSource** attribute, it will be treated as a test fixture.

#### Example:

```csharp
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  public class SuccessTests
  {
    // ...
  }
}
```


#### Inheritance

The **TestFixtureAttribute** may be applied to a base class and is
inherited by any derived classes. This includes any abstract base class,
so the well-known Abstract Fixture pattern may be implemented if desired.

In order to facilitate use of generic and/or parameterized classes,
where the derived class may require a different number of arguments (or
type arguments) from the base class, superfluous **TestFixture** 
attributes are ignored, using the following rules:

1. If all TestFixture attributes provide constructor or type arguments, 
   then all of them are used.
2. If some of the attributes provide arguments and others do not, then
   only those with arguments are used and those without arguments are ignored.
3. If none of the attributes provide arguments, one of them is selected
   for use by NUnit. It is not possible to predict which will be used, so
   this situation should generally be avoided.

This permits code like the following, which would cause an error if the
attribute on the base class were not ignored.

```csharp
[TestFixture]
public class AbstractFixtureBase
{
    ...
}

[TestFixture(typeof(string))]
public class DerivedFixture<T> : AbstractFixtureBase
{
    ...
}
```

#### Parameterized Test Fixtures

Test fixtures may take constructor arguments.
Argument values are specified as arguments to the **TestFixture**
attribute. NUnit will construct a separate instance of the fixture
for each set of arguments.
   
Individual fixture instances in a set of parameterized fixtures may be ignored. 
Set the **Ignore** named parameter of the reason for ignoring the instance.

Individual fixture instances may be given categories as well. Set the **Category**
named parameter of the attribute to the name of the category or to a comma-separated
list of categories.
   
#### Example

The following test fixture would be instantiated by NUnit three times,
passing in each set of arguments to the appropriate constructor. Note
that there are three different constructors, matching the data types
provided as arguments.
   
```csharp
[TestFixture("hello", "hello", "goodbye")]
[TestFixture("zip", "zip")]
[TestFixture(42, 42, 99)]
public class ParameterizedTestFixture
{
    private string eq1;
    private string eq2;
    private string neq;
    
    public ParameterizedTestFixture(string eq1, string eq2, string neq)
    {
        this.eq1 = eq1;
        this.eq2 = eq2;
        this.neq = neq;
    }

    public ParameterizedTestFixture(string eq1, string eq2)
        : this(eq1, eq2, null) { }

    public ParameterizedTestFixture(int eq1, int eq2, int neq)
    {
        this.eq1 = eq1.ToString();
        this.eq2 = eq2.ToString();
        this.neq = neq.ToString();
    }

    [Test]
    public void TestEquality()
    {
        Assert.AreEqual(eq1, eq2);
        if (eq1 != null && eq2 != null)
            Assert.AreEqual(eq1.GetHashCode(), eq2.GetHashCode());
    }

    [Test]
    public void TestInequality()
    {
        Assert.AreNotEqual(eq1, neq);
        if (eq1 != null && neq != null)
            Assert.AreNotEqual(eq1.GetHashCode(), neq.GetHashCode());
    }
}
```

#### Generic Test Fixtures

You may also use a generic class as a test fixture.
In order for NUnit to instantiate the fixture, you must either specify the 
types to be used as arguments to **TestFixtureAttribute** or use the
named parameter **TypeArgs=** to specify them. NUnit will construct a
separate instance of the fixture for each **TestFixtureAttribute** 
you provide.
   
#### Example

The following test fixture would be instantiated by NUnit twice,
once using an `ArrayList` and once using a `List<int>`.
   
```csharp
[TestFixture(typeof(ArrayList))]
[TestFixture(typeof(List<int>))]
public class IList_Tests<TList> where TList : IList, new()
{
  private IList list;

  [SetUp]
  public void CreateList()
  {
    this.list = new TList();
  }

  [Test]
  public void CanAddToList()
  {
    list.Add(1); list.Add(2); list.Add(3);
    Assert.AreEqual(3, list.Count);
  }
}
```

#### Generic Test Fixtures with Parameters

If a Generic fixture, uses constructor arguments, there are three
approaches to telling NUnit which arguments are type parameters 
and which are normal constructor parameters.

1. Specify both sets of parameters as arguments to the **TestFixtureAttribute**.
   Leading **System.Type** arguments are used as type parameters, while
   any remaining arguments are used to construct the instance. In the
   following example, this leads to some obvious duplication...

   ```csharp
   [TestFixture(typeof(double), typeof(int), 100.0, 42)]
   [TestFixture(typeof(int) typeof(double), 42, 100.0)]
   public class SpecifyBothSetsOfArgs<T1, T2>
   {
       T1 t1;
       T2 t2;

       public SpecifyBothSetsOfArgs(T1 t1, T2 t2)
       {
           this.t1 = t1;
           this.t2 = t2;
       }

       [TestCase(5, 7)]
       public void TestMyArgTypes(T1 t1, T2 t2)
       {
           Assert.That(t1, Is.TypeOf<T1>());
           Assert.That(t2, Is.TypeOf<T2>());
       }
   }
   ```

2. Specify normal parameters as arguments to **TestFixtureAttribute**
   and use the named parameter **TypeArgs=** to specify the type
   arguments. Again, for this example, the type info is duplicated, but
   it is at least more cleanly separated from the normal arguments...

   ```csharp
   [TestFixture(100.0, 42, TypeArgs=new Type[] { typeof(double), typeof(int) })]
   [TestFixture(42, 100.0, TypeArgs=new Type[] { typeof(int), typeof(double) })]
   public class SpecifyTypeArgsSeparately<T1, T2>
   {
       T1 t1;
       T2 t2;

       public SpecifyTypeArgsSeparately(T1 t1, T2 t2)
       {
           this.t1 = t1;
           this.t2 = t2;
       }

       [TestCase(5, 7)]
       public void TestMyArgTypes(T1 t1, T2 t2)
       {
           Assert.That(t1, Is.TypeOf<T1>());
           Assert.That(t2, Is.TypeOf<T2>());
       }
   }
   ```

3. In some cases, when the constructor makes use of all the type parameters 
   NUnit may simply be able to deduce them from the arguments provided. 
   That's the case here and the following is the preferred way to
   write this example...
   
   ```csharp
   [TestFixture(100.0, 42)]
   [TestFixture(42, 100.0)]
   public class DeduceTypeArgsFromArgs<T1, T2>
   {
       T1 t1;
       T2 t2;

       public DeduceTypeArgsFromArgs(T1 t1, T2 t2)
       {
           this.t1 = t1;
           this.t2 = t2;
       }

       [TestCase(5, 7)]
       public void TestMyArgTypes(T1 t1, T2 t2)
       {
           Assert.That(t1, Is.TypeOf<T1>());
           Assert.That(t2, Is.TypeOf<T2>());
       }
   }
   ```
