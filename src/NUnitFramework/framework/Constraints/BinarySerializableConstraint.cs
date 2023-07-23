// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private readonly BinaryFormatter _serializer = new();

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "binary serializable";

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is null)
                throw new ArgumentNullException(nameof(actual));

            MemoryStream stream = new MemoryStream();
            bool succeeded = false;

            try
            {
#pragma warning disable SYSLIB0011
                _serializer.Serialize(stream, actual);

                stream.Seek(0, SeekOrigin.Begin);

                succeeded = _serializer.Deserialize(stream) is not null;
#pragma warning restore SYSLIB0011
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
