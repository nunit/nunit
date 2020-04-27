**PropertyAttribute** provides a generalized approach to setting named
properties on any test case or fixture, using a name/value pair.
In the example below, the fixture class MathTests is given a Location
value of 723 while the test case AdditionTest is given a Severity
of "Critical"

#### Example:

```C#
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture, Property("Location", 723)]
  public class MathTests
  {
    [Test, Property("Severity", "Critical")]
    public void AdditionTest()
    { /* ... */ }
  }
}
```

#### Usage Note

The PropertyAttribute is not currently used for any purpose by NUnit itself, other
than to display them in the XML output file and in the Test Properties
dialog of the gui. You may also use properties with the `--where` option on the
command-line in order to select tests to run. See [[Test Selection Language]]. Note 
that his filtering will only work for properties where the values have type string.

User tests may access properties through the [[TestContext]] or by reflection.
   
#### Custom Property Attributes

Users can define custom attributes that derive from **PropertyAttribute**
and have them recognized by NUnit. PropertyAttribute provides a protected constructor
that takes the value of the property and sets the property name to the
name of the derived class with the 'Attribute' suffix removed. 

Here's an example that creates a Severity property. It works
just like any other property, but has a simpler syntax and is type-safe.
A custom test reporting system might make use of the property to provide special reports.

```C#
public enum SeverityLevel
{
    Critical,
    Major,
    Normal,
    Minor
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
public class SeverityAttribute : PropertyAttribute
{
    public SeverityAttribute(SeverityLevel level)
	    : base(level);
}

...

[Test, Severity(SeverityLevel.Critical)]
public void MyTest()
{ /*...*/ }
```

A PropertyAttribute may contain
multiple name/value pairs. This capability is not exposed publicly
but may be used by derived property classes. 
