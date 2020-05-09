NUnit tests run within a context, known as the `TestExecutionContext`. The context for a test case is nested within the context for its containing suite and so on, up to the assembly level. Attributes that implement `IApplyToContext` are called immediately after the context is created and before the test is run in order to make changes to the context. Once the test execution has completed, the context is discarded so that - effectively - any changes are reverted to their original values.

The `IApplyToContext` interface is defined as follows:
```C#
public interface IApplyToContext
{
    void ApplyToContext(TestExecutionContext context);
}
```

An example of the use of the context may be helpful. One item in the `TestExecutionContext` is the default timeout value for test cases. When any test is marked with `[Timeout(nnn)]` the context value is replaced by the supplied argument. The new timeout applies for any test case it appears on and any test case that is contained in a suite that it appears on. When the test or suite completes, the new value is discarded and the value contained in the original context is once against used.

Custom attributes that implement `IApplyToContext` should modify the TestExecutionContext in accordance with the arguments supplied to them. They are not called after the test is run and have no cleanup to perform.

The NUnit attributes that implement `IApplyToContext` are as follows:
* `DefaultFloatingPointToleranceAttribute`
* `ParallelizableAttribute`
* `SetCultureAttribute`
* `SetUICultureAttribute`
* `SingleThreadedAttribute`
* `TimeoutAttribute`

