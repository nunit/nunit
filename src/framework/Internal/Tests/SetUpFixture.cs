// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.IO;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// SetUpFixture extends TestSuite and supports
	/// Setup and TearDown methods.
	/// </summary>
	public class SetUpFixture : TestSuite
	{
		#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpFixture"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
		public SetUpFixture( Type type ) : base( type )
		{
            this.Name = type.Namespace;
            if (this.Name == null)
                this.Name = "[default namespace]";
            int index = this.Name.LastIndexOf('.');
            if (index > 0)
                this.Name = this.Name.Substring(index + 1);

            this.oneTimeSetUpMethods = GetSetUpTearDownMethods(typeof(NUnit.Framework.SetUpAttribute));
            this.oneTimeTearDownMethods = GetSetUpTearDownMethods(typeof(NUnit.Framework.TearDownAttribute));
		}

        private MethodInfo[] GetSetUpTearDownMethods(Type attrType)
        {
            MethodInfo[] methods = Reflect.GetMethodsWithAttribute(FixtureType, attrType, true);

            foreach (MethodInfo method in methods)
                if (method.IsAbstract ||
                     !method.IsPublic && !method.IsFamily ||
                     method.GetParameters().Length > 0 ||
                     !method.ReturnType.Equals(typeof(void)))
                {
                    this.Properties.Set(
                        PropertyNames.SkipReason,
                        string.Format("Invalid signature for SetUp or TearDown method: {0}", method.Name));
                    this.RunState = RunState.NotRunnable;
                    break;
                }

            return methods;
        }
        #endregion
    }
}
