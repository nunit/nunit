The **SequentialAttribute** is used on a test to specify that NUnit should
generate test cases by selecting individual data items provided
for the parameters of the test, without generating additional
combinations.

> [!NOTE]
> If parameter data is provided by multiple attributes, the order in which NUnit uses the data items is not guaranteed. However,
it can be expected to remain constant for a given runtime and operating system. For best results with **SequentialAttribute** use only one data attribute on each parameter.
   
#### Example

The following test will be executed three times.

```csharp
[Test, Sequential]
public void MyTest(
    [Values(1, 2, 3)] int x,
    [Values("A", "B")] string s)
{
    ...
}
```

MyTest is called three times, as follows:

```csharp
MyTest(1, "A")
MyTest(2, "B")
MyTest(3, null)
```

#### See also...
 * [Combinatorial Attribute](Combinatorial.md)
 * [Pairwise Attribute](Pairwise.md)
