// ***********************************************************************
// Copyright (c) 2012-2015 Charlie Poole
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The Test abstract class represents a test within the framework.
    /// </summary>
    public abstract class Test : ITest, IComparable
    {
        #region Fields

        /// <summary>
        /// Static value to seed ids. It's started at 1000 so any
        /// uninitialized ids will stand out.
        /// </summary>
        private static int _nextID = 1000;

        /// <summary>
        /// Used to cache the declaring type for this MethodInfo
        /// </summary>
        protected ITypeInfo DeclaringTypeInfo;

        /// <summary>
        /// Method property backing field
        /// </summary>
        private IMethodInfo _method;

        #endregion

        #region Construction

        /// <summary>
        /// Constructs a test given its name
        /// </summary>
        /// <param name="name">The name of the test</param>
        protected Test( string name )
        {
            Guard.ArgumentNotNullOrEmpty(name, "name");

            Initialize(name);
        }

        /// <summary>
        /// Constructs a test given the path through the
        /// test hierarchy to its parent and a name.
        /// </summary>
        /// <param name="pathName">The parent tests full name</param>
        /// <param name="name">The name of the test</param>
        protected Test( string pathName, string name ) 
        {
            Initialize(name);

            if (!string.IsNullOrEmpty(pathName))
                FullName = pathName + "." + name;
        }

        /// <summary>
        ///  TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="typeInfo"></param>
        protected Test(ITypeInfo typeInfo)
        {
            Initialize(typeInfo.GetDisplayName());

            string nspace = typeInfo.Namespace;
            if (nspace != null && nspace != "")
                FullName = nspace + "." + Name;
            TypeInfo = typeInfo;
        }

        /// <summary>
        /// Construct a test from a MethodInfo
        /// </summary>
        /// <param name="method"></param>
        protected Test(IMethodInfo method)
        {
            Initialize(method.Name);

            Method = method;
            TypeInfo = method.TypeInfo;
            FullName = method.TypeInfo.FullName + "." + Name;
        }

        private void Initialize(string name)
        {
            FullName = Name = name;
            Id = GetNextId();
            Properties = new PropertyBag();
            RunState = RunState.Runnable;
            SetUpMethods = new MethodInfo[0];
            TearDownMethods = new MethodInfo[0];
        }

        private static string GetNextId()
        {            
            return IdPrefix + unchecked(_nextID++);
        }

        #endregion

        #region ITest Members

        /// <summary>
        /// Gets or sets the id of the test
        /// </summary>
        /// <value></value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the test
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified name of the test
        /// </summary>
        /// <value></value>
        public string FullName { get; set; }

        /// <summary>
        /// Gets the name of the class where this test was declared.
        /// Returns null if the test is not associated with a class.
        /// </summary>
        public string ClassName
        {
            get
            {
                ITypeInfo typeInfo = TypeInfo;

                if (Method != null)
                {
                    if (DeclaringTypeInfo == null)
                        DeclaringTypeInfo = new TypeWrapper(Method.MethodInfo.DeclaringType);

                    typeInfo = DeclaringTypeInfo;
                }

                if (typeInfo == null)
                    return null;

                return typeInfo.IsGenericType
                    ? typeInfo.GetGenericTypeDefinition().FullName
                    : typeInfo.FullName;
            }
        }

        /// <summary>
        /// Gets the name of the method implementing this test.
        /// Returns null if the test is not implemented as a method.
        /// </summary>
        public virtual string MethodName
        {
            get { return null; }
        }

        /// <summary>
        /// The arguments to use in creating the test or empty array if none required.
        /// </summary>
        public abstract object[] Arguments { get; }

        /// <summary>
        /// Gets the TypeInfo of the fixture used in running this test
        /// or null if no fixture type is associated with it.
        /// </summary>
        public ITypeInfo TypeInfo { get; private set; }

        /// <summary>
        /// Gets a MethodInfo for the method implementing this test.
        /// Returns null if the test is not implemented as a method.
        /// </summary>
        public IMethodInfo Method
        {
            get { return _method; }
            set
            {
                DeclaringTypeInfo = null;
                _method = value;
            }
        } // public setter needed by NUnitTestCaseBuilder

        /// <summary>
        /// Whether or not the test should be run
        /// </summary>
        public RunState RunState { get; set; }

        /// <summary>
        /// Gets the name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public abstract string XmlElementName { get; }

        /// <summary>
        /// Gets a string representing the type of test. Used as an attribute
        /// value in the XML representation of a test and has no other
        /// function in the framework.
        /// </summary>
        public virtual string TestType
        {
            get { return this.GetType().Name; }
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
        /// Gets the properties for this test
        /// </summary>
        public IPropertyBag Properties { get; private set; }

        /// <summary>
        /// Returns true if this is a TestSuite
        /// </summary>
        public bool IsSuite
        {
            get { return this is TestSuite; }
        }

        /// <summary>
        /// Gets a bool indicating whether the current test
        /// has any descendant tests.
        /// </summary>
        public abstract bool HasChildren { get; }

        /// <summary>
        /// Gets the parent as a Test object.
        /// Used by the core to set the parent.
        /// </summary>
        public ITest Parent { get; set; }

        /// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value>A list of child tests</value>
        public abstract IList<ITest> Tests { get; }

        /// <summary>
        /// Gets or sets a fixture object for running this test.
        /// </summary>
        public virtual object Fixture { get; set; }

        #endregion

        #region Other Public Properties

        /// <summary>
        /// Static prefix used for ids in this AppDomain.
        /// Set by FrameworkController.
        /// </summary>
        public static string IdPrefix { get; set; }

        /// <summary>
        /// Gets or Sets the Int value representing the seed for the RandomGenerator
        /// </summary>
        /// <value></value>
        public int Seed { get; set; }

        /// <summary>
        /// The SetUp methods.
        /// </summary>
        public MethodInfo[] SetUpMethods { get; protected set; }

        /// <summary>
        /// The teardown methods
        /// </summary>
        public MethodInfo[] TearDownMethods { get; protected set; }

        #endregion 

        #region Internal Properties

        internal bool RequiresThread { get; set; }

        private ITestAction[] _actions;

        internal ITestAction[] Actions
        {
            get
            {
                if (_actions == null)
                {
                    // For fixtures, we use special rules to get actions
                    // Otherwise we just get the attributes
                    _actions = Method == null && TypeInfo != null
                        ? GetActionsForType(TypeInfo.Type)
                        : GetCustomAttributes<ITestAction>(false);
                }

                return _actions;
            }
        }
        
        #endregion

        #region Other Public Methods

        /// <summary>
        /// Creates a TestResult for this test.
        /// </summary>
        /// <returns>A TestResult suitable for this type of test.</returns>
        public abstract TestResult MakeTestResult();

#if PORTABLE || NETSTANDARD1_6
        /// <summary>
        /// Modify a newly constructed test by applying any of NUnit's common
        /// attributes, based on a supplied ICustomAttributeProvider, which is
        /// usually the reflection element from which the test was constructed,
        /// but may not be in some instances. The attributes retrieved are 
        /// saved for use in subsequent operations.
        /// </summary>
        /// <param name="provider">An object deriving from MemberInfo</param>
        public void ApplyAttributesToTest(MemberInfo provider)
        {
            foreach (IApplyToTest iApply in provider.GetAttributes<IApplyToTest>(true))
                iApply.ApplyToTest(this);
        }

        /// <summary>
        /// Modify a newly constructed test by applying any of NUnit's common
        /// attributes, based on a supplied ICustomAttributeProvider, which is
        /// usually the reflection element from which the test was constructed,
        /// but may not be in some instances. The attributes retrieved are 
        /// saved for use in subsequent operations.
        /// </summary>
        /// <param name="provider">An object deriving from MemberInfo</param>
        public void ApplyAttributesToTest(Assembly provider)
        {
            foreach (IApplyToTest iApply in provider.GetAttributes<IApplyToTest>())
                iApply.ApplyToTest(this);
        }
#else
        /// <summary>
        /// Modify a newly constructed test by applying any of NUnit's common
        /// attributes, based on a supplied ICustomAttributeProvider, which is
        /// usually the reflection element from which the test was constructed,
        /// but may not be in some instances. The attributes retrieved are 
        /// saved for use in subsequent operations.
        /// </summary>
        /// <param name="provider">An object implementing ICustomAttributeProvider</param>
        public void ApplyAttributesToTest(ICustomAttributeProvider provider)
        {
            foreach (IApplyToTest iApply in provider.GetCustomAttributes(typeof(IApplyToTest), true))
                iApply.ApplyToTest(this);
        }
#endif

        /// <summary>
        /// Mark the test as Invalid (not runnable) specifying a reason
        /// </summary>
        /// <param name="reason">The reason the test is not runnable</param>
        public void MakeInvalid(string reason)
        {
            Guard.ArgumentNotNullOrEmpty(reason, "reason");

            RunState = RunState.NotRunnable;
            Properties.Add(PropertyNames.SkipReason, reason);
        }

        /// <summary>
        /// Get custom attributes applied to a test
        /// </summary>
        public virtual TAttr[] GetCustomAttributes<TAttr>(bool inherit) where TAttr : class
        {
            if (Method != null)
#if PORTABLE || NETSTANDARD1_6
                return Method.GetCustomAttributes<TAttr>(inherit).ToArray();
#else
                return (TAttr[])Method.MethodInfo.GetCustomAttributes(typeof(TAttr), inherit);
#endif

            if (TypeInfo != null)
                return TypeInfo.GetCustomAttributes<TAttr>(inherit).ToArray();

            return new TAttr[0];
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Add standard attributes and members to a test node.
        /// </summary>
        /// <param name="thisNode"></param>
        /// <param name="recursive"></param>
        protected void PopulateTestNode(TNode thisNode, bool recursive)
        {
            thisNode.AddAttribute("id", this.Id.ToString());
            thisNode.AddAttribute("name", this.Name);
            thisNode.AddAttribute("fullname", this.FullName);
            if (this.MethodName != null)
                thisNode.AddAttribute("methodname", this.MethodName);
            if (this.ClassName != null)
                thisNode.AddAttribute("classname", this.ClassName);
            thisNode.AddAttribute("runstate", this.RunState.ToString());

            if (Properties.Keys.Count > 0)
                Properties.AddToXml(thisNode, recursive);
        }

        #endregion

        #region Private Methods

        private static ITestAction[] GetActionsForType(Type type)
        {
            var actions = new List<ITestAction>();

            if (type != null && type != typeof(object))
            {
                actions.AddRange(GetActionsForType(type.GetTypeInfo().BaseType));

#if PORTABLE || NETSTANDARD1_6
                foreach (Type interfaceType in TypeHelper.GetDeclaredInterfaces(type))
                    actions.AddRange(interfaceType.GetTypeInfo().GetAttributes<ITestAction>(false).ToArray());

                actions.AddRange(type.GetTypeInfo().GetAttributes<ITestAction>(false).ToArray());
#else
                foreach (Type interfaceType in TypeHelper.GetDeclaredInterfaces(type))
                    actions.AddRange((ITestAction[])interfaceType.GetTypeInfo().GetCustomAttributes(typeof(ITestAction), false));

                actions.AddRange((ITestAction[])type.GetTypeInfo().GetCustomAttributes(typeof(ITestAction), false));
#endif
            }

            return actions.ToArray();
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns the Xml representation of the test
        /// </summary>
        /// <param name="recursive">If true, include child tests recursively</param>
        /// <returns></returns>
        public TNode ToXml(bool recursive)
        {
            return AddToXml(new TNode("dummy"), recursive);
        }

        /// <summary>
        /// Returns an XmlNode representing the current result after
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public abstract TNode AddToXml(TNode parentNode, bool recursive);

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

            if (other == null)
                return -1;

            return this.FullName.CompareTo(other.FullName);
        }

        #endregion
    }
}
