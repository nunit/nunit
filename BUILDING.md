# Building NUnit 3

NUnit 3 consists of three separate layers: the Framework, the Engine and the Console Runner. The source code is kept in two GitHub repositories at https://github.com/nunit/nunit and https://github.com/nunit/nunit-console.

There are two ways to build NUnit: using the solution file in an IDE or through the build script. See also [Building and testing for Linux on a Windows machine](#building-and-testing-for-linux-on-a-windows-machine).

## Solution Build

The framework is built using a single Visual Studio solution, `nunit.sln`, which may be built with [Visual Studio 2019](https://www.visualstudio.com/vs/) on Windows and [Visual Studio for Mac](https://www.visualstudio.com/vs/) on macOS. Currently, MonoDevelop does not support the new multi-targeted `csproj` project format. Once MonoDevelop is updated, it should start working again. Until then, we recommend [Visual Studio Code](https://code.visualstudio.com/) and compiling using the build scripts on non-Windows platforms.

On all platforms, you will need to install [.NET Core 2.0.3 SDK](https://www.microsoft.com/net/download/windows) or newer. On Mac or Linux, you will need to install [Mono 5.2.0](https://www.mono-project.com/download/). Currently (as of 5.4.1), newer versions of Mono are broken and crash during the compile.

The solutions all place their output in a common bin directory under the solution root.

## Running Tests

The tests that should be run in the solution are grouped by project name:

 * `nunit.framework.tests-*`
 * `nunitlite.tests-*`

Other test projects contain tests designed to fail purposely for integration tests.

## Build Script

We use [Cake](https://cakebuild.net) to build NUnit for distribution. The primary script that controls building, running tests and packaging is build.cake. We modify build.cake when we need to add new targets or change the way the build is done. Normally build.cake is not invoked directly but through build.ps1 (on Windows) or build.sh (on Linux). These two scripts are provided by the Cake project and ensure that Cake is properly installed before trying to run the cake script. This helps the build to work on CI servers using newly created agents to run the build and we generally run it the same way on our own machines.

The build shell script and build.cmd script are provided as an easy way to run the above commands. In addition to passing their arguments through to build.cake, they can supply added arguments through the CAKE_ARGS environment variable. The rest of this document will assume use of these commands.

There is one case in which use of the CAKE_ARGS environment variable will be essential, if not necessary. If you are running builds on a 32-bit Windows system, you must always supply the -Experimental argument to the build. Use set CAKE_ARGS=-Experimental to ensure this is always done and avoid having to type it out each time.

Key arguments to build.cmd / build:
 * -Target, -t <task>                 The task to run - see below.
 * -Configuration, -c [Release|Debug] The configuration to use (default is Release)
 * -ShowDescription                   Shows all of the build tasks and their descriptions
 * -Experimental, -e                  Use the experimental build of Roslyn

Note that the above format for supplying arguments only work on Windows. On Linux one has to use double dashes to specify the argument, like `./build.sh --target=Test` or `./build.sh --target Test` to run the `Test` task.

The build.cake script contains a large number of interdependent tasks. The most important top-level tasks to use are listed here:

```
 * Build               Builds everything. This is the default if no target is given.
 * Rebuild             Cleans the output directory and builds everything
 * Test                Runs all tests. Dependent on Build.
 * Package             Creates all packages without building first. See Note below.
```

For a full list of tasks, run `build.cmd -ShowDescription`.

### Notes:

 1. By design, the Package target does not depend on Build. This is to allow re-packaging when necessary without changing the binaries themselves. Of course, this means that you have to be very careful that the build is up to date before packaging.
 2. For additional targets, refer to the build.cake script itself.

### Defined constants

NUnit often uses conditional preprocessor to light up APIs and behaviors per platform.

In general, try to use feature constants rather than platform constants.
This brings clarity to the code and makes it easy to change the mapping between features and platforms.

Feature constants are defined in [Directory.Build.props](src/NUnitFramework/Directory.Build.props):

 - `TASK_PARALLEL_LIBRARY_API` exposes NUnit APIs which depend on the TPL framework types
 - `THREAD_ABORT` enables timeouts and forcible cancellation

Platform constants are defined by convention by the csproj SDK, one per target framework.
For example, `NET45`, `NETSTANDARD2_0`, `NETCOREAPP2_1`, and so on.
It is most helpful to call out which platforms are the exception in rather than the rule
in a given scenario. Keep in mind the effect the preprocessor would have on a newly added platform.

For example, rather than this code:

```cs
#if NET45 || NETSTANDARD2_0 || NETSTANDARD2_1
// Something that .NET Framework 4.0 can't do
#endif
```

Consider this:

```cs
#if !(NET35 || NET40)
// Something that .NET Framework 4.0 can't do
#endif
```
