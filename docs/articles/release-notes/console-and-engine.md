#### NUnit Console & Engine 3.11.1 - February 15, 2020
This hotfix fixes a problem with NUnit Project file settings being ignored.

 * [730](https://github.com/nunit/nunit-console/issues/730) NUnit project file settings are ignored
 * [732](https://github.com/nunit/nunit-console/issues/732) Upgrade Cake Build to fix Linux CI

#### NUnit Console 3.11 - January 26, 2020

This release fixes a range of minor bugs, and includes a significant amount of internal restructuring work. In future, this will enable improved .NET Standard support in the engine, and a .NET Core build of the console.

 * [22](https://github.com/nunit/nunit-console/issues/22) Engine modifies TestPackage
 * [53](https://github.com/nunit/nunit-console/issues/53) Add project element to top-level sub-project before merging
 * [181](https://github.com/nunit/nunit-console/issues/181) XSLT Transform not honoring --encoding value
 * [336](https://github.com/nunit/nunit-console/issues/336) Should legacyCorruptedStateExceptionsPolicy enabled=true in nunit3-console.exe.config?
 * [386](https://github.com/nunit/nunit-console/issues/386) nUnit project loader does not work when --inprocess is set
 * [453](https://github.com/nunit/nunit-console/issues/453) build-mono-docker.ps1 fails to run out the box
 * [514](https://github.com/nunit/nunit-console/issues/514) Add higher-level unit tests for structure of TestRunners
 * [586](https://github.com/nunit/nunit-console/issues/586) Create Separate Addin File for the Engine NuGet Package
 * [588](https://github.com/nunit/nunit-console/issues/588) licenseUrl in NuGet packages are deprecated
 * [591](https://github.com/nunit/nunit-console/issues/591) Release 3.10 merge
 * [592](https://github.com/nunit/nunit-console/issues/592) Add status badge from Azure pipelines
 * [594](https://github.com/nunit/nunit-console/issues/594) Fixed typos in release notes
 * [595](https://github.com/nunit/nunit-console/issues/595) Clean extension dir before running FetchExtensions task
 * [603](https://github.com/nunit/nunit-console/issues/603) Engine returns assembly-level test-suite event twice
 * [605](https://github.com/nunit/nunit-console/issues/605) Trailing \ in --work argument causes agent to crash
 * [607](https://github.com/nunit/nunit-console/issues/607) Unload + Load changes TestPackage IDs
 * [611](https://github.com/nunit/nunit-console/issues/611) Set DisableImplicitNuGetFallbackFolder and bump Ubuntu on Travis
 * [612](https://github.com/nunit/nunit-console/issues/612) Fix logging when including exception
 * [617](https://github.com/nunit/nunit-console/issues/617) Consider expanding projects before building ITestRunner structure
 * [625](https://github.com/nunit/nunit-console/issues/625) [Feature] Extend <start-run> data for ITestEventListener
 * [628](https://github.com/nunit/nunit-console/issues/628) [Question] Possible to set both labels=After and labels=Before
 * [634](https://github.com/nunit/nunit-console/issues/634) Remove unnecessary stream creation in XML Transform writer
 * [635](https://github.com/nunit/nunit-console/issues/635) Remove all #regions from codebase
 * [636](https://github.com/nunit/nunit-console/issues/636) Labels option: Rename On as OnOutputOnly, and deprecate On and All
 * [637](https://github.com/nunit/nunit-console/issues/637) Refactor RunnerSelectionTests
 * [639](https://github.com/nunit/nunit-console/issues/639) Engine initializes DriverService too early
 * [667](https://github.com/nunit/nunit-console/issues/667) Console Runner loads wrong .NET framework version when executing tests from multiple assemblies at once
 * [669](https://github.com/nunit/nunit-console/issues/669) nunit.console-runner-with-extensions.nuspec: Remove outdated release notes
 * [671](https://github.com/nunit/nunit-console/issues/671) Manually updated .NET Core SDK on Linux build
 * [681](https://github.com/nunit/nunit-console/issues/681) Display path and version of extension assemblies
 * [683](https://github.com/nunit/nunit-console/issues/683) Safely encapsulating the atomic agent database operations
 * [684](https://github.com/nunit/nunit-console/issues/684) Split engine into upper and lower parts
 * [691](https://github.com/nunit/nunit-console/issues/691) Sign NuGet Packages and msi
 * [693](https://github.com/nunit/nunit-console/issues/693) Update Engine tests to run on LTS .NET Core version
 * [696](https://github.com/nunit/nunit-console/issues/696) Minimal compilation/test of .NET Core Console
 * [698](https://github.com/nunit/nunit-console/issues/698) Update NUnit v2 driver extension in combined packages
 * [703](https://github.com/nunit/nunit-console/issues/703) Update Console options for .NET Core Console build
 * [704](https://github.com/nunit/nunit-console/issues/704) Agent in nupkg should not be referenced and causes warnings in consuming projects
 * [706](https://github.com/nunit/nunit-console/issues/706) build.cake maintenance
 * [707](https://github.com/nunit/nunit-console/issues/707) Set agent to reference core and not full engine
 * [713](https://github.com/nunit/nunit-console/issues/713) Engine will not recognize .NET Framework versions beyond 4.5

#### NUnit Console 3.10 - March 24, 2019

This release merges the .NET Standard version of the engine back into the nunit.engine
NuGet package and adds a .NET Standard 2.0 version of the engine that re-enables most
services and extensions. This deprecates the `nunit.engine.netstandard` NuGet package.
Any test runners using the old .NET Standard version of the engine should switch to
this release.

The `--params` command line option which took multiple test parameters separated by
a semicolon is now deprecated in favor of the new `--testparam` command line option.
One of the most common uses for test parameters was to pass connection strings into
tests but this required workarounds to handle the semicolons. Now you must pass in
each test paramater separately using a `--testparam` or `--tp` option for each.

#### Issues Resolved

 * [8](https://github.com/nunit/nunit-console/issues/8) TempResourceFile.Dispose causes run to hang
 * [23](https://github.com/nunit/nunit-console/issues/23) In nunit3-console you cannot pass parameters containing ';' because they always get splitted
 * [178](https://github.com/nunit/nunit-console/issues/178) Add date and time to console output
 * [282](https://github.com/nunit/nunit-console/issues/282) "Execution terminated after first error" does not fail the console runner
 * [388](https://github.com/nunit/nunit-console/issues/388) Merge .NET Standard Engine back into the main solution
 * [389](https://github.com/nunit/nunit-console/issues/389) Update Mono.Cecil to latest
 * [433](https://github.com/nunit/nunit-console/issues/433) All messages from EventListenerTextWriter goes to console output independent on stream name
 * [454](https://github.com/nunit/nunit-console/issues/454) Misc improvements to ExtensionServiceTests
 * [455](https://github.com/nunit/nunit-console/issues/455) Remove CF, Silverlight and PORTABLE functionality
 * [464](https://github.com/nunit/nunit-console/issues/464) NUnit Console Reports Successful Exit Code When there is an Exception on Dispose
 * [473](https://github.com/nunit/nunit-console/issues/473) ArgumentException: DTD is prohibited in this XML document
 * [476](https://github.com/nunit/nunit-console/issues/476) .NET Standard engine to load extensions
 * [479](https://github.com/nunit/nunit-console/issues/479) Merge .NET Standard Engine code back into the main solution
 * [483](https://github.com/nunit/nunit-console/issues/483) Error in SetUpFixture does not result in non-zero exit code
 * [485](https://github.com/nunit/nunit-console/issues/485) Invalid integer arguments do not display properly in error message
 * [493](https://github.com/nunit/nunit-console/issues/493) Correct order of params to Guard.ArgumentValid()
 * [498](https://github.com/nunit/nunit-console/issues/498) Reset console colors after Ctrl-C
 * [501](https://github.com/nunit/nunit-console/issues/501) Create result directory if it does not exist
 * [502](https://github.com/nunit/nunit-console/issues/502) Remove unused method from build.cake
 * [506](https://github.com/nunit/nunit-console/issues/506) Dogfood NUnit.Analyzers via the nunit-console tests
 * [508](https://github.com/nunit/nunit-console/issues/508) Re-Enable OSX CI tests
 * [515](https://github.com/nunit/nunit-console/issues/515) Appveyor CI failing on master
 * [518](https://github.com/nunit/nunit-console/issues/518) Correct Refactoring Error
 * [519](https://github.com/nunit/nunit-console/issues/519) Break up multiple console error messages with colour
 * [523](https://github.com/nunit/nunit-console/issues/523) Reloading multiple files causes exception
 * [524](https://github.com/nunit/nunit-console/issues/524) .NET Standard 2.0 engine crashes when .NET Framework extensions are in Global NuGet Cache
 * [525](https://github.com/nunit/nunit-console/issues/525) Separate NuGet Restore for Appveyor build
 * [531](https://github.com/nunit/nunit-console/issues/531) Building a forked master branch results in publishing artifacts
 * [533](https://github.com/nunit/nunit-console/issues/533) Duplicate ids when loading a project
 * [544](https://github.com/nunit/nunit-console/issues/544) Deprecate nunit.netstandard.engine NuGet package
 * [546](https://github.com/nunit/nunit-console/issues/546) Cannot run a project file using --process:Separate
 * [547](https://github.com/nunit/nunit-console/issues/547) --labels=Before ignores --nocolor
 * [556](https://github.com/nunit/nunit-console/issues/556) Appveyor CI failing due to nuget restore
 * [557](https://github.com/nunit/nunit-console/issues/557) Disable CliFallbackFolder as a nuget source
 * [562](https://github.com/nunit/nunit-console/issues/562) Fix typo in comment
 * [563](https://github.com/nunit/nunit-console/issues/563) ProjectService is incorrectly initialized in agents
 * [565](https://github.com/nunit/nunit-console/issues/565) Eliminate -dbg suffix from version
 * [566](https://github.com/nunit/nunit-console/issues/566) SettingsService is not needed in agents
 * [567](https://github.com/nunit/nunit-console/issues/567) Unnecessary call to IProjectService
 * [571](https://github.com/nunit/nunit-console/issues/571) Space characters in the work directory path are not properly handled
 * [583](https://github.com/nunit/nunit-console/issues/583) NUnit Console NuGet Package Doesn't Load Extensions
 * [587](https://github.com/nunit/nunit-console/issues/587) Disable new MSBuild GenerateSupportedRuntime functionality, which breaks framework targetting

#### NUnit Console 3.9 - September 5, 2018

This release should stop the dreaded SocketException problem on shutdown. The
console also no longer returns -5 when AppDomains fail to unload at the end of a
test run. These fixes should make CI runs much more stable and predictible.

For developers working on the NUnit Console and Engine project, Visual Studio
2017 update 5 or newer is now required to compile on the command line. This does
not effect developers using NUnit or the NUnit Console, both of which support building
and running your tests in any IDE and on any .NET Framework back to .NET 2.0.

#### Issues Resolved

 * [103](https://github.com/nunit/nunit-console/issues/103) The switch statement does not cover all values of the 'RuntimeType' enum: NetCF.
 * [218](https://github.com/nunit/nunit-console/issues/218) Move Distribution back into the nunit-console project.
 * [253](https://github.com/nunit/nunit-console/issues/253) Master Chocolatey issue
 * [255](https://github.com/nunit/nunit-console/issues/255) SocketException thrown during console run
 * [312](https://github.com/nunit/nunit-console/issues/312) CI failure: SocketException
 * [360](https://github.com/nunit/nunit-console/issues/360) CommandLineOption --err does not write error input to ErrorFile
 * [367](https://github.com/nunit/nunit-console/issues/367) nunit3-console loads nunit.framework with partial name
 * [370](https://github.com/nunit/nunit-console/issues/370) Nunit.Console 3.8 - Socket Exception
 * [371](https://github.com/nunit/nunit-console/issues/371) Remove -5 exit code on app domain unload failures
 * [394](https://github.com/nunit/nunit-console/issues/394) Multi-targeted Engine Extensions
 * [399](https://github.com/nunit/nunit-console/issues/399) Fix minor doccoment issues
 * [411](https://github.com/nunit/nunit-console/issues/411) Make output received when providing user friendly messages unloading the domain more user friendly
 * [412](https://github.com/nunit/nunit-console/issues/412) Extensions not dectected for version 3.9.0-dev-03997
 * [436](https://github.com/nunit/nunit-console/issues/436) NUnitEngineException : Unable to acquire remote process agent
 * [446](https://github.com/nunit/nunit-console/issues/446) Output CI version info to console
 * [448](https://github.com/nunit/nunit-console/issues/448) Update vs-project-loader extension to 3.8.0
 * [450](https://github.com/nunit/nunit-console/issues/450) Update NUnit.Extension.VSProjectLoader to 3.8.0
 * [456](https://github.com/nunit/nunit-console/issues/456) NuGet Package : Add `repository` metadata.
 * [461](https://github.com/nunit/nunit-console/issues/461) Use MSBuild /restore

### NUnit Console 3.8 - January 27, 2018

This release includes several fixes when unloading AppDomains and better error reporting. The
aggregate NuGet packages also include updated versions of several extensions.

#### Issues Resolved

 * [6](https://github.com/nunit/nunit-console/issues/6) TypeLoadException in nunit3-console 3.0.1
 * [93](https://github.com/nunit/nunit-console/issues/93) Update Readme with information about the NuGet packages
 * [111](https://github.com/nunit/nunit-console/issues/111) Provide better info when AppDomain won't unload
 * [116](https://github.com/nunit/nunit-console/issues/116) NUnit 3.5.0 defaults to single agent process when using an nunit project file
 * [191](https://github.com/nunit/nunit-console/issues/191) Exception encountered unloading AppDomain
 * [228](https://github.com/nunit/nunit-console/issues/228) System.Reflection.TargetInvocationException with nunit3-console --debug on Mono
 * [246](https://github.com/nunit/nunit-console/issues/246) No way to specify app.config with console runner
 * [256](https://github.com/nunit/nunit-console/issues/256) Rewrite ConsoleRunnerTests.ThrowsNUnitEngineExceptionWhenTestResultsAreNotWriteable()
 * [259](https://github.com/nunit/nunit-console/issues/259) NUnit3 agent hangs after encountering an "CannotUnloadAppDomainException"
 * [262](https://github.com/nunit/nunit-console/issues/262) Transform file existence check should check current directory instead of WorkDirectory
 * [267](https://github.com/nunit/nunit-console/issues/267) Fix possible NRE
 * [273](https://github.com/nunit/nunit-console/issues/273) Insufficient error handling message in ProcessRunner -> RunTests method
 * [275](https://github.com/nunit/nunit-console/issues/275) Integrate chocolatey packages with build script
 * [284](https://github.com/nunit/nunit-console/issues/284) NUnit3: An exception occured in the driver while loading tests...   bei NUnit.Engine.Runners.ProcessRunner.RunTests(ITestEventListener listener, TestFilter filter)
 * [285](https://github.com/nunit/nunit-console/issues/285) ColorConsoleWriter.WriteLabel causes NullReferenceException
 * [289](https://github.com/nunit/nunit-console/issues/289) Warnings not displayed
 * [298](https://github.com/nunit/nunit-console/issues/298) Invalid --framework option throws exception
 * [300](https://github.com/nunit/nunit-console/issues/300) Agents do not respect the Console WORK parameter when writing log file
 * [304](https://github.com/nunit/nunit-console/issues/304) Catch agent debugger launch exceptions, and improve agent crash handling
 * [309](https://github.com/nunit/nunit-console/issues/309) No driver found if framework assembly reference has uppercase characters
 * [314](https://github.com/nunit/nunit-console/issues/314) Update NUnit.Extension.VSProjectLoader to 3.7.0
 * [318](https://github.com/nunit/nunit-console/issues/318) Update NUnit.Extension.TeamCityEventListener to 1.0.3
 * [320](https://github.com/nunit/nunit-console/issues/320) Return error code -5 when AppDomain fails to unload
 * [323](https://github.com/nunit/nunit-console/issues/323) Assertion should not be ordered in AgentDatabaseTests
 * [343](https://github.com/nunit/nunit-console/issues/343) Superfluous unload error shown in console
 * [349](https://github.com/nunit/nunit-console/issues/349) Get all TestEngine tests running under NUnitAdapter apart from those .
 * [350](https://github.com/nunit/nunit-console/issues/350) Invalid assemblies no longer give an error message
 * [355](https://github.com/nunit/nunit-console/issues/355) NuGet package links to outdated license

### NUnit Console 3.7 - July 13, 2017

#### Engine

 * Creates a .NET Standard version of the engine for use in the Visual Studio Adapter
 * Fixes several issues that caused the runner to exit with a SocketException
#### Issues Resolved

 * [10](https://github.com/nunit/nunit-console/issues/10) Create a .NET Standard version of the Engine
 * [11](https://github.com/nunit/nunit-console/issues/11) insufficient info on driver reflection exception
 * [12](https://github.com/nunit/nunit-console/issues/12) Upgrade Cake build to latest version
 * [24](https://github.com/nunit/nunit-console/issues/24) Update --labels switch with option to show real-time pass/fail results in console runner
 * [31](https://github.com/nunit/nunit-console/issues/31) Nunit 3.4.1 NUnit.Engine.Runners
 * [72](https://github.com/nunit/nunit-console/issues/72) TestContext.Progress.Write writes new line
 * [82](https://github.com/nunit/nunit-console/issues/82) Remove unused repository paths from repositories.config
 * [99](https://github.com/nunit/nunit-console/issues/99) Remove unused --verbose and --full command line options
 * [126](https://github.com/nunit/nunit-console/issues/126) Resolve differences between NUnit Console and NUnitLite implementations of @filename
 * [162](https://github.com/nunit/nunit-console/issues/162) Add namespace keyword to Test Selection Language
 * [171](https://github.com/nunit/nunit-console/issues/171) Socket Exception when stopping Remote Agent
 * [172](https://github.com/nunit/nunit-console/issues/172) Limit Language level to C#6
 * [193](https://github.com/nunit/nunit-console/issues/193) Settings are stored with invariant culture but retrieved with CurrentCulture
 * [194](https://github.com/nunit/nunit-console/issues/194) Better logging or error handling in SettingsStore.SaveSettings
 * [196](https://github.com/nunit/nunit-console/issues/196) Allow comments in @FILE files
 * [200](https://github.com/nunit/nunit-console/issues/200) Remove obsolete warnings from build script
 * [206](https://github.com/nunit/nunit-console/issues/206) Remove reference to removed noxml option
 * [207](https://github.com/nunit/nunit-console/issues/207)  Create Chocolatey package(s) for the console
 * [208](https://github.com/nunit/nunit-console/issues/208) Explore flags test update
 * [213](https://github.com/nunit/nunit-console/issues/213) Master build failing after merging .NET Standard Engine
 * [216](https://github.com/nunit/nunit-console/issues/216) Compiling mock-assembly in Visual Studio 2017 fails
 * [217](https://github.com/nunit/nunit-console/issues/217) NUnit .NET Standard NuGet package missing some values
 * [219](https://github.com/nunit/nunit-console/issues/219) Runtime.Remoting.RemotingException in NUnit.Engine.Runners.ProcessRunner.Dispose
 * [221](https://github.com/nunit/nunit-console/issues/221) Added missing nuget package info
 * [222](https://github.com/nunit/nunit-console/issues/222) Improve missing agent error message
 * [225](https://github.com/nunit/nunit-console/issues/225) SocketException thrown by nunit3-console.exe --explore option
 * [248](https://github.com/nunit/nunit-console/issues/248) Agent TestEngine contains duplicate services
 * [252](https://github.com/nunit/nunit-console/issues/252) Console crashes when specifying both format and transform for result
 * [254](https://github.com/nunit/nunit-console/issues/254) Correct misprint ".con" -> ".com"

### NUnit Console 3.6.1 - March 6, 2017

#### Engine

 * This hotfix release addresses a race condition in the Engine that caused
   tests to intermittently fail.

#### Issues Resolved

 * [168](https://github.com/nunit/docs/issues/168) Intermittent errors while running tests after updating to 3.6


### NUnit Console 3.6 - January 14, 2017

#### Console Runner

 * Added command line option --skipnontestassemblies to skip assemblies that do
   not contain tests without raising an error and to skip assemblies that contain
   the NUnit.Framework.NonTestAssemblyAttribute.
 * Messages from the new Multiple Assert blocks will be displayed individually
 * Warnings from the new Warn.If, Warn.Unless and Assert.Warn are now displayed

#### Engine

 * NUnit agents now monitor the running engine process and will terminate themselves
   if the parent runner process is killed or dies

#### Issues Resolved

 * [16](https://github.com/nunit/nunit-console/issues/16) NUnit Engine Tests fail if not run from test directory
 * [18](https://github.com/nunit/nunit-console/issues/18) Invalid file type is shown in XML as type="Assembly"
 * [23](https://github.com/nunit/nunit-console/issues/23) In nunit3-console you cannot pass parameters containing ';' because they always get split
 * [37](https://github.com/nunit/nunit-console/issues/37) NUnit 3 console should produce xml events for ITestEventListener which contain
      unique id in the scope of all test agents for NUnit 2 tests
 * [58](https://github.com/nunit/nunit-console/issues/58) System.Configuration.ConfigurationErrorsException thrown in multiple domain mode.
 * [62](https://github.com/nunit/nunit-console/issues/62) NUnit3 Fails on DLL with no Tests, Unlike NUnit2
 * [100](https://github.com/nunit/nunit-console/issues/100) Class NUnit.Engine.Services.ResultWriters.Tests.SchemaValidator is never used
 * [101](https://github.com/nunit/nunit-console/issues/101) Method NUnit.Options.OptionSet.Unprocessed always returns "false"
 * [104](https://github.com/nunit/nunit-console/issues/104) Type of variable enumerated in 'foreach' is not guaranteed to be castable
 * [110](https://github.com/nunit/nunit-console/issues/110) Writability check could give a friendlier message.
 * [113](https://github.com/nunit/nunit-console/issues/113) Add task descriptions to Build.cake
 * [127](https://github.com/nunit/nunit-console/issues/127) Modify console runner to display multiple assert information
 * [128](https://github.com/nunit/nunit-console/issues/128) Terminate agent if main process has terminated
 * [133](https://github.com/nunit/nunit-console/issues/133) NUnit downloadable packages zip file naming is confusing and non-intuitive
 * [136](https://github.com/nunit/nunit-console/issues/136) Handle early termination of multiple assert block
 * [138](https://github.com/nunit/nunit-console/issues/138) Report Warnings in console runner
 * [145](https://github.com/nunit/nunit-console/issues/145) MasterTestRunner.RunAsync no longer provides start-run and test-run events
 * [151](https://github.com/nunit/nunit-console/issues/151) Unexpected behaviour from --framework flag
 * [153](https://github.com/nunit/nunit-console/issues/153) Remove some settings used by the engine
 * [156](https://github.com/nunit/nunit-console/issues/156) Use high-quality icon for nuspecs
 * [157](https://github.com/nunit/nunit-console/issues/157) Fix Detection of invalid framework when --inprocess
 * [159](https://github.com/nunit/nunit-console/issues/159) Update extension versions in the NuSpec Files

### Earlier Releases
 * Release Notes for [[NUnit 2.9.1 through 3.5|Pre-3.5-Release-Notes]].
 * Release Notes for [NUnit 2.6 through 2.6.4](http://www.nunit.org/?p=releaseNotes&r=2.6.4)
 * Release Notes for [NUnit 2.5 through 2.5.10](http://www.nunit.org/?p=releaseNotes&r=2.5.10)
 * Release Notes for [NUnit 2.4 through 2.4.8](http://www.nunit.org/?p=releaseNotes&r=2.4.8)
 * Release Notes for [NUnit 2.0 through 2.2.10](http://www.nunit.org/?p=releaseNotes&r=2.2.10)
