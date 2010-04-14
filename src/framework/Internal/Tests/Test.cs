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
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Reflection;
using System.Xml;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// The Test abstract class represents a test within the framework.
	/// </summary>
	public abstract class Test : ITest, IComparable
    {
        #region Constants
        //private static readonly string SETCULTURE = "_SETCULTURE";
        private static readonly string DESCRIPTION = "_DESCRIPTION";
        private static readonly string IGNOREREASON = "_IGNOREREASON";
        private static readonly string CATEGORIES = "_CATEGORIES";
        #endregion

        #region Fields

        /// <summary>
        /// Static value to seed ids. It's started at 1000 so any
        /// uninitialized ids will stand out.
        /// </summary>
        private static int nextID = 1000;

        private int id;
        private string name;
        private string fullName;

		/// <summary>
		/// Indicates whether the test should be executed
		/// </summary>
		private RunState runState;

		/// <summary>
		/// Test suite containing this test, or null
		/// </summary>
		private Test parent;
		
		/// <summary>
		/// A dictionary of properties, used to add information
		/// to tests without requiring the class to change.
		/// </summary>
		private IDictionary properties;

		#endregion

        #region Properties
#if !NUNITLITE
        /// <summary>
        /// Return true if the test requires a thread
        /// </summary>
        public bool RequiresThread
        {
            get { return Properties.Contains("RequiresThread") && (bool)Properties["RequiresThread"]; }
        }

        /// <summary>
        /// Get the desired apartment state for running the test
        /// </summary>
        public ApartmentState ApartmentState
        {
            get
            {
                return Properties.Contains("APARTMENT_STATE")
                    ? (ApartmentState)Properties["APARTMENT_STATE"]
                    : GetCurrentApartment();
            }
        }

        /// <summary>
        /// Get the current apartment state of the test
        /// </summary>
        protected ApartmentState GetCurrentApartment()
        {
#if CLR_2_0
            return Thread.CurrentThread.GetApartmentState();
#else
            return Thread.CurrentThread.ApartmentState;
#endif
        }
#endif
        #endregion

        #region Construction

        /// <summary>
		/// Constructs a test given its name
		/// </summary>
		/// <param name="name">The name of the test</param>
		protected Test( string name )
		{
			this.fullName = name;
			this.name = name;
            this.id = unchecked(nextID++);

            this.runState = RunState.Runnable;
		}

		/// <summary>
		/// Constructs a test given the path through the
		/// test hierarchy to its parent and a name.
		/// </summary>
		/// <param name="pathName">The parent tests full name</param>
		/// <param name="name">The name of the test</param>
		protected Test( string pathName, string name ) 
		{ 
			this.fullName = pathName == null || pathName == string.Empty 
				? name : pathName + "." + name;
			this.name = name;
            this.id = unchecked(nextID++);

            this.runState = RunState.Runnable;
		}

		#endregion

		#region ITest Members

        #region Properties

        /// <summary>
        /// Gets or sets the id of the test
        /// </summary>
        /// <value></value>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the test
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the fully qualified name of the test
        /// </summary>
        /// <value></value>
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

		/// <summary>
		/// Whether or not the test should be run
		/// </summary>
        public RunState RunState
        {
            get { return runState; }
            set { runState = value; }
        }

		/// <summary>
		/// Reason for not running the test, if applicable
		/// </summary>
		public string IgnoreReason
		{
			get { return (string)Properties[IGNOREREASON]; }
			set 
            {
                if (value == null)
                    Properties.Remove(IGNOREREASON);
                else
                    Properties[IGNOREREASON] = value;
            }
		}

		/// <summary>
		/// Gets a count of test cases represented by
		/// or contained under this test.
		/// </summary>
		public virtual int TestCaseCount 
		{ 
			get { return 1; } 
		}

		/// <summary>
		/// Gets a list of categories associated with this test.
		/// </summary>
		public IList Categories 
		{
			get 
            {
                if (Properties[CATEGORIES] == null)
                    Properties[CATEGORIES] = new ArrayList();

                return (IList)Properties[CATEGORIES]; 
            }
			set 
            {
                Properties[CATEGORIES] = value; 
            }
		}

		/// <summary>
		/// Gets a description associated with this test.
		/// </summary>
		public String Description
		{
			get { return (string)Properties[DESCRIPTION]; }
			set 
            {
                if (value == null)
                    Properties.Remove(DESCRIPTION);
                else
                    Properties[DESCRIPTION] = value; 
            }
		}

		/// <summary>
		/// Gets the property dictionary for this test
		/// </summary>
		public IDictionary Properties
		{
			get 
			{
				if ( properties == null )
					properties = new ListDictionary();

				return properties; 
			}
			set
			{
				properties = value;
			}
		}

		/// <summary>
		/// Indicates whether this test is a test case
		/// </summary>
        public abstract bool IsTestCase
        {
            get;
        }

		/// <summary>
		/// Gets the parent as a Test object.
		/// Used by the core to set the parent.
		/// </summary>
		public Test Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the Type of the fixture used in running this test
		/// </summary>
		public virtual Type FixtureType
		{
			get { return null; }
		}

		/// <summary>
		/// Gets or sets a fixture object for running this test
		/// </summary>
		public  abstract object Fixture
		{
			get; set;
        }
        #endregion

        #region Methods
        ///// <summary>
        ///// Gets a count of test cases that would be run using
        ///// the specified filter.
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <returns></returns>
        //public virtual int CountTestCases(TestFilter filter)
        //{
        //    if (filter.Pass(this))
        //        return 1;

        //    return 0;
        //}

        #endregion

        #endregion

        #region IXmlNodeBuilder Members

        public XmlNode ToXml(bool recursive)
        {
            XmlNode topNode = XmlHelper.CreateTopLevelElement("dummy");

            XmlNode thisNode = AddToXml(topNode, recursive);

            return thisNode;
        }

        public XmlNode AddToXml(XmlNode parentNode, bool recursive)
        {
            return XmlHelper.AddElement(parentNode, "test");
        }

        #endregion

        #region IComparable Members
        /// <summary>
		/// Compares this test to another test for sorting purposes
		/// </summary>
		/// <param name="obj">The other test</param>
		/// <returns>Value of -1, 0 or +1 depending on whether the current test is less than, equal to or greater than the other test</returns>
		public int CompareTo(object obj)
		{
			Test other = obj as Test;
			
			if ( other == null )
				return -1;

			return this.FullName.CompareTo( other.FullName );
		}
		#endregion

        #region Public Methods

        /// <summary>
        /// Modify a newly constructed test by applying any of NUnit's common
        /// attributes, based on an input array of attributes. This method checks
        /// for all attributes, relying on the fact that specific attributes can only
        /// occur on those constructs on which they are allowed.
        /// </summary>
        /// <param name="attributes">An array of attributes possibly including NUnit attributes</param>
        public void ApplyCommonAttributes(Attribute[] attributes)
        {
            foreach (Attribute attribute in attributes)
            {
                IApplyToTest iApply = attribute as IApplyToTest;
                if (iApply != null)
                {
                    iApply.ApplyToTest(this);
                }
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Override this to return the proper type of TestResult for this test
        /// </summary>
        /// <returns>A concrete TestResult.</returns>
        public abstract TestResult MakeTestResult();

        /// <summary>
        /// Runs the test under a particular filter, sending
        /// notifications to a listener.
        /// </summary>
        /// <param name="listener">An event listener to receive notifications</param>
        /// <returns></returns>
        public abstract TestResult Run(ITestListener listener);

        #endregion
    }
}
