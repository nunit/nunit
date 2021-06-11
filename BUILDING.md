# Building NUnit

NUnit 3 consists of three separate layers: the Framework, the Engine and the Console Runner. The source code is kept in two GitHub repositories, https://github.com/nunit/nunit and https://github.com/nunit/nunit-console.

## Source Code

You will not have permission to create branches directly in either of the above repositories so you will need to work on a local fork. 

The development process for making code changes and submitting Pull Requests (PR) is as follows:

1. Create a fork of the NUnit repository
2. Clone the fork to your computer
3. Create the changes on a new branch
4. Ensure all NUnit tests pass locally (ie. run them on all targeted frameworks and on as many platforms that you have ie. Windows, Linux, Mac). The [Script Build](#script-build) section below gives exact instructions on how to do this from the Command Prompt / Terminal.
5. Push the branch to your fork
6. Create the PR (this is described in more detail [here](https://thenewstack.io/getting-legit-with-git-and-github-your-first-pull-request/))

## Solution Build

There is a single Visual Studio solution, `nunit.sln`, which resides in the NUnit repository root.

NUnit framework can be built from this solution using [Visual Studio 2019 16.8](https://www.visualstudio.com/vs/) or newer (Windows) and [Visual Studio for Mac](https://www.visualstudio.com/vs/) (macOS).

Currently, MonoDevelop does not support the new multi-targeted `csproj` project format <sup>[1]</sup>. Once MonoDevelop is updated, it should start working again. Until then, we recommend using [Visual Studio Code](https://code.visualstudio.com/) and compiling using the build script instead (see the [Script Build](#script-build) section below for details).

On all platforms, you will need to install [.NET 5.0 SDK](https://www.microsoft.com/net/download/windows) or newer. 

On Mac or Linux, you will need to install [Mono](https://www.mono-project.com/download/), an open source implementation of Microsoft's .NET Framework. Mono version 6.12.0 Stable (6.12.0.122) has been validated to build the NUnit solution on GNU/Linux Debian 10 'buster'.

A solution build places all of its output in a common `bin` directory under the solution directory.

As the NUnit solution targets multiple frameworks, a single build will generate files for each targeted framework. For example, a debug build <sup>[2]</sup> produces the following directory structure:

```
 (directory with nunit.sln)
    bin\
       Debug\
          net5.0
          net45
          net46
          netcoreapp2.1
          netcoreapp3.1
          netstandard2.0
```

### Notes
1. MonoDevelop 2.0 introduced support for multiple target frameworks up to and including .Net 3.0 and 3.5 ([MonoDevelop 2.0 release notes](https://www.monodevelop.com/documentation/release-notes/monodevelop-2.0-released/)). Unfortunately, subsequent releases have not extended that support to .Net Core or .Net 5 and beyond. There is a Kanban board maintained for [MonoDevelop Multi-targeting Support](https://github.com/mono/monodevelop/projects/1) however it's showing the most recent update being 9 Aug 2019. MonoDevelop Version 7.8.4 (build 2) was verified as unable to build `nunit.sln` due to lack of multi-targeting support. This footnote was correct as of 9 June 2021.
2. GitHub repository cloned and built on 9 June 2021

## Running Tests

The tests that should be run in the solution are grouped by project name:

* `nunit.framework.tests-*`
* `nunitlite.tests-*`

Other test projects contain tests designed to fail purposely for integration tests.

Normally you should be able to run the unit tests directly from within your development IDE of choice (as you would when using NUnit in any other development project). For example, this is what it looks like in JetBrains Rider (2021.1.2) when right clicking on the `AssertEqualsTests` TextFixture:

![image](https://user-images.githubusercontent.com/52075808/121511286-61775580-c9e0-11eb-8e1e-ff44d0d8873d.png)

Because NUnit solution targets multiple frameworks, JetBrains Rider knows this and offers the option to run the tests against a specific framework and/or all target frameworks.

Unfortunately, there is currently a known issue ([#3008](https://github.com/nunit/nunit/issues/3008)) preventing the tests from being run in Visual Studio and JetBrains Rider IDEs. Equally there is also a known issue ([#3867](https://github.com/nunit/nunit/issues/3867)) preventing the tests from being run from the command line using `dotnet test`

### Workarounds

#### 1. NUnit Lite Runner (run from command prompt / terminal)

The NUnit solution contains [NUnit Lite Runner](https://docs.nunit.org/articles/nunit/running-tests/NUnitLite-Runner.html), a lightweight test runner. It is similar to NUnit console but with fewer features and without the overhead of a full NUnit installation. 

If you navigate to one of the build outputs under the `bin` directory, you will see that it contains `nunitlite-runner.exe`

For example, the following command `./nunitlite-runner nunit.framework.tests.dll` (bash on Linux) will run all tests in the nunit.framework.tests.dll

NUnit Lite Runner accepts a number of [command line arguments](https://docs.nunit.org/articles/nunit/running-tests/NUnitLite-Options.html). 

For example, the following command `./nunitlite-runner nunit.framework.tests.dll --where "class == NUnit.Framework.Assertions.AssertEqualsTests"` (bash on Linux) will run all tests in the NUnit.Framework.Assertions.AssertEqualsTests TextFixture in the nunit.framework.tests.dll

#### 2. NUnit Lite Runner (run by the IDE after a build)

You can run NUnit Lite in a similar way to above, but from the development IDE after a successful build. 

This is probably the easiest way (whilst issue [#3008](https://github.com/nunit/nunit/issues/3008) remains open) for developers to step through code and debug any new contributions made to the codebase.

In your development IDE of choice, ensure there is a Run/Debug configuration which specifies `nunitlite-runner` as the startup project. 

Then specify the program arguments in exactly the same way as above. For example the following argument `nunit.framework.tests.dll --where "class == NUnit.Framework.Assertions.AssertEqualsTests"` will run all tests in the NUnit.Framework.Assertions.AssertEqualsTests TextFixture in the nunit.framework.tests.dll

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

For example, the following command `.\build.ps --target=Test --configuration=Release` (PowerShell on Windows) will perform a full release build for all target frameworks and then execute the unit tests against each target. 

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
