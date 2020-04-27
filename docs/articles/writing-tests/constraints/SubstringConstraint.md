**SubstringConstraint** tests for a substring.

#### Constructor

```csharp
SubstringConstraint(string expected)
```

#### Syntax

```csharp
Does.Contain(string expected)
```

#### Modifiers

```csharp
...IgnoreCase
```

#### Examples of Use

```csharp
string phrase = "Make your tests fail before passing!"

Assert.That(phrase, Does.Contain("tests fail"));
Assert.That(phrase, Does.Not.Contain("tests pass"));
Assert.That(phrase, Does.Contain("make").IgnoreCase);
```

