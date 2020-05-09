---
uid: TemplateBasedTestNaming
---

NUnit uses a standard naming convention for all tests - however, this can be overridden by the user if required. TestName generation is driven by a name formatting string, which may contain any of the following format specifiers:

  * `{n}` The namespace of the test or empty if there is no namespace. If empty, any immediately following '.' is ignored.

  * `{c}` The class name of the test or empty if there is no class. This name includes any type arguments, enclosed in angle braces and separated by commas.

  * `{C}` The full name of the class. Equivalent to `{n}.{c}`

  * `{m}` The method name of the test or empty if there is no method. The name includes any type arguments, enclosed in angle braces and separated by commas.

  * `{M}` The full name of the method.

  * `{a}` The full argument representation, enclosed in parentheses and separated by commas. Each argument is represented by the standard NUnit format for certain types, otherwise by the result of ToString().

  * `{p}` Same as `{a}` but with a parameter name before each argument in the same style as the [*named arguments* C# language feature](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments#named-arguments).

  * `{0}`, `{1}`...`{9}`. An individual argument. This form is only useful when setting the name of an individual test case. If used in the default format string, any arguments not used will be ignored.

  * `{i}` The test id, which is normally of the form mmm-nnn.

  * Any text not included between curly braces is copied to the name as is.

After the name is formatted, any leading or trailing '.' characters are removed. Otherwise, all non-format characters in the string are included as is.

String arguments may be truncated to a maximum length. Either the {a} specifier or any of the individual argument specifiers may be followed by a colon and a length:

  * `{a:40}` Truncate __each string argument__ to 40 characters. All strings more than 37 characters are truncated to the first 37 followed by "..."

  * `{0:20}` Truncate argument zero to 20 characters.

#### Standard Name Formats		
		
Internally, NUnit uses certain standard formats unless overridden by the user. The standard format for generating a name from a test method and its arguments is:

```
         {m}{a} // Name
```

This leads to test names like:

```
         Test1
         Test2(5, 2)
         Test3("This is the argument")
         Test4("This is quite long argument, so it is...")
```

#### Modifying the Name Format

The `SetName` method of `TestCaseData` allows setting the name of an individual test case. In normal use, the provided string simply becomes the name of the test.

However, if one of the template format specifiers is used in the argument to `SetName`, the name is regenerated using that format. For example, if the user wishes to specify only the argument portion of the name of a test method, while still retaining the method name, the name could be set to

```
         {m}(User argument)
```

This would result in the display of the test name as

```
         SomeMethod(User Argument)
```

Note that in this usage, it will generally only make sense to use `{m}`, `{a}` or `{0}` through `{9}` specifiers. However, NUnit will use whatever is provided.
