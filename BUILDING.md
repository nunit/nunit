# Building NUnit

NUnit consists of multiple components across separate layers: the Framework, the Engine, the Adapter, and the Console Runner. The source code for the main NUnit components is kept in four different GitHub repositories

- [NUnit Framework](https://github.com/nunit/nunit) The repo we're working on now.
- [NUnit Visual Studio Adapter](https://github.com/nunit/nunit3-vs-adapter)  The Visual Studio/Dotnet adapter for NUnit
- [NUnit Roslyn Analyzer](https://github.com/nunit/nunit.analyzers) Roslyn Code analysis and quick fixes for NUnit.
- [NUnit Console and Engine](https://github.com/nunit/nunit-console)  The Console is a separate runner. The engine is used also by the adapter.

## How to get the source Code

You will not have permission to create branches directly in any of the above repositories, so you will need to work on a local fork.

The development process for making code changes and submitting Pull Requests (PRs) is as follows:

1. Create a fork of the NUnit repository
2. Clone the fork to your computer
3. Create a new branch, name of your choosing
4. Work with the code to fix the issue you have
5. Ensure the code builds
6. Ensure all NUnit tests pass locally (ie. run them on all targeted frameworks and on as many platforms that you have ie. Windows, Linux, Mac). The [Script Build](#script-build) section below gives exact instructions on how to do this from the Command Prompt / Terminal.
7. Commit the code and push the branch to your fork
8. Create the PR to NUnit (this is described in more detail [here](https://thenewstack.io/getting-legit-with-git-and-github-your-first-pull-request/))
9. Follow up the PR, both with respect to the CI builds and to any comments that arise.

## Preparing for building

There is a single Visual Studio solution, `nunit.slnx`, which resides in the NUnit repository root.

NUnit framework can be built from this solution using a .NET-capable IDE such as:

- [Visual Studio 2026](https://www.visualstudio.com/vs/) or newer (Windows)
- [Visual Studio for Mac](https://www.visualstudio.com/vs/) (macOS).
- [Visual Studio Code](https://code.visualstudio.com/)
   - Compile using the [build script](#script-build) instead if using VS Code (see below for details).
- [Jetbrains Rider](https://www.jetbrains.com/rider/)

On all platforms, you will need to install [.NET 10.0 SDK](https://www.microsoft.com/net/download/windows) or newer.

On Mac or Linux, you will need to install [Mono](https://www.mono-project.com/download/), an open source implementation of Microsoft's .NET Framework. Mono version 6.12.0 Stable (6.12.0.206) has been validated to build the NUnit solution on GNU/Linux Debian 10 'buster'.

The binary files will be found under the bin directory for each C# project in the solution.

As the NUnit solution targets multiple frameworks, a single build will generate files for each targeted framework.

## Building the solution

You can build from your chosen IDE. That will work for the pure building and also for running the tests, but currently not for packaging.

We use command line builds for all aspects, build, test and packaging. You should also use this to ensure all aspects are covered.

For command line builds you can use either our own build scripts or `dotnet`. (Using `dotnet` will currently not pack.)

To build the system use `build.cmd` (Windows cmd), `build.ps1` (Powershell on Windows) or `build.sh` (bash on Linux). They all use the same parameters, and invoke the same underlying scripts. We will refer to these just as `build` further on.

Key arguments to `build`:

| Argument | Description |
|---|---|
| --target={task}                 | The task to run - see below.                        |
| --configuration=[Release\|Debug] | The configuration to use (default is Release)       |
| --showdescription               | Shows all of the build tasks and their descriptions |

The essential tasks are:

| Task | build command |dotnet command |Description |
|---|---|---| --- |
| Restore |  --- | dotnet restore  |  Restore all dependencies. | 
| Build | build    | dotnet build | Builds everything, also restore. This is the default if no target is given. |
| Rebuild| build --target Rebuild  | dotnet clean && dotnet build | Cleans the output directory and builds everything |
| Test | build --target Test     | dotnet test | Runs all tests. Dependent on Build. |
| Package | build --target Package  | (not working yet) | Creates all packages (Does not build) |

For example, the following command `build --target=Test --configuration=Release` (Windows cmd) will perform a full release build for all target frameworks and then execute the unit tests against each target.

For a full list of tasks, run `build --showdescription` (Windows cmd). 

### Running Tests with options

As shown above, tests can be run using either the build commands or directly with `dotnet test`.  There are some options with respect to output as outlined below.

| Command | Description |
|---------|-------------|
| `build --target=Test` | Run all tests with normal output |
| `build --target=Test --minimal=true` | Run tests with minimal output (summaries only) |
| `dotnet test` | Run tests directly using dotnet CLI |
| `dotnet test -v m` | Run tests with minimal MSBuild output | 
| `dotnet test -l "console;verbosity=detailed"` | Run tests with detailed test output | 
| `dotnet test --settings quiet.runsettings` | Run tests with reduced NUnit output |

The build script `Test` target produces a summary at the end showing total tests, passed, failed, and skipped counts across all frameworks.
The `--quiet=true` option shows only per-assembly summaries without individual test names.

Any failures will be shown for all commands.

### The test projects

The tests that should be run in the solution are grouped by project name:

* `nunit.framework.tests-*`
* `nunit.framework.legacy.tests-*`
* `nunitlite.tests-*`

The other test projects contain tests designed to fail purposely for integration tests and are not intended to be run separately.

### Notes

1. By design, the Package target does not depend on Build. This is to allow re-packaging when necessary without changing the binaries themselves. Of course, this means that you have to be very careful that the build is up to date before packaging.



