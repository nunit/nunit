Feature: NUnit should support TeamCity	

Background:
	Given NUnit path is ..\

Scenario Outline: The NUnit sends TeamCity service messages when I run successful test for NUnit2 framework
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created folder mocks
	And I have added reference ..\..\..\..\packages\NUnit.2.6.4\lib\nunit.framework.dll to foo.tests
	And I have copied reference ..\..\..\..\packages\NUnit.2.6.4\lib\nunit.framework.dll to folder mocks
	And I have compiled assembly foo.tests to file mocks\foo.tests.dll	
	And I have added mocks\foo.tests.dll to the list of testing assemblies
	And I want to use CmdArguments type of TeamCity integration
	When I run NUnit tests
	Then the exit code should be 0
	And the output should contain TeamCity service messages:
	|                   | name                                  | captureStandardOutput | duration | flowId | parent | message      | details                           | out    |
	| testSuiteStarted  | foo.tests.dll                         |                       |          | .+     |        |              |                                   |        |
	| testStarted       | Foo.Tests.UnitTests1.SuccessfulTest   | false                 |          | .+     |        |              |                                   |        |
	| testStdOut        | Foo.Tests.UnitTests1.SuccessfulTest   |                       |          | .+     |        |              |                                   | output |
	| testFinished      | Foo.Tests.UnitTests1.SuccessfulTest   |                       | \d+      | .+     |        |              |                                   |        |
	| testSuiteFinished | foo.tests.dll                         |                       |          | .+     |        |              |                                   |        |

Examples:
	| frameworkVersion |
	| Version45        |
	| Version40        |

Scenario Outline: The NUnit sends TeamCity service messages when I run successful test for NUnit3
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled assembly foo.tests to file mocks\foo.tests.dll	
	And I have added mocks\foo.tests.dll to the list of testing assemblies
	And I want to use CmdArguments type of TeamCity integration
	When I run NUnit tests
	Then the exit code should be 0
	And the output should contain TeamCity service messages:
	|                   | name                                  | captureStandardOutput | duration | flowId | parent | message      | details                           | out    |
	| testSuiteStarted  | foo.tests.dll                         |                       |          | .+     |        |              |                                   |        |
	| flowStarted       |                                       |                       |          | .+     | .+     |              |                                   |        |
	| testStarted       | Foo.Tests.UnitTests1.SuccessfulTest   | false                 |          | .+     |        |              |                                   |        |
	| testStdOut        | Foo.Tests.UnitTests1.SuccessfulTest   |                       |          | .+     |        |              |                                   | output |
	| testFinished      | Foo.Tests.UnitTests1.SuccessfulTest   |                       | \d+      | .+     |        |              |                                   |        |
	| flowFinished      |                                       |                       |          | .+     |        |              |                                   |        |
	| testSuiteFinished | foo.tests.dll                         |                       |          | .+     |        |              |                                   |        |

Examples:
	| frameworkVersion |
	| Version45        |
	| Version40        |
	
Scenario Outline: The NUnit sends TeamCity service messages when I run it for different types of tests
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have added failed method as FailedTest to the class Foo.Tests.UnitTests2 for foo.tests
	And I have added ignored method as IgnoredTest to the class Foo.Tests.UnitTests3 for foo.tests
	And I have added inconclusive method as InconclusiveTest to the class Foo.Tests.UnitTests4 for foo.tests
	And I have created folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled assembly foo.tests to file mocks\foo.tests.dll	
	And I have added mocks\foo.tests.dll to the list of testing assemblies
	And I want to use <teamCityIntegration> type of TeamCity integration
	And I want to use <configurationType> configuration type
	When I run NUnit tests
	Then the exit code should be 1
	And the output should contain TeamCity service messages:
	|                   | name                                  | captureStandardOutput | duration | flowId | parent | message      | details                           | out    |
	| testSuiteStarted  | foo.tests.dll                         |                       |          | .+     |        |              |                                   |        |
	| flowStarted       |                                       |                       |          | .+     | .+     |              |                                   |        |
	| testStarted       | Foo.Tests.UnitTests1.SuccessfulTest   | false                 |          | .+     |        |              |                                   |        |
	| testStdOut        | Foo.Tests.UnitTests1.SuccessfulTest   |                       |          | .+     |        |              |                                   | output |
	| testFinished      | Foo.Tests.UnitTests1.SuccessfulTest   |                       | \d+      | .+     |        |              |                                   |        |
	| flowFinished      |                                       |                       |          | .+     |        |              |                                   |        |
	| flowStarted       |                                       |                       |          | .+     | .+     |              |                                   |        |
	| testStarted       | Foo.Tests.UnitTests2.FailedTest       | false                 |          | .+     |        |              |                                   |        |
	| testFailed        | Foo.Tests.UnitTests2.FailedTest       |                       |          | .+     |        | Reason       | Foo.Tests.UnitTests2.FailedTest() |        |
	| testFinished      | Foo.Tests.UnitTests2.FailedTest       |                       | \d+      | .+     |        |              |                                   |        |
	| flowFinished      |                                       |                       |          | .+     |        |              |                                   |        |
	| flowStarted       |                                       |                       |          | .+     | .+     |              |                                   |        |
	| testStarted       | Foo.Tests.UnitTests3.IgnoredTest      | false                 |          | .+     |        |              |                                   |        |
	| testIgnored       | Foo.Tests.UnitTests3.IgnoredTest      |                       |          | .+     |        | Reason       |                                   |        |
	| flowFinished      |                                       |                       |          | .+     |        |              |                                   |        |
	| flowStarted       |                                       |                       |          | .+     | .+     |              |                                   |        |
	| testStarted       | Foo.Tests.UnitTests4.InconclusiveTest | false                 |          | .+     |        |              |                                   |        |
	| testIgnored       | Foo.Tests.UnitTests4.InconclusiveTest |                       |          | .+     |        | Inconclusive |                                   |        |
	| flowFinished      |                                       |                       |          | .+     |        |              |                                   |        |
	| testSuiteFinished | foo.tests.dll                         |                       |          | .+     |        |              |                                   |        |

Examples:
	| configurationType | frameworkVersion | teamCityIntegration |
	| ProjectFile       | Version45        | CmdArguments        |
	| ProjectFile       | Version40        | CmdArguments        |
	| CmdArguments      | Version45        | CmdArguments        |
	| CmdArguments      | Version40        | CmdArguments        |
	| ProjectFile       | Version45        | EnvVariable         |
	| ProjectFile       | Version40        | EnvVariable         |
	| CmdArguments      | Version45        | EnvVariable         |
	| CmdArguments      | Version40        | EnvVariable         |

Scenario Outline: The NUnit sends TeamCity service messages when I run it for failed setup
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have added failedSetUp method as FailedSetUp to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled assembly foo.tests to file mocks\foo.tests.dll	
	And I have added mocks\foo.tests.dll to the list of testing assemblies
	And I want to use CmdArguments type of TeamCity integration
	When I run NUnit tests
	Then the exit code should be 1
	And the output should contain TeamCity service messages:
	|                   | name                                | captureStandardOutput | duration | flowId | parent | message          | details                            | out |
	| testSuiteStarted  | foo.tests.dll                       |                       |          | .+     |        |                  |                                    |     |
	| flowStarted       |                                     |                       |          | .+     | .+     |                  |                                    |     |
	| testStarted       | Foo.Tests.UnitTests1.SuccessfulTest | false                 |          | .+     |        |                  |                                    |     |
	| testFailed        | Foo.Tests.UnitTests1.SuccessfulTest |                       |          | .+     |        | System.Exception | Foo.Tests.UnitTests1.FailedSetUp() |     |
	| testFinished      | Foo.Tests.UnitTests1.SuccessfulTest |                       | \d+      | .+     |        |                  |                                    |     |
	| flowFinished      |                                     |                       |          | .+     |        |                  |                                    |     |
	| testSuiteFinished | foo.tests.dll                       |                       |          | .+     |        |                  |                                    |     |

Examples:
	| frameworkVersion |
	| Version45        |
	| Version40        |

Scenario Outline: The NUnit sends TeamCity service messages when I run it for failed tear down
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have added failedTearDown method as FailedTearDown to the class Foo.Tests.UnitTests1 for foo.tests	
	And I have created folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled assembly foo.tests to file mocks\foo.tests.dll	
	And I have added mocks\foo.tests.dll to the list of testing assemblies
	And I want to use CmdArguments type of TeamCity integration
	When I run NUnit tests
	Then the exit code should be 1
	And the output should contain TeamCity service messages:
	|                   | name                                | captureStandardOutput | duration | flowId | parent | message          | details                               | out    |
	| testSuiteStarted  | foo.tests.dll                       |                       |          | .+     |        |                  |                                       |        |
	| flowStarted       |                                     |                       |          | .+     | .+     |                  |                                       |        |
	| testStarted       | Foo.Tests.UnitTests1.SuccessfulTest | false                 |          | .+     |        |                  |                                       |        |
	| testFailed        | Foo.Tests.UnitTests1.SuccessfulTest |                       |          | .+     |        | System.Exception | Foo.Tests.UnitTests1.FailedTearDown() |        |
	| testStdOut        | Foo.Tests.UnitTests1.SuccessfulTest |                       |          | .+     |        |                  |                                       | output |
	| testFinished      | Foo.Tests.UnitTests1.SuccessfulTest |                       | \d+      | .+     |        |                  |                                       |        |
	| flowFinished      |                                     |                       |          | .+     |        |                  |                                       |        |
	| testSuiteFinished | foo.tests.dll                       |                       |          | .+     |        |                  |                                       |        |

Examples:
	| frameworkVersion |
	| Version45        |
	| Version40        |

Scenario Outline: The NUnit sends TeamCity service messages when I run it for parallelizable tests
	Given Framework version is <frameworkVersion>	        
	And I have added SuccessfulParallelizable method as SuccessfulParallelizable1 to the class Foo.Tests.UnitTests1 for foo1.tests	
	And I have added SuccessfulParallelizable method as SuccessfulParallelizable2 to the class Foo.Tests.UnitTests1 for foo1.tests	
	And I have added SuccessfulParallelizable method as SuccessfulParallelizable3 to the class Foo.Tests.UnitTests1 for foo1.tests	
	And I have added attribute [assembly: NUnit.Framework.Parallelizable] to the assembly foo1.tests
	And I have added attribute [NUnit.Framework.Parallelizable] to the class Foo.Tests.UnitTests1 for foo1.tests
	And I have added NUnit framework references to foo1.tests
	And I have added SuccessfulParallelizable method as SuccessfulParallelizable4 to the class Foo.Tests.UnitTests1 for foo2.tests	
	And I have added SuccessfulParallelizable method as SuccessfulParallelizable5 to the class Foo.Tests.UnitTests1 for foo2.tests	
	And I have added SuccessfulParallelizable method as SuccessfulParallelizable6 to the class Foo.Tests.UnitTests1 for foo2.tests	
	And I have added attribute [assembly: NUnit.Framework.Parallelizable] to the assembly foo2.tests
	And I have added attribute [NUnit.Framework.Parallelizable] to the class Foo.Tests.UnitTests1 for foo2.tests
	And I have added NUnit framework references to foo2.tests
	And I have created folder mocks	
	And I have copied NUnit framework references to folder mocks
	And I have compiled assembly foo1.tests to file mocks\foo1.tests.dll	
	And I have compiled assembly foo2.tests to file mocks\foo2.tests.dll	
	And I have added mocks\foo1.tests.dll to the list of testing assemblies
	And I have added mocks\foo2.tests.dll to the list of testing assemblies
	And I want to use CmdArguments type of TeamCity integration
	And I have added workers=10 arg to NUnit console args
	And I have added agents=10 arg to NUnit console args
	When I run NUnit tests
	Then the exit code should be 0
	And the Test Run Summary should has following:
	| field        | value |
	| Tests run    | 6     |
	| Passed       | 6     |
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
	
Scenario Outline: The NUnit sends TeamCity service messages when I run successful tests with the same names in the several assemblies
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo1.tests
	And I have added successful method as SuccessfulTest to the class Foo.Tests.UnitTests1 for foo2.tests
	And I have created folder mocks
	And I have added NUnit framework references to foo1.tests
	And I have added NUnit framework references to foo2.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled assembly foo1.tests to file mocks\foo1.tests.dll	
	And I have compiled assembly foo2.tests to file mocks\foo2.tests.dll	
	And I have added mocks\foo1.tests.dll to the list of testing assemblies
	And I have added mocks\foo2.tests.dll to the list of testing assemblies
	And I have added workers=1 arg to NUnit console args
	And I have added agents=1 arg to NUnit console args
	And I want to use CmdArguments type of TeamCity integration
	When I run NUnit tests
	Then the exit code should be 0
	And the output should contain TeamCity service messages:
	|                   | name                                | captureStandardOutput | duration | flowId | parent | message | details | out    |
	| testSuiteStarted  | foo1.tests.dll                      |                       |          | .+     |        |         |         |        |
	| flowStarted       |                                     |                       |          | .+     | .+     |         |         |        |
	| testStarted       | Foo.Tests.UnitTests1.SuccessfulTest | false                 |          | .+     |        |         |         |        |
	| testStdOut        | Foo.Tests.UnitTests1.SuccessfulTest |                       |          | .+     |        |         |         | output |
	| testFinished      | Foo.Tests.UnitTests1.SuccessfulTest |                       | \d+      | .+     |        |         |         |        |
	| flowFinished      |                                     |                       |          | .+     |        |         |         |        |
	| testSuiteFinished | foo1.tests.dll                      |                       |          | .+     |        |         |         |        |
	| testSuiteStarted  | foo2.tests.dll                      |                       |          | .+     |        |         |         |        |
	| flowStarted       |                                     |                       |          | .+     | .+     |         |         |        |
	| testStarted       | Foo.Tests.UnitTests1.SuccessfulTest | false                 |          | .+     |        |         |         |        |
	| testStdOut        | Foo.Tests.UnitTests1.SuccessfulTest |                       |          | .+     |        |         |         | output |
	| testFinished      | Foo.Tests.UnitTests1.SuccessfulTest |                       | \d+      | .+     |        |         |         |        |
	| flowFinished      |                                     |                       |          | .+     |        |         |         |        |
	| testSuiteFinished | foo2.tests.dll                      |                       |          | .+     |        |         |         |        |

Examples:
	| frameworkVersion |
	| Version45        |
	| Version40        |