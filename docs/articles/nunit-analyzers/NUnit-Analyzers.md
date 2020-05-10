# Analyzers #

| Id       | Title       | Enabled by Default |
| :--      | :--         | :--:               |
| [NUnit1001](NUnit1001.md) | The individual arguments provided by a TestCaseAttribute must match the type of the matching parameter of the method. | :white_check_mark: |
| [NUnit1002](NUnit1002.md) | TestCaseSource should use nameof operator to specify target. | :white_check_mark: |
| [NUnit1003](NUnit1003.md) | Too few arguments provided by TestCaseAttribute. | :white_check_mark: |
| [NUnit1004](NUnit1004.md) | Too many arguments provided by TestCaseAttribute. | :white_check_mark: |
| [NUnit1005](NUnit1005.md) | The type of ExpectedResult must match the return type. | :white_check_mark: |
| [NUnit1006](NUnit1006.md) | ExpectedResult must not be specified when the method returns void. | :white_check_mark: |
| [NUnit1007](NUnit1007.md) | Method has non-void return type, but no result is expected in ExpectedResult. | :white_check_mark: |
| [NUnit1008](NUnit1008.md) | Specifying ParallelScope.Self on assembly level has no effect. | :white_check_mark: |
| [NUnit1009](NUnit1009.md) | No ParallelScope.Children on a non-parameterized test method. | :white_check_mark: |
| [NUnit1010](NUnit1010.md) | No ParallelScope.Fixtures on a test method. | :white_check_mark: |
| [NUnit1011](NUnit1011.md) | TestCaseSource argument does not specify an existing member. | :white_check_mark: |
| [NUnit1012](NUnit1012.md) | Async test method must have non-void return type. | :white_check_mark: |
| [NUnit1013](NUnit1013.md) | Async test method must have non-generic Task return type when no result is expected. | :white_check_mark: |
| [NUnit1014](NUnit1014.md) | Async test method must have Task<T> return type when a result is expected | :white_check_mark: |
| [NUnit2001](NUnit2001.md) | Consider using Assert.That(expr, Is.False) instead of Assert.False(expr). | :white_check_mark: |
| [NUnit2002](NUnit2002.md) | Consider using Assert.That(expr, Is.False) instead of Assert.IsFalse(expr). | :white_check_mark: |
| [NUnit2003](NUnit2003.md) | Consider using Assert.That(expr, Is.True) instead of Assert.IsTrue(expr). | :white_check_mark: |
| [NUnit2004](NUnit2004.md) | Consider using Assert.That(expr, Is.True) instead of Assert.True(expr). | :white_check_mark: |
| [NUnit2005](NUnit2005.md) | Consider using Assert.That(expr2, Is.EqualTo(expr1)) instead of Assert.AreEqual(expr1, expr2). | :white_check_mark: |
| [NUnit2006](NUnit2006.md) | Consider using Assert.That(expr2, Is.Not.EqualTo(expr1)) instead of Assert.AreNotEqual(expr1, expr2). | :white_check_mark: |
| [NUnit2007](NUnit2007.md) | Actual value should not be constant. | :white_check_mark: |
| [NUnit2008](NUnit2008.md) | Incorrect IgnoreCase usage. | :white_check_mark: |
| [NUnit2009](NUnit2009.md) | Same value provided as actual and expected argument. | :white_check_mark: |
| [NUnit2010](NUnit2010.md) | Use EqualConstraint. | :white_check_mark: |
| [NUnit2011](NUnit2011.md) | Use ContainsConstraint. | :white_check_mark: |
| [NUnit2012](NUnit2012.md) | Use StartsWithConstraint. | :white_check_mark: |
| [NUnit2013](NUnit2013.md) | Use EndsWithConstraint. | :white_check_mark: |
| [NUnit2014](NUnit2014.md) | Use SomeItemsConstraint. | :white_check_mark: |
| [NUnit2015](NUnit2015.md) | Consider using Assert.That(expr2, Is.SameAs(expr1)) instead of Assert.AreSame(expr1, expr2). | :white_check_mark: |
| [NUnit2016](NUnit2016.md) | Consider using Assert.That(expr, Is.Null) instead of Assert.Null(expr). | :white_check_mark: |
| [NUnit2017](NUnit2017.md) | Consider using Assert.That(expr, Is.Null) instead of Assert.IsNull(expr). | :white_check_mark: |
| [NUnit2018](NUnit2018.md) | Consider using Assert.That(expr, Is.Not.Null) instead of Assert.NotNull(expr). | :white_check_mark: |
| [NUnit2019](NUnit2019.md) | Consider using Assert.That(expr, Is.Not.Null) instead of Assert.IsNotNull(expr). | :white_check_mark: |
| [NUnit2020](NUnit2020.md) | Incompatible types for SameAs constraint. | :white_check_mark: |
| [NUnit2021](NUnit2021.md) | Incompatible types for EqualTo constraint. | :white_check_mark: |
| [NUnit2022](NUnit2022.md) | Missing property required for constraint. | :white_check_mark: |
| [NUnit2023](NUnit2023.md) | Invalid NullConstraint usage. | :white_check_mark: |
