The **Assert.Throws** method is pretty much in a class by itself. Rather than
comparing values, it attempts to invoke a code snippet, represented as
a delegate, in order to verify that it throws a particular exception.

It's also in a class by itself in that it returns an Exception, rather
than void, if the Assert is successful. See the example below for
a few ways to use this.

**Assert.Throws** may be used with a constraint argument, which is applied
to the actual exception thrown, or with the Type of exception expected.
The Type format is available in both a non-generic and generic form.

If the code under test is async, you must use [[Assert.ThrowsAsync]].

```csharp
Exception Assert.Throws(Type expectedExceptionType, TestDelegate code);
Exception Assert.Throws(Type expectedExceptionType, TestDelegate code,
                        string message, params object[] parms);

Exception Assert.Throws(IResolveConstraint constraint, TestDelegate code);
Exception Assert.Throws(IResolveConstraint constraint, TestDelegate code,
                        string message, params object[] parms);

Assert.Throws<T>(TestDelegate code);
Assert.Throws<T>(TestDelegate code,
                 string message, params object[] parms);
```

In the above code **TestDelegate** is a delegate of the form
**void TestDelegate()**, which is used to execute the code
in question. This may be an anonymous delegate or, when compiling
under C# 3.0 or greater, a lambda expression.

The following example shows different ways of writing the
same test.

```csharp
[TestFixture]
public class AssertThrowsTests
{
  [Test]
  public void Tests()
  {  
    // Using a method as a delegate
    Assert.Throws<ArgumentException>(MethodThatThrows);

    // Using an anonymous delegate
    Assert.Throws<ArgumentException>(
	  delegate { throw new ArgumentException(); });

    // Using a Lambda expression
    Assert.Throws<ArgumentException>(
      () => { throw new ArgumentException(); });
  }
  
  void MethodThatThrows()
  {
    throw new ArgumentException();
  }
}
```

This example shows use of the return value to perform
additional verification of the exception.

```csharp
[TestFixture]
public class UsingReturnValue
{
  [Test]
  public void TestException()
  {
    MyException ex = Assert.Throws<MyException>(
      delegate { throw new MyException("message", 42); });

    Assert.That(ex.Message, Is.EqualTo("message"));
    Assert.That(ex.MyParam, Is.EqualTo(42)); 
  }
}
```

This example does the same thing
using the overload that includes a constraint.

```csharp
[TestFixture]
public class UsingConstraint
{
  [Test]
  public void TestException()
  {
    Assert.Throws(Is.TypeOf<MyException>()
                 .And.Message.EqualTo("message")
                 .And.Property("MyParam").EqualTo(42),
      delegate { throw new MyException("message", 42); });
  }
}
```

Use the form that matches your style of coding.

### Exact Versus Derived Types

When used with a Type argument, **Assert.Throws** requires
that exact type to be thrown. If you want to test for any
derived Type, use one of the forms that allows specifying
a constraint. Alternatively, you may use [[Assert.Catch]],
which differs from **Assert.Throws** in allowing derived
types. See the following code for examples:

```csharp
// Require an ApplicationException - derived types fail!
Assert.Throws(typeof(ApplicationException), code);
Assert.Throws<ApplicationException>()(code);

// Allow both ApplicationException and any derived type
Assert.Throws(Is.InstanceOf(typeof(ApplicationException), code);
Assert.Throws(Is.InstanceOf<ApplicationException>;(), code);

// Allow both ApplicationException and any derived type
Assert.Catch<ApplicationException>(code);

// Allow any kind of exception
Assert.Catch(code);
```

#### See also...
 * [[Assert.Catch]]
 * [[Assert.CatchAsync]]
 * [[Assert.ThrowsAsync]]
 * [[ThrowsConstraint]]
