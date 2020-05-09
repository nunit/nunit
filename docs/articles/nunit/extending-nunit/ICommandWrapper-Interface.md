In NUnit 3, test execution is done using command objects, which are constructed for each test case. Execution of a single test case will  generally require multiple nested commands. Some attributes placed on a test method are able to contribute to the chain of commands. For example, `MaxTimeAttribute` adds a command, which examines the elapsed time to complete a test and fails it if a specified maximum was exceeded.

Attributes add to the command chain by implementing one of the two interfaces that derive from the `ICommandWrapper` interface. The interfaces are defined as follows:

```csharp
public interface ICommandWrapper
{
    TestCommand Wrap(TestCommand command);
}

public interface IWrapTestMethod : ICommandWrapper
{
}

public interface IWrapSetUpTearDown : ICommandWrapper
{
}
```

Attributes should __not__ implement the `ICommandWrapper` interface directly but should select one of the derived intervaces. NUnit applies the `IWrapSetUpTearDown` interface before SetUp and after TearDown. It applies the `IWrapTestMethod` interface after SetUp and before the test is run.

Attributes implementing one of these interfaces must be placed on a test method. Otherwise, they have no effect. The `Wrap` method should return an appropriate command in which the original command has been nested. For an example, see the implementation of `MaxTimeAttribute`.

The following NUnit attributes implement the `IWrapSetUpTearDown` interface:
* `MaxTimeAttribute`
* `RepeatAttribute`
* `RetryAttribute`

The `IWrapTestMethod`interface is not currently used by any NUnit attributes.
