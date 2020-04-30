More information and getting started tutorials are available for NUnit and .NET Core targeting [C#](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-nunit), [F#](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-fsharp-with-nunit) and [Visual Basic](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-visual-basic-with-nunit) in the .NET Core documentation's [Unit Testing in .NET Core and .NET Standard](https://docs.microsoft.com/en-us/dotnet/core/testing/) page.

Testing .NET Core and .NET Standard projects requires *each* test project to reference version 3.9.0 or later of the NUnit 3 Visual Studio Test Adapter.

It is recommended to install the adapter from NuGet if you are testing .NET Core or .NET Standard projects. The VSIX adapter does not, and will not, support .NET Core because VSIX packages cannot target multiple platforms.

Adding this adapter and `Microsoft.NET.Test.Sdk` version `15.5.0` to your NUnit test projects will also enable the `dotnet test` command for .NET Core projects.

Any tests using the new style CSPROJ format, either .NET Core or .NET 4.x, need to add a `PackageReference` to the NuGet package `Microsoft.NET.Test.Sdk`. Your test assemblies must also be .NET Core or .NET 4.x, not .NET Standard.

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.5.0" />
  <PackageReference Include="NUnit" Version="3.9.0" />
  <PackageReference Include="NUnit3TestAdapter" Version="3.9.0" />
</ItemGroup>
```

.NET Core test can be run on the command line with `dotnet test`, for example,

```cmd
> dotnet test .\test\NetCore10Tests\NetCore10Tests.csproj
```

Also, note that **Code Coverage** and **Live Unit Testing** does not work with .NET Core yet. They will be supported in a future version of Visual Studio, likely post 15.3.

For a more complete walkthrough, please see [Testing .NET Core with NUnit in Visual Studio 2017](http://www.alteridem.net/2017/05/04/test-net-core-nunit-vs2017/)

### Install the NUnit project template

The NUnit test project templates need to be installed before creating a test project. This only needs to be done once. Run `dotnet new -i NUnit3.DotNetNew.Template` to install the NUnit templates.

Once you do this, you can then run `dotnet new nunit` to create an NUnit test project.

### FAQ

#### Why can't my tests target .NET Standard?

Visual Studio and VSTest require that the tests target a specific platform. .NET Standard is like a Portable library in that it does not target any specific platform, but can run on any supported platform. Microsoft decided that your tests should be compiled to target a platform so they know which platform to run your tests under and you get the behavior you expect for the platform you are targeting. You can however target multiple platforms in your tests and compile and run each from the command line. It still only runs one platform from Visual Studio, but I would hope that is being worked on. I haven't tested 15.3 yet.

It is similar to a console application, it cannot be .NET Standard, it must target a platform, .NET Core or .NET Framework.

This limitation is the same for all test adapters including xUnit and MSTest2.

#### My tests aren't showing up in Visual Studio 2017?

- Are you using the NuGet package?
- Are you using version 3.8.0 or newer of the NuGet package?
- Do your tests target .NET Core or the full .NET Framework? (see above)
- Have you added a Package Reference to `Microsoft.NET.Test.Sdk`?
- Have you restarted Visual Studio? It is still a bit tempermental.

#### My tests multi-target .NET Core and .NET Framework, why can't I run both in Visual Studio?

This is a limitation of Visual Studio, hopefully it will be fixed in a future release.

Meanwhile, you can run specific tests using the `--framework` command line option of [dotnet test](https://docs.microsoft.com/en-ca/dotnet/core/tools/dotnet-test?tabs=netcore2x)

#### How to I produce a test results file?

`dotnet test` does not currently support passing command line options on to the test adapter, so NUnit
cannot produce TestResults.xml yet. We are looking at ways of working around this, but for now, people have
had success producing a VSTest results file. See the issue [Add support for producing XML test results](https://github.com/nunit/nunit3-vs-adapter/issues/323) for more info.
