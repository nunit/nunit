// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
[assembly: AssemblyProduct("NUnit 3.0")]
[assembly: AssemblyCopyright("Copyright (C) 2016 Charlie Poole")]
[assembly: AssemblyTrademark("NUnit is a trademark of NUnit Software")]

#if PORTABLE
[assembly: AssemblyMetadata("PCL", "True")]
#endif

#if DEBUG
#if NET_4_5
[assembly: AssemblyConfiguration(".NET 4.5 Debug")]
#elif NET_4_0
[assembly: AssemblyConfiguration(".NET 4.0 Debug")]
#elif NET_2_0
[assembly: AssemblyConfiguration(".NET 2.0 Debug")]
#elif SL_5_0
[assembly: AssemblyConfiguration("Silverlight 5.0 Debug")]
#elif SL_4_0
[assembly: AssemblyConfiguration("Silverlight 4.0 Debug")]
#elif SL_3_0
[assembly: AssemblyConfiguration("Silverlight 3.0 Debug")]
#elif NETCF_3_5
[assembly: AssemblyConfiguration("Compact Framework 3.5 Debug")]
#elif PORTABLE
[assembly: AssemblyConfiguration("Portable Debug")]
#else
[assembly: AssemblyConfiguration("Debug")]
#endif
#else
#if NET_4_5
[assembly: AssemblyConfiguration(".NET 4.5")]
#elif NET_4_0
[assembly: AssemblyConfiguration(".NET 4.0")]
#elif NET_2_0
[assembly: AssemblyConfiguration(".NET 2.0")]
#elif SL_5_0
[assembly: AssemblyConfiguration("Silverlight 5.0")]
#elif SL_4_0
[assembly: AssemblyConfiguration("Silverlight 4.0")]
#elif SL_3_0
[assembly: AssemblyConfiguration("Silverlight 3.0")]
#elif NETCF_3_5
[assembly: AssemblyConfiguration("Compact Framework 3.5")]
#elif PORTABLE
[assembly: AssemblyConfiguration("Portable")]
#else
[assembly: AssemblyConfiguration("")]
#endif
#endif
