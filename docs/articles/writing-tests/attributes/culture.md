The Culture attribute is used to specify cultures for which a test or fixture should be run. It does not affect the culture setting, but merely uses it to determine whether to run the test. If you wish to change the culture when running a test, use the SetCulture attribute instead.
	
If the specified culture requirements for a test are not met it is skipped. In the gui, the tree node for the test remains gray and the status bar color is not affected.

One use of the Culture attribute is to provide alternative tests under different cultures. You may specify either specific cultures, like "en-GB" or neutral cultures like "de".

#### Test Fixture Syntax

```C#
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  [Culture("fr-FR")]
  public class FrenchCultureTests
  {
    // ...
  }
}
```

#### Test Syntax

```C#
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  public class SuccessTests
  {
    [Test]
    [Culture(Exclude="en,de")]
    public void SomeTest()
    { /* ... */ }
}
```

#### See also...
 * [[SetCulture Attribute]]

