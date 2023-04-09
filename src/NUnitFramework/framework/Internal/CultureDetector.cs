// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Globalization;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// CultureDetector is a helper class used by NUnit to determine
    /// whether a test should be run based on the current culture.
    /// </summary>
    public class CultureDetector
    {
        private readonly CultureInfo currentCulture;

        // Set whenever we fail to support a list of platforms
        private string reason = string.Empty;

        /// <summary>
        /// Default constructor uses the current culture.
        /// </summary>
        public CultureDetector()
        {
            this.currentCulture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Construct a CultureDetector for a particular culture for testing.
        /// </summary>
        /// <param name="culture">The culture to be used</param>
        public CultureDetector( string culture )
        {
            this.currentCulture = new CultureInfo( culture );
        }

        /// <summary>
        /// Test to determine if one of a collection of cultures
        /// is being used currently.
        /// </summary>
        /// <param name="cultures"></param>
        /// <returns></returns>
        public bool IsCultureSupported( string[] cultures )
        {
            foreach( string culture in cultures )
                if ( IsCultureSupported( culture ) )
                    return true;

            return false;
        }

        /// <summary>
        /// Tests to determine if the current culture is supported
        /// based on a culture attribute.
        /// </summary>
        /// <param name="cultureAttribute">The attribute to examine</param>
        /// <returns></returns>
        public bool IsCultureSupported( CultureAttribute cultureAttribute )
        {
            string include = cultureAttribute.Include;
            string exclude = cultureAttribute.Exclude;

            if (include != null && !IsCultureSupported(include))
            {
                reason = $"Only supported under culture {include}";
                return false;
            }

            if (exclude != null && IsCultureSupported(exclude))
            {
                reason = $"Not supported under culture {exclude}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Test to determine if the a particular culture or comma-
        /// delimited set of cultures is in use.
        /// </summary>
        /// <param name="culture">Name of the culture or comma-separated list of culture ids</param>
        /// <returns>True if the culture is in use on the system</returns>
        public bool IsCultureSupported( string culture )
        {
            culture = culture.Trim();

            if ( culture.IndexOf( ',' ) >= 0 )
            {
                if ( IsCultureSupported( culture.Split( new char[] { ',' } ) ) )
                    return true;
            }
            else
            {
                if( this.currentCulture.Name == culture || this.currentCulture.TwoLetterISOLanguageName == culture)
                    return true;
            }

            this.reason = "Only supported under culture " + culture;
            return false;
        }

        /// <summary>
        /// Return the last failure reason. Results are not
        /// defined if called before IsSupported( Attribute )
        /// is called.
        /// </summary>
        public string Reason => reason;
    }
}
