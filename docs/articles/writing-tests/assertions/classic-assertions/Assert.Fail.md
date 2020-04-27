The **Assert.Fail** method provides you with the ability to generate a failure based
on tests that are not encapsulated by the other methods. It is also useful in
developing your own project-specific assertions.

```csharp
Assert.Fail();
Assert.Fail(string message, params object[] parms);
```

Here's an example of its use to create a private assertion that tests whether a
string contains an expected value.

```csharp
public void AssertStringContains(string expected, string actual)
{
    AssertStringContains(expected, actual, string.Empty);
}

public void AssertStringContains(string expected, string actual, string message)
{
    if (actual.IndexOf(expected) < 0)
        Assert.Fail(message);
}
```

#### See also...
 * [[Assert.Pass]]
 * [[Assert.Ignore]]
 * [[Assert.Inconclusive]]
