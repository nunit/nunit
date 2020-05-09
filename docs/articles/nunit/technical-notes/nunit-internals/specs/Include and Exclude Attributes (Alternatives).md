---
uid: IncludeExcludeAttributesAlternatives
---

This spec describes proposed new attributes to replace and extend the existing CultureAttribute and PlatformAttribute. These attributes derive from the abstract IncludeExcludeAttribute, which provides `Include` and `Exclude` named properties. The following examples show how these attributes are currently used:

```csharp
[Culture("fr-FR")]
[Culture(Include="fr-FR")]
[Culture(Exclude="fr-FR")]
[Platform("Net-4.5")]
[Platform(Include="Net-4.5")]
[Platform(Exclude="Net-4.5")]
```

### Alternative Approaches

**NOTE:** Three alternative approaches were identified in March, 2014. This is the final update of the document prior to choosing an alternative. Approach #3 seems to be the front-runner at this time.

#### 1. Keep the current approach

##### How it Works
Exclude has priority. If any attribute excludes a test, it remains excluded, even if another attribute matches it for inclusion. The default constructor argument represents inclusion, so you should not use it along with the Include parameter. Comma-separated alternatives form a union, so that if any item is matched, the test is included (or excluded).

##### Pros
* Having a separate attribute for each domain from which we are making a selection is easy for users to understand.
* With the new attribute-based framework extensibility model in NUnit 3.0, this is probably the easiest approach of all to extend.

##### Cons
* The current Platform attribute actually works across three different domains: the OS, the runtime and the bitness of the process. It's very difficult to define criteria like Windows 7 or 8 running .NET 4.5 using a single attribute. `[Platform("Windows7,Windows8,Net-4.5)]` will include Win 7 or 8 with any runtime and .NET 4.5 with any OS. The best we can do is `[Platform("Windows7,Windows8",Exclude="Net-2.0,Net-3.0,Net-3.5")]`.
* Boolean expressions involving the accepted values would permit the above more cleanly. For example, we might write `Platform("(Windows7 | Windows8) & !Net-4.5)")`. Such logic would have to be implemented for each attribute, most likely in the base class. However, any such logic would only apply within the single attribute: it would not be possible to include both OS and Culture (for example) in the same expression.

##### Extensibility
Create a new attribute that inherits from IncludeExcludeAttribute and implement the necessary selection logic to decide whether a given string tag is supported.

#### 2. Keep Current Approach, but Split PlatformAttribute

##### How It Would Work
As mentioned above, PlatformAttribute actually tests over three different domains. We would create a separate attribute for each domain, for example: PlatformAttribute, RuntimeAttribute and ProcessAttribute.

##### Pros
* All the same as for approach #1, plus...
* The first 'con' in approach #1 would be eliminated. The example given would work as `[Platform("Windows7,Windows8"), Runtime("Net-4.5")]`.
* Having separate attributes might make it easier to understand for some users. (see cons)

##### Cons
* Boolean expressions would become even more limited, as compared to #1
* Having separate attributes might make it harder to understand for some users. (see pros)
* We would be proliferating attributes, although that may not be a serious issue.

##### Extensibility
Create a new attribute that inherits from IncludeExcludeAttribute and implement the necessary selection logic to decide whether a given string tag is supported.

#### 3. Create Replacement Attributes

##### How It Would Work
Invert the implied hierarchy by providing an IncludeAttribute and an ExcludeAttribute, which specify what is to be tested in one of several ways. For example:

```csharp
[Include(Culture="fr-FR")]
[Exclude("Runtime:Net-4.5")]
```
> Note that the above example includes two mutually exclusive ways of implementing the syntax. See below.

##### Pros
* This seems to be at least as easy to understand as the current approach. In some cases it may be better, since use of `Include` as the attribute name makes it quite clear what is happening, whereas `Culture` may be misunderstood as **changing** the culture.
* May be extended by the user, depending on which syntactic sub-option we select below.

##### Cons
* Allows future development of an expression syntax that would combine all selection elements, but again, depending on the syntax option chosen.
* Some of the syntactic sub-options are not so easy to extend. See below.

##### Syntax Options

For this alternative we will want to use a separate namespace for each domain. There would be one set of unique names for the Operating System, one for Runtimes, one for Culture, etc. There are two options we might use to implement this, which are designated as alternatives 3A and 3B.

#### 3A. Create Replacement Attributes Using Named Properties

Map each domain to a separate named property. For example:
```csharp
[Include(Culture="fr-FR")]
[Exclude(Runtime="Net-2.0")]
```

##### Pros
* Easiest to implement
* Familiar syntax for users

##### Cons
* Need to decide how to combine tests across multiple domains. Does [Include(Culture="fr-FR", Runtime="Net-2.0")] mean both or either?
* Not possible to have logical expressions that combine domains (but maybe they are not needed).

##### Extensibility
We would need to provide a registration mechanism, whereby a program provided the name of the domain, the value and a function for evaluating whether the feature is supported or not. It would not be possible to add new domains.

#### 3B. Create Replacement Attributes Using Prefixes

Prefix each value with the domain name, thereby creating a unified namespace.

```csharp
[Include("Culture:fr-FR")]
[Exclude("Runtime:Net-2.0")]
```

##### Pros
* Similar to syntax used by VS for selecting tests
* We could create an expression syntax for combining tags.
* Could support other relations than simple equality, e.g.: ["Runtime>=4.0"].

##### Cons
* Slightly more work initially. Lots more work if we implement expressions.
* New syntax for many users

##### Extensibility
We would need to provide a registration mechanism, whereby a program provided the name of the domain, the value and a function for evaluating whether the feature is supported or not. New domains could be added in addition to new values.

### Next Steps

We need to pick the general approach and then work can begin on the feature.

