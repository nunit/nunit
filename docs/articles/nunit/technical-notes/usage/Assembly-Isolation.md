NUnit isolates test assemblies from its own code and from one another
by use of separate Processes and AppDomains.
   
By default, NUnit loads each test assembly into a separate **Process**
under the control of the [NUnit Agent](xref:nunitagent)
program. This allows NUnit to ensure that each assembly is loaded in the environment
for which it was built. Within the agent process, NUnit's own code runs in the primary   
**AppDomain** while the tests run in a separate **AppDomain**.
   
If desired, multiple test assemblies may be loaded into the same process and
even the same domain by use of the **-process** and **-domain** command-line
options. See Console Command Line](xref:ConsoleCommandLine).
