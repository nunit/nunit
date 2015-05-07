Intergation testing

Project 'src\NUnitIntegration\nunit.integration.mocks' contains mocks of tests for specific scenarios of integration. 
Project 'src\NUnitIntegration\nunit.integration.tests' contains a logic to check integration scenarios, 
it is assumed that all integration tests, including other products, will be located here.

Output directory for these tests is 'bin\IntegrationTests'. 
It contains subfolders for each version of NUnit related to specific Net Framework.

The integration tests run 'nunit-console.exe' using specific command line arguments
and analyze output to check integration scenarios. 

Each integration scenario use some subset of tests (mocks of tests) to be checked.
These mocks of tests are located in the 'nunit.integration.mocks'.
Integration scenarios are linked to the certain mocks of tests using CategoryAttribute.

Integration tests for TeamCity are located in the 
'src\NUnitIntegration\nunit.integration.tests\TeamCity\TeamCityIntegrationTests.cs'. 
These tests are run for each scenario from 
'src\NUnitIntegration\nunit.integration.tests\TeamCity\CaseLists.cs' and for each .Net Framework 
(also for some other combinations such as using '--teamcity' 
command line argument or 'TEAMCITY_PROJECT_NAME' environment variable).