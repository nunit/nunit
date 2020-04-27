**FileOrDirectoryExistsConstraint** tests that a File or Directory exists.

#### Constructor

```C#
FileOrDirectoryExistsConstraint()
```

#### Syntax

```C#
Does.Exist
Does.Not.Exist
```

#### Modifiers

```C#
IgnoreDirectories
IgnoreFiles
```

#### Examples of Use

```C#
Assert.That(fileStr, Does.Exist);
Assert.That(dirStr, Does.Exist);
Assert.That(fileStr, Does.Not.Exist);
Assert.That(dirStr, Does.Not.Exist);

Assert.That(new FileInfo(fileStr), Does.Exist);
Assert.That(new DirectoryInfo(dirStr), Does.Exist);
```
