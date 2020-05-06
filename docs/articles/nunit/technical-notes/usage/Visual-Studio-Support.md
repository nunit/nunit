## DRAFT - Out of Date

NUnit supports the Visual Studio project and solution format, which are also 
used by a number of other IDEs.

#### Running From Within Visual Studio

The most convenient way to do this is to set up a custom tool entry specifying the path to 
NUnit as the command. For a VS2003 C# project, you can use $(TargetPath) for the arguments and
$(TargetDir) for the initial directory. 

With Visual Studio VS2005 this becomes a bit harder, because that release changed the
meaning of the 'Target' macros so they now point to the intermediate 'obj' directories rather
than the final output in one of the 'bin' directories. Here are some alternatives that
work in both versions:
 * **$(ProjectDir)$(ProjectFileName)** to open the VS Project rather than the assembly.
    If you use this approach, be sure to rename your config file accordingly and put it
	in the same directory as the VS project file.
 * **$(ProjectDir)bin/Debug/$(TargetName)$(TargetExt)** to run the assembly directly.
    Note that this requires hard-coding part of the path, including the configuration.

If you would like to debug your tests, use the Visual Studio
Debug | Processes… menu item to attach to NUnit after starting it and set breakpoints in
your test code as desired before running the tests.

#### Opening Visual Studio Projects

When Visual Studio support is enabled, the File Open dialog displays the following supported
Visual Studio project types: C#, VB.Net, J# and C++. The project file is read and the
configurations and output assembly locations are identified. Since the project files do not contain
information about the most recently opened configuration, the output assembly for the first
configuration found (usually Debug) is loaded in the GUI. The tree shows the project as the top-level
node with the assembly shown as its descendant.

When tests are run for a Visual studio project, they run just as if the output assembly had been
loaded with one exception. The default location for the config file is the directory containing the
project file and its default name is the same as the project file with an extension of .config.
For example, the following command would load the tests in the nunit.tests assembly using the
configuration file nunit.tests.dll.config located in the same directory as the dll.
```
        nunit.exe nunit.tests.dll
```
On the other hand, the following command would load the tests using the configuration file
nunit.tests.config located in the same directory as the csproj file.
```
        nunit.exe nunit.tests.csproj
```
The same consideration applies to running tests using the console runner.

#### Opening Visual Studio Solutions

When Visual Studio support is enabled, solution files may be opened as well. All the output
assemblies from contained projects of the types supported will be loaded in the tree. In the case
where all contained projects are located in the subdirectories beneath the solution, it will be
possible to load and run tests using this method directly.

When a solution contains projects located elsewhere in the file system, it may not be possible to
run the tests - although the solution will generally load without problem. In this case, the Project
Editor should be use to modify and save the NUnit test project so that there is all referenced
assemblies are located in or beneath the application base directory.
