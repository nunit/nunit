**ValueSourceAttribute** is used on individual parameters of a test method to
identify a named source for the argument values to be supplied. The attribute has 
two public constructors.

```C#
ValueSourceAttribute(Type sourceType, string sourceName);
ValueSourceAttribute(string sourceName);
```

If **sourceType** is specified, it represents the class that provides
the data. It must have a default constructor.

If **sourceType** is not specified, the class containing the test
method is used.

The **sourceName**, represents the name of the source that will 
provide the arguments. It should have the following characteristics:
 * It may be a field, a non-indexed property or a method taking no arguments.
 * It must be a static member.
 * It must return an IEnumerable or a type that implements IEnumerable.
 * The individual items returned from the enumerator must be compatible
   with the type of the parameter on which the attribute appears.

#### Order of Execution

Individual test cases are executed in the order in which NUnit discovers them.
This order does **not** follow the lexical order of the attributes and will 
often vary between different compilers or different versions of the CLR.
   
As a result, when **ValueSourceAttribute** appears multiple times on a 
parameter or when other data-providing attributes are used in combination with 
**ValueSourceAttribute**, the order of the arguments is undefined.

However, when a single **ValueSourceAttribute** is used by itself, 
the order of the arguments follows exactly the order in which the data 
is returned from the source.
   
#### Note on Object Construction

NUnit locates the test cases at the time the tests are loaded, creates
instances of each class with non-static sources and builds a list of 
tests to be executed. Each source object is only created once at this
time and is destroyed after all tests are loaded. 

If the data source is in the test fixture itself, the object is created
using the appropriate constructor for the fixture parameters provided on
the **TestFixtureAttribute**, or
the default constructor if no parameters were specified. Since this object
is destroyed before the tests are run, no communication is possible between
these two phases - or between different runs - except through the parameters
themselves.
