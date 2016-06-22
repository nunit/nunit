// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    public class ExtensionAssemblyTests
    {
        private static readonly Assembly THIS_ASSEMBLY = Assembly.GetExecutingAssembly();
        private static readonly string THIS_ASSEMBLY_PATH = THIS_ASSEMBLY.Location;
        private static readonly string THIS_ASSEMBLY_NAME = THIS_ASSEMBLY.GetName().FullName;
        private static readonly Assembly ENGINE_ASSEMBLY = Assembly.GetAssembly(typeof(ExtensionAssembly));
        private static readonly string ENGINE_ASSEMBLY_PATH = ENGINE_ASSEMBLY.Location;

        private ExtensionAssembly _ea;
        private ExtensionAssembly _eaCopy;
        private ExtensionAssembly _eaBetter;
        private ExtensionAssembly _eaOther;

        [SetUp]
        public void CreateExtensionAssemblies()
        {
            _ea = new ExtensionAssembly(THIS_ASSEMBLY_PATH, false);
            _eaCopy = new ExtensionAssembly(THIS_ASSEMBLY_PATH, false);
            _eaBetter = new ExtensionAssembly(THIS_ASSEMBLY_PATH, false);
            _eaBetter.Assembly.Name.Version = new Version(7, 0);
            _eaOther = new ExtensionAssembly(ENGINE_ASSEMBLY_PATH, false);
        }

        [Test]
        public void AssemblyDefinition()
        {
            Assert.That(_ea.Assembly.FullName, Is.EqualTo(THIS_ASSEMBLY_NAME));
        }

        [Test]
        public void MainModule()
        {
            Assert.That(_ea.MainModule.Assembly.FullName, Is.EqualTo(THIS_ASSEMBLY_NAME));   
        }

        [Test]
        public void AssemblyName()
        {
            Assert.That(_ea.AssemblyName.FullName, Is.EqualTo(THIS_ASSEMBLY_NAME));
        }

        [Test]
        public void IsDuplicateOf_SameAssembly()
        {
            Assert.That(_ea.IsDuplicateOf(_eaCopy));
            Assert.That(_eaCopy.IsDuplicateOf(_ea));
        }

        [Test]
        public void IsDuplicateOf_DifferentAssembly()
        {
            Assert.False(_ea.IsDuplicateOf(_eaOther));
            Assert.False(_eaOther.IsDuplicateOf(_ea));
        }

        [Test]
        public void IsBetterVersionOf_DifferentVersions()
        {
            Assert.That(_eaBetter.IsBetterVersionOf(_ea));
            Assert.False(_ea.IsBetterVersionOf(_eaBetter));
        }

        [Test]
        public void IsBetterVersionOf_SameVersion()
        {
            Assert.False(_ea.IsBetterVersionOf(_eaCopy));
            Assert.False(_eaCopy.IsBetterVersionOf(_ea));
        }

        [Test]
        public void IsBetterVersionOf_SameVersion_OneWildCard()
        {
            var eaWild = new ExtensionAssembly(THIS_ASSEMBLY_PATH, true);
            Assert.True(_ea.IsBetterVersionOf(eaWild));
            Assert.False(eaWild.IsBetterVersionOf(_ea));
        }

#if DEBUG
        [Test]
        public void IsBetterVersionOf_ThrowsIfNotDuplicates()
        {
            Assert.That(() => { _ea.IsBetterVersionOf(_eaOther); }, Throws.TypeOf<NUnitEngineException>());
        }
#endif
    }
}
