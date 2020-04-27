`SubPathConstraint` tests that one path is under another path.

#### Constructor

```C#
SubPathConstraint(string expectedPath)
```

#### Syntax

```C#
Is.SubPath(string expectedPath)
```

#### Modifiers

```C#
...IgnoreCase
...RespectCase
```

#### Examples of Use

```C#
Assert.That("/folder1/./junk/../folder2",
	Is.SubPath("/folder1/folder2"));
Assert.That("/folder1/junk/folder2",
	Is.Not.SubPath("/folder1/folder2"));

Assert.That(@"C:\folder1\folder2\folder3",
	Is.SubPath(@"C:\Folder1\Folder2/Folder3").IgnoreCase);
Assert.That("/folder1/folder2/folder3",
	Is.Not.SubPath("/Folder1/Folder2/Folder3").RespectCase);
```

#### Notes:

1. Path constraints perform tests on paths, without reference to any
actual files or directories. This allows testing paths that are
created by an application for reference or later use, without 
any effect on the environment.
   
2. Path constraints are intended to work across multiple file systems,
and convert paths to a canonical form before comparing them. 

3. It is usually not necessary to know the file system of the paths
in order to compare them. Where necessary, the programmer may
use the **IgnoreCase** and **RespectCase** modifiers to provide 
behavior other than the system default.
      
#### See also...
 * [[SamePathConstraint]]
 * [[SamePathOrUnderConstraint]]
