**OrConstraint** combines two other constraints and succeeds if either of them succeeds.

#### Constructor

```csharp
OrConstraint(Constraint left, Constraint right)
```

#### Syntax

```csharp
<Constraint>.Or.<Constraint>
```

#### Examples of Use

```csharp
Assert.That(3, Is.LessThan(5).Or.GreaterThan(10));
```

#### Evaluation Order and Precedence

Note that the constraint evaluates the sub-constraints left to right, meaning that `Assert.That(i, Is.Null.Or.GreaterThan(9));` where `i` is a nullable `int` will work for both `12` and `null`. On the other hand, 
`Assert.That(i, Is.GreaterThan(9).Or.Null);` will only work for `12`, but throw an exception for `null`, as `null` cannot be compared to `9`.

The **OrConstraint** has precedence over the **AndConstraint**.

#### See also...
 * [AndConstraint](AndConstraint.md)
