namespace NUnit.Framework.Tests;

/// <summary>
/// This test is for verifying the output of test runs when developing the build scripts
/// </summary>
public class FailingTestForInternalUse
{
    // [Explicit]
    [Test]
    public void Test1()
    {
        Assert.Fail("Deliberately fails");
    }
}
