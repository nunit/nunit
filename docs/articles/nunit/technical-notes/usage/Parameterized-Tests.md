---
uid: ParameterizedTests
---

NUnit supports parameterized tests. Test methods
may have parameters and various attributes are available
to indicate what arguments should be supplied by NUnit.

Multiple sets of arguments cause the creation of multiple
tests. All arguments are created at the point of loading the
tests, so the individual test cases are available for 
display and selection in the Gui, if desired.

Some attributes allow you to specify arguments inline - directly on
the attribute - while others use a separate method, property or field
to hold the arguments. In addition, some attributes identify complete test cases,
including all the necessary arguments, while others only provide data
for a single argument. This gives rise to four groups of attributes,
as shown in the following table.
   
|              | Complete Test Cases          | Data for One Argument |
|--------------|------------------------------|-----------------------|
| **Inline**   | [[TestCase Attribute]]       | [[Random Attribute]]<br/>[[Range Attribute]]<br/>[[Values Attribute]] |
| **Separate** | [[TestCaseSource Attribute]] | [[ValueSource Attribute]] |

In addition, when data is specified for individual arguments, special attributes
may be added to the test method itself in order to tell NUnit how
to go about combining the arguments. Currently, the following attributes
are provided:
 * [[Combinatorial Attribute]] (default)
 * [[Pairwise Attribute]]
 * [[Sequential Attribute]]

### Order of Execution

The individual test cases are executed in the order in which NUnit discovers them. 
This order does **not** necessarily follow the lexical order of the attributes 
and will often vary between different compilers or different versions of the CLR.
   
The following specific rules for ordering apply:
 * If all arguments are specified in a **single TestCaseSource** attribute, the ordering of the cases provided will be maintained.
 * If each parameter has a single **Values**, **ValueSource** or **Range** attribute and the **Sequential** combining strategy is used - or there is only one argument - the ordering will be maintained.
 * In all other cases, including using multiple **TestCase** attributes or a combination of different types of attributes, the ordering of the test cases is undefined.
