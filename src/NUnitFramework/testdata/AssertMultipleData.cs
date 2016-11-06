using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.TestData.AssertMultipleData
{
    public class AssertMultipleSuccessFixture
    {
        private static readonly ComplexNumber complex = new ComplexNumber(5.2, 3.9);

        [Test]
        public void EmptyBlock()
        {
            Assert.Multiple(() => { });
        }

        [Test]
        public void SingleAssert()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(4));
            });
        }

        [Test]
        public void TwoAsserts()
        {
            Assert.Multiple(() =>
            {
                Assert.That(complex.RealPart, Is.EqualTo(5.2));
                Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
            });
        }

        [Test]
        public void ThreeAsserts()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(4));
                Assert.That(complex.RealPart, Is.EqualTo(5.2));
                Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
            });
        }

        [Test]
        public void NestedBlock()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(4));

                Assert.Multiple(() =>
                {
                    Assert.That(complex.RealPart, Is.EqualTo(5.2));
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
                });
            });
        }

        [Test]
        public void TwoNestedBlocks()
        {
            Assert.Multiple(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4));
                });

                Assert.Multiple(() =>
                {
                    Assert.That(complex.RealPart, Is.EqualTo(5.2));
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
                });
            });
        }

        [Test]
        public void NestedBlocksInMethodCalls()
        {
            SingleAssert();
            TwoAsserts();
        }
    }

    public class AssertMultipleFailureFixture
    {
        // NOTE: Some of these methods were getting optimized out of
        // existence in the .NET 2.0 AppVeyor build. For that reason,
        // we turned optimization off for the testdata assembly.

        private static readonly ComplexNumber complex = new ComplexNumber(5.2, 3.9);

        [Test]
        public void TwoAsserts_FirstAssertFails()
        {
            Assert.Multiple(() =>
            {
                Assert.That(complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9), "ImaginaryPart");
            });
        }

        [Test]
        public void TwoAsserts_SecondAssertFails()
        {
            Assert.Multiple(() =>
            {
                Assert.That(complex.RealPart, Is.EqualTo(5.2), "RealPart");
                Assert.That(complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
            });
        }

        [Test]
        public void TwoAsserts_BothAssertsFail()
        {
            Assert.Multiple(() =>
            {
                Assert.That(complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
            });
        }

        [Test]
        public void NestedBlock_FirstAssertFails()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(5));

                Assert.Multiple(() =>
                {
                    Assert.That(complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9), "ImaginaryPart");
                });
            });
        }

        [Test]
        public void NestedBlock_TwoAssertsFail()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(5));

                Assert.Multiple(() =>
                {
                    Assert.That(complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                });
            });
        }

        [Test]
        public void TwoNestedBlocks_FirstAssertFails()
        {
            Assert.Multiple(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(5));
                });

                Assert.Multiple(() =>
                {
                    Assert.That(complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9), "ImaginaryPart");
                });
            });
        }

        [Test]
        public void TwoNestedBlocks_TwoAssertsFail()
        {
            Assert.Multiple(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(5));
                });

                Assert.Multiple(() =>
                {
                    Assert.That(complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                });
            });
        }
    }

    class ComplexNumber
    {
        public ComplexNumber(double realPart, double imaginaryPart)
        {
            RealPart = realPart;
            ImaginaryPart = imaginaryPart;
        }

        public double RealPart;
        public double ImaginaryPart;
    }
}
