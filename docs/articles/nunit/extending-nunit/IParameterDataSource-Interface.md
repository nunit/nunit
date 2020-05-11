# IParameterDataSource Interface

The `IParameterDataSource` interface is implemented by attributes that provide data for use as an argument to a single paramerter. Other attributes and test builders combine the values in various ways to produce test cases.
The interface is defined as follows:

```csharp
public interface IParameterDataSource
{
    IEnumerable GetData(IParameterInfo parameter);
}
```

`IParameterInfo` is an NUnit internal class used to wrap a `ParameterInfo`. See the [source code](https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Interfaces/IParameterInfo.cs) for more info.

A custom attribute implementing this interface should examine the `IParameterInfo` and return data values that are compatible with the parameter. The attribute has no control over how this data is combined with other arguments - that's up to other attributes and ultimately NUnit itself.

The following NUnit attributes currently implement `IParameterDataSource`:

* `RandomAttribute`
* `ValuesAttribute`, with the derived class
  * `RangeAttribute`
* `ValueSourceAttribute`
