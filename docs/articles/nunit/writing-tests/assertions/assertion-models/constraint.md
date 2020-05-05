The constraint-based Assert model uses a single method of the Assert class
for all assertions. The logic necessary to carry out each assertion is
embedded in the constraint object passed as the second parameter to that
method.
   
Here's a very simple assert using the constraint model:

```csharp
      Assert.That(myString, Is.EqualTo("Hello"));
```

The second argument in this assertion uses one of NUnit's **syntax helpers**
to create an **EqualConstraint**. The same assertion could also be made in this form:

```csharp
      Assert.That(myString, new EqualConstraint("Hello"));
```

Using this model, all assertions are made using one of the forms of the
`Assert.That()` method, which has a number of overloads...
   
```csharp
Assert.That(bool condition);
Assert.That(bool condition, string message, params object[] parms);
Assert.That(bool condition, Func<string> getExceptionMessage);

Assert.That(Func<bool> condition);
Assert.That(Func<bool> condition, string message, params object[] parms);
Assert.That(Func<bool> condition, Func<string> getExceptionMessage);

Assert.That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint constraint)
Assert.That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint constraint,
    string message, object[] parms)
Assert.That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
    Func<string> getExceptionMessage)

Assert.That<TActual>(TActual actual, IResolveConstraint constraint)
Assert.That<TActual>(TActual actual, IResolveConstraint constraint, string message,
    params object[] parms)
Assert.That<TActual>(TActual actual, IResolveConstraint expression,
    Func<string> getExceptionMessage)

Assert.That(TestDelegate del, IResolveConstraint constraint)
Assert.That(TestDelegate code, IResolveConstraint constraint, string message,
    params object[] args)
Assert.That(TestDelegate code, IResolveConstraint constraint,
    Func<string> getExceptionMessage)
```

The overloads that take a bool work exactly like `Assert.IsTrue`, except that `Assert.IsTrue` does not accept a `Func<string>` for the exception message.

For overloads taking a constraint, the argument must be a object implementing 
the **IResolveConstraint** interface, which supports performing a test
on an actual value and generating appropriate messages. This interface
is described in more detail under [[Custom Constraints]].
   
NUnit provides a number of constraint classes similar to the **EqualConstraint**
used in the example above. Generally, these classes may be used directly or
through a syntax helper. The valid forms are described on the pages related to
each constraint.
   
### See also...
 * [[Classic Model]]
