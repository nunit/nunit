Feature: NUnit allows to load config files for tests

Background:
	Given NUnit path is ..\
	
Scenario Outline: I can the test with config file
	Given Framework version is <frameworkVersion>	
	And I have added successfulWithConfig method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll
	And I have added config file mocks\foo.tests.dll.config
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies		
	And I want to use <configurationType> configuration type
	When I run NUnit console
	Then the exit code should be 0
	And the Test Run Summary should has following:
	| field        | value |
	| Tests run    | 1     |
	| Passed       | 1     |
	| Errors       | 0     |
	| Failures     | 0     |
	| Inconclusive | 0     |
	| Not run      | 0     |
	| Invalid      | 0     |
	| Ignored      | 0     |
	| Explicit     | 0     |
	| Skipped      | 0     |

		
Examples:
	| configurationType | frameworkVersion |
	| CmdArguments      | Version45        |
	| CmdArguments      | Version40        |
	| ProjectFile       | Version45        |
	| ProjectFile       | Version40        |

Scenario Outline: I can the test with config file for several assemblies using the command line for the list of assemblies
	Given Framework version is <frameworkVersion>	
	And I have added successfulWithConfig method as SuccessfulTest to the class Foo1.Tests.UnitTests1 for foo1.tests
	And I have added successfulWithConfig method as SuccessfulTest to the class Foo2.Tests.UnitTests1 for foo2.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo1.tests
	And I have added NUnit framework references to foo2.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo1.tests to file mocks\foo1.tests.dll
	And I have compiled the assembly foo2.tests to file mocks\foo2.tests.dll
	And I have added config file mocks\foo1.tests.dll.config
	And I have added config file mocks\foo2.tests.dll.config
	And I have added the assembly mocks\foo1.tests.dll to the list of testing assemblies
	And I have added the assembly mocks\foo2.tests.dll to the list of testing assemblies
	And I want to use <configurationType> configuration type
	And I have added the arg Agents=0 to NUnit console command line
	And I have added the arg TeamCity to NUnit console command line
	When I run NUnit console
	Then the exit code should be 0
	And the Test Run Summary should has following:
	| field        | value |
	| Tests run    | 2     |
	| Passed       | 2     |
	| Errors       | 0     |
	| Failures     | 0     |
	| Inconclusive | 0     |
	| Not run      | 0     |
	| Invalid      | 0     |
	| Ignored      | 0     |
	| Explicit     | 0     |
	| Skipped      | 0     |
	And the output should contain TeamCity service messages:
	|                   | name                                 | out                   |
	| testSuiteStarted  | foo1.tests.dll                       |                       |
	| flowStarted       |                                      |                       |
	| testStarted       | Foo1.Tests.UnitTests1.SuccessfulTest |                       |
	| testStdOut        | Foo1.Tests.UnitTests1.SuccessfulTest | foo1.tests.dll.config |
	| testFinished      | Foo1.Tests.UnitTests1.SuccessfulTest |                       |
	| flowFinished      |                                      |                       |
	| testSuiteFinished | foo1.tests.dll                       |                       |
	| testSuiteStarted  | foo2.tests.dll                       |                       |
	| flowStarted       |                                      |                       |
	| testStarted       | Foo2.Tests.UnitTests1.SuccessfulTest |                       |
	| testStdOut        | Foo2.Tests.UnitTests1.SuccessfulTest | foo2.tests.dll.config |
	| testFinished      | Foo2.Tests.UnitTests1.SuccessfulTest |                       |
	| flowFinished      |                                      |                       |
	| testSuiteFinished | foo2.tests.dll                       |                       |


		
Examples:
	| configurationType | frameworkVersion |
	| CmdArguments      | Version45        |
	| CmdArguments      | Version40        |
	
Scenario Outline: I can the test with config file for several assemblies using the project file for the list of assemblies
	Given Framework version is <frameworkVersion>	
	And I have added successfulWithConfig method as SuccessfulTest to the class Foo1.Tests.UnitTests1 for foo1.tests
	And I have added successfulWithConfig method as SuccessfulTest to the class Foo2.Tests.UnitTests1 for foo2.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo1.tests
	And I have added NUnit framework references to foo2.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo1.tests to file mocks\foo1.tests.dll
	And I have compiled the assembly foo2.tests to file mocks\foo2.tests.dll
	And I have added config file mocks\foo1.tests.dll.config
	And I have added config file mocks\foo2.tests.dll.config
	And I have added the assembly mocks\foo1.tests.dll to the list of testing assemblies
	And I have added the assembly mocks\foo2.tests.dll to the list of testing assemblies
	And I want to use <configurationType> configuration type
	And I have added the arg Agents=0 to NUnit console command line
	And I have added the arg TeamCity to NUnit console command line
	When I run NUnit console
	Then the exit code should be 0
	And the Test Run Summary should has following:
	| field        | value |
	| Tests run    | 2     |
	| Passed       | 2     |
	| Errors       | 0     |
	| Failures     | 0     |
	| Inconclusive | 0     |
	| Not run      | 0     |
	| Invalid      | 0     |
	| Ignored      | 0     |
	| Explicit     | 0     |
	| Skipped      | 0     |
	And the output should contain TeamCity service messages:
	|                   | name                                 | out                   |
	| testSuiteStarted  | foo1.tests.dll                       |                       |
	| flowStarted       |                                      |                       |
	| testStarted       | Foo1.Tests.UnitTests1.SuccessfulTest |                       |
	| testStdOut        | Foo1.Tests.UnitTests1.SuccessfulTest | foo1.tests.dll.config |
	| testFinished      | Foo1.Tests.UnitTests1.SuccessfulTest |                       |
	| flowFinished      |                                      |                       |
	| testSuiteFinished | foo1.tests.dll                       |                       |
	| testSuiteStarted  | foo2.tests.dll                       |                       |
	| flowStarted       |                                      |                       |
	| testStarted       | Foo2.Tests.UnitTests1.SuccessfulTest |                       |
	| testStdOut        | Foo2.Tests.UnitTests1.SuccessfulTest | foo1.tests.dll.config |
	| testFinished      | Foo2.Tests.UnitTests1.SuccessfulTest |                       |
	| flowFinished      |                                      |                       |
	| testSuiteFinished | foo2.tests.dll                       |                       |


		
Examples:
	| configurationType | frameworkVersion |
	| ProjectFile       | Version45        |
	| ProjectFile       | Version40        |