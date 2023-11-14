// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Reflection;

//
// Common Information for the NUnit assemblies
//
[assembly: AssemblyCompany("NUnit Software")]
[assembly: AssemblyProduct("NUnit 4")]
[assembly: AssemblyCopyright("Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt")]
[assembly: AssemblyTrademark("NUnit is a trademark of NUnit Software")]

#if DEBUG
#if NET462
[assembly: AssemblyConfiguration(".NET Framework 4.6.2 Debug")]
#elif NET6_0
[assembly: AssemblyConfiguration(".NET 6.0 Debug")]
#elif NET8_0
[assembly: AssemblyConfiguration(".NET 8.0 Debug")]
#else
#error Missing AssemblyConfiguration attribute for this target.
#endif
#else
#if NET462
[assembly: AssemblyConfiguration(".NET Framework 4.6.2")]
#elif NET6_0
[assembly: AssemblyConfiguration(".NET 6.0")]
#elif NET8_0
[assembly: AssemblyConfiguration(".NET 8.0")]
#else
#error Missing AssemblyConfiguration attribute for this target.
#endif
#endif
