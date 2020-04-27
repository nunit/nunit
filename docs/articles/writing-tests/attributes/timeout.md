> From version 3.12 this is also available in the .NET Standard 2.0 builds of the framework.

Normally, NUnit simply runs tests and waits for them to terminate - the test may is allowed to run indefinitely. For certain kinds of tests, however, it may be desired to specify a timeout value.

The **TimeoutAttribute** is used to specify a timeout value in milliseconds
for a test case. If the test case runs longer than the time specified it
is immediately cancelled and reported as a failure, with a message 
indicating that the timeout was exceeded.
   
The specified timeout value covers the test setup and teardown as well as the test method itself. Before and after actions may also be included, depending on where they were specified. Since the timeout may occur during any of these execution phases, no guarantees can be made as to what will be run and any of these phases of execution may be incomplete. Specifically, once a test has timed out, no further attempt is made to execute its teardown methods.
   
The attribute may also be specified on a fixture or assembly, in which
case it indicates the default timeout for any subordinate test cases. When using the console runner, it is also possible to specify a default timeout on the command-line.
   
#### Example

```csharp
[Test, Timeout(2000)]
public void PotentiallyLongRunningTest()
{
    ...
}
```

**Note:** When debugging a unit test - i.e. when a debugger is attached to the process - then the timeout is not enforced.

#### See Also...
 * [[MaxTime Attribute]]
