The Platform attribute is used to specify platforms for which a test or fixture
should be run. Platforms are specified using case-insensitive string values
and may be either included or excluded from the run by use of the Include or 
Exclude properties respectively. Platforms to be included may alternatively
be specified as an argument to the PlatformAttribute constructor. In either
case, multiple comma-separated values may be specified.

If a test or fixture with the Platform attribute does not satisfy the specified
platform requirements it is skipped. The test does not affect the outcome of 
the run at all: it is not considered as ignored and is not even counted in 
the total number of tests. _[Ed.: Check this.]_ In the gui, the tree node for the test remains 
gray and the status bar color is not affected.

#### Test Fixture Syntax

```C#
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  [Platform("NET-2.0")]
  public class DotNetTwoTests
  {
    // ...
  }
}
```

#### Test Syntax

```C#
namespace NUnit.Tests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  public class SuccessTests
  {
    [Test]
    [Platform(Exclude="Win98,WinME")]
    public void SomeTest()
    { /* ... */ }
}
```

#### Platform Specifiers

The following values are recognized as platform specifiers.
They may be expressed in upper, lower or mixed case.

###### Operating System
 * Win
 * Win32
 * Win32S
 * Win32Windows
 * Win32NT
 * WinCE
 * Win95
 * Win98
 * WinMe
 * NT3
 * NT4
 * NT5
 * NT6
 * Win2K
 * WinXP
 * Win2003Server
 * Vista
 * Win2008Server
 * Win2008ServerR2
 * Windows7
 * Win2012Server
 * Windows8
 * Unix
 * Linux
 * MacOsX
 * XBox

###### Architecture

* 32-Bit
* 32-Bit-Process
* 32-Bit-OS (.NET 4.0 and higher only)
* 64-Bit
* 64-Bit-Process
* 64-Bit-OS (.NET 4.0 and higher only)

###### Runtime
 * Net
 * Net-1.0
 * Net-1.1
 * Net-2.0
 * Net-3.0 (1)
 * Net-3.5 (2)
 * Net-4.0
 * Net-4.5 (3)
 * NetCF
 * SSCLI
 * Rotor
 * Mono
 * Mono-1.0
 * Mono-2.0
 * Mono-3.0 (4)
 * Mono-3.5 (5)
 * Mono-4.0

#### Notes:

1. Includes Net-2.0
2. Includes Net-2.0 and Net-3.0
3. Includes Net-4.0
4. Includes Mono-2.0
5. Includes Mono-2.0 and Mono-3.0

