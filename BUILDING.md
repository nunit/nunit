# Building NUnit 3.0

NUnit 3.0 consists of three separate layers: the Framework, the Engine and the Console Runner.
The source code is kept in two GitHub repositories at http://github.com/nunit/nunit and
http://github.com/nunit/nunit-console.

There are two ways to build NUnit: using the solution file in an IDE or through the build script.

## Solution Build

The framework is built using a single Visual Studio solution (nunit.sln on Windows
and nunit.linux.sln on Linux), which may be built with Visual Studio 2012+, SharpDevelop.
or MonoDevelop.

The solutions all place their output in a common bin directory. Console runner and engine
components are placed directly in the bin directory while framework components end up in
subdirectories net-2.0, net-3.5, net4.0, net4.5 and portable. Future platform
builds will cause new subdirectories to be created.

## Build Script

We use **Cake** (http://cakebuild.net) to build NUnit for distribution. The primary script that controls
building, running tests and packaging is build.cake. We modify build.cake when we need to add new
targets or change the way the build is done. Normally build.cake is not invoked directly but through
build.ps1 (on Windows) or build.sh (on Linux). These two scripts are provided by the Cake project
and ensure that Cake is properly installed before trying to run the cake script. This helps the
build to work on CI servers using newly created agents to run the build and we generally run it
the same way on our own machines.

The build shell script and build.cmd script are provided as an easy way to run the above commands.
In addition to passing their arguments through to build.cake, they can supply added arguments
through the CAKE_ARGS environment variable. The rest of this document will assume use of these commands.

There is one case in which use of the CAKE_ARGS environment variable will be essential, if not necessary.
If you are running builds on a 32-bit Windows system, you must always supply the -Experimental argument
to the build. Use set CAKE_ARGS=-Experimental to ensure this is always done and avoid having to type
it out each time.

Key arguments to build.cmd / build:
 * -Target, -t <task>                 The task to run - see below.
 * -Configuration, -c [Release|Debug] The configuration to use (default is Release)
 * -ShowDescription                   Shows all of the build tasks and their descriptions
 * -Experimental, -e                  Use the experimental build of Roslyn

The build.cake script contains a large number of interdependent tasks. The most
important top-level tasks to use are listed here:

```
 * Build               Builds everything. This is the default if no target is given.
 * Rebuild             Cleans the output directory and builds everything
 * Test                Runs all tests. Dependent on Build.
 * TestAllFrameworks   Runs all framework tests. Dependent on Build.
 * Test45              Tests the 4.5 framework without building first.
 * Test40              Tests the 4.0 framework without building first.
 * Test35              Tests the 3.5 framework without building first.
 * Test20              Tests the 2.0 framework without building first.
 * TestPortable        Tests the portable framework without building first.
 * Package             Creates all packages without building first. See Note below.
```

For a full list of tasks, run `build.cmd -ShowDescription`.

### Notes:
 1. By design, the Package target does not depend on Build. This is to allow re-packaging
    when necessary without changing the binaries themselves. Of course, this means that
    you have to be very careful that the build is up to date before packaging.

 2. For additional targets, refer to the build.cake script itself.
