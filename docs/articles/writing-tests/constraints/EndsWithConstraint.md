**EndsWithConstraint** tests for an ending string.

#### Constructor

```C#
EndsWithConstraint(string expected)
```

#### Syntax

```C#
Does.EndWith(string expected)
EndsWith(string expected)
```

#### Modifiers

```C#
...IgnoreCase
```

#### Examples of Use

```C#
string phrase = "Make your tests fail before passing!"

Assert.That(phrase, Does.EndWith("!"));
Assert.That(phrase, Does.EndWith("PASSING!").IgnoreCase);
```

#### Notes
1. **EndsWith** may appear only in the body of a constraint 
   expression or when the inherited syntax is used.

