// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
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

using System.Runtime.Serialization;

namespace NUnit.Framework.Attributes
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.All)]
    public class LifeCycleAttributeParallelTests
    {
        static ObjectIDGenerator generator = new ObjectIDGenerator();
        int constructorCount = 0;
        int setupCount = 0;

        public LifeCycleAttributeParallelTests()
        {
            OutputReferenceId("Ctor");
            constructorCount++;
        }

        [SetUp]
        public void SetUp()
        {
            OutputReferenceId("SetUp");
            setupCount++;
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance1()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance1");
            Assert.That(constructorCount, Is.EqualTo(1)); 
            Assert.That(setupCount, Is.EqualTo(1));
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance2()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance2");
            Assert.That(constructorCount, Is.EqualTo(1));
            Assert.That(setupCount, Is.EqualTo(1));
        }

        [Test]
        public void EnsureParallelTestsRunInNewInstance3()
        {
            OutputReferenceId("EnsureParallelTestsRunInNewInstance3");
            Assert.That(constructorCount, Is.EqualTo(1));
            Assert.That(setupCount, Is.EqualTo(1));
        }

        private void OutputReferenceId(string location)
        {
            long id = generator.GetId(this, out bool _);
            TestContext.WriteLine($"{location}: {id}>");
        }
    }
}
