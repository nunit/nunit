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
	And I want to use CmdArguments type of TeamCity integration
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