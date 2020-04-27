Usually, once an assertion fails, we want the test to terminate. But sometimes, it's desirable to continue and accumulate any additional failures so they may all be fixed at once. This is particularly useful for testing things like object initialization and UI appearance as well as certain kinds of integration testing.

### Syntax

Multiple asserts are implemented using the `Assert.Multiple` method. Here is an example of its use:

```csharp
[Test]
public void ComplexNumberTest()
{
    ComplexNumber result = SomeCalculation();

    Assert.Multiple(() =>
    {
        Assert.AreEqual(5.2, result.RealPart, "Real part");
        Assert.AreEqual(3.9, result.ImaginaryPart, "Imaginary part");
    });
}
```

Functionally, this results in NUnit storing any failures encountered in the block and reporting all of them together upon exit from the block. If both asserts failed, then both would be reported. The test itself would terminate at the end of the block if any failures were encountered, but would continue otherwise.

#### Notes:

1. The multiple assert block may contain any arbitrary code, not just asserts.

2. Multiple assert blocks may be nested. Failure is not reported until the  outermost block exits.

3. If the code in the block calls a method, that method may also contain multiple assert blocks.

4. The test will be terminated immediately if any exception is thrown that is not handled. An unexpected exception is often an indication that the test itself is in error, so it must be terminated. If the exception occurs after one or more assertion failures have been recorded, those failures will be reported along with the terminating exception itself.

5. Assert.Fail is handled just as any other assert failure. The message and stack trace are recorded but the test continues to execute until the end of the block.

6. An error is reported if any of the following are used inside a multiple assert block:
   * Assert.Pass
   * Assert.Ignore
   * Assert.Inconclusive
   * Assume.That

7. Use of Warnings (Assert.Warn, Warn.If, Warn.Unless) is permitted inside a multiple assert block. Warnings are reported normally along with any failures that occur inside the block.

### Runner Support

Multiple assertion failures per test are stored in the representation of the test result using new XML elements, which are not recognized by older runners. The following runners are known to support display of the new elements:

 * NUnit Console Runner 3.6
 * NUnit 3 Visual Studio Adapter 3.7
 * NUnit Gui Runner (under development)

#### Compatibility

Older runners generally display a single failure message and stack trace for each test. For compatibility purposes, the framework creates a single message that lists all the failures. The stack trace in such a case will indicate the end of the assert multiple block.
