The **Assert.ThrowsAsync** is the async equivalent to [Assert.Throws](Assert.Throws.md)
for asynchronous code. See [Assert.Throws](Assert.Throws.md) for more information.

```csharp
Exception Assert.ThrowsAsync(Type expectedExceptionType, AsyncTestDelegate code);
Exception Assert.ThrowsAsync(Type expectedExceptionType, AsyncTestDelegate code,
                             string message, params object[] parms);

Exception Assert.ThrowsAsync(IResolveConstraint constraint, AsyncTestDelegate code);
Exception Assert.ThrowsAsync(IResolveConstraint constraint, AsyncTestDelegate code,
                             string message, params object[] parms);

TActual Assert.ThrowsAsync<TActual>(AsyncTestDelegate code);
TActual Assert.ThrowsAsync<TActual>(AsyncTestDelegate code,
                                    string message, params object[] parms);
```

In the above code **AsyncTestDelegate** is a delegate of the form
**Task AsyncTestDelegate()**, which is used to execute the code
in question. This will likely be a lambda expression.

The following example shows the most common way of writing tests.

```csharp
[TestFixture]
public class AssertThrowsTests
{
  [Test]
  public void Tests()
  {  
    // Using a method as a delegate
    Assert.ThrowsAsync<ArgumentException>(async () => await MethodThatThrows());
  }
  
  async Task MethodThatThrows()
  {
    await Task.Delay(100);
    throw new ArgumentException();
  }
}
```

This example shows use of the return value to perform
additional verification of the exception. Note that you do not need to await the result.

```csharp
[TestFixture]
public class UsingReturnValue
{
  [Test]
  public async Task TestException()
  {
    MyException ex = Assert.ThrowsAsync<MyException>(async () => await MethodThatThrows());

    Assert.That(ex.Message, Is.EqualTo("message"));
    Assert.That(ex.MyParam, Is.EqualTo(42)); 
  }
}
```

#### See also...
 * [Assert.Catch](Assert.Catch.md)
 * [Assert.CatchAsync](Assert.CatchAsync.md)
 * [Assert.Throws](Assert.Throws.md)
 * [[ThrowsConstraint]]
