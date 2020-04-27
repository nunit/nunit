**FileOrDirectoryExistsConstraint** tests that a File or Directory exists.

#### Constructor

```csharp
FileOrDirectoryExistsConstraint()
```

#### Syntax

```csharp
Does.Exist
Does.Not.Exist
```

#### Modifiers

```csharp
IgnoreDirectories
IgnoreFiles
```

#### Examples of Use

```csharp
Assert.That(fileStr, Does.Exist);
Assert.That(dirStr, Does.Exist);
Assert.That(fileStr, Does.Not.Exist);
Assert.That(dirStr, Does.Not.Exist);

Assert.That(new FileInfo(fileStr), Does.Exist);
Assert.That(new DirectoryInfo(dirStr), Does.Exist);
```
