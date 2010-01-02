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

#if !NUNITLITE
using System;
using System.Globalization;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	/// <summary>
	/// Abstract base for Attributes that are used to include tests
	/// in the test run based on environmental settings.
	/// </summary>
	public abstract class IncludeExcludeAttribute : Attribute
	{
		private string include;
		private string exclude;
		private string reason;

        protected bool shouldRun;

		/// <summary>
		/// Constructor with no included items specified, for use
		/// with named property syntax.
		/// </summary>
		public IncludeExcludeAttribute() { }

		/// <summary>
		/// Constructor taking one or more included items
		/// </summary>
		/// <param name="include">Comma-delimited list of included items</param>
		public IncludeExcludeAttribute( string include )
		{
			this.include = include;
		}

		/// <summary>
		/// Name of the item that is needed in order for
		/// a test to run. Multiple itemss may be given,
		/// separated by a comma.
		/// </summary>
		public string Include
		{
			get { return this.include; }
			set { include = value; }
		}

		/// <summary>
		/// Name of the item to be excluded. Multiple items
		/// may be given, separated by a comma.
		/// </summary>
		public string Exclude
		{
			get { return this.exclude; }
			set { this.exclude = value; }
		}

		/// <summary>
		/// The reason for including or excluding the test
		/// </summary>
		public string Reason
		{
			get { return reason; }
			set { reason = value; }
		}
	}

	/// <summary>
	/// PlatformAttribute is used to mark a test fixture or an
	/// individual method as applying to a particular platform only.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method|AttributeTargets.Assembly, AllowMultiple=true)]
	public class PlatformAttribute : IncludeExcludeAttribute, ISetRunState
	{
        private PlatformHelper platformHelper = new PlatformHelper();

		/// <summary>
		/// Constructor with no platforms specified, for use
		/// with named property syntax.
		/// </summary>
		public PlatformAttribute() { }

		/// <summary>
		/// Constructor taking one or more platforms
		/// </summary>
		/// <param name="platforms">Comma-deliminted list of platforms</param>
		public PlatformAttribute( string platforms ) : base( platforms ) { }

        #region ISetRunState members

        public RunState GetRunState()
        {
            return platformHelper.IsPlatformSupported(this)
                ? RunState.Runnable
                : RunState.Skipped;
        }

        public string GetReason()
        {
            return Reason != null
                ? Reason
                : platformHelper.Reason;
        }

        #endregion
    }

	/// <summary>
	/// CultureAttribute is used to mark a test fixture or an
	/// individual method as applying to a particular Culture only.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method|AttributeTargets.Assembly, AllowMultiple=false)]
	public class CultureAttribute : IncludeExcludeAttribute, ISetRunState
	{
        private CultureDetector cultureDetector = new CultureDetector();
        private CultureInfo currentCulture = CultureInfo.CurrentCulture;

		/// <summary>
		/// Constructor with no cultures specified, for use
		/// with named property syntax.
		/// </summary>
		public CultureAttribute() { }

		/// <summary>
		/// Constructor taking one or more cultures
		/// </summary>
		/// <param name="cultures">Comma-deliminted list of cultures</param>
		public CultureAttribute( string cultures ) : base( cultures ) { }

        #region ISetRunState members

        public RunState GetRunState()
        {
            return IsCultureSupported()
                ? RunState.Runnable
                : RunState.Skipped;
        }

        public string GetReason()
        {
            return Reason;
        }

        #endregion

        /// <summary>
        /// Tests to determine if the current culture is supported
        /// based on the properties of this attribute.
        /// </summary>
        /// <returns>True, if the current culture is supported</returns>
        private bool IsCultureSupported()
        {
            try
            {
                if (Include != null && !cultureDetector.IsCultureSupported(Include))
                {
                    Reason = string.Format("Only supported under culture {0}", Include);
                    return false;
                }

                if (Exclude != null && cultureDetector.IsCultureSupported(Exclude))
                {
                    Reason = string.Format("Not supported under culture {0}", Exclude);
                    return false;
                }
            }
            catch (ArgumentException ex)
            {
                Reason = string.Format("Invalid culture: {0}", ex.ParamName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Test to determine if the a particular culture or comma-
        /// delimited set of cultures is in use.
        /// </summary>
        /// <param name="culture">Name of the culture or comma-separated list of culture names</param>
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
        /// Test to determine if one of a collection of culturess
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
#endif