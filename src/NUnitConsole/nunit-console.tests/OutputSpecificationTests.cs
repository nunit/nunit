// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

namespace NUnit.ConsoleRunner.Tests
{
    using Common;
    using Framework;

    public class OutputSpecificationTests
    {
        [Test]
        public void SpecMayNotBeNull()
        {
            Assert.That(
                () => new OutputSpecification(null),
                Throws.TypeOf<NullReferenceException>());
        }


        [Test]
        public void SpecOptionMustContainEqualSign()
        {
            Assert.That(
                () => new OutputSpecification("MyFile.xml;transform.xslt"),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SpecOptionMustContainJustOneEqualSign()
        {
            Assert.That(
                () => new OutputSpecification("MyFile.xml;transform=xslt=transform.xslt"),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void FileNameOnly()
        {
            var spec = new OutputSpecification("MyFile.xml");
            Assert.That(spec.OutputPath, Is.EqualTo("MyFile.xml"));
            Assert.That(spec.Format, Is.EqualTo("nunit3"));
            Assert.Null(spec.Transform);
        }

        [Test]
        public void FileNamePlusFormat()
        {
            var spec = new OutputSpecification("MyFile.xml;format=nunit2");
            Assert.That(spec.OutputPath, Is.EqualTo("MyFile.xml"));
            Assert.That(spec.Format, Is.EqualTo("nunit2"));
            Assert.Null(spec.Transform);
        }

        [Test]
        public void FileNamePlusTransform()
        {
            var spec = new OutputSpecification("MyFile.xml;transform=transform.xslt");
            Assert.That(spec.OutputPath, Is.EqualTo("MyFile.xml"));
            Assert.That(spec.Format, Is.EqualTo("user"));
            Assert.That(spec.Transform, Is.EqualTo("transform.xslt"));
        }

        [Test]
        public void UserFormatMayBeIndicatedExplicitlyAfterTransform()
        {
            var spec = new OutputSpecification("MyFile.xml;transform=transform.xslt;format=user");
            Assert.That(spec.OutputPath, Is.EqualTo("MyFile.xml"));
            Assert.That(spec.Format, Is.EqualTo("user"));
            Assert.That(spec.Transform, Is.EqualTo("transform.xslt"));
        }

        [Test]
        public void UserFormatMayBeIndicatedExplicitlyBeforeTransform()
        {
            var spec = new OutputSpecification("MyFile.xml;format=user;transform=transform.xslt");
            Assert.That(spec.OutputPath, Is.EqualTo("MyFile.xml"));
            Assert.That(spec.Format, Is.EqualTo("user"));
            Assert.That(spec.Transform, Is.EqualTo("transform.xslt"));
        }

        [Test]
        public void MultipleFormatSpecifiersNotAllowed()
        {
            Assert.That(
                () => new OutputSpecification("MyFile.xml;format=nunit2;format=nunit3"),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void MultipleTransformSpecifiersNotAllowed()
        {
            Assert.That(
                () => new OutputSpecification("MyFile.xml;transform=transform1.xslt;transform=transform2.xslt"),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void TransformWithNonUserFormatNotAllowed()
        {
            Assert.That(
                () => new OutputSpecification("MyFile.xml;format=nunit2;transform=transform.xslt"),
                Throws.TypeOf<ArgumentException>());
        }
    }
}
