---
uid: customconstraints
---

## Page Under Development

You can implement your own custom constraints by creating a class that 
inherits from the `Constraint` abstract class, which supports performing a 
test on an actual value and generating appropriate messages.

### `Constraint` Abstract Class

Implementations must override the one abstract method `ApplyTo<TActual>` which
evaluates the previously stored expected value (if any) against the method's 
parameter, the actual value. There are also several virtual methods that may be
overridden to change some default behaviors.

The relevant portions of the `Constraint` class are represented below.
   
```C#
namespace NUnit.Framework.Constraints
{
    public abstract class Constraint
    {
        protected Constraint(params object[] args) {}
        public abstract ConstraintResult ApplyTo<TActual>(TActual actual);
        ...
        public virtual ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del) {}
        public virtual ConstraintResult ApplyTo<TActual>(ref TActual actual) {}
        protected virtual object GetTestObject<TActual>(ActualValueDelegate<TActual> del) {}
        public virtual string Description { get; protected set; }
        protected virtual string GetStringRepresentation() {}
    }
}
```

#### `Constraint` Constructor

The `Constraint` constructor accepts zero or more arguments and saves them to be used
in the printed description later. Constraints like `NullConstraint` or `UniqueItemsConstraint`
take no arguments and simply state some condition about the actual value supplied. Constraints
with a single argument usually treat it as the expected value resulting from some operation.
Multiple arguments can be provided where the semantics of the constraint call for it.

#### `ApplyTo` Implementation

The `ApplyTo<TActual>(TActual actual)` method must be overridden and provides for the
core implementation of the custom constraint. Whatever logic defines pass or fail
of the constraint and actual/expected values goes into the `ApplyTo<TActual>(TActual actual)`
method.

For example, a very naive implementation of a reference equality constraint might look 
like this:

```C#
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, ReferenceEquals(actual, Arguments[0]));
        }
```

The key here is there needs to be some evaluation of the constraint logic, and the return value
must be a `ConstraintResult` or subclass thereof. Custom subclasses of `ConstraintResult` may
be used to further customize the message provided upon failure, as described below.

#### `ApplyTo` Overloads

Constraints may be called with a delegate to return the actual value instead of the actual
value itself. This serves to delay evaluation of the value. The default implementation
of `ApplyTo<TActual>(ActualValueDelegate<TActual> del)` waits for the delegate to
complete if it's an async operation, other immediately calls the delegate if synchronous, and 
then calls the abstract `ApplyTo<TActual>(TActual actual)` method with the value.

Another overload also exists, `ApplyTo<TActual>(ref TActual actual)`. The default implementation
dereferences the value and then calls the abstract `ApplyTo<TActual>(TActual actual)` method 
with the value. This public virtual method is available by use from calling code but currently
is not used from any framework calls within NUnit itself.

#### `GetTestObject` Optional Override

The default implementation of `ApplyTo<TActual>(ActualValueDelegate<TActual> del)` does not
simply execute the delegate but actually calls out to another virtual method, 
`GetTestObject<TActual>(ActualValueDelegate<TActual> del)`. This method can be overridden to
 keep the default behavior of `ApplyTo<TActual>(ActualValueDelegate<TActual> del)` while still
 customizing how the actual value delegate is invoked.

### `Description` Property

This virtual property is used to provide a description of the constraint for messages. Simple
constant values can be set in the custom constraint's constructor. If more complex logic is
needed, override the property and provide a custom implementation of `get`.

Here are a few simple examples from built-in constraints.

```C#
public class FalseConstraint : Constraint
{
    public FalseConstraint()
    {
        this.Description = "False";
    }
}

public class NullConstraint : Constraint
{
    public NullConstraint()
    {
        this.Description = "null";
    }
}
```
    
Here are a few complex examples from built-in constraints.

```C#
public class AndConstraint : BinaryConstraint
{
    public override string Description
    {
        get { return Left.Description + " and " + Right.Description; }
    }
}

public abstract class PrefixConstraint : Constraint
{
    public override string Description
    {
        get
        {
            return string.Format(
                baseConstraint is EqualConstraint ? "{0} equal to {1}" : "{0} {1}", 
                descriptionPrefix, 
                baseConstraint.Description);
        }
    }
}
```

#### `GetStringRepresentation` Method

NUnit calls the `GetStringRepresentation` method to return a string representation of the
constraint, including the expected value(s). The default implementation returns the lowercase
display name of the constraint followed by all expected values, separated by a space. 

For example, a custom constraint `ReferenceEqualsConstraint` with an instance of a custom
`MyObject` class as expected value would result in a default string representation of 
`<referenceequals MyObject>`.

You can override the initial display name only by setting `DisplayName` in your constructor.
This public property cannot be overridden, but the `Constraint` base class sets it in the 
base constructor to be the name of the class, minus the "Constraint" suffix and minus
any generic suffixes.

### Custom Constraint Usage Syntax

Having written a custom constraint class, you can use it directly through its constructor:

```C#
Assert.That(myObject, new CustomConstraint());
```

You may also use it in expressions through NUnit's `Matches` syntax element:

```C#
Assert.That(myObject, Is.Not.Null.And.Matches(new CustomConstraint());
```

The direct construction approach is not very convenient or easy to read.
For its built-in constraints, NUnit includes classes that implement a special 
constraint syntax, allowing you to write things like...

```C#
Assert.That(actual, Is.All.InRange(1, 100));
```

Custom constraints can support this syntax by providing a static helper class and
extension method on `ConstraintExpression`, such as this.

```C#
public static class CustomConstraintExtensions
{
    public static ContentsEqualConstraint ContentsEqual(this ConstraintExpression expression, object expected)
    {
        var constraint = new ContentsEqualConstraint(expected);
        expression.Append(constraint);
        return constraint;
    }
}
```
    
To fully utilize your custom constraint the same way built-in constraints are used, you'll
need to implement two additional classes (which can cover all your constraints, not
for each custom constraint).

1. Provide a static class patterned after NUnit's `Is` class, with properties
   or methods that construct your custom constructor. If you like, you can even call it
   `Is` and extend NUnit's `Is`, provided you place it in your own namespace and avoid 
   any conflicts. This allows you to write things like:

   ```C#
   Assert.That(actual, Is.Custom(x, y));
   ```
   
   with this sample implementation:
   
   ```C#
    public class Is : NUnit.Framework.Is
    {
        public static CustomConstraint Custom(object expected)
        {
            return new CustomConstraint(expected);
        }
    }   
    ```
    
2. Provide an extension method for NUnit's `ConstraintExpression`, allowing
   you to write things like:

   ```C#
   Assert.That(actual, Is.Not.Custom(x, y));
   ```

    with this sample implementation:
    
    ```C#
    public static class CustomConstraintExtensions
    {
        public static CustomConstraint Custom(this ConstraintExpression expression, object expected)
        {
            var constraint = new CustomConstraint(expected);
            expression.Append(constraint);
            return constraint;
        }
    }    
    ```
    
