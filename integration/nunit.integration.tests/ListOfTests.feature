Feature: NUnit should support the list of tests to explore and run

Background:
	Given NUnit path is ..\
	
@teamcity
Scenario Outline: I can explore tests and write the list of tests to file
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest1 to the class Foo.Tests.UnitTests1 for foo.tests
	And I have added successful method as SuccessfulTest2 to the class Foo.Tests.UnitTests1 for foo.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll	
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies
	And I have added the arg Explore=mocks\AllTests.xml to NUnit console command line
	When I run NUnit console
	Then the exit code should be 0
	And the xml file mocks\AllTests.xml contains items by xPath .//test-suite[@type='Assembly']:
	| name          |
	| foo.tests.dll |	
	And the xml file mocks\AllTests.xml contains items by xPath .//test-suite[@type='TestFixture']/test-case:
	| fullname                             |
	| Foo.Tests.UnitTests1.SuccessfulTest1 |
	| Foo.Tests.UnitTests1.SuccessfulTest2 |

		
Examples:
	| frameworkVersion |
	| Version45        |
	| Version40        |

@teamcity
Scenario Outline: I can explore tests and write the list of tests to file for several assemblies
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest1 to the class Foo.Tests.UnitTests1 for foo1.tests
	And I have added successful method as SuccessfulTest2 to the class Foo.Tests.UnitTests1 for foo1.tests
	And I have added successful method as SuccessfulTest1 to the class Foo.Tests.UnitTests2 for foo1.tests
	And I have added successful method as SuccessfulTest1 to the class Foo.Tests.UnitTests1 for foo2.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo1.tests
	And I have added NUnit framework references to foo2.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo1.tests to file mocks\foo1.tests.dll	
	And I have compiled the assembly foo2.tests to file mocks\foo2.tests.dll	
	And I have added the assembly mocks\foo1.tests.dll to the list of testing assemblies
	And I have added the assembly mocks\foo2.tests.dll to the list of testing assemblies
	And I have added the arg Explore=mocks\AllTests.xml to NUnit console command line
	When I run NUnit console
	Then the exit code should be 0
	And the xml file mocks\AllTests.xml contains items by xPath .//test-suite[@type='Assembly']:
	| name          |
	| foo1.tests.dll |
	| foo2.tests.dll |
	And the xml file mocks\AllTests.xml contains items by xPath .//test-suite[@type='TestFixture']/test-case:
	| fullname                             |
	| Foo.Tests.UnitTests1.SuccessfulTest1 |
	| Foo.Tests.UnitTests1.SuccessfulTest2 |
	| Foo.Tests.UnitTests2.SuccessfulTest1 |
	| Foo.Tests.UnitTests1.SuccessfulTest1 |

		
Examples:
	| frameworkVersion |
	| Version45        |
	| Version40        |


@teamcity
Scenario Outline: I can run tests from the list of tests
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest1 to the class Foo.Tests.UnitTests1 for foo.tests
	And I have added successful method as SuccessfulTest2 to the class Foo.Tests.UnitTests2 for foo.tests
	And I have added successful method as SuccessfulTest3 to the class Foo.Tests.UnitTests3 for foo.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo.tests to file mocks\foo.tests.dll	
	And I have added the assembly mocks\foo.tests.dll to the list of testing assemblies
	And I have append the line Foo.Tests.UnitTests1 to file mocks\ListOfTests.txt
	And I have append the line Foo.Tests.UnitTests2 to file mocks\ListOfTests.txt
	And I have added the arg TestList=mocks\ListOfTests.txt to NUnit console command line
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

		
Examples:
	| frameworkVersion |
	| Version45        |
	| Version40        |

@teamcity
Scenario Outline: I can run tests from the list of tests for several assemblies
	Given Framework version is <frameworkVersion>	
	And I have added successful method as SuccessfulTest1 to the class Foo.Tests.UnitTests1 for foo1.tests
	And I have added successful method as SuccessfulTest2 to the class Foo.Tests.UnitTests2 for foo1.tests
	And I have added successful method as SuccessfulTest3 to the class Foo.Tests.UnitTests3 for foo1.tests
	And I have added successful method as SuccessfulTest1 to the class Foo.Tests.UnitTests4 for foo2.tests
	And I have added successful method as SuccessfulTest1 to the class Foo.Tests.UnitTests1 for foo2.tests
	And I have created the folder mocks
	And I have added NUnit framework references to foo1.tests
	And I have added NUnit framework references to foo2.tests
	And I have copied NUnit framework references to folder mocks
	And I have compiled the assembly foo1.tests to file mocks\foo1.tests.dll	
	And I have compiled the assembly foo2.tests to file mocks\foo2.tests.dll
	And I have added the assembly mocks\foo1.tests.dll to the list of testing assemblies
	And I have added the assembly mocks\foo2.tests.dll to the list of testing assemblies
	And I have append the line Foo.Tests.UnitTests1 to file mocks\ListOfTests.txt
	And I have append the line Foo.Tests.UnitTests2 to file mocks\ListOfTests.txt
	And I have added the arg TestList=mocks\ListOfTests.txt to NUnit console command line
	When I run NUnit console
	Then the exit code should be 0
	And the Test Run Summary should has following:
	| field        | value |
	| Tests run    | 3     |
	| Passed       | 3     |
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