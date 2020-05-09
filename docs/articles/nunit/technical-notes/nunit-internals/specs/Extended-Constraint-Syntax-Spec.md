### DRAFT - Not Yet Implemented
This specification is about a new syntax for expressing NUnit assertions using the features of .NET framework 3.5 including language improvements.  There are two key features this enables: Extension methods permit a modular, extensible fluent constraint syntax, and LINQ expression trees permit using plain C# (or VB) as a expression-based constraint language.

### User Stories

###### Fluent Constraint Syntax

  * A user expresses assertions in a fluent fashion
  * A user extends NUnit's constraints without modifying NUnit
  * An extension developer packages new constraints and their syntax in a single extension

This improves NUnit by making extensibility easier.

###### Expression-based Constraints

  * A user expresses assertions in C# or VB without needing to learn NUnit-specific constraints
  * Any boolean expression convertible to an expression tree can form a constraint
  * Failed assertions include detailed error messages such as subexpression values - i.e. unlike Debug.Assert.

This improves NUnit by making it easier to learn and less necessary to extend.

### Existing Implementations

There are already a couple of implementations implementing a fluent syntax or expression-based syntax or both:

  * [[Extension Methods for NUnit|extension_methods_for_nunit]]: this is substantially an aliasing mechanism which, without touching the constraints themselves, provides a fluent way to express them
  * [[SharpTestsEx|http://sharptestsex.codeplex.com/]]: this is an improvement of [[NUnitEx|http://code.google.com/p/nunitex/]] and provides an extension-method based constraint syntax in addition to a starting point for an expression-based mechanism of assertions.
  * [[ExpressionToCode|http://code.google.com/p/expressiontocode/]]: this is an expression-based assertion library.  It is a reimplementation of [[Power Assert .NET|http://powerassert.codeplex.com/]], which is itself a port of [[Groovy's Power Assert|http://dontmindthelanguage.wordpress.com/2009/12/11/groovy-1-7-power-assert/]].

### Extensible Fluent Constraint Proposal
Ideally we would like to be able to use a constraint syntax similar to the current fluent syntax that is extensible.  This requires using extension methods instead of static classes, as the following example demonstrates:

```csharp
Assert.That(1, Is.GreaterThan(0))
```

This example is not extensible since it uses the static ''Is'' class.  If I wanted to write something like ''Is.MuchGreaterThan(int x)'' and thus a constraint ''MuchGreaterThan'' which NUnit doesn't provide, I would have to alter and recompile NUnit's code.  Patching NUnit like this and maintaining such a patch is a high barrier to entry.

The same assertion could be written as:

```csharp
Assert.That(1).Is.GreaterThan(0)
```

This allows for extensions since any user can define a new extension method for the class of the ''Is'' property.  Say ''Is'' were to return a value of type ''IIsConstraint'', then I could just write

```csharp
public static void MuchGreaterThan(this IIsConstraint iis) {  ...  }
```

Taking syntax shown on the [[SharpTestsEx homepage|http://sharptestex.codeplex.com/]] as an example, some assertions with the new NUnit syntax could be written as following:

| SharpTestsEx                                              | NUnit 3.0 proposal              |
|-----------------------------------------------------------|---------------------------------|
| ''true.Should().Be.True(); ''                             | ''Assert.That(true).Is.True''   | 
| ''"something".Should().Contain("some");''                  | ''Assert.That("something").Contains("some") '' |
| ''"something".Should().StartWith("so").And.EndWith("ing")''| ''Assert.That("something").StartsWith("so").And.EndsWith("ing") ''  |
| ''new[] { 1, 2, 3 }.Should().Have.SameSequenceAs(new[] { 1, 2, 3 });'' | ''Assert.That(new[] { 1, 2, 3 }).Is.EquivalentTo(new[] { 1, 2, 3 })'' |
| ''%%ActionAssert.Throws<ArgumentException>(() => new SillyClass(null))%% ''| ''%%Assert.That(() => new SillyClass(null)).Throws<ArgumentException>()%%'' |

#### Unresolved Issues

How would the following actually work?
```csharp
Assert.That(“something”).StartsWith(“so”).Or.EndsWith(“ing”);
```
How would ''StartsWith'' know that it's not supposed to do anything if it fails, since there is a chance ''EndsWith'' might succeed?

Remember, the above code is semantically identical to this:

```csharp
var temp = Assert.That(“something”).StartsWith(“so”);
temp.Or.EndsWith(“ing”);
```

Extension methods can lead to namespace pollution; in particular if defined on ''object'' (which we therefore should try to avoid).


### Expression Based Constraint Proposal

NUnit provides a wealth of constraints. This means that knowing how to expression non-trivial constraints isn't always easy (particularly for new or casual users).  Finding the appropriate constraint (or combination of constraints) requires knowledge NUnit's many constraints, and the semantics of a particular constraint may not be clear without reading the documentation.

For example, consider the assertion (valid for many uppercase strings, for most cultures):

```csharp
Assert.That(() => x == x.ToLower().ToUpper());
```

This expression can be expressed as a standard equality constraint, but doing so means not showing the intermediate steps in the computation.  Using expression trees, a failure could be rendered as:

```
Assert.That failed for:

x == x.ToLower().ToUpper()
|  | |    |         |
|  | |    |         "ABC I"
|  | |    "abc i"
|  | "ABC İ"
|  false
"ABC İ"
```

This variant requires very little knowledge of NUnit, yet is still usable even for complex constraints by leveraging a language the user already knows (namely VB or C#).

Possible extensions to this concept could be "Helpers" that recognize specific patterns and improve readability.  For instance, if an expression consists of a sequence of ''&&'' operators, a helper might suppress showing the details of non-failing clauses.  Or, if an expression contains multiple DateTime, the helper could ensure the accuracy of the DateTime.ToString is high enough to represent any differences.  If an ''=='' operator fails but ''.Equals'' would have succeeded, this could be mentioned.
