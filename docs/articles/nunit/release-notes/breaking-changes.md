## NUnit 3.10

* `NUnit.Framework.Constraints.NUnitEqualityComparer.Default` was deprecated in favor of `new NUnitEqualityComparer()`.

## NUnit 3.8

* Removed some deprecated attributes:
  - TestFixtureSetUpAttribute (use OneTimeSetUpAttribute)
  - TestFixtureTearDownAttribute (use OneTimeTearDownAttribute)
  - RequiresSTAAttribute (use ApartmentAttribute)
  - RequiresMTAAttribute (use ApartmentAttribute)

* Removed deprecated Is.StringXxxx syntax elements:
  - Is.StringStarting (use Does.StartWith)
  - Is.StringEnding (use Does.EndWith)
  - Is.StringContaining (use Does.Contain)
  - Is.StringMatching (use Does.Match)

## NUnit 3.7

* The AssertionHelper class has now been deprecated. Users can use the [NUnit.StaticExpect](https://github.com/fluffynuts/NUnit.StaticExpect) library as a near drop-in replacement.

## NUnit 3.4

Breaking changes introduced in NUnit 3.4

 * The `--teamcity` console command-line option now requires the TeamCityEventListener extension to be installed. This only affects users who install the extensions individually or copy them to another directory. If you install using the Windows installer or the NUnit.Console NuGet package the TeamCity extension is included.

 * String arguments in the names of test cases are no longer truncated to 40 characters.

 * The .NET 2.0 build of the nunit framework uses a private implementation of System.Linq. NUnit installs the NUnit.System.Linq assembly alongside the nunit.framework assembly. If you copy the framework to another location, you must ensure that both are copied. The extra assembly is not used in other builds because System.Linq is already supported in those environments.

## NUnit 3.0

A relatively large number of features present in NUnit 2.6, were either removed in NUnit 3.0 or had their behavior modified in a way that will break existing code.

A key change is that the NUnit Test Engine will not recognize a test assembly that does not reference the NUnit framework directly. Normally, test assemblies use NUnit Asserts, attributes and other Types and methods. However, certain third-party frameworks are designed to completely isolate the user from the details of NUnit. They mediate between the test assembly and the NUnit framework in order to run tests. In such a case, NUnit will indicate that the assembly either contains no tests or the proper driver could not be found. To resolve this situation, simply add one NUnit attribute or other reference. An assembly-level `ParallelizableAttribute` is useful for this purpose.

Other breaking changes are grouped in the following tables.

###### Attributes

|            Name              |          Notes                                        |
|------------------------------|-------------------------------------------------------|
| ExpectedExceptionAttribute   | No longer supported. Use `Assert.Throws` or `Assert.That`. |
| IgnoreAttribute              | The reason is now mandatory |
| RequiredAddinAttribute       | No longer supported. |
| RequiresMTAAttribute         | Deprecated. Use `ApartmentAttribute`                    |
| RequiresSTAAttribute         | Deprecated. Use `ApartmentAttribute`                    |
| SuiteAttribute               | No longer supported. |
| System.MTAThreadAttribute    | No longer treated as `RequiresMTAAttribute`             |
| System.STAThreadAttribute    | No longer treated as `RequiresSTAAttribute`             | 
| TearDown and OneTimeTearDown | There is a change to the logic by which teardown methods are called. See [Setup and Teardown Changes](xref:setupteardownchanges for details. |
| TestCaseAttribute            | Named parameter `Result=` is no longer supported. Use `ExpectedResult=`. Named parameter `Ignore=` now takes a string, giving the reason for ignoring the test.|
| TestCaseSourceAttribute      | The attribute forms using a string argument to refer to the data source must now use only static fields, properties or methods. |
| TestFixtureAttribute         | Named parameter `Ignore=` now takes a string, giving the reason for ignoring the test. |
| TestFixtureSetUpAttribute    | Deprecated. Use `OneTimeSetUpAttribute`.  |
| TestFixtureTearDownAttribute | Deprecated. Use `OneTimeTearDownAttribute`.  |
| ValueSourceAttribute         | The source name of the data source must now use only static fields, properties or  methods. |

###### Assertions and Constraints

|          Feature                 |          Notes                                        |
|----------------------------------|-------------------------------------------------------|
| Assert.IsNullOrEmpty             | No longer supported. Use `Assert.That(..., Is.Null.Or.Empty)` |
| Assert.IsNotNullOrEmpty          | No longer supported. Use `Assert.That(..., Is.Not.Null.And.Not.Empty)` |
| Is.InstanceOfType                | No longer supported. Use `Is.InstanceOf`                    |
| Is.StringStarting                | Deprecated. Use `Does.StartWith` |
| Is.StringContaining              | Deprecated. Use `Does.Contain` |
| Is.StringEnding                  | Deprecated. Use `Does.EndWith` |
| Is.StringMatching                | Deprecated. Use `Does.Match` |
| NullOrEmptyStringConstraint      | No longer supported. See `Assert.IsNullOrEmpty` above   |
| Text.All                         | No longer supported. Use `Has.All` or `Is.All` |
| Text.Contains                    | No longer supported. Use `Does.Contain` or `Contains.Substring` |
| Text.DoesNotContain              | No longer supported. Use `Does.Not.Contain` |
| Text.StartsWith                  | No longer supported. Use `Does.StartWith` |
| Text.DoesNotStartWith            | No longer supported. Use `Does.Not.StartWith` |
| Text.EndsWith                    | No longer supported. Use `Does.EndWith` |
| Text.DoesNotEndWith              | No longer supported. Use `Does.Not.EndWith` |
| Text.Matches                     | No longer supported. Use `Does.Match` |
| Text.DoesNotMatch                | No longer supported. Use `Does.Not.Match` |

###### Other Framework Features

|      Feature       |          Notes                                        |
|--------------------|-------------------------------------------------------|
| Addins             | No longer supported. See [Addin Replacement in the Framework](xref:addinreplacementintheframework). |
| CurrentDirectory   | No longer set to the directory containing the test assembly. Use `TestContext.CurrentContext.TestDirectory` to locate that directory. |
| NUnitLite          | NUnitLite executable tests must now reference nunit.framework in addition to NUnitLite. |
| SetUpFixture       | Now uses `OneTimeSetUpAttribute` and `OneTimeTearDownAttribute` to designate higher-level setup and teardown methods. `SetUpAttribute` and `TearDownAttribute` are no longer allowed. |
| TestCaseData       | The `Throws` Named Property is no longer available. Use `Assert.Throws` or `Assert.That` in your test case. |
| TestContext        | The fields available in the [TestContext](xref:TestContext)]] have changed, although the same information remains available as for NUnit V2. |
| Async void tests   | No longer supported. Use `async Task` as the method signature instead. |

###### Console Features

The console runner is now called `nunit3-console`. The following breaking changes apply to the options that  the new runner supports.

|      Option       |     Function                            |     Notes                |
|-------------------|-----------------------------------------|--------------------------|
| --fixture=STR     | Test fixture or namespace to be loaded  | No longer supported. Use --test instead. |
| --run=STR         | List of tests to run                    | No longer supported. Replaced by --test. |
| --runlist=PATH    | File containing list of tests to run    | No longer supported. Replaced by --testlist. |
| --include=LIST    | List of categories to include           | No longer supported. Use --where instead. |
| --exclude=LIST    | List of categories to exclude           | No longer supported. Use --where instead. |
| --process=PROCESS | ProcessModel for test assemblies        | Default value is now Separate for a single assembly, Multiple for more than one. Multiple assemblies run in parallel by default. |
| --domain=DOMAIN   | DomainUsage for test assemblies         | Default value is now Separate for a single assembly, Multiple for more than one. |
| --apartment=APT   | Apartment in which to run tests         | No longer supported. Use ApartmentAttribute. |
| --xml=PATH        | Output result xml to path               | No longer supported. Use --result=SPEC instead.    |
| --noxml           | Disable xml result output               | No longer supported. Use --noresult instead.    |
| --xmlconsole      | Display XML to the console              | No longer supported.     |
| --basepath        | Set ApplicationBase for execution       | No longer supported.     |
| --privatebinpath  | Specify probing path for execution      | No longer supported.     |
| --cleanup         | Remove left-over cache files            | No longer supported.     |
| --noshadow        | Disable shadow copy                     | No longer supported. The console runner now disables shadow copy by default. use **--shadowcopy** on the command-line to turn it on. |
| --nothread        | Disable use of a separate thread for tests  | No longer supported. |
| --nodots          | Do not display dots as a progress indicator | No longer supported. |
