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
using System.Linq;
using System.Collections.Generic;
#if SSHARP
using Crestron.SimplSharp;
#else
using System.Threading;
#endif
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// Attribute used to specify runtime parameters used by the assembly/testsuite/test,
    /// and their properties
    /// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class TestParameterAttribute : NUnitAttribute, IApplyToTest, IXmlNodeBuilder, IEquatable<TestParameterAttribute>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
		public TestParameterAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
		public TestParameterAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
		public string Name { get; private set; }
        /// <summary>
        /// 
        /// </summary>
		public Type Type { get; private set; }
        /// <summary>
        /// 
        /// </summary>
		public string MethodParameterName { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public object MinimumValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public object MaximumValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public object[] ValidValues { get; set; }


        #region IApplyToTest Members

        /// <summary>
        /// Modifies a test by adding a TestParameter to it.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            if (test.TestParameterAttributes == null)
                test.TestParameterAttributes = new List<TestParameterAttribute>();

            test.TestParameterAttributes.Add(this);

            if (Name.IndexOfAny(new char[] { ';', '=' }) >= 0)
            {
                test.RunState = RunState.NotRunnable;
                test.Properties.Set(PropertyNames.SkipReason, "TestParameterAttribute Name must not contain ';', '='");
            }
            else if (!String.IsNullOrEmpty(MethodParameterName))
            {
                var pars = test.Method.GetParameters();
                var pi = pars.FirstOrDefault(par => par.ParameterInfo.Name.Equals(MethodParameterName));
                if (pi != null)
                {
                    if (Type == null)
                        Type = pi.ParameterType;
                    else if (Type != pi.ParameterType)
                    {
                        test.RunState = RunState.NotRunnable;
                        test.Properties.Set(PropertyNames.SkipReason, "TestParameterAttribute Type must be the same a the type of \"" + pi.ParameterInfo.Name + "\"");
                    }
                }
                else
                {
                    test.RunState = RunState.NotRunnable;
                    test.Properties.Set(PropertyNames.SkipReason, "TestParameterAttribute MethodParameterName must be one of the test method parameters");
                }
            }

            if (Type == null)
                Type = typeof(string);
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public TNode ToXml(bool recursive)
        {
            return AddToXml(new TNode("dummy"), recursive);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode prop = parentNode.AddElement("test-parameter");
            prop.AddAttribute("name", Name);
            if (Type != typeof(string))
                prop.AddAttribute("type", Type.FullName);
            if (!String.IsNullOrEmpty(MethodParameterName))
                prop.AddAttribute("inject", MethodParameterName);
            if (MinimumValue != null)
                prop.AddAttribute("minvalue", MinimumValue.ToString());
            if (MaximumValue != null)
                prop.AddAttribute("maxvalue", MaximumValue.ToString());
            if (ValidValues != null)
                foreach (var obj in ValidValues)
                    prop.AddElement("valid-value").AddAttribute("value", obj.ToString());

            return prop;
        }

        #endregion

        #region IEquatable<TestParameterAttribute> Members

        /// <summary>
        /// Define Equals member if IEquatable interface
        /// </summary>
        /// <param name="other"></param>
        /// <returns>tue if equal, false if not</returns>
        public bool Equals(TestParameterAttribute other)
        {
            return Name.Equals(other.Name);
        }

        #endregion
    }
}