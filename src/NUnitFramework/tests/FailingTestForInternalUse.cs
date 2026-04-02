namespace NUnit.Framework.Tests;

public class FailingTestForInternalUse
{
    [Test]
    public void Test1()
    {
        Assert.Fail("Deliberately fails");
    }
}
