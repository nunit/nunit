**StartsWithConstraint** tests for an initial string.

#### Constructor

```C#
StartsWithConstraint(string expected)
```

#### Syntax

```C#
Does.StartWith(string expected)
StartsWith(string expected)
```

#### Modifiers

```C#
...IgnoreCase
```

#### Examples of Use

```C#
string phrase = "Make your tests fail before passing!"

Assert.That(phrase, Does.StartWith("Make"));
Assert.That(phrase, Does.Not.StartWith("Break"));
```

#### Notes

1. **StartsWith** may appear only in the body of a constraint 
   expression or when the inherited syntax is used.


