### NUnit Test Adapter for Visual Studio - Version 2.2.0 - June 5, 2019

#### Features

* [#180](https://github.com/nunit/nunit-vs-adapter/issues/180) Nunit 2 test adapter does not support Visual Studio 2019 
* [#175](https://github.com/nunit/nunit-vs-adapter/issues/175) NuGet Package : Add `repository` metadata. Thanks to [MaximRouiller](https://github.com/MaximRouiller) for the PR
* [#174](https://github.com/nunit/nunit-vs-adapter/issues/174)  NUnitTestAdapter 2.1.1 not working with Visual Studio 2017 15.8.0  

#### Bugfixes
* [#147](https://github.com/nunit/nunit-vs-adapter/issues/147) Fails to resolve assembly for base type of TestFixture if placed in a different dll
* [#178](https://github.com/nunit/nunit-vs-adapter/issues/178) Test Explorer Picks up Zero Tests in VS 2017 for NUnit 2

---

### NUnit Test Adapter for Visual Studio - Version 2.1.1 - March 19, 2017

Hotfix release

#### Bug Fixes
 * #142 NUnit Test Adapter 2.1 doesn't work with projects that target .NET Framework 3.5 / CLR 2.0 
 * #144 Adapter requires test project pdb's to be generated for tests to be executed
 
---
### NUnit Test Adapter for Visual Studio - Version 2.1 - March 4, 2017

##### Features
 * #135 Support for VS 2017
 * #127 Change adapter package to use tools directory
 * #116 Use Mono.Cecil to retrieve source code locations

##### Bug Fixes
 * #84 NUnit load failure 
 * #87 Can't overload async/await methods

##### Notes
 * The NUnit V2 adapter does not support the Live Unit Testing feature in VS 2017.  That support is only included with the NUnit V3 adapter. 
 * The package including both the adapter and the framework are discontinued. Please install the separate packages instead when upgrading. 
---

### NUnit Test Adapter for Visual Studio - Version 2.0 - April 1, 2015

##### Features

 * Tested for up to VS2015 Pre-release CTP 6
 * Updated to use NUnit 2.6.4
 * Adapter does not try to discover tests if the nunit.framework version is 3.0 or greater

##### Bug Fixes

 * #61 Confusing NUnit version message made clearer
 * #62 Adapter uses shadowcopy setting in discoverer but not in the executor
---
### NUnit Test Adapter for Visual Studio (RTM) - Version 1.2 - September 17, 2014

##### Features

 * Tested for up to VS2013 Update 3
 * Bugs 39 and 40 was inability to run under VS2012. This is now fixed.

##### Bug Fixes

 * #24 Long-running tests ignored
 * #34 Adapter causes ArgumentException to be thrown by Microsoft logger proxy's SendMessage method
 * #37 TestExecution throws Exception System.InvalidOperationException in TFS Build
 * #38 NUnit only accepts absolute paths to test assembly
 * #39 VSTest unable to find NUnit tests since 1.1.0.0
 * #40 NUnit version 1.1.0.0 is broken with test class which ran under 1.0.0.0
---
### NUnit Test Adapter for Visual Studio (RTM) - Version 1.1 - April 26, 2014

##### Features

 * Support for NUnit 2.6.3
 * Tested for up to VS2013 Update 2 RC
 * Shadow copy now disabled by default, see issue #7 Unable to disable shadow copy.
 * Registry settings added for some customization options, see Tips and Tricks
 * All code moved to github

##### Bug Fixes

 * #13 Category attribute not working with TFS test case filter
 * #21 Xamarin.iOS NUnit project causes adapter to throw
---
#### NUnit Test Adapter for Visual Studio (RTM) - Version 1.0 - September 12, 2013

##### Features

 * This is the release version 1.0 of the test adapter.

##### Bug Fixes

 * #1208148 The test result output node is not shown for debug/trace statements
---
#### NUnit Test Adapter for Visual Studio (RC) - Version 0.97 - September 12, 2013

##### Features

 * This is the release candidate for version 1.0 of the test adapter.

##### Bug Fixes

 * #1208161 NUnit Test Adapter runs [Explicit] unit tests in TFS Build
 * #1210536 No Source Available for Async Tests
 * #1165188 Clicking "Run Selected Tests" doesn't show Trace.WriteLine() output
---
#### NUnit Test Adapter for Visual Studio (Beta 6) - Version 0.96 - June 28, 2013

##### Features

 * Support for Visual Studio 2013 Preview

##### Bug Fixes

 * #1189268 Profile a test will crash with exception
---
#### NUnit Test Adapter for Visual Studio (Beta 5) - Version 0.95.2 - June 7, 2013

##### Bug Fixes

 * #1188000, adapter dont work with solutions with only .net 2.0/3.5 project
---
#### NUnit Test Adapter for Visual Studio (Beta 5) - Version 0.95.1 Hotfix- May 28, 2013

##### Bug Fixes

 * Hotfix for debug issue
---
#### NUnit Test Adapter for Visual Studio (Beta 5) - Version 0.95 - May 10, 2013

##### Features

 * #1174925 Add support for installing the adapter from NuGet

##### Bug Fixes

 * #1155617 Grouping by class name in VS 2012 doesn't work
 * #1165359 Exception after building Coded UI test
 * #1116747 vstest.executionengine.x86.exe does not terminate
 * #1093178 Eliminate unnecessary files from VSIX
---
#### NUnit Test Adapter for Visual Studio (Beta 4) - Version 0.94 - December 22, 2012

##### Features

 * Works with Visual Studio 2012 Update 1 as well as the RTM.
 * Supports filtering and sorting tests by Traits under Update 1.
 * Supports use of standard filter expressions when running under TFS Update 1.
 * NUnit Categories specified on the fixture class are now recognized and honored. 

##### Bug Fixes

 * 1074891 Can't test multiple assemblies referencing different NUnit versions
 * 1075893 Test execution fails if solution contains native C++ project
 * 1076012 No source information found for async test methods
 * 1087629 TestFixture Category not being recognised as traits in VS2012 update 1
 * 1091020 Adapter doesn't support TFS Build traits/test case filtering 
---
#### NUnit Test Adapter for Visual Studio (Beta 3-2) - Version 0.93.2 - November 2, 2012

##### Bug Fixes

 * 1074544 Failures in Test Discovery not reporting sufficient information 
---
#### NUnit Test Adapter for Visual Studio (Beta 3-1) - Version 0.93.1 - October 26, 2012

##### Bug Fixes

 * 1072150 NUnit adapter 0.93 won't run selected tests 
---
#### NUnit Test Adapter for Visual Studio (Beta 3) - Version 0.93 - October 24, 2012

##### Features

 * Works with Visual Studio 2012 RTM. Some features require the November CTP update.
 * The adapter now uses NUnit 2.6.2. Among other things, this allows us to support async test methods. See the NUnit Release Notes for more info.
 * Source file and line number can now be found for test cases that have an alternate name set.
 * Console output from tests is now displayed in the Visual Studio Output window.
 * TestFixtureSetUp and TestFixtureTearDown errors are now displayed in the Output window.
 * The caret line (------^) is no longer displayed in the IDE since it depends on use of a fixed font.
 * Tests may now be grouped and filtered by Category (only under the November CTP update for VS2012). 

##### Bug Fixes

 * 1021144 Text output from tests not displayed in Visual Studio IDE
 * 1033623 Not possible to include or exclude tests based on [Category] attribute Released
 * 1040779 Null reference exception on generic test fixtures
 * 1064620 Support async test methods
 * 1065209 Should call both RecordEnd and RecordResult at end of a test
 * 1065212 Upgrade NUnit to 2.6.2
 * 1065223 Error messages assume a fixed font, but don't get one
 * 1065225 No display for TestFixtureSetUp/TearDown or SetUpFixture errors
 * 1065254 Cannot open test from Test Explorer for tests in a Parameterized Test Fixture
 * 1065306 Generic Fixtures aren't discovered.
 * 1066393 Unable to display source for test cases with an alternate name set
 * 1066518 Executed fast test appears in Not Run category in Test Explorer 
---
#### NUnit Test Adapter for Visual Studio (Beta 2) - Version 0.92 - May 3, 2012

##### Features

 * Works with Visual Studio 2012 Release Candidate
 * Uses NUnit 2.6 

##### Bug Fixes

 * 992837 Unable to Debug using VS Test Adapter
 * 994146 Can't run tests under .NET 2.0/3.5 
---
#### NUnit Test Adapter for Visual Studio (Beta 1) - Version 0.91 - February 29, 2012

##### Features

 * Built against Visual Studio 11 Beta 1
 * Uses NUnit 2.6 
---
#### NUnit Test Adapter for Visual Studio (Alpha) - Version 0.90 - February 21, 2012

##### Features

 * First release of the test adapter. Compatible with the Visual Studio 11 Developer Preview.
 * Uses NUnit 2.6. 
