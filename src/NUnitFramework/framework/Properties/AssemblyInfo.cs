// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

using System;
using System.Reflection;

#if NET_4_5
[assembly: AssemblyTitle("NUnit Framework .NET 4.5")]
#elif NET_4_0
[assembly: AssemblyTitle("NUnit Framework .NET 4.0")]
#elif NET_3_5
[assembly: AssemblyTitle("NUnit Framework .NET 3.5")]
#elif NET_2_0
[assembly: AssemblyTitle("NUnit Framework .NET 2.0")]
#elif SL_5_0
[assembly: AssemblyTitle("NUnit Framework Silverlight 5.0")]
#elif NETCF_3_5
[assembly: AssemblyTitle("NUnit Framework CF 3.5")]
#elif PORTABLE
[assembly: AssemblyTitle("NUnit Framework Portable")]
#else
[assembly: AssemblyTitle("NUnit Framework")]
#endif

[assembly: AssemblyDescription("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]