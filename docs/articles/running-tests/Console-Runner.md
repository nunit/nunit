The nunit3-console.exe program is a text-based runner for listing and running
our tests from the command-line. It is able to run all NUnit 3.0 or higher tests 
natively and can run NUnit 2.x tests if the v2 driver is installed.
  
This runner is useful for automation of tests and integration into other systems.
It automatically saves its results in XML format, allowing you to produce reports 
or otherwise process the results. The following is a screenshot of the console 
program output.

![Screen shot of nunit-console](nunit/images/console-mock.png)

In this example, nunit3-console has just run selected tests in the mock-nunit-assembly.exe 
assembly that is part of the NUnit distribution. This assembly contains a number of tests, some
of which are either ignored or marked explicit. The summary line shows the
result of the test run.
