// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks an assembly, test fixture or test method as applying to a specific Culture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false, Inherited=false)]
    public class CultureAttribute : IncludeExcludeAttribute, IApplyToTest
    {
        private readonly CultureDetector cultureDetector = new CultureDetector();
        private readonly CultureInfo currentCulture = CultureInfo.CurrentCulture;

        /// <summary>
        /// Constructor with no cultures specified, for use
        /// with named property syntax.
        /// </summary>
        public CultureAttribute() { }

        /// <summary>
        /// Constructor taking one or more cultures
        /// </summary>
        /// <param name="cultures">Comma-delimited list of cultures</param>
        public CultureAttribute(string? cultures) : base(cultures) { }

        #region IApplyToTest members

        /// <summary>
        /// Causes a test to be skipped if this CultureAttribute is not satisfied.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            if (test.RunState != RunState.NotRunnable && !IsCultureSupported(out var reason))
            {
                test.RunState = RunState.Skipped;

                // Discards the existing user-specified reason, if any.
                Reason = reason;
                test.Properties.Set(PropertyNames.SkipReason, reason);
            }
        }

        #endregion

        /// <summary>
        /// Tests to determine if the current culture is supported
        /// based on the properties of this attribute.
        /// </summary>
        /// <returns>True, if the current culture is supported</returns>
        private bool IsCultureSupported([NotNullWhen(false)] out string? reason)
        {
            if (Include != null && !cultureDetector.IsCultureSupported(Include))
            {
                reason = $"Only supported under culture {Include}";
                return false;
            }

            if (Exclude != null && cultureDetector.IsCultureSupported(Exclude))
            {
                reason = $"Not supported under culture {Exclude}";
                return false;
            }

            reason = null;
            return true;
        }

        /// <summary>
        /// Test to determine if the a particular culture or comma-
        /// delimited set of cultures is in use.
        /// </summary>
        /// <param name="culture">Name of the culture or comma-separated list of culture ids</param>
        /// <returns>True if the culture is in use on the system</returns>
        public bool IsCultureSupported(string culture)
        {
            culture = culture.Trim();

            if (culture.IndexOf(',') >= 0)
            {
                if (IsCultureSupported(culture.Split(new char[] { ',' })))
                    return true;
            }
            else
            {
                if (currentCulture.Name == culture || currentCulture.TwoLetterISOLanguageName == culture)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Test to determine if one of a collection of cultures
        /// is being used currently.
        /// </summary>
        /// <param name="cultures"></param>
        /// <returns></returns>
        public bool IsCultureSupported(string[] cultures)
        {
            foreach (string culture in cultures)
                if (IsCultureSupported(culture))
                    return true;

            return false;
        }
    }
}
