Counting tests sounds simple but there are a few issues. This page documents how the framework counts tests in various categories.

1. In general, when counting tests, we are talking about test cases. In the case of non-parameterized test methods, that's the same as the number of methods. Where there are parameters, we count the number of individual cases.

2. When all the tests in an assembly are run, the total number of tests is the number of tests present in the assembly. When selection is made - either via the command line or through a Gui - we count only tests that are actually selected. Non-selected tests don't appear at all in the XML result file and are not taken into account by the counts.

3. Tests are categorized in one of four statuses: Passed, Failed, Inconclusive and Skipped.

    1. Passed tests currently only have one ResultState, Success.

    2. Failed tests are caused by any of the following:
        * Failure of an assertion (ResultState.Failure)
        * An unexpected exception (ResultState.Error)
        * An invalid test (ResultState.NotRunnable)
        * User cancellation (ResultState.Cancelled)

    3. Inconclusive tests currently only have one ResultState, Inconclusive

    4. Skipped tests are caused by
        * Ignoring the test (ResultState.Ignored)
        * The test was marked explicit (ResultState.Skipped)
        * Note: this may change to ResultState.Explicit in the future
        * A Platform, Culture or UICulture attribute test failed (ResultState.Skipped)

4. Users may define new ResultStates in any of the four statuses.

5. Failures, errors, skipping or ignoring in the SetUp for a test counts the same as if the action happened in the test method itself.

6. When fixtures are ignored or otherwise skipped using an attribute on the class, all tests within the fixture are given the corresponding result.

7. Failures, errors, skipping or ignoring in the OneTimeSetUp for a fixture causes all the test cases in the fixture to get the corresponding result.

8. An invalid fixture causes all of its test cases to count as invalid.
