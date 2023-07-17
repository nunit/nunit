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
#elif NETSTANDARD2_0
[assembly: AssemblyConfiguration(".NET Standard 2.0 Debug")]
#elif NETCOREAPP3_1
[assembly: AssemblyConfiguration(".NET Core 3.1 Debug")]
#elif NET5_0
[assembly: AssemblyConfiguration(".NET 5.0 Debug")]
#elif NET6_0
[assembly: AssemblyConfiguration(".NET 6.0 Debug")]
#elif NET7_0
[assembly: AssemblyConfiguration(".NET 7.0 Debug")]
#else
#error Missing AssemblyConfiguration attribute for this target.
#endif
#else
#if NET462
[assembly: AssemblyConfiguration(".NET Framework 4.6.2")]
#elif NETSTANDARD2_0
[assembly: AssemblyConfiguration(".NET Standard 2.0")]
#elif NETCOREAPP3_1
[assembly: AssemblyConfiguration(".NET Core 3.1")]
#elif NET5_0
[assembly: AssemblyConfiguration(".NET 5.0")]
#elif NET6_0
[assembly: AssemblyConfiguration(".NET 6.0")]
#elif NET7_0
[assembly: AssemblyConfiguration(".NET 7.0")]
#else
#error Missing AssemblyConfiguration attribute for this target.
#endif
#endif
