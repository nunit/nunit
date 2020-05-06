---
uid: apartment-attribute
---

The `ApartmentAttribute` is used on a test method, class or assembly
to specify that the tests should be run in a particular [apartment](https://docs.microsoft.com/en-us/windows/desktop/com/processes--threads--and-apartments), either
the STA or the MTA.
   
When running tests in parallel, the test is simply scheduled to execute 
from a queue that uses the apartment specified. When the parallel feature 
is not in use, it causes creation of a new thread if the parent test is 
not already running in the correct apartment.
   
When this attribute is not specified, tests run in the MTA.

This attribute replaces the RequiresMTA and RequiresSTA attributes, which
are now considered obsolete.
   
#### Assembly Level Examples
   
```csharp

// All the tests in this assembly will use the MTA by default. Since
// this is the general default, the attribute is not actually needed.
[assembly:Apartment(ApartmentState.MTA)]

...

// All the tests in this assembly will use the STA by default
[assembly:Apartment(ApartmentState.STA)]

```

#### Test Fixture Examples
   
```csharp

// TestFixture requiring use of the MTA. The attribute is not 
// needed unless the STA was specified at a higher level.
[TestFixture, Apartment(ApartmentState.MTA)]
public class FixtureRequiringMTA
{
  // All tests in the fixture will run in the MTA.
}

// TestFixture requiring use of the STA.
[TestFixture, Apartment(ApartmentState.STA)]
public class FixtureRequiringSTA
{
  // All tests in the fixture will run in the STA.
}

```

#### Test Method Examples
   
```csharp
[TestFixture]
public class AnotherFixture
{
  [Test, Apartment(ApartmentState.MTA)]
  public void TestRequiringMTA()
  {
    // This test will run in the MTA.
  }
  
  [Test, Apartment(ApartmentState.STA)]
  public void TestRequiringSTA()
  {
    // This test will run in the STA.
  }
}
```

#### See also...
 * [RequiresThread Attribute](RequiresThread.md)
   