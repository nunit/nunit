# Building NUnit

NUnit consists of multiple components across separate layers: the Framework, the Engine, the Adapter, and the Console Runner. The source code is kept in three GitHub repositories
- https://github.com/nunit/nunit
- https://github.com/nunit/nunit-vs-adapter
- https://github.com/nunit/nunit-console

## Source Code

You will not have permission to create branches directly in either of the above repositories so you will need to work on a local fork. 

The development process for making code changes and submitting Pull Requests (PRs) is as follows:

1. Create a fork of the NUnit repository
2. Clone the fork to your computer
3. Create the changes on a new branch
4. Ensure all NUnit tests pass locally (ie. run them on all targeted frameworks and on as many platforms that you have ie. Windows, Linux, Mac). The [Script Build](#script-build) section below gives exact instructions on how to do this from the Command Prompt / Terminal.
5. Push the branch to your fork
6. Create the PR (this is described in more detail [here](https://thenewstack.io/getting-legit-with-git-and-github-your-first-pull-request/))

## Solution Build

There is a single Visual Studio solution, `nunit.slnx`, which resides in the NUnit repository root.

NUnit framework can be built from this solution using a .NET-capable IDE such as:

- [Visual Studio 2026](https://www.visualstudio.com/vs/) or newer (Windows)
- [Visual Studio for Mac](https://www.visualstudio.com/vs/) (macOS).
- [Visual Studio Code](https://code.visualstudio.com/)
   - Compile using the [build script](#script-build) instead if using VS Code (see below for details).
- [Jetbrains Rider](https://www.jetbrains.com/rider/)

On all platforms, you will need to install [.NET 10.0 SDK](https://www.microsoft.com/net/download/windows) or newer.

On Mac or Linux, you will need to install [Mono](https://www.mono-project.com/download/), an open source implementation of Microsoft's .NET Framework. Mono version 6.12.0 Stable (6.12.0.122) has been validated to build the NUnit solution on GNU/Linux Debian 10 'buster'.

A solution build places all of its output in a common `bin` directory under the solution directory.

As the NUnit solution targets multiple frameworks, a single build will generate files for each targeted framework. For example, a debug build <sup>[2]</sup> produces the following directory structure:

```
 (directory with nunit.slnx)
    bin\
       Debug\
          net8.0
          net6.0
          net462
```

## Running Tests

The tests that should be run in the solution are grouped by project name:

* `nunit.framework.tests-*`
* `nunit.framework.legacy.tests-*`
* `nunitlite.tests-*`

Other test projects contain tests designed to fail purposely for integration tests.

You should then be able to run the unit tests directly from within your development IDE of choice against one or all target frameworks (as you would when using NUnit in any other development project). For example, this is what it looks like in JetBrains Rider (2021.1.2) when right clicking on the `AssertEqualsTests` TextFixture:

![image](https://user-images.githubusercontent.com/52075808/121511286-61775580-c9e0-11eb-8e1e-ff44d0d8873d.png)

### Known Issues and Workarounds

Unfortunately, there are currently some known issues with building and running tests locally.

#### Tests will not run within Rider ([#3008](https://github.com/nunit/nunit/issues/3008))

You can run NUnit Lite in a similar way to above, but from the development IDE after a successful build. 

This is probably the easiest way (whilst issue [#3008](https://github.com/nunit/nunit/issues/3008) remains open) for developers to step through code and debug any new contributions made to the codebase.

In your development IDE of choice, ensure there is a Run/Debug configuration which specifies `nunitlite-runner` as the startup project. 

Then specify the program arguments in exactly the same way as above. For example the following argument `nunit.framework.tests.dll --where "class == NUnit.Framework.Assertions.AssertEqualsTests"` will run all tests in the NUnit.Framework.Assertions.AssertEqualsTests TextFixture in the nunit.framework.tests.dll.

You could now set a breakpoint anywhere in the AssertEqualsTests class and have the debugger pause on it when performing a debug run of the solution (ie. ALT+F5 in JetBrains Rider).

## Script Build

We use [Cake](https://cakebuild.net) to build NUnit for distribution. The primary script that controls building, running tests and packaging is `build.cake`. We modify `build.cake` when we need to add new targets or change the way the build is done. Normally `build.cake` is not invoked directly but through `build.ps1` (PowerShell on Windows) or `build.sh` (bash on Linux). These two scripts are provided by the Cake project and ensure that Cake is properly installed before trying to run the cake script.

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

For example, the following command `.\build.ps1 --target=Test --configuration=Release` (PowerShell on Windows) will perform a full release build for all target frameworks and then execute the unit tests against each target. 

For a full list of tasks, run `.\build.ps1 --showdescription` (PowerShell on Windows) or `./build.sh --showdescription` (bash on Linux).

### Notes

1. By design, the Package target does not depend on Build. This is to allow re-packaging when necessary without changing the binaries themselves. Of course, this means that you have to be very careful that the build is up to date before packaging.
2. For additional targets, refer to the `build.cake` script itself.

## Defined constants

NUnit often uses conditional preprocessor to light up APIs and behaviors per platform.

In general, try to use feature constants rather than platform constants.
This brings clarity to the code and makes it easy to change the mapping between features and platforms.

Feature constants are defined in [Directory.Build.props](src/NUnitFramework/Directory.Build.props):

- `THREAD_ABORT` enables timeouts and forcible cancellation

Platform constants are defined by convention by the csproj SDK, one per target framework.
For example, `NET462`, `NETSTANDARD2_0`, `NET6_0`, and so on.
It is most helpful to call out which platforms are the exception in rather than the rule
in a given scenario. Keep in mind the effect the preprocessor would have on a newly added platform.

For example, rather than this code:

```cs
#if NETSTANDARD2_0 || NET6_0
// Something that .NET Framework can't do
#endif
```

Consider this:

```cs
#if !NETFRAMEWORK
// Something that .NET Framework can't do
#endif
```

Or this for the opposite:

```cs
#if NETFRAMEWORK
// Something that only .NET Framework can do
#endif
```
