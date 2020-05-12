---
uid: TestSelectionLanguage
---

# Test Selection Language

The console command-line allows you to specify a filter, which will select which tests are executed. This is done using the --where option, followed by an expression in NUnit's Test Selection Language (TSL), a simple domain-specific language designed for this purpose.

Some of the characters used in the expression, such as space, | or &, may have a special meaning when entered on the command-line. In such a case, you should place the expression in quotation marks.

```cmd
  nunit3-console mytest.dll --where "cat == Urgent || Priority == High"
```

Note that TSL is handled by the NUnit engine but requires framework support to actually select the tests. The NUnit 3.0 framework supports it fully. See below for support limitations in NUnit V2 tests.

## Simple Expressions

Simple Expressions are essentially comparisons, consisting of a key word or property name on the left-hand side, an operator and some constant value on the right-hand side. Here are some examples:

```text
  cat == Data
  test =~ /TestCaseAttributeTest/
  method == SomeMethodName
  cat != Slow
  Priority == High
  namespace == My.Name.Space
```

The following key words are recognized on the left-hand side of the comparison:

* `test` - The fully qualified test name as assigned by NUnit, e.g. My.Name.Space.TestFixture.TestMethod(5)
* `name` - The test name assigned by NUnit, e.g. TestMethod(5)
* `class` - The fully qualified name of the class containing the test, e.g. My.Name.Space.TestFixture
* `namespace` - The fully qualified name of the namespace containing the test(s), e.g. My.Name.Space
* `method` - The name of the method, e.g. TestMethod
* `cat` - A category assigned to the test, e.g. SmokeTests

If the left-hand side of the comparison does not consist of a key word, it is treated as the name of a property on the test whose value is to be checked. See below for restrictions on use of properties.

The following operators are supported

* `==` to test for equality - a single equal sign (`=`) may be used as well and has the same meaning
* `!=` to test for inequality
* `=~` to match a regular expression
* `!~` to not match a regular expression

The right-hand side of the comparison may be a sequence of non-blank, non-special characters or a quoted string. Quoted strings may be surrounded by single quotes (`'`), double quotes (`"`) or slashes (`/`) and may contain any character except the quote character used to delimit them. If it is necessary to include the quote character in the string, it may be escaped using a backslash (\) as may the backslash itself should you need to include one. The following expressions all do the same thing:

```text
  test =~ /TestCaseAttributeTest/
  test =~ "TestCaseAttributeTest"
  test =~ 'TestCaseAttributeTest'
  test =~ TestCaseAttributeTest
  test=~TestCaseAttributeTest
```

For matching regular expressions, NUnit uses .NET's `Regex.IsMatch` method. For detailed information on the syntax of regular expressions in .NET, see <https://msdn.microsoft.com/en-us/library/az24scfc%28v=vs.110%29.aspx>.

For specifying qualified names, the same format as used for reflection should be used. For example `My.Name.Space.TestFixture+NestedFixture` can be used to select a nested fixture. For detailed information see: [Specifying Special Characters](https://msdn.microsoft.com/en-us/library/yfsftwz6(v=vs.110).aspx#Anchor_1)

## Filtering By Namespace

Using the `namespace` keyword with `==` will _not_ match on sub-namespaces. For example by using the filter `namespace == My.Name.Space`, a test `My.Name.Space.MyFixture` will be selected but a test `My.Name.Space.SubNamespace.MyFixture` will not, since its namespace is not __equal__ to the namespace provided.

In order to inclusively select namespaces, a regular expression can be used. For example to match _all_ namespaces under the root namespace `My.Name.Space`, the following filter can be used `namespace =~ ^My\.Name\.Space($|\.)`

## Filtering Based on Properties

Although the syntax will accept any property name - including names that don't actually exist - filtering will only work on existing, string-valued properties.  The following properties are created by NUnit and have string values:

* Author
* Category
* Description
* SetCulture
* SetUICulture
* TestOf
* IgnoreUntilDate

In general, these properties were not created with filtering in mind, but you can use them if it suits your needs. Using the Category property currently accomplishes the same thing as the cat keyword. You should be aware that the use of these properties by NUnit is considered an implementation detail and they may change in the future.

We envision that most filtering by property will be based on user-defined properties, created for this purpose by inheriting from [Property Attribute](xref:PropertyAttribute). When defining a property, you should keep the limitation to string values in mind. For example, a PriorityAttribute taking values of "High", "Medium" and "Low" could be used for filtering, while one that took the integers 1, 2 and 3 could not.

## Filtering by Test Id

In addition to the left-hand-side items listed, NUnit supports filtering by the test id through the `id` keyword. The id may only be selected using the `==` operator and is intended only for use by programs that have explored the tests and cached the ids, not for general use by users. The reason for this restriction is that users have no way of predicting the id that will be assigned to a test. The id is not persistent across test runs and its format can differ between different framework drivers.

## Compound Expressions

Simple expressions may be combined using logical and, logical or, parentheses or negation operators.

Logical and is expressed as `&&`, `&` or `and`. Logical or is expressed as `||`, `|`, or `or`. The negation operator is `!` and may only appear before a left parenthesis. The letter variants, `and` and `or`, are provided for use on the command-line in systems that give `&` and `|` a special meaning.

The following are valid compound expressions:

```text
  test == "My.Namespace" and cat == Urgent
  test == "My.Namespace" and (cat == Urgent or Priority == High)
  test == "My.Namespace" and (cat == Urgent or Priority == High)
  method =~ /Source.*Test/ and class =~ "My.Namespace.ClassName"
```

## Usage on the Command Line

Because TSL contains special characters and may contain blank spaces, you will usually want to put the expression in quotes on the command line. Consequently, any strings within the TSL expression will most likely need to use an alternate quote character. For example:

```cmd
  nunit-console test.dll --where "method =~ /Source.*Test/ && class =~ 'My.Namespace.Classname'"
```

## Support in NUnit V2

The driver for NUnit V2 supports a subset of TSL. Because the V2 NUnit framework only allowed filtering on test names and categories, you may only use the `cat` and `test` keywords in comparisons. In addition, the regular expression operators `=~` and `!~` are not supported.

If you use any of the unsupported keywords or operators with V2 tests, an error message is displayed and the tests are not run.
