A SameAsConstraint is used to test whether the object passed
as an actual value has the same identity as the object supplied
in its constructor.

#### Constructor


```C#
SameAsConstraint(object expected)
```


#### Syntax

```C#
Is.SameAs(object expected)
```


#### Examples of Use

```C#
Exception ex1 = new Exception();
Exception ex2 = ex1;
Assert.That(ex2, Is.SameAs(ex1));
Exception ex3 = new Exception();
Assert.That(ex3, Is.Not.SameAs(ex1));
```
