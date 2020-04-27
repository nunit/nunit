The FileAssert class provides methods for comparing or verifying the existence of files,
which may be provided as Streams, as FileInfos or as strings 
giving the path to each file.

```csharp
FileAssert.AreEqual(Stream expected, Stream actual);
FileAssert.AreEqual(
    Stream expected, Stream actual, string message, params object[] args);

FileAssert.AreEqual(FileInfo expected, FileInfo actual);
FileAssert.AreEqual(
    FileInfo expected, FileInfo actual, string message, params object[] args);

FileAssert.AreEqual(string expected, string actual);
FileAssert.AreEqual(
    string expected, string actual, string message, params object[] args);

FileAssert.AreNotEqual(Stream expected, Stream actual);
FileAssert.AreNotEqual(
    Stream expected, Stream actual, string message, params object[] args);

FileAssert.AreNotEqual(FileInfo expected, FileInfo actual);
FileAssert.AreNotEqual(
    FileInfo expected, FileInfo actual, string message, params object[] args);

FileAssert.AreNotEqual(string expected, string actual);
FileAssert.AreNotEqual(
    string expected, string actual, string message, params object[] args);

FileAssert.Exists(FileInfo actual);
FileAssert.Exists(
    FileInfo actual, string message, params object[] args);

FileAssert.Exists(string actual);
FileAssert.Exists(
    string actual, string message, params object[] args);

FileAssert.DoesNotExist(FileInfo actual);
FileAssert.DoesNotExist(
    FileInfo actual, string message, params object[] args);

FileAssert.DoesNotExist(string actual);
FileAssert.DoesNotExist(
    string actual, string message, params object[] args);
```

#### See also...
 * [File and Directory Constraints](Constraints#file-and-directory-constraints)
