The **DatapointSource** attribute is used
to provide data for **Theories** and is ignored for ordinary
tests - including tests with parameters.
   
Collections of datapoints may be provided by use of the **DatapointSourceAttribute**.
This attribute may be placed on methods or
properties in addition to fields. The returned value must be
either an array of the required type or an **IEnumerable<T>** returning an enumeration
of the required type. The data Type must exactly match the argument 
for which data is being supplied.
   
> In earlier versions of NUnit, the obsolete **DatapointsAttribute**
> was used in place of **DatapointSourceAttribute**.
   
#### Automatically Supplied Datapoints

It is normally not necessary to specify datapoints for 
**boolean** or **enum** arguments.
NUnit automatically supplies values of **true** 
and **false** for **boolean** arguments and will supply all 
defined values of any enumeration.
   
If for some reason you don't wish to use all possible values, you
can override this behavior by supplying your own datapoints. If you
supply any datapoints for an argument, automatic datapoint generation 
is suppressed.
   
#### Example

For an example of use, see [Theory Attribute](Theory.md)
   
#### See also...

 * [Theory Attribute](Theory.md)
 * [Parameterized Tests](xref:ParameterizedTests)
   
