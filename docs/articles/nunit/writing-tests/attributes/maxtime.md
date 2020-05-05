The **MaxTimeAttribute** is used on test methods to specify a maximum time 
in milliseconds for a test case. If the test case takes longer than the 
specified time to complete, it is reported as a failure.
   
#### Example

```csharp
[Test, MaxTime(2000)]
public void TimedTest()
{
    ...
}
```

#### Notes:

1. Any assertion failures take precedence over the elapsed time check.

2. This attribute does not cancel the test if the time
   is exceeded. It merely waits for the test to complete and then
   compares the elapsed time to the specified maximum. If you want to
   cancel long-running tests, see [[Timeout Attribute]].
