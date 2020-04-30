> AssertionHelper has been deprecated as of NUnit Framework 3.7. The syntax is now maintained and being enhanced as an independant library, [NUnit.StaticExpect](https://github.com/fluffynuts/NUnit.StaticExpect).


Some users prefer a shorter form of assertion than is given by Assert.That.
If you derive your test fixture class from **AssertionHelper**, the
Expect() method may be used instead...

```csharp
Expect(bool condition);
Expect(bool condition, string message, params object[] parms);

Expect(ActualValueDelegate del, IResolveConstraint constraint)
Expect(ActualValueDelegate del, IResolveConstraint constraint,
       string message, params object[] parms)

Expect<TActual>(TActual actual, IResolveConstraint constraint)
Expect<TActual>(TActual actual, IResolveConstraint constraint,
                string message, params object[] parms)

Expect(TestDelegate del, IResolveConstraint constraint);
```

In addition, **AssertionHelper** allows the derived class to make direct use of many of the syntactic elements that would normally require you to specify the **Is**, **Has** or **Does** class in order to use them. For example, you can write...

```csharp
Expect(actual, EqualTo("Hello"));
```

Use Intellisense to determine which syntactic elements are available in the current release.
