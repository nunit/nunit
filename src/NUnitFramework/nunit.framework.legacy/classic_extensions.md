# Classic Assert Extension Methods

This document provides a comprehensive mapping of all ClassicAssert methods to their corresponding C#14 static extension methods on `Assert`.

## Overview

All extension methods provide 1:1 mapping with their corresponding ClassicAssert methods. Each ClassicAssert overload has exactly one matching extension method with identical signatures.

---

## Core Assertions (LegacyExtensions.cs)

### AreEqual

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.AreEqual(double, double, double)` | `Assert.AreEqual(double, double, double)` |
| `ClassicAssert.AreEqual(double, double, double, string, object[])` | `Assert.AreEqual(double, double, double, string, object[])` |
| `ClassicAssert.AreEqual(object, object)` | `Assert.AreEqual(object, object)` |
| `ClassicAssert.AreEqual(object, object, string, object[])` | `Assert.AreEqual(object, object, string, object[])` |

### AreNotEqual

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.AreNotEqual(object, object)` | `Assert.AreNotEqual(object, object)` |
| `ClassicAssert.AreNotEqual(object, object, string, object[])` | `Assert.AreNotEqual(object, object, string, object[])` |

### AreSame

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.AreSame(object, object)` | `Assert.AreSame(object, object)` |
| `ClassicAssert.AreSame(object, object, string, object[])` | `Assert.AreSame(object, object, string, object[])` |

### AreNotSame

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.AreNotSame(object, object)` | `Assert.AreNotSame(object, object)` |
| `ClassicAssert.AreNotSame(object, object, string, object[])` | `Assert.AreNotSame(object, object, string, object[])` |

### True / IsTrue

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.True(bool)` | `Assert.True(bool)` |
| `ClassicAssert.True(bool, string, object[])` | `Assert.True(bool, string, object[])` |
| `ClassicAssert.True(bool?)` | `Assert.True(bool?)` |
| `ClassicAssert.True(bool?, string, object[])` | `Assert.True(bool?, string, object[])` |
| `ClassicAssert.IsTrue(bool)` | `Assert.IsTrue(bool)` |
| `ClassicAssert.IsTrue(bool, string, object[])` | `Assert.IsTrue(bool, string, object[])` |
| `ClassicAssert.IsTrue(bool?)` | `Assert.IsTrue(bool?)` |
| `ClassicAssert.IsTrue(bool?, string, object[])` | `Assert.IsTrue(bool?, string, object[])` |

### False / IsFalse

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.False(bool)` | `Assert.False(bool)` |
| `ClassicAssert.False(bool, string, object[])` | `Assert.False(bool, string, object[])` |
| `ClassicAssert.False(bool?)` | `Assert.False(bool?)` |
| `ClassicAssert.False(bool?, string, object[])` | `Assert.False(bool?, string, object[])` |
| `ClassicAssert.IsFalse(bool)` | `Assert.IsFalse(bool)` |
| `ClassicAssert.IsFalse(bool, string, object[])` | `Assert.IsFalse(bool, string, object[])` |
| `ClassicAssert.IsFalse(bool?)` | `Assert.IsFalse(bool?)` |
| `ClassicAssert.IsFalse(bool?, string, object[])` | `Assert.IsFalse(bool?, string, object[])` |

### NotNull / IsNotNull

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.NotNull(object)` | `Assert.NotNull(object)` |
| `ClassicAssert.NotNull(object, string, object[])` | `Assert.NotNull(object, string, object[])` |
| `ClassicAssert.IsNotNull(object)` | `Assert.IsNotNull(object)` |
| `ClassicAssert.IsNotNull(object, string, object[])` | `Assert.IsNotNull(object, string, object[])` |

### Null / IsNull

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.Null(object)` | `Assert.Null(object)` |
| `ClassicAssert.Null(object, string, object[])` | `Assert.Null(object, string, object[])` |
| `ClassicAssert.IsNull(object)` | `Assert.IsNull(object)` |
| `ClassicAssert.IsNull(object, string, object[])` | `Assert.IsNull(object, string, object[])` |

### IsNaN

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.IsNaN(double)` | `Assert.IsNaN(double)` |
| `ClassicAssert.IsNaN(double, string, object[])` | `Assert.IsNaN(double, string, object[])` |
| `ClassicAssert.IsNaN(double?)` | `Assert.IsNaN(double?)` |
| `ClassicAssert.IsNaN(double?, string, object[])` | `Assert.IsNaN(double?, string, object[])` |

### IsEmpty

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.IsEmpty(string)` | `Assert.IsEmpty(string)` |
| `ClassicAssert.IsEmpty(string, string, object[])` | `Assert.IsEmpty(string, string, object[])` |
| `ClassicAssert.IsEmpty(IEnumerable)` | `Assert.IsEmpty(IEnumerable)` |
| `ClassicAssert.IsEmpty(IEnumerable, string, object[])` | `Assert.IsEmpty(IEnumerable, string, object[])` |

### IsNotEmpty

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.IsNotEmpty(string)` | `Assert.IsNotEmpty(string)` |
| `ClassicAssert.IsNotEmpty(string, string, object[])` | `Assert.IsNotEmpty(string, string, object[])` |
| `ClassicAssert.IsNotEmpty(IEnumerable)` | `Assert.IsNotEmpty(IEnumerable)` |
| `ClassicAssert.IsNotEmpty(IEnumerable, string, object[])` | `Assert.IsNotEmpty(IEnumerable, string, object[])` |

### Contains

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.Contains(object, ICollection)` | `Assert.Contains(object, ICollection)` |
| `ClassicAssert.Contains(object, ICollection, string, object[])` | `Assert.Contains(object, ICollection, string, object[])` |

---

## Numeric Assertions (LegacyExtensions.Numeric.cs)

### Zero

For each numeric type (int, uint, long, ulong, decimal, double, float):

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.Zero(T)` | `Assert.Zero(T)` |
| `ClassicAssert.Zero(T, string, object[])` | `Assert.Zero(T, string, object[])` |

### NotZero

For each numeric type (int, uint, long, ulong, decimal, double, float):

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.NotZero(T)` | `Assert.NotZero(T)` |
| `ClassicAssert.NotZero(T, string, object[])` | `Assert.NotZero(T, string, object[])` |

### Positive

For each numeric type (int, uint, long, ulong, decimal, double, float):

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.Positive(T)` | `Assert.Positive(T)` |
| `ClassicAssert.Positive(T, string, object[])` | `Assert.Positive(T, string, object[])` |

### Negative

For each numeric type (int, uint, long, ulong, decimal, double, float):

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.Negative(T)` | `Assert.Negative(T)` |
| `ClassicAssert.Negative(T, string, object[])` | `Assert.Negative(T, string, object[])` |

---

## Comparison Assertions (LegacyExtensions.Comparisons.cs)

### Greater

For each comparable type (int, uint, long, ulong, decimal, double, float, IComparable):

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.Greater(T, T)` | `Assert.Greater(T, T)` |
| `ClassicAssert.Greater(T, T, string, object[])` | `Assert.Greater(T, T, string, object[])` |

### Less

For each comparable type (int, uint, long, ulong, decimal, double, float, IComparable):

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.Less(T, T)` | `Assert.Less(T, T)` |
| `ClassicAssert.Less(T, T, string, object[])` | `Assert.Less(T, T, string, object[])` |

### GreaterOrEqual

For each comparable type (int, uint, long, ulong, decimal, double, float, IComparable):

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.GreaterOrEqual(T, T)` | `Assert.GreaterOrEqual(T, T)` |
| `ClassicAssert.GreaterOrEqual(T, T, string, object[])` | `Assert.GreaterOrEqual(T, T, string, object[])` |

### LessOrEqual

For each comparable type (int, uint, long, ulong, decimal, double, float, IComparable):

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.LessOrEqual(T, T)` | `Assert.LessOrEqual(T, T)` |
| `ClassicAssert.LessOrEqual(T, T, string, object[])` | `Assert.LessOrEqual(T, T, string, object[])` |

---

## Type Assertions (LegacyExtensions.Types.cs)

### IsAssignableFrom

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.IsAssignableFrom(Type, object)` | `Assert.IsAssignableFrom(Type, object)` |
| `ClassicAssert.IsAssignableFrom(Type, object, string, object[])` | `Assert.IsAssignableFrom(Type, object, string, object[])` |
| `ClassicAssert.IsAssignableFrom<T>(object)` | `Assert.IsAssignableFrom<T>(object)` |
| `ClassicAssert.IsAssignableFrom<T>(object, string, object[])` | `Assert.IsAssignableFrom<T>(object, string, object[])` |

### IsNotAssignableFrom

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.IsNotAssignableFrom(Type, object)` | `Assert.IsNotAssignableFrom(Type, object)` |
| `ClassicAssert.IsNotAssignableFrom(Type, object, string, object[])` | `Assert.IsNotAssignableFrom(Type, object, string, object[])` |
| `ClassicAssert.IsNotAssignableFrom<T>(object)` | `Assert.IsNotAssignableFrom<T>(object)` |
| `ClassicAssert.IsNotAssignableFrom<T>(object, string, object[])` | `Assert.IsNotAssignableFrom<T>(object, string, object[])` |

### IsInstanceOf

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.IsInstanceOf(Type, object)` | `Assert.IsInstanceOf(Type, object)` |
| `ClassicAssert.IsInstanceOf(Type, object, string, object[])` | `Assert.IsInstanceOf(Type, object, string, object[])` |
| `ClassicAssert.IsInstanceOf<T>(object)` | `Assert.IsInstanceOf<T>(object)` |
| `ClassicAssert.IsInstanceOf<T>(object, string, object[])` | `Assert.IsInstanceOf<T>(object, string, object[])` |

### IsNotInstanceOf

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `ClassicAssert.IsNotInstanceOf(Type, object)` | `Assert.IsNotInstanceOf(Type, object)` |
| `ClassicAssert.IsNotInstanceOf(Type, object, string, object[])` | `Assert.IsNotInstanceOf(Type, object, string, object[])` |
| `ClassicAssert.IsNotInstanceOf<T>(object)` | `Assert.IsNotInstanceOf<T>(object)` |
| `ClassicAssert.IsNotInstanceOf<T>(object, string, object[])` | `Assert.IsNotInstanceOf<T>(object, string, object[])` |

---

## Collection Assertions (LegacyExtensions.Collection.cs)

### AllItemsAreInstancesOfType

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.AllItemsAreInstancesOfType(IEnumerable, Type)` | `Assert.AllItemsAreInstancesOfType(IEnumerable, Type)` |
| `CollectionAssert.AllItemsAreInstancesOfType(IEnumerable, Type, string, object[])` | `Assert.AllItemsAreInstancesOfType(IEnumerable, Type, string, object[])` |

### AllItemsAreNotNull

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.AllItemsAreNotNull(IEnumerable)` | `Assert.AllItemsAreNotNull(IEnumerable)` |
| `CollectionAssert.AllItemsAreNotNull(IEnumerable, string, object[])` | `Assert.AllItemsAreNotNull(IEnumerable, string, object[])` |

### AllItemsAreUnique

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.AllItemsAreUnique(IEnumerable)` | `Assert.AllItemsAreUnique(IEnumerable)` |
| `CollectionAssert.AllItemsAreUnique(IEnumerable, string, object[])` | `Assert.AllItemsAreUnique(IEnumerable, string, object[])` |

### AreEquivalent

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.AreEquivalent(IEnumerable, IEnumerable)` | `Assert.AreEquivalent(IEnumerable, IEnumerable)` |
| `CollectionAssert.AreEquivalent(IEnumerable, IEnumerable, string, object[])` | `Assert.AreEquivalent(IEnumerable, IEnumerable, string, object[])` |

### AreNotEquivalent

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.AreNotEquivalent(IEnumerable, IEnumerable)` | `Assert.AreNotEquivalent(IEnumerable, IEnumerable)` |
| `CollectionAssert.AreNotEquivalent(IEnumerable, IEnumerable, string, object[])` | `Assert.AreNotEquivalent(IEnumerable, IEnumerable, string, object[])` |

### IsSubsetOf

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.IsSubsetOf(IEnumerable, IEnumerable)` | `Assert.IsSubsetOf(IEnumerable, IEnumerable)` |
| `CollectionAssert.IsSubsetOf(IEnumerable, IEnumerable, string, object[])` | `Assert.IsSubsetOf(IEnumerable, IEnumerable, string, object[])` |

### IsNotSubsetOf

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.IsNotSubsetOf(IEnumerable, IEnumerable)` | `Assert.IsNotSubsetOf(IEnumerable, IEnumerable)` |
| `CollectionAssert.IsNotSubsetOf(IEnumerable, IEnumerable, string, object[])` | `Assert.IsNotSubsetOf(IEnumerable, IEnumerable, string, object[])` |

### IsSupersetOf

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.IsSupersetOf(IEnumerable, IEnumerable)` | `Assert.IsSupersetOf(IEnumerable, IEnumerable)` |
| `CollectionAssert.IsSupersetOf(IEnumerable, IEnumerable, string, object[])` | `Assert.IsSupersetOf(IEnumerable, IEnumerable, string, object[])` |

### IsNotSupersetOf

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.IsNotSupersetOf(IEnumerable, IEnumerable)` | `Assert.IsNotSupersetOf(IEnumerable, IEnumerable)` |
| `CollectionAssert.IsNotSupersetOf(IEnumerable, IEnumerable, string, object[])` | `Assert.IsNotSupersetOf(IEnumerable, IEnumerable, string, object[])` |

### DoesNotContain

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.DoesNotContain(IEnumerable, object)` | `Assert.DoesNotContain(IEnumerable, object)` |
| `CollectionAssert.DoesNotContain(IEnumerable, object, string, object[])` | `Assert.DoesNotContain(IEnumerable, object, string, object[])` |

### IsOrdered

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `CollectionAssert.IsOrdered(IEnumerable)` | `Assert.IsOrdered(IEnumerable)` |
| `CollectionAssert.IsOrdered(IEnumerable, string, object[])` | `Assert.IsOrdered(IEnumerable, string, object[])` |
| `CollectionAssert.IsOrdered(IEnumerable, IComparer)` | `Assert.IsOrdered(IEnumerable, IComparer)` |
| `CollectionAssert.IsOrdered(IEnumerable, IComparer, string, object[])` | `Assert.IsOrdered(IEnumerable, IComparer, string, object[])` |

---

## String Assertions (LegacyExtensions.String.cs)

### StringContains

| ClassicAssert Method | Extension Method | Notes |
|---------------------|------------------|-------|
| `StringAssert.Contains(string, string)` | `Assert.StringContains(string, string)` | Renamed to avoid conflict with collection Contains |
| `StringAssert.Contains(string, string, string, object[])` | `Assert.StringContains(string, string, string, object[])` | Renamed to avoid conflict with collection Contains |

### DoesNotContain

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.DoesNotContain(string, string)` | `Assert.DoesNotContain(string, string)` |
| `StringAssert.DoesNotContain(string, string, string, object[])` | `Assert.DoesNotContain(string, string, string, object[])` |

### StartsWith

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.StartsWith(string, string)` | `Assert.StartsWith(string, string)` |
| `StringAssert.StartsWith(string, string, string, object[])` | `Assert.StartsWith(string, string, string, object[])` |

### DoesNotStartWith

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.DoesNotStartWith(string, string)` | `Assert.DoesNotStartWith(string, string)` |
| `StringAssert.DoesNotStartWith(string, string, string, object[])` | `Assert.DoesNotStartWith(string, string, string, object[])` |

### EndsWith

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.EndsWith(string, string)` | `Assert.EndsWith(string, string)` |
| `StringAssert.EndsWith(string, string, string, object[])` | `Assert.EndsWith(string, string, string, object[])` |

### DoesNotEndWith

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.DoesNotEndWith(string, string)` | `Assert.DoesNotEndWith(string, string)` |
| `StringAssert.DoesNotEndWith(string, string, string, object[])` | `Assert.DoesNotEndWith(string, string, string, object[])` |

### AreEqualIgnoringCase

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.AreEqualIgnoringCase(string, string)` | `Assert.AreEqualIgnoringCase(string, string)` |
| `StringAssert.AreEqualIgnoringCase(string, string, string, object[])` | `Assert.AreEqualIgnoringCase(string, string, string, object[])` |

### AreNotEqualIgnoringCase

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.AreNotEqualIgnoringCase(string, string)` | `Assert.AreNotEqualIgnoringCase(string, string)` |
| `StringAssert.AreNotEqualIgnoringCase(string, string, string, object[])` | `Assert.AreNotEqualIgnoringCase(string, string, string, object[])` |

### IsMatch

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.IsMatch(string, string)` | `Assert.IsMatch(string, string)` |
| `StringAssert.IsMatch(string, string, string, object[])` | `Assert.IsMatch(string, string, string, object[])` |

### DoesNotMatch

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `StringAssert.DoesNotMatch(string, string)` | `Assert.DoesNotMatch(string, string)` |
| `StringAssert.DoesNotMatch(string, string, string, object[])` | `Assert.DoesNotMatch(string, string, string, object[])` |

---

## File Assertions (LegacyExtensions.File.cs)

### AreEqual (Streams)

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `FileAssert.AreEqual(Stream, Stream)` | `Assert.AreEqual(Stream, Stream)` |
| `FileAssert.AreEqual(Stream, Stream, string, object[])` | `Assert.AreEqual(Stream, Stream, string, object[])` |

### AreEqual (FileInfo)

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `FileAssert.AreEqual(FileInfo, FileInfo)` | `Assert.AreEqual(FileInfo, FileInfo)` |
| `FileAssert.AreEqual(FileInfo, FileInfo, string, object[])` | `Assert.AreEqual(FileInfo, FileInfo, string, object[])` |

### AreEqual (File Paths)

| ClassicAssert Method | Extension Method | Notes |
|---------------------|------------------|-------|
| `FileAssert.AreEqual(string, string)` | `Assert.FileAreEqual(string, string)` | Renamed to avoid conflict with object AreEqual |
| `FileAssert.AreEqual(string, string, string, object[])` | `Assert.FileAreEqual(string, string, string, object[])` | Renamed to avoid conflict with object AreEqual |

### AreNotEqual (Streams)

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `FileAssert.AreNotEqual(Stream, Stream)` | `Assert.AreNotEqual(Stream, Stream)` |
| `FileAssert.AreNotEqual(Stream, Stream, string, object[])` | `Assert.AreNotEqual(Stream, Stream, string, object[])` |

### AreNotEqual (FileInfo)

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `FileAssert.AreNotEqual(FileInfo, FileInfo)` | `Assert.AreNotEqual(FileInfo, FileInfo)` |
| `FileAssert.AreNotEqual(FileInfo, FileInfo, string, object[])` | `Assert.AreNotEqual(FileInfo, FileInfo, string, object[])` |

### AreNotEqual (File Paths)

| ClassicAssert Method | Extension Method | Notes |
|---------------------|------------------|-------|
| `FileAssert.AreNotEqual(string, string)` | `Assert.FileAreNotEqual(string, string)` | Renamed to avoid conflict with object AreNotEqual |
| `FileAssert.AreNotEqual(string, string, string, object[])` | `Assert.FileAreNotEqual(string, string, string, object[])` | Renamed to avoid conflict with object AreNotEqual |

### Exists (FileInfo)

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `FileAssert.Exists(FileInfo)` | `Assert.Exists(FileInfo)` |
| `FileAssert.Exists(FileInfo, string, object[])` | `Assert.Exists(FileInfo, string, object[])` |

### Exists (File Path)

| ClassicAssert Method | Extension Method | Notes |
|---------------------|------------------|-------|
| `FileAssert.Exists(string)` | `Assert.FileExists(string)` | Renamed for clarity |
| `FileAssert.Exists(string, string, object[])` | `Assert.FileExists(string, string, object[])` | Renamed for clarity |

### DoesNotExist (FileInfo)

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `FileAssert.DoesNotExist(FileInfo)` | `Assert.DoesNotExist(FileInfo)` |
| `FileAssert.DoesNotExist(FileInfo, string, object[])` | `Assert.DoesNotExist(FileInfo, string, object[])` |

### DoesNotExist (File Path)

| ClassicAssert Method | Extension Method | Notes |
|---------------------|------------------|-------|
| `FileAssert.DoesNotExist(string)` | `Assert.FileDoesNotExist(string)` | Renamed for clarity |
| `FileAssert.DoesNotExist(string, string, object[])` | `Assert.FileDoesNotExist(string, string, object[])` | Renamed for clarity |

---

## Directory Assertions (LegacyExtensions.Directory.cs)

### AreEqual

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `DirectoryAssert.AreEqual(DirectoryInfo, DirectoryInfo)` | `Assert.AreEqual(DirectoryInfo, DirectoryInfo)` |
| `DirectoryAssert.AreEqual(DirectoryInfo, DirectoryInfo, string, object[])` | `Assert.AreEqual(DirectoryInfo, DirectoryInfo, string, object[])` |

### AreNotEqual

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `DirectoryAssert.AreNotEqual(DirectoryInfo, DirectoryInfo)` | `Assert.AreNotEqual(DirectoryInfo, DirectoryInfo)` |
| `DirectoryAssert.AreNotEqual(DirectoryInfo, DirectoryInfo, string, object[])` | `Assert.AreNotEqual(DirectoryInfo, DirectoryInfo, string, object[])` |

### Exists (DirectoryInfo)

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `DirectoryAssert.Exists(DirectoryInfo)` | `Assert.Exists(DirectoryInfo)` |
| `DirectoryAssert.Exists(DirectoryInfo, string, object[])` | `Assert.Exists(DirectoryInfo, string, object[])` |

### Exists (Directory Path)

| ClassicAssert Method | Extension Method | Notes |
|---------------------|------------------|-------|
| `DirectoryAssert.Exists(string)` | `Assert.DirectoryExists(string)` | Renamed for clarity |
| `DirectoryAssert.Exists(string, string, object[])` | `Assert.DirectoryExists(string, string, object[])` | Renamed for clarity |

### DoesNotExist (DirectoryInfo)

| ClassicAssert Method | Extension Method |
|---------------------|------------------|
| `DirectoryAssert.DoesNotExist(DirectoryInfo)` | `Assert.DoesNotExist(DirectoryInfo)` |
| `DirectoryAssert.DoesNotExist(DirectoryInfo, string, object[])` | `Assert.DoesNotExist(DirectoryInfo, string, object[])` |

### DoesNotExist (Directory Path)

| ClassicAssert Method | Extension Method | Notes |
|---------------------|------------------|-------|
| `DirectoryAssert.DoesNotExist(string)` | `Assert.DirectoryDoesNotExist(string)` | Renamed for clarity |
| `DirectoryAssert.DoesNotExist(string, string, object[])` | `Assert.DirectoryDoesNotExist(string, string, object[])` | Renamed for clarity |

---

## Usage Notes

### Requirements

- **C# Version**: These extensions require C# 14 or later
- **Framework**: Available in nunit.framework.legacy package
- **Reference**: Add `using NUnit.Framework;` to access extension methods

### Documentation

All extension methods use `/// <inheritdoc cref="..."/>` to inherit their documentation from the corresponding ClassicAssert methods, ensuring consistency and maintainability.

### Design Principles

1. **1:1 Mapping**: Each ClassicAssert overload has exactly one corresponding extension method
2. **No Optional Parameters**: Separate overloads are used instead of optional parameters
3. **Type Safety**: Extension methods preserve all type constraints and signatures from the original methods
4. **Naming Conflicts**: Where method names would conflict (e.g., string Contains vs. collection Contains), prefixes are added for clarity

### Migration Example

**Before (Classic Assert):**

```csharp
ClassicAssert.AreEqual(expected, actual);
ClassicAssert.IsTrue(condition, "Custom message", arg1, arg2);
StringAssert.Contains("substring", str);
```

**After (Extension Methods):**

```csharp
Assert.AreEqual(expected, actual);
Assert.IsTrue(condition, "Custom message", arg1, arg2);
Assert.StringContains("substring", str);
```
