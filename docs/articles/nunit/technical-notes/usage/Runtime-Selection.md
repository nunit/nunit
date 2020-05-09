By default, NUnit runs tests under the runtime version for which the test 
assembly was built, provided it is available on the test machine. If it is not available,
NUnit runs the assembly under the best available runtime that will allow it to run. If
no suitable runtime can be found, an error is reported.

#### Overriding the Defaults

The default runtime framework may be overridden using command line options.
In all cases, NUnit will attempt to honor the options given, issuing an
error message if the assembly cannot be loaded.
See [Console Command Line](xref:ConsoleCommandLine) for more information.

 * The **/framework** option of console runner allows you to specify
   the framework type and version to be used for a test run. When this option
   is used, NUnit will attempt to run the tests under the framework specified
   even if the assembly targets a different runtime.

 * The **/process:Single** command-line option indicates that tests should
   be run in the NUnit process itself. This forces usage of the runtime under which
   NUnit is already running.
  
 * The **process:Separate** causes a single separate process to be used
   for all assemblies. In this case, NUnit will use the highest level runtime targeted
   by any of the assemblies, if it is available.

