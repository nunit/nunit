namespace NUnit.Framework.Tests;

/// <summary>
/// This test is for verifying the output of test runs when developing the build scripts
/// </summary>
public class FailingTestForInternalUse
{
    // [Explicit]
    [Test]
    public void DeliberatelyFailingTest()
    {
        int x = 42;
        Assert.That(x, Is.Not.EqualTo(42), "Deliberately fails");
    }
}
