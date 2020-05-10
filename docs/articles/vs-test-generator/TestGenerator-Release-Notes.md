#### TestGenerator Extension for NUnit in Visual Studio 2017 - Version 2.3 - September 20, 2019

#### Features

* [PR 33](https://github.com/nunit/nunit-vs-testgenerator/pull/33) [Create Unit Test] Fixing package versions.  This is needed in order to work for .net core projects.  With Visual Studio 2019, Update 3, it is possible to use this extension to create tests. 


#### TestGenerator Extension for NUnit in Visual Studio 2017 - Version 2.1 - November 22, 2018

Visual Studio 2017 15.9 and forward should have the adapter added to each test project.  This release ensures that by fixing #25. 


##### Features

* [25](https://github.com/nunit/nunit-vs-testgenerator/issues/25)  Add the test adapter as part of the package.  Thanks to [yowko](https://github.com/yowko) for the PR


#### TestGenerator Extension for NUnit in Visual Studio 2017 - Version 2 - March 5, 2017

##### Features
 * #8 Support for VS 2017


##### Bugs and minor fixes
  * #6 NUnit3 tests using removed ExpectedException attribute
  * #10 Specify NUnit versions as v2 and v3
  * #11 Update more information link
  * #14 Update NUnit version (will use latest version)



##### Notes
 * The TestGenerator extension is released as separate VSIXes for VS 2015 and VS 2017.  See [Installation](TestGenerator-Installation.md) for details.  The Version 2 is for VS2017, the Version 1 is for VS 2015. 