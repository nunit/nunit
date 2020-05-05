Sometimes - especially in integration testing - it's desirable to give a warning message but continue execution. Beginning with release 3.6, NUnit supports this with the `Warn` class and the `Assert.Warn` method.

### Syntax

```csharp
  // Use Warn with reversed condition
  Warn.If(2+2 != 5);
  Warn.If(() => 2 + 2 != 5);
  Warn.If(2+2, Is.Not.EqualTo(5));
  Warn.If(() => 2+2, Is.Not.EqualTo(5).After(3000));

  // Use Warn with original condition
  Warn.Unless(2+2 == 5);
  Warn.Unless(() => 2 + 2 == 5);
  Warn.Unless(2+2, Is.EqualTo(5));
  Warn.Unless(() => 2+2, Is.EqualTo(5).After(3000));

  // Issue a warning message
  Assert.Warn("Warning message");
```

Each of the above items would fail. The test would continue to execute, however, and the warning messages would only be reported at the end of the test. If the test subsequently fails, the warnings will be reported along with the failure message or - in the case of `Assert.Multiple` - messages.

All of the overloads above also have a variant that accepts a function, `Func<string>`, that is used to build the message included with the Exception.

> [!NOTE]
> The framework reports warnings by including information about them in the XML result that is sent to the runner in use. If you are using this facility, make sure the runner you use supports warnings. Generally, the runners released by the NUnit team after the release of the 3.6 framework will have this support. Earlier releases will not. Many third party runners will not know what to do with warnings either.
