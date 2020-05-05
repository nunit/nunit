There are two ways of installing the adapter within Visual Studio. See below for info on how to choose.

 * Use the Extension Manager
 * Add it as a NuGet package to your solution. (Requires VS 2012 Update 1 or later)

##### Installing With the Extension Manager

To install the NUnit Test Adapter using the Extension Manager, follow these steps:

1. From within Visual Studio, select Tools | Extension Manager.
2. In the left panel of the Extension Manager, select Online Extensions
3. Locate (search for) the NUnit 3.0 Test Adapter in the center panel and highlight it.
4. Click 'Download' and follow the instructions.

Use the Extension Manager to ensure that the NUnit 3.0 Test Adapter is enabled.

##### Installing the NuGet Package

To add it is a NuGet package, you must have an active solution, then follow these steps:

1. From Tools menu, use Library Package Manager and select Manage NuGet packages for solution
2. In the left panel, select Online
3. Locate (search for) NUnit 3.0 Test Adapter in the center panel and highlight it
4. Click 'Install'
5. In the "Select Projects" dialog, you need to select at least one project to add the adapter to, see notes below.

##### How to choose between Extension and NuGet package

The Extension will apply to Visual Studio itself, and will work for all projects you use. All users of your solution need to install the Extension. If you use TFS Build you must also install the extension to the build system there.

The NuGet Package will apply to the solution, and will work for any other user too, as it follows the solution, but requires the user to have VS 2012 Update 1 or above. It will also work for any TFS 2012 Update 1 or above server build, including TF Service and requires no further installation.

If you are testing **.NET Core or .NET Standard** you must use the NuGet package and Visual Studio 2017 or newer. Visual Studio does not support running .NET Core tests using an extension. The extension will only work with the full .NET Framework.
