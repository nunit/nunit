# NUnit 4 Framework


[![Follow NUnit](https://img.shields.io/twitter/follow/nunit.svg?style=social)](https://twitter.com/nunit) [![Slack](https://img.shields.io/badge/chat-on%20Slack-brightgreen)](https://join.slack.com/t/nunit/shared_invite/zt-jz58jw68-Led8y3WH4n2a~Y5WjuOpKA) [![NUnit issues marked with "help wanted" label](https://img.shields.io/github/issues/nunit/nunit/help%20wanted.svg)](https://github.com/nunit/nunit/issues?q=is%3Aopen+is%3Aissue+label%3A%22help+wanted%22) [![NUnit issues marked with "good first issue" label](https://img.shields.io/github/issues/nunit/nunit/good%20first%20issue.svg)](https://github.com/nunit/nunit/issues?q=is%3Aopen+is%3Aissue+label%3A%22good+first+issue%22)

NUnit is a unit-testing framework for all .NET languages. 
It can run on macOS, Linux and Windows operating systems. 
NUnit can be used for a wide range of testing, from unit testing with TDD to full fledged system and integration testing.
It is a non-opinionated, broad and deep framework with multiple different ways to assert that your code behaves as expected. Many aspects of NUnit can be extended to suit your specific purposes.

The latest version, version 4, is an upgrade from the groundbreaking NUnit 3 framework. It is a modernized version, aimed at taking advantage of the latest .NET features and C# language constructs.

If you are upgrading from NUnit 3, be aware of the [breaking changes](https://docs.nunit.org/articles/nunit/release-notes/breaking-changes.html#nunit-40). Please see the [NUnit 4 Migration Guide](https://docs.nunit.org/articles/nunit/release-notes/Nunit4.0-MigrationGuide.html) and take care to prepare your NUnit 3 code before you do the upgrade.

## Table of Contents ##

- [Downloads](#downloads)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [License](#license)
- [NUnit Projects](#nunit-projects)

## Downloads ##

The latest stable release of the NUnit Framework is [available on NuGet](https://www.nuget.org/packages/NUnit/) or can be [downloaded from GitHub](https://github.com/nunit/nunit/releases). Pre-release builds are [available on MyGet](https://www.myget.org/feed/nunit/package/nuget/NUnit).

## Documentation ##

Documentation for all NUnit projects can be found at the [documentation site](https://docs.nunit.org).

- [NUnit Documentation](https://docs.nunit.org/articles/nunit/intro.html)
- [Installation](https://docs.nunit.org/articles/nunit/getting-started/installation.html)
- [Release Notes](https://docs.nunit.org/articles/nunit/release-notes/framework.html)
- [Code Samples](https://docs.nunit.org/articles/nunit/getting-started/samples.html)

## Contributing ##

For more information on contributing to the NUnit project, please see [CONTRIBUTING.md](https://github.com/nunit/nunit/blob/master/CONTRIBUTING.md) and the [Developer Docs](https://docs.nunit.org/articles/developer-info/Team-Practices.html#technical-practices).

NUnit 3.0 was created by [Charlie Poole](https://github.com/CharliePoole), [Rob Prouse](https://github.com/rprouse), [Simone Busoli](https://github.com/simoneb), [Neil Colvin](https://github.com/oznetmaster) and numerous community contributors. A complete list of contributors since NUnit migrated to GitHub can be [found on GitHub](https://github.com/nunit/nunit/graphs/contributors).

Earlier versions of NUnit were developed by Charlie Poole, James W. Newkirk, Alexei A. Vorontsov, Michael C. Two and Philip A. Craig.

## License ##

NUnit is Open Source software and NUnit 4 is released under the [MIT license](https://raw.githubusercontent.com/nunit/nunit/master/LICENSE.txt). Earlier releases used the [NUnit license](https://nunit.org/nuget/license.html). Both of these licenses allow the use of NUnit in free and commercial applications and libraries without restrictions.

## NUnit Projects ##

NUnit is made up of several projects. When reporting issues, please try to report issues in the correct project.

### Core Projects ###

- [NUnit Test Framework](https://github.com/nunit/nunit) - The test framework used to write NUnit tests (this repository)
- [NUnit Visual Studio Adapter](https://github.com/nunit/nunit3-vs-adapter) - Visual Studio/Dotnet adapter for running NUnit 3 and 4 tests in Visual Studio or the `dotnet` command line.
- [NUnit Console and Engine](https://github.com/nunit/nunit-console) - Runs unit tests from the command line and provides the engine that is used by other test runners to run NUnit tests.

### Visual Studio Extensions ###

- [NUnit Visual Studio Adapter](https://github.com/nunit/nunit3-vs-adapter) - Visual Studio adapter for running NUnit 3 and 4 tests in Visual Studio or the `dotnet` command line.
- [NUnit Visual Studio Templates](https://github.com/nunit/nunit-vs-templates) - Project templates and snippets for writing unit tests in Visual Studio. This repo is for reference only, as the templates have been donated to the [Dotnet project](https://github.com/dotnet/test-templates) and are maintained there, and also released as part of the dotnet releases.
- [Visual Studio Test Generator](https://github.com/nunit/nunit-vs-testgenerator) - Generates NUnit tests in Visual Studio
- [NUnit 2 Visual Studio Adapter](https://github.com/nunit/nunit-vs-adapter) - Visual Studio adapter for running older NUnit 2.x tests in Visual Studio and in VSTS/TFS builds

### NUnit Engine Extensions ###

- [NUnit 2 Driver](https://github.com/nunit/nunit-v2-framework-driver) - Allows the NUnit 3 engine to run NUnit 2 tests
- [NUnit 2 Result Writer](https://github.com/nunit/nunit-v2-result-writer) - Writes test results in the legacy NUnit 2 format
- [Visual Studio Project Loader](https://github.com/nunit/vs-project-loader) - Loads and parses Visual Studio projects and solutions
- [NUnit Project Loader](https://github.com/nunit/nunit-project-loader) - Loads NUnit projects
