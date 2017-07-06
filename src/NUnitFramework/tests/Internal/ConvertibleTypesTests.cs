// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
using Conversion = NUnit.Framework.Internal.ConvertibleTypes.Conversion;

namespace NUnit.Framework.Internal
{
    public class ConvertibleTypesTests
    {
        public ConvertibleTypes Types;
        public Conversion SimpleConversion;

        [SetUp]
        public void SetUp()
        {
            Types = new ConvertibleTypes();
            SimpleConversion = new Conversion(typeof(byte), typeof(int));
        }

        [Test]
        public void Add_SimpleConversion_ProperlyAdded()
        {
            Types.Add(SimpleConversion);

            Assert.That(Types[typeof(byte), typeof(int)]);
            Assert.That(Types, Is.EquivalentTo(new[] { SimpleConversion }));
        }

        [Test]
        public void Add_ConversionAlreadyExists_Skipped()
        {
            Types.Add(SimpleConversion);
            Types.Add(SimpleConversion);

            Assert.That(Types[typeof(byte), typeof(int)]);
            Assert.That(Types, Is.EquivalentTo(new[] { SimpleConversion }));
        }

        [Test]
        public void Add_SourceTypeAndTargetType_ProperlyAdded()
        {
            Types.Add(SimpleConversion.SourceType, SimpleConversion.TargetType);

            Assert.That(Types[typeof(byte), typeof(int)]);
            Assert.That(Types, Is.EquivalentTo(new[] { SimpleConversion }));
        }

        [Test]
        public void Add_SourceTypeAndTargetTypes_ProperlyAdded()
        {
            Types.Add(typeof(byte), typeof(int), typeof(long));

            Assert.That(Types[typeof(byte), typeof(int)]);
            Assert.That(Types[typeof(byte), typeof(long)]);
            Assert.That(Types, Is.EquivalentTo(new[]
            {
                new Conversion(typeof(byte), typeof(int)),
                new Conversion(typeof(byte), typeof(long))
            }));
        }

        [Test]
        public void Add_SourceTypesAndTargetType_ProperlyAdded()
        {
            Types.Add(new Type[] { typeof(byte), typeof(short) }, typeof(int));

            Assert.That(Types[typeof(byte), typeof(int)]);
            Assert.That(Types[typeof(short), typeof(int)]);
            Assert.That(Types, Is.EquivalentTo(new[]
            {
                new Conversion(typeof(byte), typeof(int)),
                new Conversion(typeof(short), typeof(int))
            }));
        }

        [Test]
        public void Add_SourceTypesAndTargetTypes_ProperlyAdded()
        {
            Types.Add(
                new Type[] { typeof(byte), typeof(short) },
                new Type[] { typeof(int), typeof(long) }
            );

            Assert.That(Types[typeof(byte), typeof(int)]);
            Assert.That(Types[typeof(byte), typeof(long)]);
            Assert.That(Types[typeof(short), typeof(int)]);
            Assert.That(Types[typeof(short), typeof(long)]);
            Assert.That(Types, Is.EquivalentTo(new[]
            {
                new Conversion(typeof(byte), typeof(int)),
                new Conversion(typeof(short), typeof(int)),
                new Conversion(typeof(byte), typeof(long)),
                new Conversion(typeof(short), typeof(long))
            }));
        }

        [Test]
        public void Validate_ValidConversion_NoChanges()
        {
            Types.Add(SimpleConversion);

            Types.Validate();

            Assert.That(Types[typeof(byte), typeof(int)]);
            Assert.That(Types, Is.EquivalentTo(new[] { SimpleConversion }));
        }

        [Test]
        public void Validate_ForbiddenConversion_Removed()
        {
            Types.Add(typeof(int), typeof(TimeSpan));

            Types.Validate();

            Assert.False(Types[typeof(int), typeof(TimeSpan)]);
            Assert.That(Types, Is.Empty);
        }
    }
}
