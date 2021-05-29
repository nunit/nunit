# Building NUnit

NUnit 3 consists of three separate layers: the Framework, the Engine and the Console Runner. The source code is kept in two GitHub repositories at https://github.com/nunit/nunit and https://github.com/nunit/nunit-console.

There are two ways to build NUnit: using the solution file in an IDE or through the build script. See also [Building and testing for Linux on a Windows machine](#building-and-testing-for-linux-on-a-windows-machine).

## Solution Build

The framework is built using a single Visual Studio solution, `nunit.sln`, which may be built with [Visual Studio 2019 16.8](https://www.visualstudio.com/vs/) or newer on Windows and [Visual Studio for Mac](https://www.visualstudio.com/vs/) on macOS. Currently, MonoDevelop does not support the new multi-targeted `csproj` project format. Once MonoDevelop is updated, it should start working again. Until then, we recommend [Visual Studio Code](https://code.visualstudio.com/) and compiling using the build scripts on non-Windows platforms.

On all platforms, you will need to install [.NET 5.0 SDK](https://www.microsoft.com/net/download/windows) or newer. On Mac or Linux, you will need to install [Mono 5.2.0](https://www.mono-project.com/download/). Currently (as of 5.4.1), newer versions of Mono are broken and crash during the compile.

The solutions all place their output in a common bin directory under the solution root.

## Running Tests

The tests that should be run in the solution are grouped by project name:

 * `nunit.framework.tests-*`
 * `nunitlite.tests-*`

Other test projects contain tests designed to fail purposely for integration tests.

## Build Script

We use [Cake](https://cakebuild.net) to build NUnit for distribution. The primary script that controls building, running tests and packaging is `build.cake`. We modify `build.cake` when we need to add new targets or change the way the build is done. Normally `build.cake` is not invoked directly but through `build.ps1` (on Windows) or `build.sh` (on Linux). These two scripts are provided by the Cake project and ensure that Cake is properly installed before trying to run the cake script.

Key arguments to `build.ps1` / `build.sh`:

| Argument | Description |
|---|---|
| --target={task}                 | The task to run - see below.                        |
| --configuration=[Release\|Debug] | The configuration to use (default is Release)       |
| --showdescription               | Shows all of the build tasks and their descriptions |

The build.cake script contains a large number of interdependent tasks. The most important top-level tasks to use are listed here:

| Task | Description |
|---|---|
| Build    | Builds everything. This is the default if no target is given. |
| Rebuild  | Cleans the output directory and builds everything |
| Test     | Runs all tests. Dependent on Build. |
| Package  | Creates all packages without building first. See Note below. |

For a full list of tasks, run `.\build.ps1 --showdescription`.

### Notes:

 1. By design, the Package target does not depend on Build. This is to allow re-packaging when necessary without changing the binaries themselves. Of course, this means that you have to be very careful that the build is up to date before packaging.
 2. For additional targets, refer to the `build.cake` script itself.

### Defined constants

NUnit often uses conditional preprocessor to light up APIs and behaviors per platform.

In general, try to use feature constants rather than platform constants.
This brings clarity to the code and makes it easy to change the mapping between features and platforms.

Feature constants are defined in [Directory.Build.props](src/NUnitFramework/Directory.Build.props):

 - `THREAD_ABORT` enables timeouts and forcible cancellation

Platform constants are defined by convention by the csproj SDK, one per target framework.
For example, `NET45`, `NETSTANDARD2_0`, `NETCOREAPP2_1`, and so on.
It is most helpful to call out which platforms are the exception in rather than the rule
in a given scenario. Keep in mind the effect the preprocessor would have on a newly added platform.

For example, rather than this code:

```cs
#if NETSTANDARD2_0 || NETSTANDARD2_1
// Something that .NET Framework 4.5 can't do
#endif
```

Consider this:

```cs
#if !NET45
// Something that .NET Framework 4.5 can't do
#endif
```

Or this for the opposite:

```cs
#if NETFRAMEWORK
// Something that only .NET Framework can do
#endif
```
