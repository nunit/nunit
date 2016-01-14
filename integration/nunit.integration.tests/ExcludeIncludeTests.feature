Feature: NUnit should support excluding and including tests

Background:
	Given NUnit path is ..\
	
@teamcity
Scenario Outline: I can run all tests except those in the CatA category
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have added successfulCatA method as SuccessfulTestCatA to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created the folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll	
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies
	And I have added the arg Where=cat!=CatA to NUnit console command line
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
	| frameworkVersion |
	| Version45        |
	| Version40        |

@teamcity
Scenario Outline: I can run only the tests in the CatA category
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have added successfulCatA method as SuccessfulTestCatA to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have added failed method as FailedTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have added ignored method as IgnoredTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll	
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies
	And I have added the arg Where=cat==CatA to NUnit console command line
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
	| frameworkVersion |
	| Version45        |
	| Version40        |
	
@teamcity
Scenario Outline: I can run all tests except those in the CatA category fron NUnit 2 framework
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have added successfulCatA method as SuccessfulTestCatA to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created the folder mocks
	And I have added the reference ..\..\..\..\packages\NUnit.2.6.4\lib\nunit.framework.dll to foo.tests
	And I have copied the reference ..\..\..\..\packages\NUnit.2.6.4\lib\nunit.framework.dll to folder mocks
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll	
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies
	And I have added the arg Where=cat!=CatA to NUnit console command line
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
	| frameworkVersion |
	| Version45        |
	| Version40        |
		