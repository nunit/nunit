﻿Feature: NUnit should running from different locations and appBase

Background:
	Given NUnit path is ..\
	
Scenario Outline: I can run tests from nested directory
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll	
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies
	And I want to use <configurationType> configuration type
	And I have change current directory to WorkingDirectory
	When I run NUnit console
	Then the exit code should be 0
	And the Test Run Summary should has following:
	| field        | value |
	| Test Count   | 1     |
	| Passed       | 1     |
	| Failed       | 0     |
	| Inconclusive | 0     |
	| Skipped      | 0     |

		
Examples:
	| configurationType | frameworkVersion |
	| CmdArguments      | Version45        |
	| CmdArguments      | Version40        |

Scenario Outline: I can run tests from parent directory
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll	
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies
	And I want to use <configurationType> configuration type
	And I have change current directory to ..\..\..\
	When I run NUnit console
	Then the exit code should be 0
	And the Test Run Summary should has following:
	| field        | value |
	| Test Count   | 1     |
	| Passed       | 1     |
	| Failed       | 0     |
	| Inconclusive | 0     |
	| Skipped      | 0     |

		
Examples:
	| configurationType | frameworkVersion |
	| ProjectFile       | Version45        |
	| ProjectFile       | Version40        |
	| CmdArguments      | Version45        |
	| CmdArguments      | Version40        |
