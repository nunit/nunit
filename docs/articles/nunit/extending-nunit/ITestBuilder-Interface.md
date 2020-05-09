This interface is used by attributes that know how to build one or more parameterized `TestMethod` instances from a `MethodInfo`. `ITestMethodBuilder` is defined as follows:

```C#
public interface ITestBuilder
{
    IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite);
}
```

`IMethodInfo` is an NUnit internal class used to wrap a MethodInfo. See the [source code](https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Interfaces/IMethodInfo.cs) for more info. The `suite` argument is the test suite that contains the method in question.

A custom attribute implementing this interface should examine the IMethodInfo and return as many `TestMethod` instances as it is able to construct, using the parameters available to it. Some attributes will only return a single test, just as `TestCaseAttribute` does. Others, working like `TheoryAttribute` may return multiple tests. If no data is available to create tests, an empty collection should be returned. 

If the returned tests are standard NUnit TestMethods, the helper class `NUnitTestCaseBuilder` may be used to create them. 

The following NUnit attributes currently implement `ITestBuilder`:
* `CombiningStrategyAttribute`, with the following derived classes:
  * `CombinatorialAttribute`
  * `PairwiseAttribute`
  * `SequentialAttribute`
* `TestCaseAttribute`
* `TestCaseSourceAttribute`
* `TheoryAttribute`

