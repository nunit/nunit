// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("nunitlite.tests, PublicKey=002400000480000094" +
                              "000000060200000024000052534131000400000100010031eea" +
                              "370b1984bfa6d1ea760e1ca6065cee41a1a279ca234933fe977" +
                              "a096222c0e14f9e5a17d5689305c6d7f1206a85a53c48ca0100" +
                              "80799d6eeef61c98abd18767827dc05daea6b6fbd2e868410d9" +
                              "bee5e972a004ddd692dec8fa404ba4591e847a8cf35de21c2d3" +
                              "723bc8d775a66b594adeb967537729fe2a446b548cd57a6")]

#if NET462
[assembly: AssemblyTitle("NUnitLite Runner (.NET Framework 4.6.2)")]
#elif NETSTANDARD2_0
[assembly: AssemblyTitle("NUnitLite Runner (.NET Standard 2.0)")]
#else
#error Missing AssemblyTitle attribute for this target.
#endif

[assembly: AssemblyDescription("")]
