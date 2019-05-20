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
        [Test]
        public static void NextDecimalIsNotBiased([Range(0, 95)] int bit)
        {
            var randomizer = new Randomizer();

            const int totalCount = 100000;
            var bitSetCount = 0;

            var part = bit / 32;
            var mask = 1 << (bit % 32);

            for (var i = 0; i < totalCount; i++)
            {
                var value = randomizer.NextDecimal();
                var parts = decimal.GetBits(value);

                if ((parts[part] & mask) != 0)
                {
                    bitSetCount++;
                }
            }

            Assert.That(bitSetCount / (double)totalCount, Is.EqualTo(0.5).Within(0.01));
        }

        [Test]
        public static void NextDecimalWithMaximumIsNotBiased()
        {
            var randomizer = new Randomizer();

            const decimal wholeRange = decimal.MaxValue * (2 / 3m);
            const decimal halfRange = wholeRange / 2;

            const int totalCount = 10000;
            var countInTopHalf = 0;

            for (var i = 0; i < totalCount; i++)
            {
                if (randomizer.NextDecimal(wholeRange) >= halfRange)
                {
                    countInTopHalf++;
                }
            }

            Assert.That(countInTopHalf / (double)totalCount, Is.EqualTo(0.5).Within(0.01));
        }
    }
}
