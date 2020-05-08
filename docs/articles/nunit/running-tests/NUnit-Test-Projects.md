---
uid: nunittestprojects
---

Although you may simply enter multiple assembly names on the console command-line, running tests from multiple assemblies is facilitated by the use of NUnit test projects. These are
files with the extension .nunit containing information about the assemblies to be loaded. The
following is an example of a hypothetical test project file:

```xml
<NUnitProject>
  <Settings activeconfig="Debug"/>
  <Config name="Debug">
    <assembly path="LibraryCore\bin\Debug\Library.dll"/>
    <assembly path="LibraryUI\bin\Debug\LibraryUI.dll"/>
  </Config>
  <Config name="Release">
    <assembly path="LibraryCore\bin\Release\Library.dll"/>
    <assembly path="LibraryUI\bin\Release\LibraryUI.dll"/>
  </Config>
</NUnitProject>
```

This project contains two configurations, each of which contains two assemblies. The Debug
configuration is currently active. By default, the assemblies will be loaded using the directory
containing this file as the ApplicationBase. The PrivateBinPath will be set automatically to
`LibraryCore\bin\Debug;LibraryUI\bin\Debug` or to the corresonding release path.
XML attributes are used to specify non-default values for the ApplicationBase, Configuration
File and PrivateBinPath. 

The [Project Editor](https://github.com/CharliePoole/nunit-project-editor/wiki/Project-Editor) may be used to create or modify NUnit projects. If you edit the XML manually, you should make sure to
use a path relative to the directory containing the project file as the location of any assemblies.

For details, see [[NUnit Project XML Format]]

#### Command-line Overrides

The following command-line options override what is specified in the NUnit project file:

* --config
* --domain
* --process
* --framework
