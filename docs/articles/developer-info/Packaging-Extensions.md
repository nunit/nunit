The NUnit Console/Engine currently ships with the following extensions,

* NUnit.Extension.NUnitProjectLoader
* NUnit.Extension.VSProjectLoader
* NUnit.Extension.NUnitV2ResultWriter
* NUnit.Extension.NUnitV2Driver
* NUnit.Extension.TeamCityEventListener

All but the TeamCityEventListener are built and shipped by the NUnit team. These extensions must be built and released before building and releasing the Console/Engine, but only if they are changed and a release is planned. For the 3.5 release, all extensions will be built and released with the console. Future releases of each extension will be on an as-needed basis and the version numbers of the extensions and the console/engine will diverge over time.

#### Create a Release Branch

All work on releases should be done on a branch.

1. Fetch and pull latest from master
2. Create a branch in the form release-3.5
3. As you make the changes below, push the branch to GitHub and create a Pull Request to allow other team members to review your changes.
4. **Do not merge this branch/PR**, we will create a separate PR to merge the changes back into master.

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

#### Check Versioning

`AssemblyVersion` and `AssemblyFileVersion` are set in `src\extension\Properties\AssemblyInfo.cs` and should match the version in `build.cake`. These values are normally incremented after a release, but should be checked.

#### Update Copyright Year

The copyright year in all the source files is only updated as they are changed, but the copyright in the `[assembly: AssemblyCopyright("...")]` should be updated to the year of the release.

If necessary, update the year in the general copyright notice `LICENSE.txt`. The `.nuspec` files in solution root contains a copyright line, which should also be updated.

Notices at the top of each source code file are only updated when copyrightable changes are made to the file, not at the time of release.

#### Push All Changes

Make sure the release branch and any changes are pushed to GitHub, reviewed in a PR and all CI servers are passing.

#### Creating the Release

1. Clear the package directory to avoid confusion:

      `erase package\*`

   This is not absolutely required, but will be helpful if you have other release packages present
   in the directory.

2. You should be working on the release branch. Do a pull to make sure you have everything up to date. If changes of any significance were merged, you should test again before creating the release.

3. Ensure that the release build is up to date. If you have any doubt whether the latest code changes 
   have actually been built, do a clean build. If the build is up to date you may skip this step.

      `build -Target Build`

4. Create the packages by running:

      `build -Target Package`

5. Verify that the correct package has been created in the `package` sub-directory.

  * NUnit..Extension.{NAME}.{VERSION}.nupkg

Testing the Release
-------------------

Open the NuGet package from the `package` sub-directory using [NuGet Package Explorer](https://github.com/NuGetPackageExplorer/NuGetPackageExplorer). Inspect the package metadata for errors and check that the tools folder contains the extension dll and any dependent assemblies.

Next install the extension into a project with the `NUnit.ConsoleRunner` package and make sure you can run tests. If you are releasing the V2 driver, run NUnit 2 tests. If you are releasing the V2 Result Writer, write out V2 results.

Archiving the Release
---------------------

Packages are archived on nunit.org in the downloads directory. Create a new subfolder under `downloads/extensions/v3/EXTENSION_NAME` for the release. Upload all the package files into that directory.

Publishing the Release
----------------------

#### Github

1. Log onto Github and go to the extension repository.

2. Select Releases and then click on the "Draft a new release" button.

3. Enter a tag to be used for the release in the format v3.x.y **Select your release branch** as the target for the tag.

4. Enter a title for the release, like NUnit Project Loader Extension 3.5 release. If you type 'N' you'll get some hints.

5. Add a description of the release. It will be easier if you have prepared this in advance and can just paste it in.

6. If this is an Alpha or Beta release, check the box that indicates a pre-release.

7. Upload the `nupkg` package you created.

8. Click the "Publish release" button to publish the release on Github.

#### NuGet

1. Sign on to nuget.org.

2. Select Upload package.

3. Browse to the location of the `nupkg` you created and upload it.

4. Verify that the info is correct and click the "Submit" button.

#### Merge into Master

1. Close your release Pull Request
2. Create a new branch off of your release branch
3. Increment the version in the various locations. For example 3.5 -> 3.6. See the Update Package Versions section above.
4. Push your changes to GitHub
5. Create a pull request from your branch making sure it is based off master

#### Notify Users

Send notifications to the mailing list and twitter if the changes to the extension warrant it.

#### Close the Milestone

The milestone representing this release should be closed at this time.