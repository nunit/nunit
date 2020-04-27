**EndsWithConstraint** tests for an ending string.

#### Constructor

```csharp
EndsWithConstraint(string expected)
```

#### Syntax

```csharp
Does.EndWith(string expected)
EndsWith(string expected)
```

#### Modifiers

```csharp
...IgnoreCase
```

#### Examples of Use

```csharp
string phrase = "Make your tests fail before passing!"

Assert.That(phrase, Does.EndWith("!"));
Assert.That(phrase, Does.EndWith("PASSING!").IgnoreCase);
```

#### Notes
1. **EndsWith** may appear only in the body of a constraint 
   expression or when the inherited syntax is used.

