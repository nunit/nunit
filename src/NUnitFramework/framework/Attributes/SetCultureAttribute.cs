// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

#if !NETSTANDARD1_6
using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Sets the current Culture for the duration of a test.
    /// <para>
    /// It may be specified at the level of a test or a fixture.
    /// The culture remains set until the test or fixture completes and is then reset to its original value.
    /// </para>
    /// </summary>
    /// <seealso cref="SetUICultureAttribute"/>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method|AttributeTargets.Assembly, AllowMultiple=false, Inherited=true)]
    public class SetCultureAttribute : PropertyAttribute, IApplyToContext
    {
        private readonly string _culture;

        /// <summary>
        /// Construct given the name of a culture
        /// </summary>
        /// <param name="culture"></param>
        public SetCultureAttribute( string culture ) : base( PropertyNames.SetCulture, culture ) 
        {
            _culture = culture;
        }

        #region IApplyToContext Members

        void IApplyToContext.ApplyToContext(TestExecutionContext context)
        {
            context.CurrentCulture = new System.Globalization.CultureInfo(_culture, false);
        }

        #endregion
    }
}
#endif
