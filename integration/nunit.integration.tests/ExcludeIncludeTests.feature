Feature: NUnit should support excluding and including tests

Background:
	Given NUnit path is ..\
	
Scenario Outline: I can run all tests except those in the CatA category
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have added successfulCatA method as SuccessfulTestCatA to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled assembly foo.tests to file mocks\foo.tests.dll	
	And I have added mocks\foo.tests.dll to the list of testing assemblies
	And I have added Where=cat!=CatA arg to NUnit console args
	When I run NUnit tests
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
	| frameworkVersion |
	| Version45        |
	| Version40        |

	Scenario Outline: I can run only the tests in the CatA category
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have added successfulCatA method as SuccessfulTestCatA to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have added failed method as FailedTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have added ignored method as IgnoredTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have created folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled assembly foo.tests to file mocks\foo.tests.dll	
	And I have added mocks\foo.tests.dll to the list of testing assemblies
	And I have added Where=cat==CatA arg to NUnit console args
	When I run NUnit tests
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
	| frameworkVersion |
	| Version45        |
	| Version40        |
	
Scenario Outline: I can run all tests except those in the CatA category fron NUnit 2 framework
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests
	And I have added successfulCatA method as SuccessfulTestCatA to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created folder mocks
	And I have added reference ..\..\..\..\packages\NUnit.2.6.4\lib\nunit.framework.dll to foo.tests
	And I have copied reference ..\..\..\..\packages\NUnit.2.6.4\lib\nunit.framework.dll to folder mocks
	And I have compiled assembly foo.tests to file mocks\foo.tests.dll	
	And I have added mocks\foo.tests.dll to the list of testing assemblies
	And I have added Where=cat!=CatA arg to NUnit console args
	When I run NUnit tests
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
	| frameworkVersion |
	| Version45        |
	| Version40        |
		