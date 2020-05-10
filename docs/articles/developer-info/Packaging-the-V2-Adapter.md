Introduction
------------
There are two purposes for building the adapter, one is for creating the packages for a  release - which is what this page is about, the other is for creating whatever you need for debugging or testing purposes.

The procedure described here is for those people who need to release a new version of the adapter.

Preparing the source code
-------------------------

#### Versioning
The version numbers follow the basic principles of [semantic versioning]. 
(The fourth number is used for debug versions under development, and will always be 0 for release versions.)

The version numbers have to be edited in the following files, and should match:

* **AssemblyInfo.cs**,  found in the NUnitTestAdapter project
-- change both file and assembly version number
* **source.extensions.vsixmanifest**, found under the NUnitTestAdapterInstall project
-- change Version tag
* **nunit-vs-adapter.build**, found under the Solution Items folder. -- change the version number, but only use the two first digits.
* **license.rtf**, found under the NUNitTestAdapterInstall project.  If the major/minor number has changed, update that here, 2nd line. If year is changed, update copyright years accordingly. 


Build
-----
Build a release version, AnyCPU.



Packaging
------

Use NAnt and use the package target `NAnt package`
Run this from the solution root folder

The resulting files can be found in the "package" folder:

  * **NUnitVisualStudioTestAdapter-[VERSION].vsix**  This is the extension for Visual Studio, which is uploaded to the [Visual Studio Gallery]. 

  * **NUnitVisualStudioTestAdapter-[VERSION].zip**  This is a zipped package for use with TFS Server Builds when you don't use the NuGet package in your solution. See  [this blog] for more information. 

  * **NUnitVisualStudioTestAdapter-[VERSION].nupkg** This is the NuGet package, which is uploaded to [NuGet for the adapter]

  * **NUnitVisualStudioTestAdapterAndFramework-[VERSION].nupkg** This is a NuGet package which includes the NUnit 2.6.3 framework, uploaded to [NuGet for the adapter with framework]   

#### Publishing the Release

1. Create a release on GitHub. Few people use this directly, but it is the benchmark release and provides an archive of all past releases, so we do this first. Github will automatically create zip and tar files containing the source. In addition, upload all four packages created above as a part of the release.

2. Upload the vsix package to the [Visual Studio Gallery] using the NUnitDeveloper account. If you don't have access to that account, ask one of the committers with access to do the upload for you.

3. Upload the two NuGet packages to nuget.org. You use your own account for this but you must have been pre-authorized in order for it to work. If you are not authorized, ask a committer with access to do it for you.

4. Update the documentation pages in this wiki as needed. In order to do this quickly after publishing the packages, you may want to clone the wiki repository and prepare the update in advance.

5. Update the website as needed. The website is maintained in the [nunit.org repository] to which all committers have access. You should create a branch like 'release-n.n' and make the necessary changes there. There are three vsAdapterXxxxx files that will probably require updating. Add an announcement to the home page and remove any announcement for an older version of the adapter.Create a pull request to merge your changes into the master branch. For rapid publication, you should create the PR and have it reviewed in advance, performing the merge after the packages are published.

6. Publicize the release, first announcing it on the nunit-developer and nunit-discuss lists and then more widely as appropriate. [We should develop a list of places.]

##### Notes:
  * Publishing the release requires access to various online accounts, which are mentioned above. For obvious reasons, the passwords are not provided. Contact Charlie or Terje if you need this access.
  * The website and wiki contain duplicate information at this time. In future, the duplication will be eliminated.
  * When a change is merged into the nunit.org master branch, one of the project leaders uploads it manually to the web site. This will be automated in the future.

Prerequisites
-----
1. **Visual Studio 2013**
You need Visual Studio 2013.  We use the ultimate edition, but it should be enough with the premium edition.  (I will probably work with both the Pro or the Express editions too, but we haven't tried them).  The latest 1.1 version is built using Update 2 RC. 

1. **Visual Studio 2013 SDK**  
You need this to work with the vsix.  Download from <http://www.microsoft.com/en-us/download/details.aspx?id=40758>

1. **NAnt**
Download from <http://nant.sourceforge.net/>.  We use the 0.92 version.

1. **NuGet**
You need the nuget.exe in your path.  Download the exe from <http://nuget.codeplex.com/downloads/get/784779>.  We use the 2.8 version

1. **VS2012 TestPlatform object model**
You need to have this around, the adapter and the test project refer to this.  The easiest way to get it, is to have VS2012 installed and get it from there. 
It is located at a location similar to "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow" and is named Microsoft.VisualStudio.TestPlatform.ObjectModel.dll
1. 1. You might need to fix up these references if the locations doesn't match what has been used.


Github
------
1) Close the Milestone

2) Draft and publish a Release, named like "Version 2.0".  Add all the binaries to the same release.  Tag the release like V2.0

Documentation
------
The adapter release notes should be updated.  

The file is named **vsTestAdapterReleaseNotes.html**, and is found under Docs/2.6.4 in the nunit.org repository, branch "vs-adapter-2.0"

Also check that the files **vsTestAdapterLicense.html** and **vsTestAdapterReleaseNotes.html** is up to date.









[semantic versioning]:http://semver.org/
[Visual Studio Gallery]:http://visualstudiogallery.msdn.microsoft.com/6ab922d0-21c0-4f06-ab5f-4ecd1fe7175d
[NuGet for the adapter]:http://www.nuget.org/packages/NUnitTestAdapter/
[NuGet for the adapter with framework]:http://www.nuget.org/packages/NUnitTestAdapter.WithFramework/
[nunit.org repository]:http://github.com/nunit/nunit.org