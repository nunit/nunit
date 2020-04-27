The **RequiresThreadAttribute** is used to indicate that a test method, 
class or assembly should be run on a separate thread. Optionally, the 
desired apartment for the thread may be specified in the constructor.
   
**Note:** This attribute, used with or without an ApartmentState
argument will **always** result in creation of a new thread. To
create a thread **only** if the current ApartmentState is not appropriate,
use the **ApartmentAttribute**.
   
#### Examples
   
```C#
// A thread will be created and used to run
// all the tests in the assembly
[assembly:RequiresThread]

...

// TestFixture requiring a separate thread
[TestFixture, RequiresThread]
public class FixtureOnThread
{
  // A separate thread will be created and all
  // tests in the fixture will run on it.
}

[TestFixture]
public class AnotherFixture
{
  [Test, RequiresThread]
  public void TestRequiringThread()
  {
    // A separate thread will be created for this test
  }
  
  [Test, RequiresThread(ApartmentState.STA)]
  public void TestRequiringSTAThread()
  {
    // A separate STA thread will be created for this test.
  }
}
```

#### See also...

 * [[Apartment Attribute]]
