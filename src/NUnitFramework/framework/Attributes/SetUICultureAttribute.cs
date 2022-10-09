// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
    /// <summary>
    /// Sets the current UI Culture on an assembly, test fixture or test method
    /// for the duration of a test. The UI culture remains set until the test or
    /// fixture completes and is then reset to its original value.
    /// </summary>
    /// <seealso cref="SetCultureAttribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false, Inherited=true)]
    public class SetUICultureAttribute : PropertyAttribute, IApplyToContext
    {
        private readonly string _culture;

        /// <summary>
        /// Construct given the name of a culture
        /// </summary>
        /// <param name="culture"></param>
        public SetUICultureAttribute(string culture) : base("SetUICulture", culture)
        {
            _culture = culture;
        }

        #region IApplyToContext Members

        void IApplyToContext.ApplyToContext(TestExecutionContext context)
        {
            context.CurrentUICulture = new System.Globalization.CultureInfo(_culture, false);
        }

        #endregion
    }
}
