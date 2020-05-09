### DRAFT - Not Yet Implemented

This spec describes proposed new attributes to replace and extend the existing CultureAttribute and PlatformAttribute. We considered several alternative approaches to doing this and selected the approach described here. See [Include and Exclude Attributes (Alternatives)](xref:IncludeExcludeAttributesAlternatives) for the original discussion of choices.

#### Attributes

We would define two new attributes, **IncludeAttribute** and **ExcludeAttribute**:

```csharp
[Include(Culture="fr-FR")]
[Include(Platform="Win7,Win8")]
[Exclude("Runtime:Net-4.5")]
[Exclude("Linux")]
[Include("Runtime >= 4.0 and Platform == "Linux"")]
```

#### Syntax Options

There are a number of possible alternatives for specifying when a test is to be included or excluded. Each of them is illustrated in the examples above, from which we need to select a subset.

##### 1. Named Properties

As illustrated in the first two examples, this approach allows us to clearly separate the namespaces for each selection domain. Comma-separated values are intended to be alternatives, so the second example would include the test under either Win7 or Win8. Separate properties (domains) would be anded together.

##### 2. Prefixes

By prefixing each value with the domain name, we can create a unified namespace. The third example above uses this approach, making it clear that Net-4.5 represents a runtime and nothing else. As in #1, comma-separated values are alternatives. There is no other way to create logical combinations.

##### 3. Implied Domain

To the extent that the accepted values are exclusive, it's possible to deduce the domain from the value. The fourth example above takes advantage of this. Note that this approach can be combined with #2 as a kind of shorthand.

##### 4. Logical Expression

We can define a DSL that allows us to specify arbitrary expressions for including or excluding tests. The last example above does this. If the expression language included **not**, then we could do away with the ExcludeAttribute.

#### Comparison of Options

**Note:** The following are Charlie's ratings. Feel free to disagree!

##### Ease of implementation (easy to hard):
    1 > 2 > 3 > 4

##### Expressiveness (least to greatest):
    3 > 2 > 1 > 4

##### Learning Curve (shallow to steep):
    1 > 3 > 2 > 4

##### Extensibility to new domains (easy to hard):
    1 > (2, 3, 4)

##### Extensibility with new values (easy to hard):
    3 > (2, 4) > 1