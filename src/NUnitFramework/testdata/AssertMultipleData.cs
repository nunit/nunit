using System;
using NUnit.Framework;

#if ASYNC
using System.Threading.Tasks;
#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif
#endif

namespace NUnit.TestData.AssertMultipleData
{
    // NOTE: Some of these methods were getting optimized out of
    // existence in the .NET 2.0 AppVeyor build. For that reason,
    // we turned optimization off for the testdata assembly.

    public class AssertMultipleFixture
    {
        private static readonly ComplexNumber complex = new ComplexNumber(5.2, 3.9);

        [Test]
        public void EmptyBlock()
        {
            Assert.Multiple(() => { });
        }

        [Test]
        public void SingleAssertSucceeds()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(4));
            });
        }

        [Test]
        public void TwoAssertsSucceed()
        {
            Assert.Multiple(() =>
            {
                Assert.That(complex.RealPart, Is.EqualTo(5.2));
                Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
            });
        }

        [Test]
        public void ThreeAssertsSucceed()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(4));
                Assert.That(complex.RealPart, Is.EqualTo(5.2));
                Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
            });
        }

        [Test]
        public void NestedBlock_ThreeAssertsSucceed()
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
        public void TwoNestedBlocks_ThreeAssertsSucceed()
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
            SingleAssertSucceeds();
            TwoAssertsSucceed();
        }

        [Test]
        public void MethodCallsFail()
        {
            Assert.Multiple(() =>
            {
                Assert.Fail("Message from Assert.Fail");
            });
        }

        [Test]
        public void MethodCallsFailAfterTwoAssertsFail()
        {
            Assert.Multiple(() =>
            {
                Assert.That(complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                Assert.Fail("Message from Assert.Fail");
            });
        }

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

        [Test]
        public void ExceptionThrown()
        {
            Assert.Multiple(() =>
            {
                throw new Exception("Simulated Error");
            });
        }

        [Test]
        public void ExceptionThrownAfterTwoFailures()
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual(5, 2 + 2, "Failure 1");
                Assert.True(1 == 0, "Failure 2");
                throw new Exception("Simulated Error");
            });
        }

        [Test]
        public void AssertPassInBlock()
        {
            Assert.Multiple(() =>
            {
                Assert.Pass("Message from Assert.Pass");
            });
        }

        [Test]
        public void AssertIgnoreInBlock()
        {
            Assert.Multiple(() =>
            {
                Assert.Ignore("Message from Assert.Ignore");
            });
        }

        [Test]
        public void AssertInconclusiveInBlock()
        {
            Assert.Multiple(() =>
            {
                Assert.Inconclusive("Message from Assert.Inconclusive");
            });
        }

        [Test]
        public void AssumptionInBlock()
        {
            Assert.Multiple(() =>
            {
                Assume.That(2 + 2 == 4);
            });
        }

#if ASYNC
        [Test]
        public void ThreeAssertsSucceed_Async()
        {
            Assert.Multiple(async () =>
            {
                await Task.Delay(100);
                Assert.That(2 + 2, Is.EqualTo(4));
                Assert.That(complex.RealPart, Is.EqualTo(5.2));
                Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
            });
        }

        [Test]
        public void NestedBlock_ThreeAssertsSucceed_Async()
        {
            Assert.Multiple(async () =>
            {
                await Task.Delay(100);
                Assert.That(2 + 2, Is.EqualTo(4));

                Assert.Multiple(async () =>
                {
                    await Task.Delay(100);
                    Assert.That(complex.RealPart, Is.EqualTo(5.2));
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
                });
            });
        }

        [Test]
        public void TwoNestedBlocks_ThreeAssertsSucceed_Async()
        {
            Assert.Multiple(() =>
            {
                Assert.Multiple(async () =>
                {
                    await Task.Delay(100);
                    Assert.That(2 + 2, Is.EqualTo(4));
                });

                Assert.Multiple(async () =>
                {
                    Assert.That(complex.RealPart, Is.EqualTo(5.2));
                    await Task.Delay(100);
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(3.9));
                });
            });
        }

        [Test]
        public void TwoAsserts_BothAssertsFail_Async()
        {
            Assert.Multiple(async () =>
            {
                await Task.Delay(100);
                Assert.That(complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
            });
        }

        [Test]
        public void TwoNestedBlocks_TwoAssertsFail_Async()
        {
            Assert.Multiple(() =>
            {
                Assert.Multiple(async () =>
                {
                    await Task.Delay(100);
                    Assert.That(2 + 2, Is.EqualTo(5));
                });

                Assert.Multiple(async () =>
                {
                    await Task.Delay(100);
                    Assert.That(complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                });
            });
        }
#endif
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
