using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    public class OrderAttribute : NUnitAttribute, IImplyFixture, IApplyToTest
    {
          /// <summary>
         /// Defines the order that the test will run in
         /// </summary>
         public readonly int Order;
 
         /// <summary>
         /// Defines the order that the test will run in
         /// </summary>
         /// <param name="order"></param>
         public OrderAttribute(int order)
         {
             Order = order;
         }
 
         /// <summary>
         /// Modifies a test as defined for the specific attribute.
         /// </summary>
         /// <param name="test">The test to modify</param>
         public void ApplyToTest(Test test)
        {
            if (!test.Properties.ContainsKey(PropertyNames.Order))
                test.Properties.Set(PropertyNames.Order, Order);
        }
     }
 }