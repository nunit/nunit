---
uid: NUnit3Architecture2009
---

> This is the original - now out of date - document created to describe the planned architecture for NUnit 3.0. We are keeping it for whatever historical interest it may have.

>The diagram was created first and was shown for the first time by Charlie Poole at a Mono gathering in Madrid in 2008.  The complete document was published the following year.

### Summary

This specification describes the overall layered architecture of the NUnit Platform. Details of individual components are covered only as they are relevant to the overall architecture.

##### Launchpad Blueprint
  * http://blueprints.launchpad.net/nunit-3.0/+spec/nunit-3.0-architecture

### User Stories

##### An NUnit User...

  * upgrades selected components of the NUnit platform without changing others.
  * runs tests using alternate runners and frameworks under the NUnit platform.
  * installs and uses addins and other extensions to the NUnit platform.

##### An NUnit Developer...

  * understands how each component fits into the overall architecture of NUnit.
  * chooses the proper component and project for making a fix or adding a feature.
  * is able to focus attention on the component being modified, without worrying about the details of other components.

##### An NUnit Extension Developer...

  * can develop an extension to a specific NUnit component, without dealing with other parts.
  * can develop extensions to multiple NUnit components, which work together to achieve their result.

### Layers

The basic architecture of the NUnit Extended testing platform consists of three layers: 

  * [Architectural-Overview.md#Test-Runner-Layer)
  * [Architectural-Overview.md#Test-Engine-Layer)
  * [Architectural-Overview.md#Framework-Layer)

![](~/images/nunit-xtp-2008.png)

#### Test Runner Layer

The Test Runner or UI layer contains various runners, some provided by the NUnit team, others by independent projects leveraging the NUnit platform. Some runners are stand-alone programs, while others are tasks or plugins running under the control of an IDE or other application. This diversity of runners is part of the reason we refer to NUnit 3.0 as a Testing Platform – we expect many different runners to come into existence and will facilitate their development by providing reusable controls for several key environments.

Programs in this layer are able to participate in the NUnit platform plugin architecture, providing extension points that allow them to be extended. Plugins at this level will usually add some functionality to the UI. Some of them may be standalone while others may require the presence of specific test engine plugins in order to operate.

The NUnit project will continue to provide both a console runner and a WinForms-based GUI with extended capabilities those in NUnit 2.5. In addition, two new GUI runners will be developed, one based on WPF, the other on GTK#.

We’ll work with the NAnt project to provide updates to the NAnt task for use with NUnit 3.0, with the goal of keeping that task current as new versions of NUnit are released. We will provide an NUnit plugin for the new Gallio platform.

In the area of IDE integration, we will deliver a Visual Studio addin or package for running NUnit tests. Since other folks are already providing open source plugins for SharpDevelop and MonoDevelop, we’ll work with them to ensure compatibility.

There is a long-standing need for a runner that executes tests in an actual or simulated web server environment. While good practice calls for extracting as much functionality as possible into separately testable assemblies, more complex applications often contain code which can only be tested in such an environment. For that reason, NUnit 3.0 will feature a web runner, which allows tests to be executed on a web server, reporting results back to the desktop.

Each of the runners will have the option of participating in the NUnit plugin architecture and using functionality provided by NUnit to whatever degree desired. For the runners developed directly by the NUnit team, this capability will be used to the max, allowing others to add GUI features that function by themselves or in conjunction with other plugins operating at the level of the test engine.

#### Test Engine Layer

The Test Engine Layer is the core of the NUnit platform. It provides a public API for use by applications that want to locate, load and run tests and display test results. Many aspects of the Test Engine are already present in NUnit 2.4, while others are new. I’ll focus on the new features here.

NUnit 2.5 already supports running tests in a separate process, allowing selection of the CLR version under which the test is to be run. NUnit 3.0 will extend this feature to allow test processes to run on other machines. It will also support distribution of test execution across a network of remote Test Agents. This facility is intended to be used in several distinct scenarios:
  * Simple load sharing when test execution time is excessive
  * Testing applications on multiple platforms 
  * Testing applications, which are themselves distributed
A great deal of distributed function is already present in NUnit 2.5 through pNUnit, the distributed testing runner contributed to our project by Codice Software. With NUnit 3.0, we plan to integrate this function more completely into NUnit, allowing them to be executed by any runner that uses the NUnit Test Engine.

NUnit will support performance and coverage analysis, test result reporting and maintenance of a history of test results. These features will function only as called upon by the user. That is, we will not carry on data collection activities with the potential of impacting performance when the user simply wants to run tests.

Through use of plugins, NUnit will be able to support a wide variety of test types beyond low-level, isolated programmer tests. What is available in this area will be dependent on the interests of users and their willingness to contribute their efforts to creating them. Examples of the kinds of things we envision, some of which we will provide directly, are:
  * Randomization of test execution order
  * Deterministic test ordering for integration tests
  * Parameterized (data-driven) tests
  * Transactional test behavior
  * Timed tests and parameterized timeout failures
  * Dynamic test generation based on data input
  * Repetitive test execution
  * Tests written as non-managed code
  * Test generation for legacy applications

#### Framework Layer

In NUnit 3.0, the NUnit framework itself – the assembly that is referenced by user tests – will be split along two dimensions. First, there will be separate framework assemblies for different version levels of the Common Language Runtime. By splitting the framework in this way, we will be able to take advantage of newer features, and allow users to take advantage of them, without compromising basic support for older runtimes.

The second split we plan is between the core framework capabilities and the syntactic features that make it easy to access those features. A key example of this is the fluent interface introduced in NUnit 2.4 – the “Assert.That” syntax. One thing we learned through that experiment is that the same syntactic “sugar” does not work well for different language environments. Many of the 2.4/2.5 constructs are unusable or very difficult to use in other languages – C++ for example. By a combination of separate namespaces and separate assemblies, we will allow users to select the appropriate syntax for the work they are doing. Other people will be able to build on the syntax we provide or create entirely new syntactic overlays for their own purposes.

Through use of plugins in the Test Engine layer, NUnit will be able to recognize, load and run tests written using other frameworks. Our focus will be on facilitating the creation of plugins in support of external frameworks by the projects that produce those frameworks or by interested users. For frameworks where that sort of support is not possible – commercial frameworks, for example – we have the option of creating the plugins ourselves.

In some cases, individuals with an idea for a new framework may be able to create them more simply by writing an adapter on top of the NUnit framework itself. We will encourage and support this by giving priority to framework modifications that provide the necessary infrastructure for such projects. 

NUnit will also provide or re-package some framework extensions for specific types of applications, including Windows Forms development, WPF development, web page access, XML testing,  performance measurement and load testing. At this point, it is not yet possible to state which of these will involve existing third-party packages and which will be developed from scratch, because discussions are ongoing.

The NUnitLite framework will be supported running on devices and reporting results back to the Test Engine through a communications channel.

Various popular mock frameworks will be supported. One framework will be selected for use by NUnit’s own self-tests and will be packaged with NUnit.

### Further Details

More detailed specifications are being developed for each of the layers. Consult the [Specifications](xref:Specifications) index for their current status.