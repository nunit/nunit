// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

#if PORTABLE || NETSTANDARD1_6
using System.Linq;
#endif

namespace NUnit.Framework.Internal
{
    using Interfaces;

    internal class ActionsHelper
    {
#if PORTABLE || NETSTANDARD1_6
        public static ITestAction[] GetActionsFromAttributeProvider(Assembly attributeProvider)
        {
            return attributeProvider != null
                ? attributeProvider.GetAttributes<ITestAction>().ToArray()
                : new ITestAction[0];
        }

        public static ITestAction[] GetActionsFromAttributeProvider(MemberInfo attributeProvider)
        {
            return attributeProvider != null
                ? attributeProvider.GetAttributes<ITestAction>(false).ToArray()
                : new ITestAction[0];
        }
#else
        public static ITestAction[] GetActionsFromAttributeProvider(ICustomAttributeProvider attributeProvider)
        {
            return attributeProvider != null
                ? (ITestAction[])attributeProvider.GetCustomAttributes(typeof(ITestAction), false)
                : new ITestAction[0];
        }
#endif

        public static ITestAction[] GetActionsFromTypesAttributes(Type type)
        {
            var actions = new List<ITestAction>();

            if (type != null && type != typeof(object))
            {
                actions.AddRange(GetActionsFromTypesAttributes(type.GetTypeInfo().BaseType));

                foreach (Type interfaceType in TypeHelper.GetDeclaredInterfaces(type))
                    actions.AddRange(GetActionsFromAttributeProvider(interfaceType.GetTypeInfo()));

                actions.AddRange(GetActionsFromAttributeProvider(type.GetTypeInfo()));
            }

            return actions.ToArray();
        }
    }
}
