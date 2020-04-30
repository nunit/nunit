The DirectoryAssert class provides methods for comparing two directories
or verifying the existence of a directory. Directories may be provided
as DirectoryInfos or as strings giving the path to each directory.

```csharp
DirectoryAssert.AreEqual(DirectoryInfo expected, DirectoryInfo actual);
DirectoryAssert.AreEqual(DirectoryInfo expected, DirectoryInfo actual, 
    string message, params object[] args);

DirectoryAssert.AreNotEqual(DirectoryInfo expected, DirectoryInfo actual);
DirectoryAssert.AreNotEqual(DirectoryInfo expected, DirectoryInfo actual, 
    string message, params object[] args);

DirectoryAssert.Exists(DirectoryInfo actual);
DirectoryAssert.Exists(DirectoryInfo actual, 
    string message, params object[] args);

DirectoryAssert.Exists(string actual);
DirectoryAssert.Exists(string actual, 
    string message, params object[] args);

DirectoryAssert.DoesNotExist(DirectoryInfo actual);
DirectoryAssert.DoesNotExist(DirectoryInfo actual, 
    string message, params object[] args);

DirectoryAssert.DoesNotExist(string actual);
DirectoryAssert.DoesNotExist(string actual, 
    string message, params object[] args);
```

#### See also...
* [File and Directory Constraints](xref:constraints#file-and-directory-constraints)

