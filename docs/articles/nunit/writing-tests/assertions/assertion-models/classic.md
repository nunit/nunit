The classic Assert model uses a separate method to express each 
individual assertion of which it is capable.
   
Here's a simple assert using the classic model:

```csharp
      StringAssert.AreEqualIgnoringCase("Hello", myString);
```
   
The Assert class provides the most common assertions in NUnit:
 * [[Assert.True]]
 * [[Assert.False]]
 * [[Assert.Null]]
 * [[Assert.NotNull]]
 * [[Assert.Zero]]
 * [[Assert.NotZero]]
 * [[Assert.IsNaN]]
 * [[Assert.IsEmpty]]
 * [[Assert.IsNotEmpty]]
 * [[Assert.AreEqual]]
 * [[Assert.AreNotEqual]]
 * [[Assert.AreSame]]
 * [[Assert.AreNotSame]]
 * [[Assert.Contains]]
 * [[Assert.Greater]]
 * [[Assert.GreaterOrEqual]]
 * [[Assert.Less]]
 * [[Assert.LessOrEqual]]
 * [[Assert.Positive ]]
 * [[Assert.Negative]]
 * [[Assert.IsInstanceOf]]
 * [[Assert.IsNotInstanceOf]]
 * [[Assert.IsAssignableFrom]]
 * [[Assert.IsNotAssignableFrom]]
 * [[Assert.Throws]]
 * [[Assert.ThrowsAsync]]
 * [[Assert.DoesNotThrow]]
 * [[Assert.DoesNotThrowAsync]]
 * [[Assert.Catch]]
 * [[Assert.CatchAsync]]
 * [[Assert.Pass]]
 * [[Assert.Fail]]
 * [[Assert.Ignore]]
 * [[Assert.Inconclusive]]
 
Additional assertions are provided by the following classes:
 * [[String Assert]]
 * [[Collection Assert]]
 * [[File Assert]]
 * [[Directory Assert]]

#### See also... 
 * [[Constraint Model]]
