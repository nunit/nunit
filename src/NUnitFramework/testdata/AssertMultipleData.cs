// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using System.Threading.Tasks;
#pragma warning disable IDE0053 // Use expression body for lambda expression

namespace NUnit.TestData.AssertMultipleData
{
    // NOTE: Some of these methods were getting optimized out of
    // existence in the AppVeyor build. For that reason, we turned
    // optimization off for the testdata assembly.

    public class AssertMultipleFixture
    {
        private static readonly ComplexNumber Complex = new(5.2, 3.9);

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
                Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
            });
        }

        [Test]
        public void ThreeAssertsSucceed()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(4));
                Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
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
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
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
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
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
        public void ThreeAssertWarns()
        {
            Assert.Multiple(() =>
            {
                Assert.Warn("WARNING1");
                Assert.Warn("WARNING2");
                Assert.Warn("WARNING3");
            });
        }

        [Test]
        public void ThreeWarnIf_AllPass()
        {
            Assert.Multiple(() =>
            {
                Warn.If(false, "WARNING1");
                Warn.If(false, "WARNING2");
                Warn.If(false, "WARNING3");
            });
        }

        [Test]
        public void ThreeWarnIf_TwoFail()
        {
            Assert.Multiple(() =>
            {
                Warn.If(true, "WARNING1");
                Warn.If(false, "WARNING2");
                Warn.If(true, "WARNING3");
            });
        }

        [Test]
        public void ThreeWarnUnless_AllPass()
        {
            Assert.Multiple(() =>
            {
                Warn.Unless(true, "WARNING1");
                Warn.Unless(true, "WARNING2");
                Warn.Unless(true, "WARNING3");
            });
        }

        [Test]
        public void ThreeWarnUnless_TwoFail()
        {
            Assert.Multiple(() =>
            {
                Warn.Unless(false, "WARNING1");
                Warn.Unless(true, "WARNING2");
                Warn.Unless(false, "WARNING3");
            });
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
                Assert.That(Complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                Assert.Fail("Message from Assert.Fail");
            });
        }

        [Test]
        public void WarningAfterTwoAssertsFail()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                Assert.Warn("WARNING");
            });
        }

        [Test]
        public void TwoAssertsFailAfterWarning()
        {
            Assert.Multiple(() =>
            {
                Assert.Warn("WARNING");
                Assert.That(Complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
            });
        }

        [Test]
        public void TwoAsserts_FirstAssertFails()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9), "ImaginaryPart");
            });
        }

        [Test]
        public void TwoAsserts_SecondAssertFails()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Complex.RealPart, Is.EqualTo(5.2), "RealPart");
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
            });
        }

        [Test]
        public void TwoAsserts_BothAssertsFail()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
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
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9), "ImaginaryPart");
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
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
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
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9), "ImaginaryPart");
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
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
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
                Assert.That(2 + 2, Is.EqualTo(5), "Failure 1");
                Assert.That(1, Is.EqualTo(0), "Failure 2");
                throw new Exception("Simulated Error");
            });
        }

        [Test]
        public void ExceptionThrownAfterTwoFailures_EnterScope()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(2 + 2, Is.EqualTo(5), "Failure 1");
                Assert.That(1, Is.EqualTo(0), "Failure 2");
                throw new Exception("Simulated Error");
            }
        }

        [Test]
        public void ExceptionThrownAfterWarning()
        {
            Assert.Multiple(() =>
            {
                Assert.Warn("WARNING");
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

        [Test]
        public void ThreeAssertsSucceed_Async()
        {
            Assert.Multiple(async () =>
            {
                await Task.Delay(100);
                Assert.That(2 + 2, Is.EqualTo(4));
                Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
            });
        }

        [Test]
        public async Task ThreeAssertsSucceed_Async_EnterScope()
        {
            using (Assert.EnterMultipleScope())
            {
                await Task.Delay(100);
                Assert.That(2 + 2, Is.EqualTo(4));
                Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
            }
        }

        [Test]
        public void NestedBlock_ThreeAssertsSucceed_Async()
        {
            Assert.Multiple(async () =>
            {
                await Task.Delay(100);
                Assert.That(2 + 2, Is.EqualTo(4));

                await Assert.MultipleAsync(async () =>
                {
                    await Task.Delay(100);
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
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
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                    await Task.Delay(100);
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
                });
            });
        }

        [Test]
        public async Task TwoNestedBlocks_ThreeAssertsSucceed_Async_EnterScope()
        {
            using (Assert.EnterMultipleScope())
            {
                using (Assert.EnterMultipleScope())
                {
                    await Task.Delay(100);
                    Assert.That(2 + 2, Is.EqualTo(4));
                }

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2));
                    await Task.Delay(100);
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
                }
            }
        }

        [Test]
        public void TwoAsserts_BothAssertsFail_Async()
        {
            Assert.Multiple(async () =>
            {
                await Task.Delay(100);
                Assert.That(Complex.RealPart, Is.EqualTo(5.0), "RealPart");
                Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
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
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                });
            });
        }

        [Test]
        public async Task TwoNestedBlocks_TwoAssertsFail_Async_EnterScope()
        {
            using (Assert.EnterMultipleScope())
            {
                using (Assert.EnterMultipleScope())
                {
                    await Task.Delay(100);
                    Assert.That(2 + 2, Is.EqualTo(5));
                }

                using (Assert.EnterMultipleScope())
                {
                    await Task.Delay(100);
                    Assert.That(Complex.RealPart, Is.EqualTo(5.2), "RealPart");
                    Assert.That(Complex.ImaginaryPart, Is.EqualTo(4.2), "ImaginaryPart");
                }
            }
        }

        [Test]
        public void NonReleasedScope()
        {
            Assert.EnterMultipleScope();
            Assert.That(Complex.RealPart, Is.EqualTo(5.2));
            Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
        }

        [Test]
        public void NonReleasedScopes()
        {
            Assert.EnterMultipleScope();
            Assert.That(2 + 2, Is.EqualTo(4));

            Assert.EnterMultipleScope();
            Assert.That(Complex.RealPart, Is.EqualTo(5.2));
            Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));
        }

        [Test]
        public void ScopeReleasedOutOfOrder()
        {
            IDisposable outerScope = Assert.EnterMultipleScope();
            Assert.That(2 + 2, Is.EqualTo(4));

            IDisposable innerScope = Assert.EnterMultipleScope();
            Assert.That(Complex.RealPart, Is.EqualTo(5.2));
            Assert.That(Complex.ImaginaryPart, Is.EqualTo(3.9));

            outerScope.Dispose();
            innerScope.Dispose();
        }
    }

    internal class ComplexNumber
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
