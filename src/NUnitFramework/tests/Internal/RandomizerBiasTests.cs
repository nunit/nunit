// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Internal
{
    public static class RandomizerBiasTests
    {
        private const int TrialCount = 100000;
        private const double ConfidenceInterval = 0.00965802342765787;

        /* Random generators wouldn’t be truly random if it was impossible to hit a highly biased sequence.
        These tests fail as a false negative whenever an actually-*unbiased* generator provides a sequence at
        random that turns out to be biased this time.
        What we can do is quantify the *chance* of such a false negative and choose that chance to be sufficiently
        small to not bother us with spurious failures, while maintaining a precise enough signal to fail loud
        and clear if the generator is biased.

        Assuming around 100 CI runs per month, 6 invocations of the build script per CI run, 5 target frameworks
        with these same tests, and 97 current tests in this class, a binomial trial
        (https://en.wikipedia.org/wiki/Binomial_proportion_confidence_interval) is run 100×6×5×97 = ‭291,000‬ times
        per month.

        Let’s see what happens if we set the chance of a false negative to one in a million, a 99.9999% chance of
        success per trial. The chance of all 291,000 trials per month having absolutely no false negative is
        99.9999% ^ ‭291,000‬, or ~74.75%. The chance of there being at least one false negative per month is then
        100% - 74.75%, or 25.25‬%, or one false negative on average every four months. This seems too frequent.

        Rather than expecting at least one false negative every four months, we can drive that chance down by
        choosing the per-trial confidence to be 99.9999999%, or a one-in-a-billion chance of false negative.
        This brings the per-month chance of all trials having no false negative to 99.97%. One or more false
        negatives would then only occur on average only one month out of every 286 years.

        One in a billion, or 99.9999999% confidence, seems desirable. To obtain this confidence level, we need a
        sufficiently high trial count or a sufficiently high tolerance. I chose a trial count of 100000 because
        that takes only about 10ms on my machine. That results in a confidence interval (see Wikipedia link above)
        of ~±0.0097 around 0.5. 0.5 of the trials are expected to succeed, where success means something
        like "the third bit is set"—something which we expect to happen half the time if the generator is not
        biased. If we check whether the actual tested fraction of the time is within ~±0.0097 of 0.5, we will
        have the desired 99.9999999% confidence level per trial.

        I used this tool to calculate the confidence interval for trial count 100000, success count 50000,
        confidence 99.9999999%, and method Agresti-Coull:
        http://epitools.ausvet.com.au/content.php?page=CIProportion&SampleSize=100000&Positive=50000&Conf=0.999999999&method=5&Digits=17
        The result is placed in the ConfidenceInterval constant above.

        I chose Agresti-Coull based a citation in https://stats.stackexchange.com/a/82724, Interval Estimation for
        a Binomial Proportion by Brown, Cai and DasGupta in Statistical Science 2001, vol. 16, no. 2, pages 101–133.
        > [W]e recommend the Wilson interval or the equal-tailed Jeffreys prior interval for small n and the
        > interval suggested in Agresti and Coull for larger n.
        */

        private static void AssertProbability50PercentWithinConfidenceInterval(int successCount)
        {
            Assert.That(
                successCount / (double)TrialCount,
                Is.EqualTo(0.5).Within(ConfidenceInterval));
        }

        [Test]
        public static void NextDecimalIsNotBiased([Range(0, 95)] int bit)
        {
            var randomizer = new Randomizer();

            var bitSetCount = 0;

            var part = bit / 32;
            var mask = 1 << (bit % 32);

            for (var i = 0; i < TrialCount; i++)
            {
                var value = randomizer.NextDecimal();
                var parts = decimal.GetBits(value);

                if ((parts[part] & mask) != 0)
                {
                    bitSetCount++;
                }
            }

            AssertProbability50PercentWithinConfidenceInterval(bitSetCount);
        }

        [Test]
        public static void NextDecimalWithMaximumIsNotBiased()
        {
            var randomizer = new Randomizer();

            const decimal wholeRange = decimal.MaxValue * (2 / 3m);
            const decimal halfRange = wholeRange / 2;

            var countInTopHalf = 0;

            for (var i = 0; i < TrialCount; i++)
            {
                if (randomizer.NextDecimal(wholeRange) >= halfRange)
                {
                    countInTopHalf++;
                }
            }

            AssertProbability50PercentWithinConfidenceInterval(countInTopHalf);
        }
    }
}
