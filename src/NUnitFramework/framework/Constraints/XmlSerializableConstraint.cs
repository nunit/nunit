// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// XmlSerializableConstraint tests whether
    /// an object is serializable in XML format.
    /// </summary>
    public class XmlSerializableConstraint : Constraint
    {
        private XmlSerializer serializer;

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description
        {
            get { return "XML serializable"; }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if(actual == null)
                throw new ArgumentNullException(nameof(actual));

            MemoryStream stream = new MemoryStream();

            bool succeeded = false;

            try
            {
                serializer = new XmlSerializer(actual.GetType());

                serializer.Serialize(stream, actual);

                stream.Seek(0, SeekOrigin.Begin);

                succeeded = serializer.Deserialize(stream) != null;
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
