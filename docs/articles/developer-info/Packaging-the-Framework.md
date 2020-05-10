This note describes how to create release packages for the NUnit Framework. Currently, all the builds and packaging must be done on a single Windows machine. This is likely to change in the future as we add more platforms.

> [!NOTE]
> These instructions assume that you are creating releases for SilverLight and the Compact Framework at the same time as the general release. If that's not the case, you may skip the steps marked CF or SL. To release either build separately, perform those steps in sequence.

Software Prerequisites
----------------------

Various software combinations and environments may be used to build NUnit components. The following software is what we have used and tested for building everything and creating the release packages. We'll add options to the list as we find them.

1. Visual Studio 2017 with the NuGet Package manager.
2. .NET Core 1.1.4

Preparing for Release
---------------------

#### Create a Release Branch

All work on releases should be done on a branch.

1. Fetch and pull latest from master
2. Check out the release branch
3. Merge latest into the release branch
4. As you make the changes below, push the branch to GitHub and create a Pull Request to allow other team members to review your changes.
5. **Do not merge the release branch or PR**, we will create a separate PR to merge the changes back into master.

#### Make Sure it Works!

1. Close all instances of Visual Studio or other IDE to ensure that no changes are left unsaved.

2. Do a clean build and run all the tests on Windows. You may use the command below or three separate commands if preferred. If you encounter errors at any stage, you're not actually ready to release!

      `build.cmd -Target=Clean`
      `build.cmd -Target=Test`

3. Repeat the build on a Linux system, if available. If this is not possible, be sure to scrutinize the results from the Travis CI build carefully. On Linux, you may use the command

      `./build -Target=Test`

4. Make sure that the most recent commits of master passed all tests in the CI builds. Check the builds on both Travis and AppVeyor.

#### Review Milestone Status

1. Check the milestone for the current release to see that there are no open issues. Any issues that are not going to be in this release should be moved to a future milestone. This may be the time to create the next milestone.

2. Make sure that completed issues are marked with the appropriate 'closed' label depending on disposition. The release notes will end up reflecting issues marked closed:done.

3. Check all future open milestones for completed issues. Anything that is completed will be included in this release so change its milestone to the current release.

#### Check Assembly Versioning

AssemblyVersion and AssemblyFileVersion are set separately for the framework, engine, engine api and console runner. Each is kept in a separate file and they may be updated separately. Using the 3.4.1 release as an example, version information was set as follows:

      Component             | File to Update      | AssemblyVersion | AssemblyFileVersion 
      --------------------- | ------------------- | --------------- | -------------------
      Framework             | FrameworkVersion.cs |     3.6.0.0     |      3.6.0.0

#### Update Copyright Year

The copyright year in all the source files is only updated as they are changed, but the copyright in the `[assembly: AssemblyCopyright("...")]` and the copyright text displayed by `nunitlite` should be updated to the year of the release. Search for `AssemblyCopyright` in the solution and update it where needed, then check `TextUI.cs` in `nunitlite-runner` for default values used when no attribute is found.

If necessary, update the year in the general copyright notice LICENSE.txt. Note that these copyright notices refer to each of the packages in their entirety. Each of the `.nuspec` files in the `nuget` subdirectory contains a copyright line, which should also be updated.

Notices at the top of each source code file are only updated when copyrightable changes are made to the file, not at the time of release.

#### Update Package Versions

The package version is updated in the `build.cake` file. The following lines appear near the beginning of the file. Update the versions and modifiers as necessary.

```csharp
var version="3.6.0";
var modifier=""
```

The version variables are three-part version numbers that follow the basic principles of [semantic versioning]. Since we publish a number of NuGet packages, we use the NuGet implementation of semantic versioning. 

For NUnit, the major version is updated only rarely. Normal releases will update the minor version and set the third component to zero. The third component is incremented when "hot fixes" are made to a production release or for builds created for a special purpose. 

For pre-release versions, a non-empty modifier is specified. This is a suffix added to the version. Our standard suffixes are currently `-alpha-n`, `-beta-n` and `-rc-n` The build script adds an additional suffix of -dbg to any packages created using a Debug build.

> [!NOTE]
> The first alpha, beta or rc release may omit the `-n`. In that case, any following alpha, beta or rc should use `-2`.

#### Update CHANGES File

The `CHANGES.md` file in the project root contains all relevant changes for each release. It contains the same information as the release notes in the project documentation, in text format. Because the CHANGES file includes the **date** of the release, you must know when the release is coming out in order to edit it. Otherwise, it will be necessary to make a final change to the file at the point of making the release.

Create new sections in the CHANGES file to match those for prior releases. To ensure that all changes are included, review closed issues in the current and any future milestones. If an issue for a previous milestone was actually completed and closed, move it to the current milestone, since that's where it is being released. Include all issues resolved as closed:done in the issues section of the file. Significant feature additions and changes should be documented, even if they are also listed with issue numbers. Reviewing commits and merged pull requests may help in catching additional changes.

You should commit the CHANGES file separately from the version number changes since that commit will be merged back into master while the version changes will not.

#### Update the Documentation

The [Release Notes](https://github.com/nunit/docs/wiki/Release-Notes) section of the documentation wiki should match the content of the CHANGES.txt file except for any format differences.

> [!NOTE]
Now that the documentation is being kept in a github wiki, it may be possible to use the github markdown text directly in this file for future releases.

For any significant changes to how NUnit is used or what it does, the appropriate pages of the documentation should be updated or new pages created. If you do this in advance of the release (which is actually a good idea) you should do it in a way that the new documentation is not visible until the release is actually made.

#### Regenerate the Public API Files

TODO: Add detail during the next release...

During each release, we will have to manually delete the entire contents of each of the eight Shipped/Unshipped files, then use the light bulb fix on the warnings in the error list to repopulate the Unshipped files, then cut the contents of each Unshipped file and paste

#### Push All Changes

If you made any changes to the files in git as part of the preceding steps. Make sure you have pushed them and they have been reviewed in the PR.

Creating the Release
--------------------

1. Clear the package directory to avoid confusion:

      `erase package\*`

   This is not absolutely required, but will be helpful if you have other release packages present
   in the directory.

2. You should be working on the release branch. Do a pull to make sure you have everything up to date. If changes of any significance were merged, you should test again before creating the release.

3. Ensure that the release build is up to date. If you have any doubt whether the latest code changes 
   have actually been built, do a clean build. If the build is up to date you may skip this step.

      `build -Target Build`

4. Create the image directory

      `build -Target CreateImage`

   You do this to ensure that the latest build is used for packaging. If the `images` directory does
   not already contain a subdirectory named for this release (package version and suffix) you may skip
   this step as a new one will be created automatically.

5. Create the packages by running:

      `build -Target Package`

6. Verify that the correct packages have been created in the `package` sub-directory:

  * NUnit-VERSION.zip
  * NUnit.VERSION.nupkg
  * NUnitLite.VERSION.nupkg

Testing the Release
-------------------

The degree to which each package needs testing may vary depending on what has been changed. Here is a minimal set of tests for each package.

 * **Binary Zip** Unzip the file, change into the bin directory and use an install of the console to run all the tests.

```
nunit3-console.exe net-2.0\nunit.framework.tests.dll
nunit3-console.exe net-3.5\nunit.framework.tests.dll
nunit3-console.exe net-4.0\nunit.framework.tests.dll
nunit3-console.exe net-4.5\nunit.framework.tests.dll
portable\nunitlite-runner.exe portable\nunit.framework.tests.dll
portable\nunitlite.tests.exe
```

 * **NuGet Packages** Create a test project. Install the packages and verify that they apply to the project correctly. Run tests.

Archiving the Release
---------------------

Packages are archived on nunit.org in the downloads directory. Create a new subfolder under downloads/nunit/v3 for the release. Upload all the package files into that directory.

> [!NOTE]
> We need to develop a plan for additionally archiving the image directory. For the moment, the binary zip package provides a backup but it's possible that the two may diverge in the future.

Publishing the Release
----------------------

#### Github

1. Log onto Github and go to the main nunit repository at https://github.com/nunit.nunit.

2. Select Releases and then click on the "Draft a new release" button.

3. Enter a tag to be used for the release. Currently our tags are simply the version of the release, like 3.0.0-alpha-5. If you start typing with '3' you'll get a list of earlier release tags so you can see the format. **Select your release branch** as the target for the tag.

4. Enter a title for the release, like NUnit 3.0 RC release. If you type 'N' you'll get some hints.

5. Add a description of the release. It will be easier if you have prepared this in advance and can just paste it in.

6. If this is an Alpha or Beta release, check the box that indicates a pre-release.

7. Upload the packages you created earlier either directly from the package directory or from their archive location. Note that we upload all the packages, including those that are also published on NuGet.

8. Click the "Publish release" button to publish the release on Github.

#### NuGet

1. Sign on to nuget.org.

2. Select Upload package.

3. Browse to the location of the NUnit.VERSION.nupkg you created and upload it.

4. Verify that the info is correct and click the "Submit" button.

5. Repeat steps 2-4 for NUnitLite.VERSION.nupkg

#### Merge into Master

1. Close your release Pull Request
2. Create a new branch off of your release branch
3. Increment the version in the various locations. For example 3.5 -> 3.6. See the Update Package Versions section above.
4. Push your changes to GitHub
5. Create a pull request from your branch making sure it is based off master

#### Website

If changes to the website have been accumulated in a branch, now is the time to merge it and upload the pages to the site.

#### Notify Users

Send notifications to the mailing list and twitter.

#### Close the Milestone

The milestone representing this release should be closed at this time.

[semantic versioning]:http://semver.org/