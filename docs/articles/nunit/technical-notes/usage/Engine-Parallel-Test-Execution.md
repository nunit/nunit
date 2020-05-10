The NUnit test engine is able to offer a certain degree of parallelization by running the tests in each test assembly in a different `Process`. This is a separate facility from [Framework Parallel Test Execution](Framework-Parallel-Test-Execution.md) although the two may be used concurrently.

If tests are already split across multiple assemblies, this is the simplest way to improve performance through parallel execution. By running in separate processes, the tests in each assembly are independent of one another so long as they do not use any common external resources such as files or databases. **Parallel execution is the default behavior** when running multiple assemblies together using the `nunit3-console` runner.

Normally, __all__ the test processes run simultaneously. If you need to reduce the number of processes allowed to run at one time, you may specify a value for the `--agents` option on the `nunit3-console` command-line. For example, if you are running tests in 10 different processes, a setting of `--agents=3` will allow no more than three of them to execute simultaneously.

> [!NOTE]
> This facility does not depend on the test framework used in any way. Test assemblies that use older versions of NUnit may be run in parallel processes just as easily as those using NUnit 3. If extensions are created to support additional frameworks, the NUnit engine will run those assemblies in parallel as well.

#### Process Model

NUnit 3 uses the `ProcessModel` enumeration to specify how assemblies are split across processes. The `ProcessModel` for a run may specified either on the console command-line or in the NUnit project file.

As in NUnit V2, three values are defined:
  * `ProcessModel.Single` causes all tests to be run within the NUnit process itself.
  * `ProcessModel.Separate` loads and runs all the tests in a single separate process.
  * `ProcessModel.Multiple` loads and runs each test assembly in a separate process.

In NUnit 3, if `ProcessModel.Multiple` is used, the processes are executed in parallel. This is also the default if the `ProcessModel` is not specified.
