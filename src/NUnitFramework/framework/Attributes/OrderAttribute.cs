// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Defines the order that the test will run in
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class OrderAttribute : NUnitAttribute, IApplyToTest
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
