using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// Attribute used to define to order that test will run in.
    /// </summary>
    public class TestOrderAttribute : NUnitAttribute, IImplyFixture, IApplyToTest
    {
        /// <summary>
        /// Defines the order that the test will run in
        /// </summary>
        public readonly double Order;

        /// <summary>
        /// Defines the order that the test will run in
        /// </summary>
        /// <param name="order"></param>
        public TestOrderAttribute(double order)
        {
            Order = order;
        }

        /// <summary>
        /// Modifies a test as defined for the specific attribute.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            test.TestOrder = Order;
        }
    }
}
