The TestOf attribute adds information about the class that is being tested. It can be applied to test fixtures and to tests.

The constructor takes the string name or the type of the class being tested. TestOf can also be specified on
a TestFixture or Test attribute.

```csharp
[TestFixture]
[TestOf(typeof(MyClass)]
public class MyTests
{
 [Test]
 public void Test1() { /* ... */ }

 [Test]
 [TestOf("MySubClass")]
 public void Test2() { /* ... */ }
}

[TestFixture(TestOf = typeof(MyClass))]
public class MyOtherTests
{
 [Test]
 public void Test1() { /* ... */ }

 [Test(TestOf = typeof(MySubClass))]
 public void Test2() { /* ... */ }
}
```

> [!NOTE]
> you can currently only have one TestOf attribute per fixture or test.
