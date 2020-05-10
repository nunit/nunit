# Coding Standards

In order to keep the code consistent, we follow certain conventions. Many of the choices we have made are somewhat arbitrary and could easily have gone another way. At this point, most of these conventions are already well-established, so please don't re-open a discussion about them unless you have new issues to present.

Notice that these standards are all stylistic. Do not write standards that tell people how to program. For example, we don't need a standard that tells us to always dispose of disposable objects because that's part of the normal "standard" for C# programming. In other words, these standards are about relatively trivial things that we have all agreed to do the same way.

## Making Changes

If you do want to make changes, please don't just edit the wiki. Initiate a discussion of what you want to change on the developer list first. If you add an entire new section, you can edit first and then present it to the list for discussion. However, if you are intending to standardize more things than we usually standardize, it's wise to discuss it first to avoid wasting time!

## General - Please Read This

Follow these guidelines unless you have an extremely good reason not to. Add a comment explaining why you are not following them so others will know your reasoning.

Don't make arbitrary changes in existing code merely to conform them to these guidelines. Normally, you should only change the parts of a file that you have to edit in order to fix a bug or implement new functionality. In particular, don't use automatic formatting to change an entire file at once as this makes it difficult to identify the underlying code changes when we do a code review.

In cases where we make broad changes in layout or naming, they should be committed separately from any bug fixes or feature changes in order to keep the review process as simple as possible. That said, we don't do this very often, since we have real work to do!

Visual Studio can be set up to match the coding standards by importing the [nunit.vssettings](https://github.com/nunit/docs/blob/master/nunit.vssettings) file from this repository. This file will only change the C# indentation and formatting settings. It will not modify any other Visual Studio settings. It can be imported into Visual Studio 2010 and later by going to Tools | Import and Export Settings...

## Copyright

NUnit is licensed under the MIT / X11 license. Each file is prefixed by the [NUnit Copyright Notice](NUnit-Copyright-Notice.md) enclosed in appropriate comment characters for the language of the file.

### Notes

1. Charlie Poole is the copyright holder for the NUnit code on behalf of the NUnit community, at least until some other arrangement is made. Do not place your name on the copyright line if you wish to contribute code.

2. The year given is the year of the file's creation. Subsequently, as copyrightable (non-trivial) changes are made, additional years or ranges of years may be added. For example, some file might include the statement `Copyright (c) 2007-2015 Charlie Poole`.

3. Do not update the copyright years when no changes or only trivial changes are made to a file.

### Language Level

Each NUnit project sets its own C# language level. We generally aim to keep the code buildable by folks who don't necessarily have the latest compilers, so this is sometimes not the very latest level. Currently, the maximum level for most NUnit projects is C# 6.

Note that targeting a particular level of C# does not mean that all features are available. Features are limited based on the framework targeted as well. For example, the engine currently targets .NET 2.0, which means that `System.Linq` extensions cannot be used in the code.

### Layout

#### Namespace, Class, Structure, Interface, Enumeration and Method Definitions

Place the opening and closing braces on a line by themselves and at the same level of indentation as their parent.

```csharp
public class MyClass : BaseClass, SomeInterface
{
    public void SomeMethod(int num, string name)
    {
        // code of the method
    }
}
```

An exception may be made if a method body or class definition is empty

```csharp
public virtual void SomeMethod(int num, string name) { }
```

```csharp
public class GadgetList : List<Gadget> { }
```

#### Property Definitions

Prefer automatic backing variables wherever possible

```csharp
public string SomeProperty { get; private set; }
```

If a getter or setter has only one statement, a single line should normally be used

```csharp
public string SomeProperty
{
    get { return _innerList.SomeProperty; }
}
```

If there is more than one statement, use the same layout as for method definitions

```csharp
public string SomeProperty
{
    get
    {
        if (_innerList == null)
            InitializeInnerList();

        return _innerList.SomeProperty;
    }
}
```

#### Spaces

Method declarations and method calls should not have spaces between the method name and the parenthesis, nor within the parenthesis. Put a space after a comma between parameters.

```csharp
public void SomeMethod(int x, int y)
{
    Console.WriteLine("{0}+{1}={2}", x, y, x + y);
}
```

Control flow statements should have a space between the keyword and the parenthesis, but not within the parenthesis.

```csharp
for (int i = 1; i < 10; i++)
{
    // Do Something
}
```

There should be no spaces in expression parenthesis, type casts, generics or array brackets, but there should be a space before and after binary operators.

```csharp
int x = a * (b + c);
var list = new List<int>();
list.Add(x);
var y = (double)list[0];
```

#### Indentation

Use four consecutive spaces per level of indent. Don't use tabs - except where the IDE can be set to convert tabs to four spaces. In Visual Studio, set the tab size to 4, the indent size to 4 and make sure Insert spaces is selected.

Indent content of code blocks.

In switch statements, indent both the case labels and the case blocks. Indent case blocks even if not using braces.

```csharp
switch (name)
{
    case "John":
        break;
}
```

### Newlines

Methods and Properties should be separated by one blank line. Private member variables should have no blank lines.

Blocks of related code should have not have any blank lines. Blank lines can be used to visually group sections of code, but there should never be multiple blank lines.

If brackets are not used on a control flow statement with a single line, a blank line should follow.

```csharp
public static double GetAttribute(XmlNode result, string name, double defaultValue)
{
    var attr = result.Attributes[name];

    double attributeValue;
    if (attr == null || !double.TryParse(attr.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out attributeValue))
        return defaultValue;

    return attributeValue;
}
```

### Naming

The following table shows our naming standard for various types of names. All names should be clear enough that somebody unfamiliar with the code can learn about the code by reading them, rather than having to understand the code in order to figure out the names. We don't use any form of "Hungarian" notation.

For items that may vary by project, the project’s root .editorconfig and solution DotSettings may provide the defaults and diagnostics expected for PRs in that project.

|Named Item|Naming Standard|Notes|
|--- |--- |--- |
|Namespaces|PascalCasing||
|Types|PascalCasing|For framework types with a special C# name, we use the C# name. So... int rather than System.Int32.|
|Methods|PascalCasing||
|Properties|PascalCasing||
|Events|PascalCasing||
|Public and Protected Fields|PascalCasing(with allowance for API compatibility)|Includes constant fields.Public and protected variable fields should be avoided.|
|Private and Internal Fields|(may vary by project)|Includes constant fields.Do not use this with fields designated by a leading underscore. Keep each file to the same standard, renaming when changes are made.Use `readonly` wherever appropriate for private fields.|
|Parameters|camelCasing||
|Local Variables|camelCasing||

### Comments

Use doc comments on all publicly accessible members. Keep the audience in mind. For example, comments on publicly used framework methods or attributes should be written for easy understanding by users, while comments on internal methods should target folks who work on NUnit.

Don't comment what is obvious.

Do comment unusual algorithms and the reasoning behind some choices made.

Use TODO comments when needed, but make sure to go back periodically and do whatever it is!

### File Organization

Normally, have one public type per source file. An exception is made for a simple enumeration, which is used in the interface of the public type and seems to "belong" to it. Example: TestResult and ResultState

Name the source file after the public type it represents.

The Directory hierarchy and Namespace hierarchy should match. For example, if the root namespace for a project is `NUnit.Framework`, files in the Constraints subdirectory should be in the `NUnit.Framework.Constraints` namespace.

Wherever possible, classes should be laid out in the following order,

1. Private member variables
2. Constructors
3. Dispose
4. Public Properties
5. Protected Properties
6. Private Properties
7. Public Methods
8. Protected Methods
9. Private Methods

Using statements should be sorted as follows:

* All System namespaces
* All Other namespaces, including NUnit's

It is permissible, but not required, to place using statements inside the namespace block, in shortened form, for namespaces that are descendants of the namespace itself. Note that the compiler will permit other uses of shortened namespaces within the namespace block, but we prefer to limit ourselves to descendants. Non-descendant namespaces should be listed in full form in the main using block.

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Engine.Interfaces;
using NUnit.Engine.Internal.Execution; // OK, but inside the namespace is preferred

namespace NUnit.Engine.Internal
{
    using Execution; // Preferred location
    ...
}
```

### Use of Regions

[Needs to be filled in]

### Use of the var keyword

The `var` keyword should be used where the type is obvious to someone reading the code, for example when creating a new object. Use the full type whenever the type is not obvious, for example when initializing a variable with the return value of a method.

```csharp
var i = 12;
var list = new List<int>();
Foo foo = GetFoo();
``
