**RegexConstraint** tests that a pattern is matched.

#### Constructor

```csharp
RegexConstraint(string pattern)
```

#### Syntax

```csharp
Does.Match(string pattern)
Matches(string pattern)
```

#### Modifiers

```csharp
...IgnoreCase
```

#### Examples of Use

```csharp
string phrase = "Make your tests fail before passing!"

Assert.That(phrase, Does.Match("Make.*tests.*pass"));
Assert.That(phrase, Does.Not.Match("your.*passing.*tests"));
```

#### Notes
1. **Matches** may appear only in the body of a constraint 
   expression or when the inherited syntax is used.
