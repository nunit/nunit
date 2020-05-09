Assertions are central to unit testing in any of the xUnit frameworks, and NUnit 
is no exception. NUnit provides a rich set of assertions as static methods of 
the Assert class.

If an assertion fails, the method call does not return and an error is reported. 
If a test contains multiple assertions, any that follow the one that failed 
will not be executed. For this reason, it's usually best to try for one 
assertion per test.

Each method may be called without a message, with a simple text message or with 
a message and arguments. In the last case the message is formatted using the 
provided text and arguments.
	
### Two Models

In NUnit 3.0, assertions are written primarily using the `Assert.That` method,
which takes [constraint objects](xref:constraints) as an argument. We call this
the [Constraint Model](xref:constraintmodel) of assertions.

In earlier versions of NUnit, a separate method of the Assert class was used 
for each different assertion. This [Classic Model](xref:classicmodel) is still supported but
since no new features have been added to it for some time, the constraint-based
model must be used in order to have full access to NUnit's capabilities.

For example, the following code must use the constraint model. There is no real classic equivalent.

```csharp
int[] array = new int[] { 1, 2, 3 };
Assert.That(array, Has.Exactly(1).EqualTo(3));
Assert.That(array, Has.Exactly(2).GreaterThan(1));
Assert.That(array, Has.Exactly(3).LessThan(100));
```

Where equivalents do exist, the two approaches will always give the same result,
because the methods of the classic approach have all been implemented internally
using constraints. For example...

```csharp
Assert.AreEqual(4, 2+2);
Assert.That(2+2, Is.EqualTo(4));
```
