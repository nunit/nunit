// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using NUnit.Common;

namespace NUnitLite
{
    /// <summary>
    /// NUnitLite-Runner is a console program that is able to run NUnitLite
    /// tests as an alternative to using an executable NUnitLite test.
    /// 
    /// Since it references a particular version of NUnitLite, it may only
    /// be used for test assemblies built against that framework version.
    /// 
    /// In the special case of the portable version of nunitlite-runner,
    /// the program is a .NET 4.5 console application. In that case, we
    /// create a ColorConsoleWriter, since the portable build can't do it.
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
#if PORTABLE
            return new TextRunner().Execute(new ColorConsoleWriter(), Console.In, args);
#else
            return new TextRunner().Execute(args);
#endif
        }
    }
}
