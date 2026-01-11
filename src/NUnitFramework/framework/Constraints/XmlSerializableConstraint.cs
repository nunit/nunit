// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Xml.Serialization;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// XmlSerializableConstraint tests whether
    /// an object is serializable in XML format.
    /// </summary>
    public class XmlSerializableConstraint : Constraint
    {
        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description => "XML serializable";

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            ArgumentNullException.ThrowIfNull(actual);

            MemoryStream stream = new MemoryStream();

            bool succeeded = false;

            try
            {
                var serializer = new XmlSerializer(actual.GetType());

                serializer.Serialize(stream, actual);

                stream.Seek(0, SeekOrigin.Begin);

                succeeded = serializer.Deserialize(stream) is not null;
            }
            catch (NotSupportedException)
            {
                // Ignore and return failure
            }
            catch (InvalidOperationException)
            {
                // Ignore and return failure
            }

            return new ConstraintResult(this, actual.GetType(), succeeded);
        }

        /// <summary>
        /// Returns the string representation of this constraint
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return "<xmlserializable>";
        }
    }
}
