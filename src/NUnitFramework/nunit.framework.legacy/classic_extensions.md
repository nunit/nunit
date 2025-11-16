# Classic Assert Extension Methods

This document provides a comprehensive mapping of all ClassicAssert methods to their corresponding C#14 static extension methods on `Assert`.

## Overview

All extension methods provide 1:1 mapping with their corresponding ClassicAssert methods. Each ClassicAssert overload has exactly one matching extension method with identical signatures.

---

## Core Assertions (ClassicExtensions.cs)

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

## Numeric Assertions (ClassicExtensions.Numeric.cs)

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

## Comparison Assertions (ClassicExtensions.Comparisons.cs)

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

## Type Assertions (ClassicExtensions.Types.cs)

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

## Usage Notes

### Requirements

- **C# Version**: These extensions require C# 14 or later
- **Framework**: Available in nunit.framework.legacy package
- **Reference**: Add `using NUnit.Framework;` to access extension methods

### Documentation

All extension methods use `/// <inheritdoc cref="..."/>` to inherit their documentation from the corresponding ClassicAssert methods, ensuring consistency and maintainability.

### Design Principles

1. **1:1 Mapping**: Each ClassicAssert overload has exactly one corresponding extension method
2. **No Optional Parameters**: Separate overloads are used instead of optional parameters, as in the Classic Asserts.
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
