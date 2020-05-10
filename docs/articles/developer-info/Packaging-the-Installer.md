Before packaging the installer, you must first package and release the Console and Engine. See [Packaging the Console and Engine](Packaging-the-Console-and-Engine.md)

## Prepare the Release

1. Get latest from master
1. Update CHANGES.TXT. Set the date of the release, and list any packages which have been upgraded since the last release.
1. Check the `version` and `displayVersion` in `build.cake`. They should have been updated at the last release.
1. Package the release, `.\build.ps1` or `.\build.cmd`
1. Check the `distribution` directory for `NUnit.{VERSION}.msi` and `NUnit.{VERSION}.zip`

## Test the Release

### Test the Installer

1. Install `NUnit.{VERSION}.msi`
1. Ensure it installs correctly
1. Check that the extensions included in `build.cake` are installed
1. Run unit tests using the install
1. Ensure the extensions work by running NUnit 2 tests and by creating NUnit 2 test results
1. Check the version in the `nunit3-console.exe` output headers when running tests.

### Test the ZIP File

1. Unzip `NUnit.{VERSION}.zip`
2. Check that the extensions included in `build.cake` are installed
3. Run unit tests using the install
4. Ensure the extensions work by running NUnit 2 tests and by creating NUnit 2 test results
5. Check the version in the `nunit3-console.exe` output headers when running tests.

## Archiving the Release

Packages are archived on nunit.org in the downloads directory. Add the MSI and ZIP to the existing downloads/nunit/v3 for the Console/Engine release.

## Publishing the Release

1. Log onto Github and go to the main nunit-console repository at https://github.com/nunit/nunit-console.
2. Go to the releases tab and edit the existing Console release
3. Add the MSI and ZIP files to the release
4. Update the website if required

## Tag the Release

1. Still on `master`, tag the release with the version, `git tag v3.5`
2. Push the tags to GitHub, `git push --tags`

## Increment the Version

1. Create a release branch `git checkout -b release-3.5`
2. Increment the `version` and `displayVersion` in `build.cake`.
3. Check-in your changes and push to GitHub
4. Create a Pull Request, have it reviewed, merged and delete the branch
5. Close the milestone if one exists

> [!NOTE]
> The release branch in this project is not like the release branches in other projects, we don't save it. If we need to go back and do hotfixes, we will branch off the tag that was created for the release. We do this because there are usually no changes required to this repository for a release except incrementing the version post release. Any larger changes to this repository should be done and tested prior to a release using the normal Pull Request workflow.