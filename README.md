# NUnit 3 Framework #

[![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/3xfkxtnkrts1x06q/branch/master?svg=true)](https://ci.appveyor.com/project/CharliePoole/nunit/branch/master) [![Travis Build Status](https://travis-ci.org/nunit/nunit.svg?branch=master)](https://travis-ci.org/nunit/nunit) [![NuGet Version and Downloads count](https://buildstats.info/nuget/NUnit)](https://www.nuget.org/packages/NUnit)

[![Follow NUnit](https://img.shields.io/twitter/follow/nunit.svg?style=social)](https://twitter.com/nunit) [![Gitter](https://img.shields.io/gitter/room/nwjs/nw.js.svg)](https://gitter.im/nunit/nunit) [![nunit-discuss Google Groups](https://img.shields.io/badge/mailing%20list-nunit--discuss-blue.svg)](https://groups.google.com/forum/#!forum/nunit-discuss)

NUnit is a unit-testing framework for all .NET languages. Initially ported from JUnit, the current production release, version 3, has been completely rewritten with many new features and support for a wide range of .NET platforms.

## Table of Contents ##

- [Downloads](#downloads)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [Debugger source-stepping](#debugger-source-stepping)
- [License](#license)
- [NUnit Projects](#nunit-projects)

## Downloads ##

The latest stable release of the NUnit Framework is [available on NuGet](https://www.nuget.org/packages/NUnit/) or can be [downloaded from GitHub](https://github.com/nunit/nunit/releases). Pre-release builds are [available on MyGet](https://www.myget.org/feed/nunit/package/nuget/NUnit).

## Documentation ##

Documentation for all NUnit projects are hosted on GitHub at [https://github.com/nunit/docs/wiki](https://github.com/nunit/docs/wiki).

- [NUnit Documentation](https://github.com/nunit/docs/wiki/NUnit-Documentation)
- [Installation](https://github.com/nunit/docs/wiki/Installation)
- [Release Notes](https://github.com/nunit/docs/wiki/Release-Notes)
- [Code Samples](https://github.com/nunit/docs/wiki/Samples)

## Contributing ##

For more information on contributing to the NUnit project, please see [CONTRIBUTING.md](https://github.com/nunit/nunit/blob/master/CONTRIBUTING.md) and the [Developer Docs](https://github.com/nunit/docs/wiki/Team-Practices#technical-practices).

NUnit 3.0 was created by [Charlie Poole](https://github.com/CharliePoole), [Rob Prouse](https://github.com/rprouse), [Simone Busoli](https://github.com/simoneb), [Neil Colvin](https://github.com/oznetmaster) and numerous community contributors. A complete list of contributors since NUnit migrated to GitHub can be [found on GitHub](https://github.com/nunit/nunit/graphs/contributors).

Earlier versions of NUnit were developed by Charlie Poole, James W. Newkirk, Alexei A. Vorontsov, Michael C. Two and Philip A. Craig.

## Debugger source-stepping ##

The NUnit and NUnitLite NuGet packages contain a source-indexed PDB for each binary from this repository.
If you’re in the middle of a debugging session and realize you’d like to be able to step into NUnit code,
set breakpoints and watch variables, [follow these steps](https://github.com/nunit/docs/wiki/Debugger-Source-Stepping).

## License ##

NUnit is Open Source software and NUnit 3 is released under the [MIT license](https://raw.githubusercontent.com/nunit/nunit/master/LICENSE.txt). Earlier releases used the [NUnit license](http://www.nunit.org/nuget/license.html). Both of these licenses allow the use of NUnit in free and commercial applications and libraries without restrictions.

## NUnit Projects ##

NUnit is made up of several projects. When reporting issues, please try to report issues in the correct project.

### Core Projects ###

- [NUnit Test Framework](https://github.com/nunit/nunit) - The test framework used to write NUnit tests (this repository)
- [NUnit Console and Engine](https://github.com/nunit/nunit-console) - Runs unit tests from the command line and provides the engine that is used by other test runners to run NUnit tests

### Visual Studio Extensions ###

- [NUnit 3 Visual Studio Adapter](https://github.com/nunit/nunit3-vs-adapter) - Visual Studio adapter for running NUnit 3 tests in Visual Studio and in VSTS/TFS builds
- [NUnit Visual Studio Templates](https://github.com/nunit/nunit-vs-templates) - Project templates and snippets for writing unit tests in Visual Studio
- [Visual Studio Test Generator](https://github.com/nunit/nunit-vs-testgenerator) - Generates NUnit tests in Visual Studio
- [NUnit 2 Visual Studio Adapter](https://github.com/nunit/nunit-vs-adapter) - Visual Studio adapter for running older NUnit 2.x tests in Visual Studio and in VSTS/TFS builds

### Other Projects ###

- [NUnit GUI](https://github.com/nunit/nunit-gui) - Runs NUnit 3 tests in a GUI based runner
- [NUnit Xamarin Runner](https://github.com/nunit/nunit.xamarin) - Runs NUnit 3 tests on mobile devices using the Xamarin framework
- [NUnit .NET Core Runner](https://github.com/nunit/dotnet-test-nunit) - Runs NUnit tests compiled against .NET Core
- [NUnit Project Editor](https://github.com/nunit/nunit-project-editor) - Edits the NUnit Project format

### NUnit Engine Extensions ###

- [NUnit 2 Driver](https://github.com/nunit/nunit-v2-framework-driver) - Allows the NUnit 3 engine to run NUnit 2 tests
- [NUnit 2 Result Writer](https://github.com/nunit/nunit-v2-result-writer) - Writes test results in the legacy NUnit 2 format
- [Visual Studio Project Loader](https://github.com/nunit/vs-project-loader) - Loads and parses Visual Studio projects and solutions
- [NUnit Project Loader](https://github.com/nunit/nunit-project-loader) - Loads NUnit projects
- [TeamCity Event Listener](https://github.com/nunit/teamcity-event-listener) - Allows you to run and integrate NUnit tests into TeamCity
