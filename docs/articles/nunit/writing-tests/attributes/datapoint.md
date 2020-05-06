The **Datapoint** attribute is used
to provide data for **Theories** and is ignored for ordinary
tests - including tests with parameters.
   
When a Theory is loaded, NUnit creates arguments for each
of its parameters by using any fields of the same type
as the parameter annotated with the **DatapointAttribute**.
Fields must be members of the class containing the Theory
and their Type must exactly match the argument for which
data is being supplied.
       
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
 * [[Parameterized Tests]]
   
