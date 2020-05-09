Test Names are generated in the framework and used in the other layers. There are four key issues around naming of tests.

1. NUnit test names often "look like" method names but in principal they are arbitrary strings. Since we allow the user to change the name in many cases - using the TestCase attribute for example - they can be arbitrary in practice as well. This issue is essentially the underlying cause of the remaining three issues.

2. Some clients display the names in a way that causes an error if they are too long. Such clients need a way to direct NUnit to restrict the length of names.

3. NUnit does not require the names of tests to be unique. Some clients, however, make that assumption, which leads to errors. Such clients need a way to add a unique value to each test name.

4. Test names may be used in the command line for the purpose of selecting tests. That means the user must be able to predict the name of the test. In NUnit 3.0, this problem is mitigated by the presence of command-line options that allow for selecting tests in other ways. Nevertheless, it is desirable that the user have some control over how names are generated.

Note that the term "tests" in this section refers to test cases, test methods, test fixtures and other kinds of test suites. NUnit treats all of these as "tests" for most purposes.

#### Name Formatting Strings

TestName generation is driven driven by a name formatting string, which may be specified or modified by the user. The format string may contain any of the following format specifiers, for which NUnit will make the appropriate substitution:

  * {n} The namespace of the test or empty if there is no namespace. If empty, any immediately following '.' is ignored.

  * {c} The class name of the test or empty if there is no class. This name includes any type arguments, enclosed in angle braces and separated by commas.

  * {C} The full name of the class. Equivalent to {n}.{c}

  * {m} The method name of the test or empty if there is no method. The name includes any type arguments, enclosed in angle braces and separated by commas.

  * {M} The full name of the method. Equivalent to {n}.{c}.{m} or {C}.{m}

  * {a} The full argument representation, enclosed in parentheses and separated by commas. Each argument is represented by the standard NUnit format for certain types, otherwise by the result of ToString().

  * {0}, {1} ... {9}. An individual argument. This form is only useful when setting the name of an individual test case. If used in the default format string, any arguments not used will be ignored.

  * {i} The test id, which is normally of the form mmm-nnn.

  * Any text not included between curly braces is copied to the name as is.

After the name is formatted, any leading or trailing '.' characters are removed. Otherwise, all non-format characters in the string are included as is.

String arguments may be truncated to a maximum length. Either the {a} specifier or any of the individual argument specifiers may be followed by a colon and a length:

  * {a:40} Truncate __each string argument__ to 40 characters. All strings lest than 37 characters are truncated to the first 37 followed by "..."

  * {0:20} Truncate argument 0 to 20 characters.

#### Standard Name Formats

Internally, NUnit uses certain standard formats unless overridden by the user. The standard format for generating a name from a test method and its arguments is

```
         {m}{a:40} // Name
         {M}{a:40} // FullName
```

This leads to test names like

```
         Test1
         Test2(5, 2)
         Test3("This is the argument")
         Test4("This is quite long argument, so it is...")
```

#### Modifying the Name Format

The SetName property of TestCaseData allow setting the format string for the individual test case. So long as no format specifiers are used in the name, there will be no change in how this works. Any format specifier will trigger regeneration of the test name according to what is provided. For example, if the user wishes to specify only the argument portion of the name of a test method, while retaining the method name, the name could be set to

```
         {m}(User argument)
```

This would result in the display of the test name as

```
         SomeMethod(User Argument)
```

Note that in this usage, it will generally only make sense to use `{m}`, `{a}` or `{0}` through `{9}` specifiers. However, NUnit will use whatever is provided.


