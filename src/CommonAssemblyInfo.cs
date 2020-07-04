// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Reflection;

//
// Common Information for the NUnit assemblies
//
[assembly: AssemblyCompany("NUnit Software")]
[assembly: AssemblyProduct("NUnit 3")]
[assembly: AssemblyCopyright("Copyright (c) 2019 Charlie Poole, Rob Prouse")]
[assembly: AssemblyTrademark("NUnit is a trademark of NUnit Software")]

#if DEBUG
#if NET45
[assembly: AssemblyConfiguration(".NET Framework 4.5 Debug")]
#elif NET40
[assembly: AssemblyConfiguration(".NET Framework 4.0 Debug")]
#elif NET35
[assembly: AssemblyConfiguration(".NET Framework 3.5 Debug")]
#elif NETSTANDARD2_0
[assembly: AssemblyConfiguration(".NET Standard 2.0 Debug")]
#elif NETCOREAPP2_1
[assembly: AssemblyConfiguration(".NET Core 2.1 Debug")]
#elif NETCOREAPP3_1
[assembly: AssemblyConfiguration(".NET Core 3.1 Debug")]
#elif NETCOREAPP5_0
[assembly: AssemblyConfiguration(".NET 5.0 Debug")]
#else
#error Missing AssemblyConfiguration attribute for this target.
#endif
#else
#if NET45
[assembly: AssemblyConfiguration(".NET Framework 4.5")]
#elif NET40
[assembly: AssemblyConfiguration(".NET Framework 4.0")]
#elif NET35
[assembly: AssemblyConfiguration(".NET Framework 3.5")]
#elif NETSTANDARD2_0
[assembly: AssemblyConfiguration(".NET Standard 2.0")]
#elif NETCOREAPP2_1
[assembly: AssemblyConfiguration(".NET Core 2.1")]
#elif NETCOREAPP3_1
[assembly: AssemblyConfiguration(".NET Core 3.1")]
#elif NETCOREAPP5_0
[assembly: AssemblyConfiguration(".NET 5.0")]
#else
#error Missing AssemblyConfiguration attribute for this target.
#endif
#endif
