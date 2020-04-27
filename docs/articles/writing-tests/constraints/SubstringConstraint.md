**SubstringConstraint** tests for a substring.

#### Constructor

```C#
SubstringConstraint(string expected)
```

#### Syntax

```C#
Does.Contain(string expected)
```

#### Modifiers

```C#
...IgnoreCase
```

#### Examples of Use

```C#
string phrase = "Make your tests fail before passing!"

Assert.That(phrase, Does.Contain("tests fail"));
Assert.That(phrase, Does.Not.Contain("tests pass"));
Assert.That(phrase, Does.Contain("make").IgnoreCase);
```

