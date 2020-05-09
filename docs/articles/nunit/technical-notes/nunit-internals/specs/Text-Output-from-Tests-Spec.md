> **NOTE:** This page is a specification that was used as a starting point for creating the feature in NUnit. It needs to be reviewed and revised in order to accurately reflect what was actually built. If you take it with a grain of salt, it may still be helpful to you as documentation. This notice will be removed when the page is brought up to date.

#### Background

In the past, NUnit was able to capture text output (Console, Trace and Log) and associate it with the correct test. This was possible because only one test could run at a time, therefore any output received between the start and end of a particular test could be identified with that test.

In an environment where multiple tests may be running at the same time, this is no longer possible. Let's say that NUnit has received the following messages, in order:
* Start Test A
* Start Test B
* Some text output
* End Test A
* End Test B

In this situation, the text output might be from Test A or from Test B. NUnit cannot associate the output with a particular test, although it is possible that the user might be able to interpret the result based on the actual content of the text output.

#### Approach

Since it does run tests in parallel, NUnit 3.0 needs a new approach to handling text output:

* The `TestResult` class will have a new string property, `Output`, holding text output associated with the individual test.

* Console standard output will be captured and added to the test result.

* A separate facility will be added to the NUnit `TestContext` to allow tests to write directly to the test result.
  * **TestContext.Out** gets a `TextWriter` that may be used to write output to the TestResult. 

  * **TestContext.Write(...)** is a static method that writes output to the TestResult. Multiple overloads will be provided.

  * **TestContext.WriteLine(...)** is a static method that writes output to the TestResult. Multiple overloads will be provided.

* Trace and Log output will no longer be handled by NUnit at all.

* Console error output will not be captured, but will display directly to the Console when using any of the console runners.

#### Impact of Changes

Standard output sent to the console will not display until the test completes. It will be clearly associated with the test if test labels are in use. If error output is redirected to a file either through the operating system or by using a command-line option then it will be written to the file. When running in the Gui, it will be available for display when the test is selected.

Error output sent to the console will display immediately, provided a console is available. When running under the Gui or in another environment without a console attached, it will simply not appear. If error output is redirected to a file either through the operating system or by using a command-line option then it will be written to the file.

Trace output will be processed in the normal way by .NET, depending on the configuration. NUnit will handle this output in any way or even be aware of it. This is how Trace is normally intended to work with a program.

All log output will be processed by log4net (or any other logging facility) based on the configuration. NUnit will not handle it at all.

NUnit-Console will display this output as each test completes. Since the completion of a test is a single event, all output will be associated with the correct test. However, since the output is not available until the test completes, it will not be useful for indicating progress of long-running tests.

