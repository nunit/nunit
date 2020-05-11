# Framework Release

## NUnit 3.12 - May 14, 2019

This release of NUnit finally drops support for .NET 2.0. If your application still
targets .NET 2.0, your tests will need to target at least .NET 3.5. Microsoft ended
support for .NET 2.0 on July 12, 2011. Microsoft recommends that everyone migrate
to at least .NET Framework 3.5 SP1 for security and performance fixes.

This release dramatically improves NUnit support for async tests including returning
ValueTask and custom tasks from tests, improved handling of SynchronizationContexts
and better exception handling.

The .NET Standard 2.0 version of NUnit continues to gain more functionality that
is found in the .NET 4.5 version of the framework like setting the ApartmentState
and enabling Timeout on tests.

### Issues Resolved

* [474](https://github.com/nunit/nunit/issues/474) TypeHelperTests.cs is orphaned
* [999](https://github.com/nunit/nunit/issues/999) Support multiple TestOf attributes per test
* [1638](https://github.com/nunit/nunit/issues/1638) TimeoutAttribute not available when targeting netcoreapp framework
* [2168](https://github.com/nunit/nunit/issues/2168) ThrowsAsync reports OperationCanceledException as TaskCanceledException
* [2194](https://github.com/nunit/nunit/issues/2194) How to use `Contains.Substring` with `And`
* [2286](https://github.com/nunit/nunit/issues/2286) Add support for custom Task (i.e. ValueTask)
* [2579](https://github.com/nunit/nunit/issues/2579) AppVeyor Test Failures under .NET 3.5
* [2614](https://github.com/nunit/nunit/issues/2614) TestExecutionContext.CurrentContext is saved in Remoting CallContext between test runs
* [2696](https://github.com/nunit/nunit/issues/2696) Getting WorkerId fails in debug
* [2772](https://github.com/nunit/nunit/issues/2772) Random failing of parallel test run: Unhandled Exception: System.InvalidOperationException: Stack empty.
* [2975](https://github.com/nunit/nunit/issues/2975) ComparisonConstraints are allocating string on construction
* [3014](https://github.com/nunit/nunit/issues/3014) Timeout failures on MacOS
* [3023](https://github.com/nunit/nunit/issues/3023) NUnit runner fails when test method returns ValueTask<>
* [3035](https://github.com/nunit/nunit/issues/3035) Apartment state can't be used for .NET Standard 2.0 tests
* [3036](https://github.com/nunit/nunit/issues/3036) Apartment state can't be used for .NET Standard 2.0 tests
* [3038](https://github.com/nunit/nunit/issues/3038) TestName in TestCase attribute not validated to be not empty
* [3042](https://github.com/nunit/nunit/issues/3042) RequiresThreadAttribute allows ApartmentState.Unknown, unlike ApartmentAttribute
* [3048](https://github.com/nunit/nunit/issues/3048) Add .idea folder to .gitignore
* [3053](https://github.com/nunit/nunit/issues/3053) Conversion from TestCase string parameter to DateTimeOffset
* [3059](https://github.com/nunit/nunit/issues/3059) Constraint Throws.Exception does not work with async return value
* [3068](https://github.com/nunit/nunit/issues/3068) First Chance Exception in RuntimeFramework
* [3070](https://github.com/nunit/nunit/issues/3070) End support for .NET Framework 2.0 (released in 2005)
* [3073](https://github.com/nunit/nunit/issues/3073) CollectionAssert.AreEquivalent fails for ValueTuple Wrapped Dictionary
* [3079](https://github.com/nunit/nunit/issues/3079) Regression from 3.10 to 3.11: Range in bytes
* [3082](https://github.com/nunit/nunit/issues/3082) Is.Ordered.By
* [3085](https://github.com/nunit/nunit/issues/3085) XML Test-Suite Assembly does not contain DLL path anymore
* [3089](https://github.com/nunit/nunit/issues/3089) Remove outdated comment
* [3093](https://github.com/nunit/nunit/issues/3093) Tests having TaskLike objects as their return type throws Exception
* [3094](https://github.com/nunit/nunit/issues/3094) Bad error message if collections have different types
* [3104](https://github.com/nunit/nunit/issues/3104) Removed NET20 compile output
* [3105](https://github.com/nunit/nunit/issues/3105) Add tests for use of ApartmentState.Unknown in RequiresThreadAttribute
* [3107](https://github.com/nunit/nunit/issues/3107) Declare class in Program.cs provided with NUnitLite Nuget package static
* [3109](https://github.com/nunit/nunit/issues/3109) Azure DevOps build fails in Save package artifacts
* [3124](https://github.com/nunit/nunit/issues/3124) Switch copyright notice
* [3128](https://github.com/nunit/nunit/issues/3128) Correct documentation on ParallelScope
* [3137](https://github.com/nunit/nunit/issues/3137) Fix doc-comments in NUnitTestAssemblyRunner
* [3138](https://github.com/nunit/nunit/issues/3138) Assert.Ignore breaks when a Task is returned w/o using async/await
* [3139](https://github.com/nunit/nunit/issues/3139)  Add Azure pipelines badge to frontpage
* [3144](https://github.com/nunit/nunit/issues/3144) Retry attribute should not derive from PropertyAttribute
* [3145](https://github.com/nunit/nunit/issues/3145) Capture additional exception details in the test output
* [3156](https://github.com/nunit/nunit/issues/3156) UnexpectedExceptionTests should tolerate Mono on Azure DevOps Ubuntu
* [3159](https://github.com/nunit/nunit/issues/3159) Make tests more tolerant
* [3161](https://github.com/nunit/nunit/issues/3161) https url repo
* [3166](https://github.com/nunit/nunit/issues/3166) Allow static SetUpFixture classes
* [3171](https://github.com/nunit/nunit/issues/3171) Incorrect type for Test Fixtures when using running explore with a filter
* [3175](https://github.com/nunit/nunit/issues/3175) Improve user-facing messages
* [3181](https://github.com/nunit/nunit/issues/3181) Template Based Test Naming - Incorrect truncation for individual arguments
* [3186](https://github.com/nunit/nunit/issues/3186) Fix licenseUrl element in nuspec, will be deprecated
* [3193](https://github.com/nunit/nunit/issues/3193) Cake Build Fails with Visual Studio 2019
* [3195](https://github.com/nunit/nunit/issues/3195) Drop or at least make Travis not required?
* [3231](https://github.com/nunit/nunit/issues/3231) Breaking change in filter functionality between framework 2.7 and 3.11
* [3209](https://github.com/nunit/nunit/issues/3209) Test fail when posting to SynchronizationContext.Current
* [3211](https://github.com/nunit/nunit/issues/3211) Fix logging
* [3218](https://github.com/nunit/nunit/issues/3218) Remove todos from the code base
* [3222](https://github.com/nunit/nunit/issues/3222) Our build script tests hang when run with Mono on Windows
* [3233](https://github.com/nunit/nunit/issues/3233) AndConstraint should write additional information from failed constraint

## NUnit 3.11 - October 6, 2018

* More informative assertion messages
* PlatformAttribute is available on and now detects .NET Core
* ValuesAttribute now works with nullable types
* Async tests detecting and running Windows Forms or WPF message pumps rather than deadlocking
* Support for UWP 10.0 is back via .NET Standard 1.4

### Issues Resolved

* [352](https://github.com/nunit/nunit/issues/352) Test with infinite loop in TearDown cannot be aborted
* [452](https://github.com/nunit/nunit/issues/452) Deprecate the existing Chocolatey framework package
* [660](https://github.com/nunit/nunit/issues/660) Order dependence of And and Or constraints should be documented
* [1200](https://github.com/nunit/nunit/issues/1200) async test + Apartment(ApartmentState.STA) => await not returning on STA thread
* [2123](https://github.com/nunit/nunit/issues/2123) Task.Run inside a test will result in deadlock if a control was created previously
* [2146](https://github.com/nunit/nunit/issues/2146) Assert.That with a Throws constraint does not provide as much info as Assert.Throws
* [2427](https://github.com/nunit/nunit/issues/2427) PropertyConstraint throws away the more helpful message in the base constraint result
* [2432](https://github.com/nunit/nunit/issues/2432) Ability to exclude/include the platform .NET Core
* [2450](https://github.com/nunit/nunit/issues/2450) NullReferenceException in ExceptionHelper.BuildMessage on Mono
* [2536](https://github.com/nunit/nunit/issues/2536) SetArgDisplayNames for TestCaseData and TestFixtureData
* [2611](https://github.com/nunit/nunit/issues/2611) Enable .NET Standard 1.6 tests on non-Windows
* [2693](https://github.com/nunit/nunit/issues/2693) Ensure that resharper settings are consistent with the editorconfig configuration
* [2757](https://github.com/nunit/nunit/issues/2757) Broken `char` comparison in v3.7 and higher
* [2759](https://github.com/nunit/nunit/issues/2759) Test fails with "No arguments were provided" error when no values returned from IParameterDataSource
* [2761](https://github.com/nunit/nunit/issues/2761) Infinite loop in nunit 3.9
* [2781](https://github.com/nunit/nunit/issues/2781) Fixed precompiler typo
* [2786](https://github.com/nunit/nunit/issues/2786) Timeout value not resetting on Retry of failed test
* [2790](https://github.com/nunit/nunit/issues/2790) Removing ITypeInfo abstraction
* [2798](https://github.com/nunit/nunit/issues/2798) [Request] Show actual count value when test fail on Has.Exactly(x).Items
* [2814](https://github.com/nunit/nunit/issues/2814) Remove public marker types
* [2819](https://github.com/nunit/nunit/issues/2819) Only run AppVeyor PR build against open PRs
* [2821](https://github.com/nunit/nunit/issues/2821) Save and restore the SynchronizationContext before and after each test case
* [2823](https://github.com/nunit/nunit/issues/2823) SetUp failed for test fixture - Array was not a one-dimensional array.  Issue seems related to byte[,] method parameters
* [2829](https://github.com/nunit/nunit/issues/2829) Obsoletion warning for DataAttribute
* [2831](https://github.com/nunit/nunit/issues/2831) Regular "BusyExecIdle after 200 milliseconds delay" CI failures
* [2833](https://github.com/nunit/nunit/issues/2833)  Use longer BusyExecIdle to avoid CI failures
* [2836](https://github.com/nunit/nunit/issues/2836) NUnit.Framework.Does cannot be extended
* [2837](https://github.com/nunit/nunit/issues/2837) DictionaryContainsKeyConstraint behaviour is inconstant with Dictionary.ContainsKey when the dictionary uses a custom Comparer
* [2842](https://github.com/nunit/nunit/issues/2842) Supporting inheritance of Assert and related classes
* [2854](https://github.com/nunit/nunit/issues/2854) Has.All.../Has.None... - show non-matching items in error message
* [2863](https://github.com/nunit/nunit/issues/2863) Make tests robust without depending on the order of attributes
* [2867](https://github.com/nunit/nunit/issues/2867) Skip executing TestCaseSources for tests which are not included in the filter
* [2876](https://github.com/nunit/nunit/issues/2876) Implement Discovery-time filtering for NUnitLite
* [2883](https://github.com/nunit/nunit/issues/2883) Our public ConcurrentQueue causes type conflicts
* [2885](https://github.com/nunit/nunit/issues/2885) Copy/paste error in Assert.That documentation
* [2887](https://github.com/nunit/nunit/issues/2887) NETStandard 1.3 support dropped in NUnit 3.10  
* [2896](https://github.com/nunit/nunit/issues/2896) Some tests are silently skipped on netstandard1.x since #2796
* [2898](https://github.com/nunit/nunit/issues/2898) AssemblyPath contains invalid charaters
* [2901](https://github.com/nunit/nunit/issues/2901) Values attribute support for nullable bool and enum types
* [2923](https://github.com/nunit/nunit/issues/2923) Update outdated CategoryAttribute xmldoc
* [2928](https://github.com/nunit/nunit/issues/2928) Improve error message on EmptyConstraint
* [2929](https://github.com/nunit/nunit/issues/2929) Added NUnit XML schemas
* [2940](https://github.com/nunit/nunit/issues/2940) Increase StackTracesAreFiltered amount to 5
* [2955](https://github.com/nunit/nunit/issues/2955) Potential threading issue in IsolatedContext
* [2965](https://github.com/nunit/nunit/issues/2965) NuGet Package : Add `repository` metadata.
* [2970](https://github.com/nunit/nunit/issues/2970) InvalidCastException @ NUnit.Framework.TestFixtureSourceAttribute.BuildFrom
* [2979](https://github.com/nunit/nunit/issues/2979) Warn.If in Assert.Multiple
* [2994](https://github.com/nunit/nunit/issues/2994) Error in .NET Standard 1.4 DictionaryContainsKeyConstraint MetadataToken compatibility methods
* [2996](https://github.com/nunit/nunit/issues/2996) Remove unused enum
* [3009](https://github.com/nunit/nunit/issues/3009) Fix failing CI Builds by upgrading to NUnit Console 3.9.0
* [3020](https://github.com/nunit/nunit/issues/3020) Upgrade nunit-vs-adapter to 3.10 for nUnit 3.11
* [3024](https://github.com/nunit/nunit/issues/3024) Unable to add `.IgnoreCase` modifier to an `AnyOf` constraint in collection constraints
* [3032](https://github.com/nunit/nunit/issues/3032) APIs to restore before 3.11

## NUnit 3.10.1 - March 12, 2018

Added a namespace to the props file included in the NuGet package to make it
compatible with versions of Visual Studio prior to VS 2017.

## NUnit 3.10 - March 12, 2018

This release adds a .NET Standard 2.0 version of the framework which re-enables
most of the features that have been missing in our earlier .NET Standard builds
like parallelism, timeouts, directory and path based asserts, etc. It also contains
numerous bug fixes and smaller enhancements. We've improved our XML docs,
fixed performance issues and added more detail to Multiple Asserts.

This release also contains source-indexed PDB files allowing developers to debug
into the NUnit Framework. This allows you to track down errors or see how the
framework works.

In order to support the .NET Standard 2.0 version, the NUnit project switched to
the new CSPROJ format and now requires Visual Studio 2017 to compile. This only
effects people contributing to the project. NUnit still supports building and
compiling your tests in older .NET IDEs and NUnit still supports older versions
of the .NET Framework back to 2.0. For contributors, NUnit can now compile all
supported targets on Windows, Linux and Mac using the Cake command line build.

### Issues Resolved

* [1212](https://github.com/nunit/nunit/issues/1212) Separate AssertionHelper project or assembly
* [1373](https://github.com/nunit/nunit/issues/1373) Setting with a null value
* [1382](https://github.com/nunit/nunit/issues/1382) Use array argument contents in name of parameterized tests rather than just array type.
* [1578](https://github.com/nunit/nunit/issues/1578) TestContext.CurrentTest exposes too much internal info
* [1678](https://github.com/nunit/nunit/issues/1678) Result Message: OneTimeSetUp: Category name must not contain ',', '!', '+' or '-'
* [1944](https://github.com/nunit/nunit/issues/1944) Removing Compact Framework workarounds
* [1958](https://github.com/nunit/nunit/issues/1958) System.Reflection.TargetInvocationException after run finished
* [2033](https://github.com/nunit/nunit/issues/2033) Nameof refactor
* [2202](https://github.com/nunit/nunit/issues/2202) Best practices for XML doc comments
* [2325](https://github.com/nunit/nunit/issues/2325) Retry attribute doesn't retry the test.
* [2331](https://github.com/nunit/nunit/issues/2331) Repo does not build in VS without running `build -t build` first
* [2405](https://github.com/nunit/nunit/issues/2405) Improve PropertyConstraint error output
* [2421](https://github.com/nunit/nunit/issues/2421) Publishing symbols with releases
* [2494](https://github.com/nunit/nunit/issues/2494) CollectionAssert.AllItemsAreUnique() very slow
* [2515](https://github.com/nunit/nunit/issues/2515) Retarget Solution to use the New CSPROJ Format
* [2518](https://github.com/nunit/nunit/issues/2518) Bug in CollectionAssert.AreEqual for ValueTuples.
* [2530](https://github.com/nunit/nunit/issues/2530) Running tests on main thread. Revisiting #2483
* [2542](https://github.com/nunit/nunit/issues/2542) NUnit does not support parallelism on .NET Core 2.0
* [2555](https://github.com/nunit/nunit/issues/2555) CI timeout: NUnit.Framework.Assertions.CollectionAssertTest.PerformanceTests
* [2564](https://github.com/nunit/nunit/issues/2564) Add minClientVersion to .nuspec files
* [2566](https://github.com/nunit/nunit/issues/2566) Refactor `SimpleEnumerableWithIEquatable` test object
* [2577](https://github.com/nunit/nunit/issues/2577) Warning in TearDown is inconsistent with Assertion failure
* [2580](https://github.com/nunit/nunit/issues/2580) Remove unused defines
* [2591](https://github.com/nunit/nunit/issues/2591) NUnitEqualityComparer.Default should be replaced with new NUnitEqualityComparer()
* [2592](https://github.com/nunit/nunit/issues/2592) Add .props with ProjectCapability to suppress test project service GUID item
* [2608](https://github.com/nunit/nunit/issues/2608) Culture differences on .NET Core on non-Windows causes test failures
* [2622](https://github.com/nunit/nunit/issues/2622) Fix flakey test
* [2624](https://github.com/nunit/nunit/issues/2624) Prevent emails for successful builds on Travis
* [2626](https://github.com/nunit/nunit/issues/2626) SetUp/TearDown methods are invoked multiple times before/after test in .NET Standard targeted projects
* [2627](https://github.com/nunit/nunit/issues/2627) Breaking change in CollectionAssert.AllItemsAreUnique with NUnit 3.9
* [2628](https://github.com/nunit/nunit/issues/2628) Error during installing tools when running build script
* [2630](https://github.com/nunit/nunit/issues/2630) Framework throws NullReferenceException if test parameter is marked with [Values(null)]
* [2632](https://github.com/nunit/nunit/issues/2632) Parallel tests are loading 100% CPU when nested SetUpFixture exists
* [2639](https://github.com/nunit/nunit/issues/2639) ValuesAttribute causes ExpectedResult to have no effect
* [2647](https://github.com/nunit/nunit/issues/2647) Add Current Attempt indicator in TestContext for use with RetryAttribute
* [2654](https://github.com/nunit/nunit/issues/2654) Address feedback from @oznetmaster
* [2656](https://github.com/nunit/nunit/issues/2656) NuGet package links to outdated license
* [2659](https://github.com/nunit/nunit/issues/2659) Naming Errors
* [2662](https://github.com/nunit/nunit/issues/2662) NullReferenceException after parallel tests have finished executing
* [2663](https://github.com/nunit/nunit/issues/2663) Building NUnit .NET 4.5 in VS2017 fails
* [2669](https://github.com/nunit/nunit/issues/2669) Removed vestigial build script helper method
* [2670](https://github.com/nunit/nunit/issues/2670) Invalid assemblies no longer give an error message
* [2671](https://github.com/nunit/nunit/issues/2671) Ensure that FailureSite.Child is used where appropriate.
* [2685](https://github.com/nunit/nunit/issues/2685) Remove Rebracer file
* [2688](https://github.com/nunit/nunit/issues/2688) Assert.Throws swallows console output
* [2695](https://github.com/nunit/nunit/issues/2695) MultipleAssertException doesn't provide proper details on failures
* [2698](https://github.com/nunit/nunit/issues/2698) Syntax suggestions errors as warnings
* [2704](https://github.com/nunit/nunit/issues/2704) Add Constraint to test whether actual item is contained in expected collection
* [2711](https://github.com/nunit/nunit/issues/2711) NUnitLite: Add support for --nocolor option
* [2714](https://github.com/nunit/nunit/issues/2714) AnyOfConstraint enumerates multiple times
* [2725](https://github.com/nunit/nunit/issues/2725) Enable 'strict' compilation flag
* [2726](https://github.com/nunit/nunit/issues/2726) Replace the ConcurrentQueue and SpinWait compatibility classes
* [2727](https://github.com/nunit/nunit/issues/2727) Avoid treating warnings as errors inside the IDE
* [2734](https://github.com/nunit/nunit/issues/2734) TestCaseAttribute: ExpectedResult should support same value conversion as normal method arguments
* [2742](https://github.com/nunit/nunit/issues/2742) FailureSite not correctly set on containing suites when tests are ignored.
* [2749](https://github.com/nunit/nunit/issues/2749) Update Travis SDK versions

## NUnit 3.9 - November 10, 2017

This release addresses numerous parallelization issues that were introduced in 3.8 when method level parallelization was added. Most of the parallelization issues resolved were tests never completing when using some combinations of parallel tests and `ApartmentState` not being properly applied to tests in all cases.

### Issues Resolved

* [893](https://github.com/nunit/nunit/issues/893) Inconsistent Tuple behavior.
* [1239](https://github.com/nunit/nunit/issues/1239) NUnit3 sometimes hangs if SetUpFixtures are run in parallel
* [1346](https://github.com/nunit/nunit/issues/1346) NullReferenceException when [TestFixtureSource] refers to data in a generic class.
* [1473](https://github.com/nunit/nunit/issues/1473) Allow Is.Ordered to Compare Null Values
* [1899](https://github.com/nunit/nunit/issues/1899) Constraint Throws.Exception does not catch exception with async lambdas
* [1905](https://github.com/nunit/nunit/issues/1905) SetupFixture without namespace will make assembly-level Parallelable attribute useless
* [2091](https://github.com/nunit/nunit/issues/2091) When a native exception of corrupted state is thrown, nunit test thread crashes and the nunit-console process hangs
* [2102](https://github.com/nunit/nunit/issues/2102) NUnitLite incorrectly reports Win 10 OS name
* [2271](https://github.com/nunit/nunit/issues/2271) When CollectionAssert.AreEqual do compare each element, it will ignore the IEquatable of the element too
* [2289](https://github.com/nunit/nunit/issues/2289) ResolveTypeNameDifference does not handle generic types well
* [2311](https://github.com/nunit/nunit/issues/2311) Resolve test projects' namespace situation
* [2319](https://github.com/nunit/nunit/issues/2319) Add .editorconfig to set file encodings so that people don't have to think about it
* [2364](https://github.com/nunit/nunit/issues/2364) Parallelizable attribute not invalidating invalid parallel scope combinations
* [2372](https://github.com/nunit/nunit/issues/2372) Create testing for compounded ConstraintFilters
* [2388](https://github.com/nunit/nunit/issues/2388) Parallelization causes test cases to stop respecting fixture's apartment state
* [2395](https://github.com/nunit/nunit/issues/2395) NUnit 3.8+ does not finish running tests
* [2398](https://github.com/nunit/nunit/issues/2398) NUnit CI spurious failures, NUnit.Framework.Internal.ThreadUtilityTests.Kill
* [2402](https://github.com/nunit/nunit/issues/2402) --labels=All doesn't show anything in console output executing NUnitLite Console Runner
* [2406](https://github.com/nunit/nunit/issues/2406) Summary descriptions replaced by more detailed ones
* [2411](https://github.com/nunit/nunit/issues/2411) And constraint on Has.Member throws
* [2412](https://github.com/nunit/nunit/issues/2412) Using fluent syntax unintentionally removed in 3.8
* [2418](https://github.com/nunit/nunit/issues/2418) Support equality comparison delegate
* [2422](https://github.com/nunit/nunit/issues/2422) Has.Property causes AmbiguousMatchException for shadowing properties
* [2425](https://github.com/nunit/nunit/issues/2425) XML doc typo fix
* [2426](https://github.com/nunit/nunit/issues/2426) Regression in 3.8.1: ApartmentAttribute no longer works when applied to an assembly
* [2428](https://github.com/nunit/nunit/issues/2428) Fix NullReferenceExceptions caused by WorkItemQueue not being thread-safe
* [2429](https://github.com/nunit/nunit/issues/2429) Stack trace shown for Assert.Warn
* [2438](https://github.com/nunit/nunit/issues/2438) [Parallelizable] hangs after a few tests
* [2441](https://github.com/nunit/nunit/issues/2441) Allows to override load-time/execution-time interfaces in built-in tests attributes
* [2446](https://github.com/nunit/nunit/issues/2446) CI failure in mono Warning tests
* [2448](https://github.com/nunit/nunit/issues/2448) Inherited Test SetUp, TearDown, etc. are not executed in .NET Core if they are not public
* [2451](https://github.com/nunit/nunit/issues/2451) Compile RegEx to improve performance
* [2454](https://github.com/nunit/nunit/issues/2454) SetUpFixture not respecting NonParallelizable tag on TestFixtures.
* [2459](https://github.com/nunit/nunit/issues/2459) [Parallelizable(ParallelScope.Children)] Unable to finish tests
* [2465](https://github.com/nunit/nunit/issues/2465) Possible wrong properties are returned by reflection in ReflectionExtensions.cs
* [2467](https://github.com/nunit/nunit/issues/2467) Test execution hangs when using [SetUpFixture] with NUnit 3.8.x
* [2469](https://github.com/nunit/nunit/issues/2469) Allow RangeAttribute to be specified multiple times for the same argument
* [2471](https://github.com/nunit/nunit/issues/2471) Parametrized testcases not running in parallel
* [2475](https://github.com/nunit/nunit/issues/2475) Framework incorrectly identifies Win 10 in xml results
* [2478](https://github.com/nunit/nunit/issues/2478) Attributes on SetUpFixture are not applied
* [2486](https://github.com/nunit/nunit/issues/2486) Message when asserting null with Is.EquivalentTo could be more helpful
* [2497](https://github.com/nunit/nunit/issues/2497) Use ConstraintUtils.RequireActual through out the codebase
* [2504](https://github.com/nunit/nunit/issues/2504) Support changing test display name on TestFixtureData
* [2508](https://github.com/nunit/nunit/issues/2508) Correct divergence from shadowed Is / Has members.
* [2516](https://github.com/nunit/nunit/issues/2516) When test writes something to the stdErr there is no guaranteed way to link a test-output event to a target test using ITestEventListener
* [2525](https://github.com/nunit/nunit/issues/2525) Remove unwanted space from comment
* [2526](https://github.com/nunit/nunit/issues/2526) SerializationException in low trust floating point equality test
* [2533](https://github.com/nunit/nunit/issues/2533) Matches`<T>(Predicate<T>)` throws ArgumentException or Fails when actual is null
* [2534](https://github.com/nunit/nunit/issues/2534) SetUpFixture causes NUnit to lock with Apartment( STA )
* [2551](https://github.com/nunit/nunit/issues/2551) CollectionItemsEqualConstraint is missing Using(Func<T, T, bool>)
* [2554](https://github.com/nunit/nunit/issues/2554) Made TestFixtureData.SetName internal for 3.9

## NUnit 3.8.1 - August 28, 2017

This release fixes two critical regressions in the 3.8 release. The first caused the console runner to crash if you are using test parameters. The second issue caused collection constraints checking for multiple items in a collection to fail.

### Issues Resolved

* [2386](https://github.com/nunit/nunit/issues/2386) Contains.Item() fails for collections in NUnit 3.8
* [2390](https://github.com/nunit/nunit/issues/2390) Missing value attribute in test parameters setting causes NullReferenceException in console

## NUnit 3.8 - August 27, 2017

This release removes several methods and attributes that were marked obsolete in the
original 3.0 release. Support for iOS and Android has been improved.

An issue that caused unit tests to run slower was addressed as was a bug that prevented
the use of Assert.Multiple in async code.

The Order attribute can now also be applied to the class level to set the order
that test fixtures will be run.

### Issues Resolved

* [345](https://github.com/nunit/nunit/issues/345) Order of Fixture Execution
* [1151](https://github.com/nunit/nunit/issues/1151) Include differences in output for Is.EquivalentTo
* [1324](https://github.com/nunit/nunit/issues/1324) Remove CollectionContainsConstraint
* [1670](https://github.com/nunit/nunit/issues/1670) Attaching files to the test result
* [1674](https://github.com/nunit/nunit/issues/1674) InRange-Constraint must work with object
* [1851](https://github.com/nunit/nunit/issues/1851) TestCaseSource unable to pass one element byte array
* [1996](https://github.com/nunit/nunit/issues/1996) Timeout does not work if native code is running at the time
* [2004](https://github.com/nunit/nunit/issues/2004) Has.One as synonym for Has.Exactly(1).Items
* [2062](https://github.com/nunit/nunit/issues/2062) TestCaseSource attribute causes test to pass when source is not defined
* [2144](https://github.com/nunit/nunit/issues/2144) Allow option on RandomAttribute to produce distinct values
* [2179](https://github.com/nunit/nunit/issues/2179) Some NUnit project's tests fail on systems with CultureInfo other than en
* [2195](https://github.com/nunit/nunit/issues/2195) Contains.Substring with custom StringComparison
* [2196](https://github.com/nunit/nunit/issues/2196) Expose ParallelizableAttribute (and other attribute) constructor arguments as properties
* [2201](https://github.com/nunit/nunit/issues/2201) Invalid platform name passed to PlatformAttribute should mark test NotRunnable
* [2208](https://github.com/nunit/nunit/issues/2208) StackFIlter trims leading spaces from each line
* [2213](https://github.com/nunit/nunit/issues/2213) SetCultureAttribute: CultureInfo ctor should use default culture settings
* [2217](https://github.com/nunit/nunit/issues/2217) Console runner performance varies wildly depending on environmental characteristics
* [2219](https://github.com/nunit/nunit/issues/2219) Remove Obsolete Attributes
* [2225](https://github.com/nunit/nunit/issues/2225) OneTimeTearDown and Dispose Ordering
* [2237](https://github.com/nunit/nunit/issues/2237) System.Runtime.Loader not available for iOS/Android
* [2242](https://github.com/nunit/nunit/issues/2242) Running tests directly should never surface a NullReferenceException
* [2244](https://github.com/nunit/nunit/issues/2244) Add KeyValuePair<TKey, TValue> to the default formatters
* [2251](https://github.com/nunit/nunit/issues/2251) Randomizer.NextGuid()
* [2253](https://github.com/nunit/nunit/issues/2253) Parallelizable(ParallelScope.Fixtures) doesn't work on a TestFixture
* [2254](https://github.com/nunit/nunit/issues/2254) EqualTo on ValueTuple with Nullable unexpected
* [2261](https://github.com/nunit/nunit/issues/2261) When an assembly is marked with ParallelScope.None and there are Parallelizable tests Nunit hangs
* [2269](https://github.com/nunit/nunit/issues/2269) Parallelizable and NonParallelizable attributes on setup and teardown silently ignored
* [2276](https://github.com/nunit/nunit/issues/2276) Intermittent test failures in Travic CI: TestContextTests
* [2281](https://github.com/nunit/nunit/issues/2281) Add type constraint for Throws and any method requiring Exception
* [2288](https://github.com/nunit/nunit/issues/2288) Killing thread cancels test run
* [2292](https://github.com/nunit/nunit/issues/2292) Is.Ordered.By() with a field throws NullReferenceException
* [2298](https://github.com/nunit/nunit/issues/2298) Write TestParametersDictionary to xml result file in readable format
* [2299](https://github.com/nunit/nunit/issues/2299) NUnitLite NuGet package no longer installs NUnit NuGet package
* [2304](https://github.com/nunit/nunit/issues/2304) Revert accidental doc removal
* [2305](https://github.com/nunit/nunit/issues/2305) Correct misprint ".con" -> ".com"
* [2312](https://github.com/nunit/nunit/issues/2312) Prevent crash on invalid --result parsing in NUnitLite
* [2313](https://github.com/nunit/nunit/issues/2313) Incorrect xmldoc on RetryAttribute
* [2332](https://github.com/nunit/nunit/issues/2332) Update build script to use NUnitConsoleRunner v3.7.0
* [2335](https://github.com/nunit/nunit/issues/2335) Execute OneTimeTearDown as early as possible when running fixtures in parallel
* [2342](https://github.com/nunit/nunit/issues/2342) Remove deprecated Is.String* Constraints
* [2348](https://github.com/nunit/nunit/issues/2348) Can't use Assert.Multiple with async code
* [2353](https://github.com/nunit/nunit/issues/2353) Provide additional Result information through TestContext
* [2358](https://github.com/nunit/nunit/issues/2358) Get framework to build under Mono 5.0
* [2360](https://github.com/nunit/nunit/issues/2360) Obsolete CollectionContainsConstraint Constructors
* [2361](https://github.com/nunit/nunit/issues/2361) NUnit Parallelizable and OneTimeSetUp with no namespace results in single-threaded test execution
* [2370](https://github.com/nunit/nunit/issues/2370) TestCaseAttribute can't convert int to nullable long

## NUnit 3.7.1 - June 6, 2017

This is a hotfix release that addresses occasional hangs when using test parallelization and fixes crashes in NCrunch prior to version 3.9.

### Issues Resolved

* 2205 NCrunch: System.Xml.XmlException: Root element is missing, when adding NUnit 3.7.0
* 2209 NUnit occasionally hangs when parallelizable TestFixture has OneTimeSetUp and OneTimeTearDown

## NUnit 3.7 - May 29, 2017

This release of NUnit expands on parallel test execution to allow test methods to
be run in parallel. Please see the [Parallelizable Attribute](https://github.com/nunit/docs/wiki/Parallelizable-Attribute)
for more information.

NUnit 3.7 also drops the Portable build of the framework and replaces it with a
.NET Standard 1.3 version to compliment the .NET Standard 1.6 version. This change
enables several constraints and other features in the .NET Standard builds that
weren't available in portable like Path and Directory based asserts.

The AssertionHelper class has been deprecated because it is seldom used and has
not received any of the updates that Asserts and Constraints receive. If your code
is using the AssertionHelper class, we recommend that you migrate your asserts.

### Issues Resolved

* [164](https://github.com/nunit/nunit/issues/164) Run test methods within a fixture in parallel
* [391](https://github.com/nunit/nunit/issues/391) Multiple Assertions
* [652](https://github.com/nunit/nunit/issues/652) Add ability to execute test actions before SetUp or OneTimeSetUp
* [1000](https://github.com/nunit/nunit/issues/1000) Support multiple Author attributes per test
* [1096](https://github.com/nunit/nunit/issues/1096) Treat OneTimeSetup and OneTimeTearDown as separate work items
* [1143](https://github.com/nunit/nunit/issues/1143) NUnitLite - Explore flag does not apply where filter to output
* [1238](https://github.com/nunit/nunit/issues/1238) Feature request: Print LoaderExceptions when fixture loading fails
* [1363](https://github.com/nunit/nunit/issues/1363) Make Timeouts work without running test on its own thread
* [1474](https://github.com/nunit/nunit/issues/1474) Several SetUpFixtures at the same level may be active at the same time
* [1819](https://github.com/nunit/nunit/issues/1819) TestContext.Progress.Write writes new line
* [1830](https://github.com/nunit/nunit/issues/1830) Add --labels switch changes to NUnitLite and NUnitLite tests
* [1859](https://github.com/nunit/nunit/issues/1859) ConcurrentQueue is duplicate with System.Threading.dll package
* [1877](https://github.com/nunit/nunit/issues/1877) Resolve differences between NUnit Console and NUnitLite implementations of @filename
* [1885](https://github.com/nunit/nunit/issues/1885) Test parameter containing a semicolon
* [1896](https://github.com/nunit/nunit/issues/1896) Test has passed however Reason with an empty message is printed in the xml
* [1918](https://github.com/nunit/nunit/issues/1918) Changing DefaultFloatingPointTolerance breaks tests running in parallel
* [1932](https://github.com/nunit/nunit/issues/1932) NUnit Warn class should be removed from stack trace by filter
* [1934](https://github.com/nunit/nunit/issues/1934) NullReferenceException when null arguments are used in TestFixtureAttribute
* [1952](https://github.com/nunit/nunit/issues/1952) TestContext.Out null when used in task with .NET Core
* [1963](https://github.com/nunit/nunit/issues/1963) Investigate removing SpecialValue
* [1965](https://github.com/nunit/nunit/issues/1965) TestContext does not flow in async method
* [1971](https://github.com/nunit/nunit/issues/1971) Switch CHANGES.txt to Markdown
* [1973](https://github.com/nunit/nunit/issues/1973) Implemented TestExecutionContext to use AsyncLocal<> for NETSTANDARD1_6
* [1975](https://github.com/nunit/nunit/issues/1975) TestFixtureSource doesn't work with a class that has no namespace
* [1983](https://github.com/nunit/nunit/issues/1983) Add missing ConstraintExpression.Contain overload
* [1990](https://github.com/nunit/nunit/issues/1990) Add namespace filter
* [1997](https://github.com/nunit/nunit/issues/1997) Remove unused --verbose and --full command line options
* [1999](https://github.com/nunit/nunit/issues/1999) Author Tests assume ICustomAttributeProvider.GetCustomAttributes return order is defined
* [2003](https://github.com/nunit/nunit/issues/2003) Better user info about ParallelizableAttribute and ParallelScope
* [2005](https://github.com/nunit/nunit/issues/2005) Exclude empty failure messages from results xml
* [2007](https://github.com/nunit/nunit/issues/2007) 3.6 Multiple assertion backwards compatibility
* [2010](https://github.com/nunit/nunit/issues/2010) Add DelayedConstraint in NetStandard 1.6 build
* [2020](https://github.com/nunit/nunit/issues/2020) Better message when timeout fails
* [2023](https://github.com/nunit/nunit/issues/2023) Ability to abort threads running a message pump
* [2025](https://github.com/nunit/nunit/issues/2025) NullReferenceException using Is.EqualTo on two unequal strings
* [2030](https://github.com/nunit/nunit/issues/2030) Add method to mark tests as invalid with a reason
* [2031](https://github.com/nunit/nunit/issues/2031) Limit Language level to C#6
* [2034](https://github.com/nunit/nunit/issues/2034) Remove SilverLight project - no longer used
* [2035](https://github.com/nunit/nunit/issues/2035) NullReferenceException inside failing Assert.That call
* [2040](https://github.com/nunit/nunit/issues/2040) Cannot catch AssertionException
* [2045](https://github.com/nunit/nunit/issues/2045) NUnitLite-runner crashes if no file is provided
* [2050](https://github.com/nunit/nunit/issues/2050) Creation of TestExecutionContext should be explicit
* [2052](https://github.com/nunit/nunit/issues/2052) NullReferenceException with TestCaseSource if a property has no setter
* [2061](https://github.com/nunit/nunit/issues/2061) TestContext.WorkDirectory not initialized during build process
* [2079](https://github.com/nunit/nunit/issues/2079) Make TestMethod.Arguments public or otherwise accessible (e.g. TestContext)
* [2080](https://github.com/nunit/nunit/issues/2080) Allow comments in @FILE files
* [2087](https://github.com/nunit/nunit/issues/2087) Enhance error message: Test is not runnable in single-threaded context. Timeout
* [2092](https://github.com/nunit/nunit/issues/2092) Convert Portable library to .NET Standard 1.3
* [2095](https://github.com/nunit/nunit/issues/2095) Extend use of tolerance to ComparisonConstraints
* [2099](https://github.com/nunit/nunit/issues/2099) Include type in start-suite/start-test report elements
* [2110](https://github.com/nunit/nunit/issues/2110) NullReferenceException when getting TestDirectory from TestContext
* [2115](https://github.com/nunit/nunit/issues/2115) Mark AssertionHelper as Obsolete
* [2121](https://github.com/nunit/nunit/issues/2121) Chained PropertyConstraint constraints report incorrect ActualValue
* [2131](https://github.com/nunit/nunit/issues/2131) Remove "Version 3" suffix from NUnitLite NuGet Package
* [2132](https://github.com/nunit/nunit/issues/2132) TestFixtureTests.CapturesArgumentsForConstructorWithMultipleArgsSupplied assumes order of custom attributes
* [2143](https://github.com/nunit/nunit/issues/2143) Non-parallel fixture with parallel children runs in parallel with other fixtures
* [2147](https://github.com/nunit/nunit/issues/2147) Test Assembly using NUnitLite & NUnit 3.6.1 hangs under .NET Core when `--timeout` is supplied on command line
* [2150](https://github.com/nunit/nunit/issues/2150) Add portable-slow-tests to Cake file
* [2152](https://github.com/nunit/nunit/issues/2152) Allow attaching files to TestResults
* [2154](https://github.com/nunit/nunit/issues/2154) Fix execution of non-parallel test fixtures
* [2157](https://github.com/nunit/nunit/issues/2157) Getting WorkerId inside Assert.Throws / DoesNotThrow returns null instead of previous non-null value
* [2158](https://github.com/nunit/nunit/issues/2158) Update SetupFixtureAttribute XML Docs
* [2159](https://github.com/nunit/nunit/issues/2159) Prevent crash in .NET standard with log file path
* [2165](https://github.com/nunit/nunit/issues/2165) Trying to install NUnit 3.6.1 on .NET Framework asks for download of 20 more packages
* [2169](https://github.com/nunit/nunit/issues/2169) Incorrect xmldocs for SetUpAttribute
* [2170](https://github.com/nunit/nunit/issues/2170) Cake build fails if only Visual Studio 2017 installed
* [2173](https://github.com/nunit/nunit/issues/2173) Remove PreTestAttribute and PostTestAttribute
* [2186](https://github.com/nunit/nunit/issues/2186) Replace special characters as part of converting branch names to package versions
* [2191](https://github.com/nunit/nunit/issues/2191) System.Reflection.TargetInvocationException with nunit3-console --debug on Mono

## NUnit 3.6.1 - February 26, 2017

This is a hotfix release of the framework that addresses critical issues found in
the 3.6 release.

### Issues Resolved

* [1962](https://github.com/nunit/nunit/issues/1962) A Theory with no data passes
* [1986](https://github.com/nunit/nunit/issues/1986) NUnitLite ignores --workers option
* [1994](https://github.com/nunit/nunit/issues/1994) NUnitLite runner crashing when --trace is specified
* [2017](https://github.com/nunit/nunit/issues/2017) Two NUnit project's tests fail on systems with comma decimal mark settings
* [2043](https://github.com/nunit/nunit/issues/2043) Regression in 3.6.0 when catching AssertionException

## NUnit 3.6 - January 9, 2017

This release of the framework no longer includes builds for Compact Framework or for SilverLight, but adds a .NET Standard 1.6 build. If anyone still using Compact Framework or SilverLight and would like to continue development on those versions of the framework, please contact the NUnit team.

### Framework

* .NET Standard 1.6 is now supported
* Adds support for Multiple Assert blocks
* Added the --params option to NUnitLite
* Theories now support Nullable enums
* Improved assert error messages to help differentiate differences in values
* Added warnings with Warn.If(), Warn.Unless() and Assert.Warn()
* Enabled Path, File and Directory Asserts/Constraints for .NET Core testing
* Added NonTestAssemblyAttribute for use by third-party developers to indicate that their assemblies reference the NUnit framework, but do not contain tests

### Issues Resolved

* [406](https://github.com/nunit/nunit/issues/406) Warning-level Assertions
* [890](https://github.com/nunit/nunit/issues/890) Allow file references anywhere in the command line.
* [1380](https://github.com/nunit/nunit/issues/1380) AppVeyor Failures when branch name is too long
* [1589](https://github.com/nunit/nunit/issues/1589) Split the nunit repository into multiple repositories
* [1599](https://github.com/nunit/nunit/issues/1599) Move Compact Framework to separate project
* [1601](https://github.com/nunit/nunit/issues/1601) Move SilverLight to a separate project
* [1609](https://github.com/nunit/nunit/issues/1609) Upgrade Cake build to latest version
* [1661](https://github.com/nunit/nunit/issues/1661) Create .NET Standard Framework Build
* [1668](https://github.com/nunit/nunit/issues/1668) Need implementation-independent way to test number of items in a collection
* [1743](https://github.com/nunit/nunit/issues/1743) Provide multiple results for a test case in the XML output
* [1758](https://github.com/nunit/nunit/issues/1758) No direct inverse for Contains.Key
* [1765](https://github.com/nunit/nunit/issues/1765) TestCaseSourceAttribute constructor for method with parameters
* [1802](https://github.com/nunit/nunit/issues/1802) Design Multiple Assert syntax as seen by users
* [1808](https://github.com/nunit/nunit/issues/1808) Disambiguate error messages from EqualConstraint
* [1811](https://github.com/nunit/nunit/issues/1811) Build.ps1 fails if spaces in path
* [1823](https://github.com/nunit/nunit/issues/1823) Remove engine nuspecs and old global.json
* [1827](https://github.com/nunit/nunit/issues/1827) Remove unused repository paths from repositories.config
* [1828](https://github.com/nunit/nunit/issues/1828) Add Retry for failed tests only
* [1829](https://github.com/nunit/nunit/issues/1829) NUnitLite accepts --params option but does not make any use of it.
* [1836](https://github.com/nunit/nunit/issues/1836) Support nullable enums in Theories
* [1837](https://github.com/nunit/nunit/issues/1837) [Request] AfterConstraint to support more readable usage
* [1840](https://github.com/nunit/nunit/issues/1840) Remove SL and CF #Defined source
* [1866](https://github.com/nunit/nunit/issues/1866) [Request] More readable way to set polling interval in After constraint
* [1870](https://github.com/nunit/nunit/issues/1870) EqualConstraint result failure message for DateTime doesn't show sufficient resolution
* [1872](https://github.com/nunit/nunit/issues/1872) Parameterized method being called with no parameter
* [1876](https://github.com/nunit/nunit/issues/1876) What should we do about Env.cs
* [1880](https://github.com/nunit/nunit/issues/1880) AttributeUsage for various Attributes
* [1889](https://github.com/nunit/nunit/issues/1889) Modify NUnitLite to display multiple assert information
* [1891](https://github.com/nunit/nunit/issues/1891) TestContext.Progress and TestContext.Error silently drop text that is not properly XML encoded
* [1901](https://github.com/nunit/nunit/issues/1901) Make NUnitLite-runner Prefer32Bit option consistent across Debug/Release
* [1904](https://github.com/nunit/nunit/issues/1904) Add .NET Standard 1.6 Dependencies to the Nuspec Files
* [1907](https://github.com/nunit/nunit/issues/1907) Handle early termination of multiple assert block
* [1911](https://github.com/nunit/nunit/issues/1911) Changing misleading comment that implies that every `ICollection<T>` is a list
* [1912](https://github.com/nunit/nunit/issues/1912) Add new warning status and result state
* [1913](https://github.com/nunit/nunit/issues/1913) Report Warnings in NUnitLite
* [1914](https://github.com/nunit/nunit/issues/1914) Extra AssertionResult entries in TestResults
* [1915](https://github.com/nunit/nunit/issues/1915) Enable Path, File and Directory Assert/Constraints in the .NET Standard Build
* [1917](https://github.com/nunit/nunit/issues/1917) Use of IsolatedContext breaks tests in user-created AppDomain
* [1924](https://github.com/nunit/nunit/issues/1924) Run tests using the NUnit Console Runner
* [1929](https://github.com/nunit/nunit/issues/1929) Rename zip and remove source zip
* [1933](https://github.com/nunit/nunit/issues/1933) Tests should pass if test case source provides 0 test cases
* [1941](https://github.com/nunit/nunit/issues/1941) Use dictionary-based property for test run parameters
* [1945](https://github.com/nunit/nunit/issues/1945) Use high-quality icon for nuspecs
* [1947](https://github.com/nunit/nunit/issues/1947) Add NonTestAssemblyAttribute
* [1954](https://github.com/nunit/nunit/issues/1954) Change Error Message for Assert.Equals
* [1960](https://github.com/nunit/nunit/issues/1960) Typo fixes
* [1966](https://github.com/nunit/nunit/issues/1966) Xamarin Runner cannot reference NUnit NuGet Package

## Earlier Releases

* Release Notes for [NUnit 2.9.1 through 3.5](xref:pre35releasenotes)
* Release Notes for [NUnit 2.6 through 2.6.4](http://www.nunit.org/?p=releaseNotes&r=2.6.4)
* Release Notes for [NUnit 2.5 through 2.5.10](http://www.nunit.org/?p=releaseNotes&r=2.5.10)
* Release Notes for [NUnit 2.4 through 2.4.8](http://www.nunit.org/?p=releaseNotes&r=2.4.8)
* Release Notes for [NUnit 2.0 through 2.2.10](http://www.nunit.org/?p=releaseNotes&r=2.2.10)
