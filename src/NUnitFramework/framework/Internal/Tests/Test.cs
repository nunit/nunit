// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The Test abstract class represents a test within the framework.
    /// </summary>
    public abstract class Test : ITest, IComparable, IComparable<Test>
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
        private ITypeInfo? _declaringTypeInfo;

        /// <summary>
        /// Method property backing field
        /// </summary>
        private IMethodInfo? _method;

        #endregion

        #region Construction

        /// <summary>
        /// Constructs a test given its name
        /// </summary>
        /// <param name="name">The name of the test</param>
        protected Test(string name)
            : this(pathName: null, name, typeInfo: null, method: null)
        {
        }

        /// <summary>
        /// Constructs a test given the path through the
        /// test hierarchy to its parent and a name.
        /// </summary>
        /// <param name="pathName">The parent tests full name</param>
        /// <param name="name">The name of the test</param>
        protected Test(string? pathName, string name)
            : this(pathName, name, typeInfo: null, method: null)
        {
        }

        /// <summary>
        /// Constructs a test for a specific type.
        /// </summary>
        protected Test(ITypeInfo typeInfo)
            : this(pathName: typeInfo.Namespace, name: typeInfo.GetDisplayName(), typeInfo: typeInfo, method: null)
        {
        }

        /// <summary>
        /// Constructs a test for a specific method.
        /// </summary>
        protected Test(IMethodInfo method)
            : this(pathName: method.TypeInfo.FullName, name: method.Name, typeInfo: method.TypeInfo, method: method)
        {
        }

        private Test(string? pathName, string name, ITypeInfo? typeInfo, IMethodInfo? method)
        {
            Guard.ArgumentNotNullOrEmpty(name, nameof(name));

            Id = GetNextId();
            Name = name;
            FullName = !string.IsNullOrEmpty(pathName)
                ? pathName + '.' + name
                : name;

            TypeInfo = typeInfo;
            Method = method;
            Properties = new PropertyBag();
            RunState = RunState.Runnable;
            SetUpMethods = Array.Empty<IMethodInfo>();
            TearDownMethods = Array.Empty<IMethodInfo>();
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
        public string? ClassName
        {
            get
            {
                ITypeInfo? typeInfo = TypeInfo;

                if (Method is not null)
                {
                    if (_declaringTypeInfo is null)
                        _declaringTypeInfo = new TypeWrapper(Method.MethodInfo.DeclaringType!);

                    typeInfo = _declaringTypeInfo;
                }

                if (typeInfo is null)
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
        public virtual string? MethodName => null;

        /// <summary>
        /// The arguments to use in creating the test or empty array if none required.
        /// </summary>
        public abstract object?[] Arguments { get; }

        /// <summary>
        /// Gets the TypeInfo of the fixture used in running this test
        /// or null if no fixture type is associated with it.
        /// </summary>
        public ITypeInfo? TypeInfo { get; }

        /// <summary>
        /// Gets a MethodInfo for the method implementing this test.
        /// Returns null if the test is not implemented as a method.
        /// </summary>
        public IMethodInfo? Method
        {
            get => _method;
            set
            {
                _declaringTypeInfo = null;
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
        public virtual string TestType => GetType().Name;

        /// <summary>
        /// Gets a count of test cases represented by
        /// or contained under this test.
        /// </summary>
        public virtual int TestCaseCount => 1;

        /// <summary>
        /// Gets the properties for this test
        /// </summary>
        public IPropertyBag Properties { get; }

        /// <summary>
        /// Returns true if this is a TestSuite
        /// </summary>
        public bool IsSuite => this is TestSuite;

        /// <summary>
        /// Gets a bool indicating whether the current test
        /// has any descendant tests.
        /// </summary>
        public abstract bool HasChildren { get; }

        /// <summary>
        /// Gets the parent as a Test object.
        /// Used by the core to set the parent.
        /// </summary>
        public ITest? Parent { get; set; }

        /// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value>A list of child tests</value>
        public abstract IList<ITest> Tests { get; }

        /// <summary>
        /// Gets or sets a fixture object for running this test.
        /// </summary>
        public virtual object? Fixture { get; set; }

        #endregion

        #region Other Public Properties

        /// <summary>
        /// Static prefix used for ids in this AppDomain.
        /// Set by FrameworkController.
        /// </summary>
        public static string? IdPrefix { get; set; }

        /// <summary>
        /// Gets or Sets the Int value representing the seed for the RandomGenerator
        /// </summary>
        /// <value></value>
        public int Seed { get; set; }

        /// <summary>
        /// The SetUp methods.
        /// </summary>
        public IMethodInfo[] SetUpMethods { get; protected set; }

        /// <summary>
        /// The teardown methods
        /// </summary>
        public IMethodInfo[] TearDownMethods { get; protected set; }

        #endregion

        #region Internal Properties

        internal bool RequiresThread { get; set; }

        private ITestAction[]? _actions;

        internal ITestAction[] Actions
        {
            get
            {
                if (_actions is null)
                {
                    // For fixtures, we use special rules to get actions
                    // Otherwise we just get the attributes
                    if (Method is null && TypeInfo is not null)
                    {
                        _actions = TestMetadataCache.Get(TypeInfo.Type).TestActionAttributes;
                    }
                    else if (Method is not null)
                    {
                        _actions = MethodInfoCache.Get(Method).TestActionAttributes;
                    }
                    else
                    {
                        _actions = GetCustomAttributes<ITestAction>(false);
                    }
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

        /// <summary>
        /// Modify a newly constructed test by applying any of NUnit's common
        /// attributes, based on a supplied <see cref="ICustomAttributeProvider"/>, which is
        /// usually the reflection element from which the test was constructed,
        /// but may not be in some instances. The attributes retrieved are
        /// saved for use in subsequent operations.
        /// </summary>
        public void ApplyAttributesToTest(ICustomAttributeProvider provider)
        {
            ApplyAttributesToTest(OSPlatformTranslator.RetrieveAndTranslate(provider));
        }

        /// <summary>
        /// Recursively apply the attributes on <paramref name="type"/> to this test,
        /// including attributes on nesting types.
        /// </summary>
        /// <param name="type">The </param>
        public void ApplyAttributesToTest(Type type)
        {
            foreach (var t in GetNestedTypes(type).Reverse())
                ApplyAttributesToTest((ICustomAttributeProvider)t);
        }

        private void ApplyAttributesToTest(IEnumerable<IApplyToTest> attributes)
        {
            foreach (IApplyToTest iApply in attributes)
                iApply.ApplyToTest(this);
        }

        /// <summary>
        /// Mark the test as Invalid (not runnable) specifying a reason
        /// </summary>
        /// <param name="reason">The reason the test is not runnable</param>
        public void MakeInvalid(string reason)
        {
            Guard.ArgumentNotNullOrEmpty(reason, nameof(reason));

            RunState = RunState.NotRunnable;
            Properties.Add(PropertyNames.SkipReason, reason);
        }

        /// <summary>
        /// Mark the test as Invalid (not runnable) specifying a reason and an exception.
        /// </summary>
        /// <param name="exception">The exception that was the cause.</param>
        /// <param name="reason">The reason the test is not runnable</param>
        public void MakeInvalid(Exception exception, string reason)
        {
            Guard.ArgumentNotNull(exception, nameof(exception));
            Guard.ArgumentNotNullOrEmpty(reason, nameof(reason));

            MakeInvalid(reason + Environment.NewLine + ExceptionHelper.BuildMessage(exception));
            Properties.Add(PropertyNames.ProviderStackTrace, ExceptionHelper.BuildStackTrace(exception));
        }

        /// <summary>
        /// Get custom attributes applied to a test
        /// </summary>
        public virtual TAttr[] GetCustomAttributes<TAttr>(bool inherit)
            where TAttr : class
        {
            if (Method is not null)
                return Method.GetCustomAttributes<TAttr>(inherit);

            if (TypeInfo is not null)
                return TypeInfo.GetCustomAttributes<TAttr>(inherit);

            return Array.Empty<TAttr>();
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
            thisNode.AddAttribute("id", Id);
            thisNode.AddAttribute("name", Name);
            thisNode.AddAttribute("fullname", FullName);
            if (MethodName is not null)
                thisNode.AddAttribute("methodname", MethodName);
            if (ClassName is not null)
                thisNode.AddAttribute("classname", ClassName);
            thisNode.AddAttribute("runstate", RunState.ToString());

            if (Properties.Keys.Count > 0)
                Properties.AddToXml(thisNode, recursive);
        }

        /// <summary>
        /// Returns all nested types, inner first.
        /// </summary>
        protected IEnumerable<Type> GetNestedTypes(Type inner)
        {
            var current = inner;
            while (current is not null)
            {
                yield return current;
                current = current.DeclaringType;
            }
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns the XML representation of the test
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

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="obj">An object to compare with this instance. </param>
        public int CompareTo(object? obj)
        {
            return CompareTo(obj as Test);
        }

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object. </summary>
        /// <param name="other">An object to compare with this instance.</param>
        public int CompareTo(Test? other)
        {
            return other is null ? -1 : FullName.CompareTo(other.FullName);
        }

        #endregion
    }
}
