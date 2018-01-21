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
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("nunitlite.tests, PublicKey=002400000480000094" +
                              "000000060200000024000052534131000400000100010031eea" +
                              "370b1984bfa6d1ea760e1ca6065cee41a1a279ca234933fe977" +
                              "a096222c0e14f9e5a17d5689305c6d7f1206a85a53c48ca0100" +
                              "80799d6eeef61c98abd18767827dc05daea6b6fbd2e868410d9" +
                              "bee5e972a004ddd692dec8fa404ba4591e847a8cf35de21c2d3" +
                              "723bc8d775a66b594adeb967537729fe2a446b548cd57a6")]

#if NET45
[assembly: AssemblyTitle("NUnitLite Runner (.NET Framework 4.5)")]
#elif NET40
[assembly: AssemblyTitle("NUnitLite Runner (.NET Framework 4.0)")]
#elif NET35
[assembly: AssemblyTitle("NUnitLite Runner (.NET Framework 3.5)")]
#elif NET20
[assembly: AssemblyTitle("NUnitLite Runner (.NET Framework 2.0)")]
#elif NETSTANDARD1_6
[assembly: AssemblyTitle("NUnitLite Runner (.NET Standard 1.6)")]
#elif NETSTANDARD2_0
[assembly: AssemblyTitle("NUnitLite Runner (.NET Standard 2.0)")]
#else
#error Missing AssemblyTitle attribute for this target.
#endif

[assembly: AssemblyDescription("")]
