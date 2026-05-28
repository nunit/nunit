# Bug Analysis Report: Top 10 Open Issues

| # | Issue | Has Repro | Has Unit Test | Repro → Unit Test? |
|---|-------|-----------|---------------|-------------------|
| 1 | [#5273](https://github.com/nunit/nunit/issues/5273) AssignableToConstraint<T> generic bug | No | **Partial** | Yes |
| 2 | [#4987](https://github.com/nunit/nunit/issues/4987) NUnitLite AOT failure | No | No | Hard (requires AOT build) |
| 3 | [#4826](https://github.com/nunit/nunit/issues/4826) Invalid character in test name | No | **Partial** | Yes |
| 4 | [#4426](https://github.com/nunit/nunit/issues/4426) TestFixture ignored with DatapointSource | **Yes** | No | Yes |
| 5 | [#4226](https://github.com/nunit/nunit/issues/4226) F# Range Attribute skips tests | No | No | Not a bug (F# behavior) |
| 6 | [#4140](https://github.com/nunit/nunit/issues/4140) IEnumerable+IEquatable hides errors | No | **Partial** | Yes |
| 7 | [#3970](https://github.com/nunit/nunit/issues/3970) UWP .NET native exception | No | No | Hard (requires UWP) |
| 8 | [#3979](https://github.com/nunit/nunit/issues/3979) ExplicitAttribute category filter | No | **Partial** | Duplicate of #4589 |
| 9 | [#3849](https://github.com/nunit/nunit/issues/3849) Assert.Catch in Assert.Multiple | **Yes** | No | **Yes** |
| 10 | [#3682](https://github.com/nunit/nunit/issues/3682) StopRun(true) not cancelling | No | **Partial** | Yes |

---

## Detailed Analysis

### #5273 - AssignableToConstraint<T> generic bug
- **Repro folder:** None
- **Tests exist:** `AssignableToConstraintTests.cs` and `AssignableFromConstraintTests.cs` exist but test both generic and non-generic together without catching the generic-specific logic error
- **Gap:** Tests don't verify that generic version uses `TExpected` correctly (it ignores it!)
- **Can create unit test:** YES - test polymorphism case: `Animal myPet = new Dog(); Assert.That(myPet, Is.AssignableTo<Animal>())`

### #4987 - NUnitLite AOT failure
- **Repro folder:** None
- **Tests exist:** No AOT-specific tests
- **Gap:** AOT/NativeAOT publish causes ArgumentException
- **Can create unit test:** HARD - requires AOT build environment

### #4826 - Invalid character `\uffff` in test name
- **Repro folder:** None
- **Tests exist:** `TestOutputTests.cs` tests some invalid chars (`\u001b`) but NOT `\uffff`
- **Gap:** The specific character `\uffff` is not tested
- **Can create unit test:** YES - add test case for `\uffff` in DisplayName sanitization

### #4426 - TestFixture silently ignored with DatapointSource + constructor exception
- **Repro folder:** `Issue4426/TestProject1/TestsNotDiscovered.cs`
- **Tests exist:** No tests for this scenario
- **Gap:** When constructor throws AND DatapointSource is present, fixture is silently ignored
- **Can create unit test:** YES - directly from repro code

### #4226 - F# Range Attribute skips tests
- **Repro folder:** None
- **Tests exist:** No
- **Note:** NOT A BUG - F# type inference issue when parameter unused. Workaround: use parameter or specify type

### #4140 - IEnumerable<T> + IEquatable<T> hides error messages
- **Repro folder:** None
- **Tests exist:** `SimpleEnumerableWithIEquatable.cs` helper exists, `NUnitEqualityComparerTests.cs` tests equality but not error messages
- **Gap:** No test verifying error message clarity for types implementing both interfaces
- **Can create unit test:** YES - assert on failure message content

### #3970 - UWP .NET native toolchain exception
- **Repro folder:** None
- **Tests exist:** No UWP-specific tests
- **Can create unit test:** HARD - requires UWP environment with .NET native toolchain

### #3979 - ExplicitAttribute with category filter
- **Repro folder:** None
- **Tests exist:** `ExplicitExecutionTest.cs` exists but doesn't test category filter combination
- **Note:** CLOSED as duplicate of #4589

### #3849 - Assert.Catch in Assert.Multiple gives unclear message
- **Repro folder:** `Issue3849/UnitTest1.cs`
- **Tests exist:** NO - no tests for Assert.Catch inside Assert.Multiple
- **Gap:** Critical - when exception type doesn't match, InvalidCastException is thrown instead of assertion failure
- **Can create unit test:** **YES - HIGH PRIORITY**

```csharp
[Test]
public void AssertCatchInsideMultiple_WrongExceptionType_ShouldGiveClearMessage()
{
    var result = Assert.Multiple(() =>
    {
        Assert.Catch<ArgumentException>(() => throw new InvalidOperationException("x"));
    });
    // Should NOT throw InvalidCastException
    // Should report "Expected: ArgumentException, But was: InvalidOperationException"
}
```

### #3682 - StopRun(true) not cancelling hanging test
- **Repro folder:** None
- **Tests exist:** `TestAssemblyRunnerTests.cs` has StopRun tests, `WorkItemTests.cs` has cancellation tests
- **Gap:** No test for infinite loop with I/O operations
- **Can create unit test:** YES - test cancellation of test with infinite loop

---

## Summary: Actionable Items

### High priority unit tests that can be created from repros:

1. **#3849** - Assert.Catch in Assert.Multiple (repro exists at `Issue3849/UnitTest1.cs`)
2. **#4426** - TestFixture with DatapointSource + constructor exception (repro exists at `Issue4426/`)

### Unit tests that should be added:

3. **#5273** - Generic AssignableToConstraint with polymorphic actual values
4. **#4826** - `\uffff` character in test name sanitization
5. **#4140** - Error message content for IEnumerable+IEquatable types
6. **#3682** - StopRun cancellation with I/O-heavy infinite loop
