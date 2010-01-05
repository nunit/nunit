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

namespace NUnit.Framework.Api
{
	/// <summary>
	/// TestInfo holds common info about a test. It represents only
	/// a single test or a suite and contains no references to other
	/// tests. Since it is informational only, it can easily be passed
	/// around using .Net remoting.
	/// 
	/// TestInfo is used directly in all EventListener events and in
	/// TestResults. It contains an ID, which can be used by a 
	/// runner to locate the actual test.
	/// 
	/// TestInfo also serves as the base class for TestNode, which
	/// adds hierarchical information and is used in client code to
	/// maintain a visible image of the structure of the tests.
	/// </summary>
	[Serializable]
	public class TestInfo : ITest
	{
		#region Instance Variables

        private string name;
        private string fullName;
        private int id;

		private string testType;

        private RunState runState;

		/// <summary>
		/// Reason for not running the test
		/// </summary>
		private string ignoreReason;

		/// <summary>
		/// Number of test cases in this test or suite
		/// </summary>
		private int testCaseCount;

		/// <summary>
		/// True if this is a suite
		/// </summary>
		private bool isSuite;

		/// <summary>
		/// The test description
		/// </summary>
		private string description;

		/// <summary>
		/// A list of all the categories assigned to a test
		/// </summary>
		private ArrayList categories = new ArrayList();

		/// <summary>
		/// A dictionary of properties, used to add information
		/// to tests without requiring the class to change.
		/// </summary>
		private ListDictionary properties = new ListDictionary();

		#endregion

		#region Constructors
		/// <summary>
		/// Construct from an ITest
		/// </summary>
		/// <param name="test">Test from which a TestNode is to be constructed</param>
		public TestInfo( Test test )
		{
            this.ID = test.ID;
            this.Name = test.Name;
            this.FullName = test.FullName;
			this.testType = test.TestType;

            this.runState = test.RunState;
			this.ignoreReason = test.IgnoreReason;
			this.description = test.Description;
			this.isSuite = test.IsSuite;

			if (test.Categories != null) 
				this.categories.AddRange(test.Categories);
			if (test.Properties != null)
			{
				this.properties = new ListDictionary();
				foreach( DictionaryEntry entry in test.Properties )
					this.properties.Add( entry.Key, entry.Value );
			}

			this.testCaseCount = test.TestCount;
		}

		#endregion

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

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        /// <summary>
		/// Gets a string representing the kind of test this
		/// object represents for display purposes.
		/// </summary>
		public string TestType
		{
			get { return testType; }
		}

		/// <summary>
		/// The test description 
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// Gets the RunState for this test
		/// </summary>
        public RunState RunState
        {
            get { return runState; }
            set { runState = value; }
        }

		/// <summary>
		/// The reason for ignoring a test
		/// </summary>
		public string IgnoreReason
		{
			get { return ignoreReason; }
			set { ignoreReason = value; }
		}

		/// <summary>
		/// Count of test cases in this test.
		/// </summary>
		public int TestCount
		{ 
			get { return testCaseCount; } 
		}

		/// <summary>
		///  Gets the parent test of this test
		/// </summary>
		public virtual ITest Parent
		{
			get { return null; }
		}

		/// <summary>
		/// Gets a list of the categories applied to this test
		/// </summary>
		public IList Categories 
		{
			get { return categories; }
		}

		/// <summary>
		/// Gets a list of any child tests
		/// </summary>
		public virtual IList Tests
		{
			get { return null; }
		}

		/// <summary>
		/// True if this is a suite, false if a test case
		/// </summary>
		public bool IsSuite
		{
			get { return isSuite; }
		}

		/// <summary>
		/// Gets the Properties dictionary for this test
		/// </summary>
		public IDictionary Properties
		{
			get 
			{
				if ( properties == null )
					properties = new ListDictionary();

				return properties; 
			}
		}
		#endregion

        #region Methods
		/// <summary>
		/// Counts the test cases that would be run if this
		/// test were executed using the provided filter.
		/// </summary>
		/// <param name="filter">The filter to apply</param>
		/// <returns>A count of test cases</returns>
        public virtual int CountTestCases(TestFilter filter)
        {
            if (filter.IsEmpty)
                return TestCount;

            if (!isSuite)
                return filter.Pass(this) ? 1 : 0;

            int count = 0;
            if (filter.Pass(this))
            {
                foreach (ITest test in Tests)
                {
                    count += test.CountTestCases(filter);
                }
            }
            return count;
        }
        #endregion
    }
}
