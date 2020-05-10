# Analyzers #

| Id       | Title       | Enabled by Default |
| :--      | :--         | :--:               |
| [[NUnit1001]] | The individual arguments provided by a TestCaseAttribute must match the type of the matching parameter of the method. | :white_check_mark: |
| [[NUnit1002]] | TestCaseSource should use nameof operator to specify target. | :white_check_mark: |
| [[NUnit1003]] | Too few arguments provided by TestCaseAttribute. | :white_check_mark: |
| [[NUnit1004]] | Too many arguments provided by TestCaseAttribute. | :white_check_mark: |
| [[NUnit1005]] | The type of ExpectedResult must match the return type. | :white_check_mark: |
| [[NUnit1006]] | ExpectedResult must not be specified when the method returns void. | :white_check_mark: |
| [[NUnit1007]] | Method has non-void return type, but no result is expected in ExpectedResult. | :white_check_mark: |
| [[NUnit1008]] | Specifying ParallelScope.Self on assembly level has no effect. | :white_check_mark: |
| [[NUnit1009]] | No ParallelScope.Children on a non-parameterized test method. | :white_check_mark: |
| [[NUnit1010]] | No ParallelScope.Fixtures on a test method. | :white_check_mark: |
| [[NUnit1011]] | TestCaseSource argument does not specify an existing member. | :white_check_mark: |
| [[NUnit1012]] | Async test method must have non-void return type. | :white_check_mark: |
| [[NUnit1013]] | Async test method must have non-generic Task return type when no result is expected. | :white_check_mark: |
| [[NUnit1014]] | Async test method must have Task<T> return type when a result is expected | :white_check_mark: |
| [[NUnit2001]] | Consider using Assert.That(expr, Is.False) instead of Assert.False(expr). | :white_check_mark: |
| [[NUnit2002]] | Consider using Assert.That(expr, Is.False) instead of Assert.IsFalse(expr). | :white_check_mark: |
| [[NUnit2003]] | Consider using Assert.That(expr, Is.True) instead of Assert.IsTrue(expr). | :white_check_mark: |
| [[NUnit2004]] | Consider using Assert.That(expr, Is.True) instead of Assert.True(expr). | :white_check_mark: |
| [[NUnit2005]] | Consider using Assert.That(expr2, Is.EqualTo(expr1)) instead of Assert.AreEqual(expr1, expr2). | :white_check_mark: |
| [[NUnit2006]] | Consider using Assert.That(expr2, Is.Not.EqualTo(expr1)) instead of Assert.AreNotEqual(expr1, expr2). | :white_check_mark: |
| [[NUnit2007]] | Actual value should not be constant. | :white_check_mark: |
| [[NUnit2008]] | Incorrect IgnoreCase usage. | :white_check_mark: |
| [[NUnit2009]] | Same value provided as actual and expected argument. | :white_check_mark: |
| [[NUnit2010]] | Use EqualConstraint. | :white_check_mark: |
| [[NUnit2011]] | Use ContainsConstraint. | :white_check_mark: |
| [[NUnit2012]] | Use StartsWithConstraint. | :white_check_mark: |
| [[NUnit2013]] | Use EndsWithConstraint. | :white_check_mark: |
| [[NUnit2014]] | Use SomeItemsConstraint. | :white_check_mark: |
| [[NUnit2015]] | Consider using Assert.That(expr2, Is.SameAs(expr1)) instead of Assert.AreSame(expr1, expr2). | :white_check_mark: |
| [[NUnit2016]] | Consider using Assert.That(expr, Is.Null) instead of Assert.Null(expr). | :white_check_mark: |
| [[NUnit2017]] | Consider using Assert.That(expr, Is.Null) instead of Assert.IsNull(expr). | :white_check_mark: |
| [[NUnit2018]] | Consider using Assert.That(expr, Is.Not.Null) instead of Assert.NotNull(expr). | :white_check_mark: |
| [[NUnit2019]] | Consider using Assert.That(expr, Is.Not.Null) instead of Assert.IsNotNull(expr). | :white_check_mark: |
| [[NUnit2020]] | Incompatible types for SameAs constraint. | :white_check_mark: |
| [[NUnit2021]] | Incompatible types for EqualTo constraint. | :white_check_mark: |
| [[NUnit2022]] | Missing property required for constraint. | :white_check_mark: |
| [[NUnit2023]] | Invalid NullConstraint usage. | :white_check_mark: |
