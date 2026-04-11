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
        int x = TheAnswer();
        Verify(x);
    }

    private void Verify(int x)
    {
        Assert.That(x, Is.EqualTo(42), "Deliberately fails");
    }

    private int TheAnswer()
    {
        return 42;
    }

}
