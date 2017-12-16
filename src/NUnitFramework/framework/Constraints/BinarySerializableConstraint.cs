// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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

#if !NETSTANDARD1_6
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// BinarySerializableConstraint tests whether 
    /// an object is serializable in binary format.
    /// </summary>
    public class BinarySerializableConstraint : Constraint
    {
        readonly BinaryFormatter serializer = new BinaryFormatter();

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "binary serializable"; }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual == null)
                throw new ArgumentNullException(nameof(actual));

            MemoryStream stream = new MemoryStream();
            bool succeeded = false;

            try
            {
                serializer.Serialize(stream, actual);

                stream.Seek(0, SeekOrigin.Begin);

                succeeded = serializer.Deserialize(stream) != null;
            }
            catch (SerializationException)
            {
                // Ignore and return failure
            }

            return new ConstraintResult(this, actual.GetType(), succeeded);
        }

        /// <summary>
        /// Returns the string representation
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return "<binaryserializable>";
        }
    }
}
#endif
