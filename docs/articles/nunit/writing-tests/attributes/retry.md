RetryAttribute is used on a test method to specify that it should be rerun if it fails, up to a maximum number of times.

Notes:

1. The argument you specify is the total number of attempts and __not__ the number of retries after an initial failure. So `[Retry(1)]` does nothing and should not be used.

2. It is not currently possible to use `RetryAttribute` on a `TestFixture` or any other type of test suite. Only single tests may be repeated.

3. If a test has an unexpected exception, an error result is returned and it is not retried. Only assertion failures can trigger a retry. To convert an unexpected exception into an assertion failure, see the [ThrowsConstraint](xref:ThrowsConstraint).

