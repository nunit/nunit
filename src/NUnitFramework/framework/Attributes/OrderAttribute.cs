// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Defines the order that the test will run in
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
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