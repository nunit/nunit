**AndConstraint** combines two other constraints and succeeds only if they both succeed.

#### Constructor

```C#
AndConstraint(Constraint left, Constraint right)
```

#### Syntax

```C#
<Constraint>.And.<Constraint>
```

#### Examples of Use

```C#
Assert.That(2.3, Is.GreaterThan(2.0).And.LessThan(3.0));
```

#### Evaluation Order and Precedence

Note that the constraint evaluates the sub-constraints left to right, meaning that 
`Assert.That(i, Is.Not.Null.And.GreaterThan(9));` where `i` is a nullable `int` will work for `10`, 
but fail for `null` with the message `Expected: not null and greater than 9. But was:  null`.
`Assert.That(i, Is.GreaterThan(9).And.Not.Null);` will also succeed for `10`, but throw an exception for `null`, as `null` cannot be compared to `9`.

The **OrConstraint** has precedence over the **AndConstraint**.

#### See also...
 * [[OrConstraint]]
