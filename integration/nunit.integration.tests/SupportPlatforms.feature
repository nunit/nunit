Feature: NUnit should support platforms

Background:
	Given NUnit path is ..\
	
Scenario Outline: I can run test for different platforms
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created the folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have specified <platform> platform for assembly foo.tests
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll	
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies
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
	| frameworkVersion | platform |
	| Version45         | AnyCpu   |
	| Version40         | AnyCpu   |
	| Version20         | AnyCpu   |
	| Version45         | X86      |
	| Version40         | X86      |
	| Version20         | X86      |
	#| Version45         | X64      |
	#| Version40         | X64      |
	#| Version20         | X64      |
