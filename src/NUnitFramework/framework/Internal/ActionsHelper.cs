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

namespace NUnit.Framework.Internal
{
    using Interfaces;

    internal class ActionsHelper
    {
        public static void ExecuteBeforeActions(IEnumerable<ITestAction> actions, ITest test)
        {
            ExecuteActions(ActionPhase.Before, actions, test);
        }

        public static void ExecuteAfterActions(IEnumerable<ITestAction> actions, ITest test)
        {
            ExecuteActions(ActionPhase.After, actions, test);
        }

        private static void ExecuteActions(ActionPhase phase, IEnumerable<ITestAction> actions, ITest test)
        {
            if (actions == null)
                return;

            foreach (ITestAction action in GetFilteredAndSortedActions(actions, phase))
            {
                if(phase == ActionPhase.Before)
                    action.BeforeTest(test);
                else
                    action.AfterTest(test);
            }
        }

#if PORTABLE
        public static ITestAction[] GetActionsFromAttributeProvider(Assembly attributeProvider)
        {
            if (attributeProvider == null)
                return new ITestAction[0];

            var actions = new List<ITestAction>((ITestAction[])attributeProvider.GetCustomAttributes(typeof(ITestAction), false));
            actions.Sort(SortByTargetDescending);

            return actions.ToArray();
        }

        public static ITestAction[] GetActionsFromAttributeProvider(MemberInfo attributeProvider)
#else
        public static ITestAction[] GetActionsFromAttributeProvider(ICustomAttributeProvider attributeProvider)
#endif
        {
            if (attributeProvider == null)
                return new ITestAction[0];

            var actions = new List<ITestAction>((ITestAction[])attributeProvider.GetCustomAttributes(typeof(ITestAction), false));
            actions.Sort(SortByTargetDescending);

            return actions.ToArray();
        }

        public static ITestAction[] GetActionsFromTypesAttributes(Type type)
        {
            if(type == null)
                return new ITestAction[0];

            if(type == typeof(object))
                return new ITestAction[0];

            var actions = new List<ITestAction>();

            actions.AddRange(GetActionsFromTypesAttributes(type.BaseType));

            Type[] declaredInterfaces = GetDeclaredInterfaces(type);

            foreach(Type interfaceType in declaredInterfaces)
                actions.AddRange(GetActionsFromAttributeProvider(interfaceType));

            actions.AddRange(GetActionsFromAttributeProvider(type));

            return actions.ToArray();
        }

        private static Type[] GetDeclaredInterfaces(Type type)
        {
            List<Type> interfaces = new List<Type>(type.GetInterfaces());

            if (type.BaseType == typeof(object))
                return interfaces.ToArray();

            List<Type> baseInterfaces = new List<Type>(type.BaseType.GetInterfaces());
            List<Type> declaredInterfaces = new List<Type>();

            foreach (Type interfaceType in interfaces)
            {
                if (!baseInterfaces.Contains(interfaceType))
                    declaredInterfaces.Add(interfaceType);
            }

            return declaredInterfaces.ToArray();
        }

        private static ITestAction[] GetFilteredAndSortedActions(IEnumerable<ITestAction> actions, ActionPhase phase)
        {
            var filteredActions = new List<ITestAction>();
            foreach (var actionItem in actions)
            {
                if (filteredActions.Contains(actionItem) != true)
                    filteredActions.Add(actionItem);
            }

            if(phase == ActionPhase.After)
                filteredActions.Reverse();

            return filteredActions.ToArray();
        }

        private static int SortByTargetDescending(ITestAction x, ITestAction y)
        {
            return y.Targets.CompareTo(x.Targets);
        }

        private enum ActionPhase
        {
            Before,
            After
        }
    }
}
