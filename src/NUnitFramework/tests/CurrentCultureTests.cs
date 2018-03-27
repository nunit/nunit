// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

using System.Globalization;

namespace NUnit.Framework
{
    public static class CurrentCultureTests
    {
#if !(NET20 || NET35 || NET40 || NETCOREAPP1_1)
        [Test, RequiresThread, NonParallelizable]
        public static void CurrentCultureIsNotUnnecessarilySet()
        {
            var nonCurrent = new CultureInfo(
                CultureInfo.DefaultThreadCurrentCulture?.Name == "fr-FR" ? "en-GB" : "fr-FR");

            var original = CultureInfo.DefaultThreadCurrentCulture;
            CultureInfo.DefaultThreadCurrentCulture = nonCurrent;
            try
            {
                Assert.That(CultureInfo.CurrentCulture, Is.SameAs(nonCurrent));
            }
            finally
            {
                CultureInfo.DefaultThreadCurrentCulture = original;
            }
        }

        [Test, RequiresThread, NonParallelizable]
        public static void CurrentUICultureIsNotUnnecessarilySet()
        {
            var nonCurrent = new CultureInfo(
                CultureInfo.DefaultThreadCurrentUICulture?.Name == "fr-FR" ? "en-GB" : "fr-FR");

            var original = CultureInfo.DefaultThreadCurrentUICulture;
            CultureInfo.DefaultThreadCurrentUICulture = nonCurrent;
            try
            {
                Assert.That(CultureInfo.CurrentUICulture, Is.SameAs(nonCurrent));
            }
            finally
            {
                CultureInfo.DefaultThreadCurrentUICulture = original;
            }
        }
#endif
    }
}
