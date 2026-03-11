namespace NUnit.Framework.Tests;

public class FailingTest
{
    [Test]
    public void Test1()
    {
        Assert.Fail();
    }
}
