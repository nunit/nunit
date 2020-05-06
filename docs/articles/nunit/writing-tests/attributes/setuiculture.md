The SetUICulture attribute is used to set the current UI Culture for the duration
of a test. It may be specified at the level of a test or a fixture. The UI culture
remains set until the test or fixture completes and is then reset to its original
value. If you wish to use the current culture setting to decide whether to run
a test, use the Culture attribute instead of this one.
	
Only one culture may be specified. Running a test under
multiple cultures is a planned future enhancement. At this time, you can 
achieve the same result by factoring out your test code into a private method 
that is called by each individual test method.

#### Examples:

```csharp
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  [SetUICulture("fr-FR")]
  public class FrenchCultureTests
  {
    // ...
  }
}
```

#### See also...
 * [Culture Attribute](Culture.md)]
 * [SetCulture Attribute](SetCulture.md)
