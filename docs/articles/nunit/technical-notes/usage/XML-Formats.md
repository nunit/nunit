### DRAFT
Use of XML in NUnit 2.x is somewhat limited and is only used in external files. NUnit 3.0 uses XML in several of its interfaces for communicating between its three architectural layers.

The samples given here represent the current state of the application and are subject to change. Watch this page for new information as development continues.

#### NUnit Projects

An NUnit project is stored as a file with the extension .nunit and describes one or more test assemblies to be executed, together with certain parameters used in running them. No schema is used for this file.

  * [[Sample NUnit Project|http://nunit.org/files/nunit_project_25.txt]] 

Currently, the format is the same as for NUnit 2.x but it is likely to change as the project proceeds. Note that NUnitLite does not use or recognize NUnit projects, but only assemblies.

For details of the file contents, see [[NUnit Project XML Format]]

#### NUnit Settings

The NUnitSettings30.xml file holds default settings for NUnit 3.0. No schema is used for this file. The format is the same as for 2.x but new settings may be added and the names of certain keys are likely to change.

  * [[Sample NUnit Settings File|http://nunit.org/files/sample_nunitsettings_file.txt]]
  * List of [[NUnit Setting Names]]

#### Test Results

The results of an test run are saved in a file with the default name of TestResult.xml. The schema of this file is being modified substantially for NUnit 3.0.

  * [[Sample|http://nunit.org/files/testresult_30.txt]]
  * Schema not yet available

For details of the file layout see [[Test Result XML Format]].

#### V2 Test Results

Optionally, NUnit 3.0 is able to save results in the NUnit 2.x format for use with CI servers that do not yet understand the new format.

  * [[Sample|http://nunit.org/files/testresult_25.txt]]
  * [[Schema|http://nunit.org/files/testresult_schema_25.txt]]

#### Test Representation

When using the -explore option, a list of tests without results is returned. The format used is simply a subset of the test result format, without the result information.

See [[Test Result XML Format]] for details.

#### Test Filters

Information about which tests to run is provided to the framework using a test filter, represented as an XML fragment. See [[Test Filters]] for details.

#### Test Packages

#### Progress Reports

As a test run progresses, individual test run results are sent as xml fragments from the result file under construction. A runner can, in fact, construct a copy of the result file incrementally using these fragments if desired.

#### VisualState

The Gui runner uses a file with the suffix .VisualState.xml to save the current visual state of a project so that it may be restored upon re-opening. This file is private to the Gui but is included here for completeness. The current format is identical to that used in NUnit 2.x.

  * [[Sample Visual State File|http://nunit.org/files/sample_visual_state_25.txt]]

