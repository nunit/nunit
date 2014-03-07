// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// SetUpTearDownList holds the setup and teardown methods
    /// for a given fixture as a list of SetUpTearDownNodes.
    /// Each node in the list represents one level in the
    /// inheritance hierarchy for the fixture type.
    /// </summary>
    public class SetUpTearDownList
    {
        private SetUpTearDownNode _topLevelNode;

        /// <summary>
        /// Construct a SetUpTearDownList.
        /// </summary>
        /// <param name="fixtureType">The Type of the fixture to which the list applies</param>
        /// <param name="setUpType">The Type of the attribute used to mark setup methods</param>
        /// <param name="tearDownType">The Type of the attribute used to mark teardown methods</param>
        /// <returns></returns>
        public SetUpTearDownList(Type fixtureType, Type setUpType, Type tearDownType)
        {
            var setUpMethods = Reflect.GetMethodsWithAttribute(fixtureType, setUpType, true);
            var tearDownMethods = Reflect.GetMethodsWithAttribute(fixtureType, tearDownType, true);

            _topLevelNode = BuildList(fixtureType, setUpMethods, tearDownMethods);
        }

        /// <summary>
        /// Run all setup methods.
        /// </summary>
        /// <param name="context">The execution context to use for running.</param>
        public void RunSetUp(TestExecutionContext context)
        {
            _topLevelNode.RunSetUp(context);
        }

        /// <summary>
        /// Run the teardown methods
        /// </summary>
        /// <param name="context">The execution context to use for running.</param>
        public void RunTearDown(TestExecutionContext context)
        {
            _topLevelNode.RunTearDown(context);
        }

        #region Helper Methods

        // This method builds a list of nodes that can be used to 
        // run setup and teardown according to the NUnit specs.
        // We need to execute setup and teardown methods one level
        // at a time. However, we can't discover them by reflection
        // one level at a time, because that would cause overridden
        // methods to be called twice, once on the base class and
        // once on the derived class.
        // 
        // For that reason, we start with a list of all setup and
        // teardown methods, found using a single reflection call,
        // and then descend through the inheritance hierarchy,
        // adding each method to the appropriate level as we go.
        private static SetUpTearDownNode BuildList(Type fixtureType, IList<MethodInfo> setUpMethods, IList<MethodInfo> tearDownMethods)
        {
            // Create lists of methods for this level only.
            // Note that FindAll can't be used because it's not
            // available on all the platforms we support.
            var mySetUpMethods = SelectMethodsByDeclaringType(fixtureType, setUpMethods);
            var myTearDownMethods = SelectMethodsByDeclaringType(fixtureType, tearDownMethods);

            var node = new SetUpTearDownNode(mySetUpMethods, myTearDownMethods);

            var baseType = fixtureType.BaseType;
            if (baseType != typeof(object) && baseType != null)
            {
                var next = BuildList(baseType, setUpMethods, tearDownMethods);
                if (next.HasMethods)
                    if (!node.HasMethods)
                        return next;
                node.Next = next;
            }

            return node;
        }

        private static List<MethodInfo> SelectMethodsByDeclaringType(Type type, IList<MethodInfo> methods)
        {
            var list = new List<MethodInfo>();

            foreach (var method in methods)
                if (method.DeclaringType == type)
                    list.Add(method);

            return list;
        }

        #endregion
    }
}
