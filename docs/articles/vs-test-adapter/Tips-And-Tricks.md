> ﻿NOTE:
> As of the 3.0 final release, the registry settings are no longer recognized. Instead, use settings in the `.runsettings` file. 


## NUnit 3.x

### VS Test .Runsettings configuration
Certain NUnit Test Adapter settings are configurable using a .runsettings file. 
The following options are available:

|Key|Type|Options| Default|
|---|----|-------|--------------|
|[InternalTraceLevel](#internaltracelevel)| string |  Off, Error, Warning, Info, Verbose,  Debug| Nothing => Off|
|[NumberOfTestWorkers](#numberoftestworkers)| int | nr of workers | -1|
|[ShadowCopyFiles](#shadowcopyfiles)| bool |True, False | False|
|[Verbosity](#verbosity)| int | -1, 0-5 . -1 means quiet mode | 0|
|[UseVsKeepEngineRunning](#usevskeepenginerunning)| bool | True, False| False|
|BasePath| string | path| ?|
|[PrivateBinPath](#privatebinpath) | string| directory1;directory2;etc |?|
|[RandomSeed](#randomseed) int | seed integer| random|
|[DefaultTimeout](#defaulttimeout)|int|timeout in mS, 0 means infinite|0|
|[DefaultTestNamePattern](#defaulttestnamepattern)|string|Pattern for display name|{m}{a}|
|[WorkDirectory](#workdirectory)|string|specify directory|Test assembly location|
|[TestOutputXml](#testoutputxml)|string|specify directory|Test Result Xml output folder|
|[DumpXmlTestDiscovery](#dumpxmltestdiscovery-and-dumpxmltestresults)|bool|Enable dumping of NUnit discovery response xml|false|
|[DumpXmlTestResults](#dumpxmltestdiscovery-and-dumpxmltestresults)|bool|Enable dumping of NUnit execution response xml|false|
|[PreFilter](#prefilter)|bool|Enable prefiltering to increase performance for Visual Studio testing|false|
|[ShowInternalProperties](#showinternalproperties)| bool | Turn on showing internal NUnit properties in Test Explorer| false|
|[Where](#where)|string| NUnit Filter expression|
|[UseParentFQNForParametrizedTests](#useparentfqnforparametrizedtests)|bool|Enable parent as FQN for parametrized tests|false|
|[UseNUnitIdforTestCaseId](#usenunitidfortestcaseid)|bool|Uses NUnit test id as VSTest Testcase Id, instead of FUllyQualifiedName|false|
|[ConsoleOut](#consoleout)|int|Sends standard console output to the output window|1|
|[StopOnError]("StopOnError)|bool|Stops on first error|false|


### Visual Studio templates for runsettings
You can install [item templates for runsettings](https://marketplace.visualstudio.com/items?itemName=OsirisTerje.Runsettings-19151) in Visual Studio (applies to version 2017, 2019 and upwards) which includes the NUnit settings mentioned here.  Note that there are available seperate installs for earlier Visual Studio versions, links to these can be found in the above. 


### Example implementation
See https://github.com/nunit/nunit3-vs-adapter/blob/8a9b8a38b7f808a4a78598542ddaf557950c6790/demo/demo.runsettings

### NUnit .runsettings implementation

https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/AdapterSettings.cs#L143


### Details

#### WorkDirectory
Our WorkDirectory is the place where output files are intended to be saved for the run, whether created by NUnit or by the user, who can access the work directory using TestContext. It's different from TestDirectory, which is the directory containing the test assembly. For a run with multiple assemblies, there could be multiple TestDirectories, but only one WorkDirectory.
User sets work directory to tell NUnit where to put stuff like the XML or any text output. User may also access it in the code and save other things there. Think of it as the directory for saving stuff. 

#### TestOutputXml
If this is specified, the adapter will generate NUnit Test Result Xml data in the folder specified here.  If not specified, no such output will be generated.  
The folder can be

1) An absolute path
2) A relative path, which is then relative to either WorkDirectory, or if this is not specified, relative to the current directory, as defined by .net runtime.

*(From version 3.12)*

#### InternalTraceLevel
This setting is a diagnostic setting forwarded to NUnit, and not used by the adapter itself.  For further information see the [NUnit Tracelevel documentation](https://github.com/nunit/docs/wiki/Internal-Trace-Spec)

#### NumberOfTestWorkers
This  setting is sent to NUnit to determine how  [parallelization](https://github.com/nunit/docs/wiki/Parallelizable-Attribute) should be performed.  
Note in particular that NUnit can either run directly or for parallel runs use queue of threads.  Set to 0, it will run directly, set to 1 it will use a queue with a single thread.  

#### ShadowCopyFiles
This setting is sent to NUnit to enable/disable shadow-copying.

#### Verbosity
This controls the outputs from the adapter to the Visual Studio Output/Tests window.
A higher number includes the information from the lower numbers.
It has the following actual levels:

-1 : Quiet mode.  Only shows errors and warnings.  

0 : Default, normal information verbosity

1-3: Some more information from setting are output (in particular regarding parallelization)

4: Outputs the values from the  runsettings it has discovered.

5: Outputs all debug statements in the adapter



#### UseVsKeepEngineRunning
This setting is used by the adapter to signal to the VSTest.Execution engine to keep running after the tests have finished running.  This can speed up execution of subsequent test runs, as the execution engine already is loaded, but running the risks of either holding onto test assemblies and having some tests not properly cleaned out.   The settings is the same as using the Visual Studio  Test/Test Settings/Keep Test Execution Engine running. 

#### DumpXmlTestDiscovery and DumpXmlTestResults
These settings are used to dump the output from NUnit, as it is received by the adapter, before any processing in the adapter is done, to disk.  It is part of the diagnostics tools for the adapter.
You can find the files under your current outputfolder, in a subfolder named Dump.
(Note: This is not the same as the TestResults folder, this data is not testresults, but diagnostics dumps)

#### PreFilter
A prefilter will improve performance when testing a selection of tests from the Test Explorer.  It is default off, because there are issues in certain cases, see below. If you **don't** have any of the cases below, you can turn PreFilter on.
* Your code contains a SetupFixture  [#649](https://github.com/nunit/nunit3-vs-adapter/issues/649)
* Your code uses a TestCaseSource and uses SetName to set the display name instead of SetArgDisplayNames [#650](https://github.com/nunit/nunit3-vs-adapter/issues/650)
* You are using a version of NUnit lower than 3.11  [#648](https://github.com/nunit/nunit3-vs-adapter/issues/648)

If you just need to add this, you can add a runsettings file (any filename, extension .runsettings) containing:
```
<RunSettings>
   <NUnit>
       <PreFilter>true</PreFilter>
   </NUnit>
</RunSettings>
```
*(From version 3.15.1)*


#### ShowInternalProperties
The [NUnit internal properties](https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Internal/PropertyNames.cs) have been "over-populating" in the Test Explorer.  These are default filtered out, although you may still see these when you have [Source Based Discovery (SBD)](https://docs.microsoft.com/en-us/visualstudio/test/test-explorer-faq?view=vs-2017) turned on (which is the default in VS).  Once you have run test execution, they will be gone. We expect this part of the issue (SBD) to be fixed in VS.  If you still want to see them, set this property to true.

#### Where
A NUnit Test Selection Language filter can be added to the runsettings file.  The details are described in **[this blogpost](http://blog.prokrams.com/2019/12/16/nunit3-filter-dotnet/)**

Using the runsettings should be like:

```xml
<RunSettings>
    <NUnit>
        <Where>cat == SomeCategory or method == SomeMethodName or namespace == My.Name.Space or name == 'TestMethod(5)'</Where>
    </NUnit>
</RunSettings>
```
*(From version 3.16.0)*

#### UseParentFQNForParametrizedTests

Setting this may give more stable results when you have complex data driven/parametrized tests.  However, when this is set selecting a single test within such a group, means that **all** tests in that group is executed.

Note that this often has to be set together with [UseNUnitIdforTestCaseId](####UseNUnitIdforTestCaseId)

*(From version 3.16.1)*

#### UseNUnitIdforTestCaseId

The default setting is false, causes the VSTest Testcase ID to be based on the NUnit fullname property, which is nearly equal to a FullyQualifiedName.  The fullname is alse set into the Testcase FullyQualifiedName property.

By setting this property true, it shifts to using the NUnit id as the basis for the testcase id.  This may in certain cases give more stable results, and are more correct.  

However, it has been seen to also have adverse effects, so use with caution.

*(From version 3.16.1)*

#### ConsoleOut

When set to 1, default, will send Console standard output to the Visual Studio Output/Test window, and also with dotnet test, it will appear here. (Note: You have to use the '-v n' option)

Disable this by setting it to 0, which is also the default for version earlier than 3.17.0.

See [Issue 343](https://github.com/nunit/nunit3-vs-adapter/issues/343) for more information and discussion

*(From version 3.17.0)*

#### StopOnError

When enabled (set to true), the tests will stop after the first test failed.  Useful when working with a system with many tests running for a long time, where a few fails.  It can then speed up by stopping at the first one.

See [Issue 675](https://github.com/nunit/nunit3-vs-adapter/issues/675) for more information and discussion

*(From version 3.17.0)*

#### Some further information on directories (From [comment on issue 575](https://github.com/nunit/nunit3-vs-adapter/issues/575#issuecomment-445786421) by [Charlie](https://github.com/CharliePoole) )

NUnit also supports TestContext.TestDirectory, which is the directory where the current test assembly is located. Note that if you have several test assemblies in different directories, the value will be different when each one of them accesses it. Note also that there is no way you can set the TestDirectory because it's always where the assembly is located.

The BasePath is a .NET thing. It's the base directory where assemblies are searched for. You can also have subdirectories listed in the PrivateBinPath. NUnit take scare of all this automatically now, so the old console options are no longer supported. For finding things you want to read at runtime, the TestDirectory and the BasePath will usually be the same thing.



## NUnit 2.x


### Registry Settings

Certain settings in the registry affect how the adapter runs. All these settings are added by using RegEdit under the key **HKCU\Software\nunit.org\VSAdapter**.

#### ShadowCopy

By default NUnit no longer uses shadowcopy. If this causes an issue for you shadowcopy can be enabled by setting the DWORD value UseShadowCopy to 1.   
  
#### KeepEngineRunning

By default the NUnit adapter will "Kill" the Visual Studio Test Execution engine after each run. Visual Studio 2013 and later has a new setting under its top menu, **Test | Test Settings | Keep Test Execution Engine Running**. The adapter normally ignores this setting.

In some cases it can be useful to have the engine running, e.g. during debugging of the adapter itself. You can then set the adapter to follow the VS setting by setting the DWORD value UseVsKeepEngineRunning to 1.

#### Verbosity

Normally the adapter reports exceptions using a short format, consisting of the message only. You can change it to report a verbose format that includes the stack trace, by setting a the DWORD value Verbosity to 1.




