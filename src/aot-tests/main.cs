using NUnit.Framework;
using NUnitLite;

Console.WriteLine(typeof(CustomTests).Assembly.GetName());

return new AutoRun(
    new TypedFixtureBuilder().Add<CustomTests>()
).Execute(args);

[TestFixture]
public class CustomTests
{
    [Test]
    public void Test1()
    {
        Assert.That(1, Is.EqualTo(1));
    }
}
