// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Class that can build a tree of automatic namespace
    /// suites from a group of fixtures.
    /// </summary>
    public class NamespaceTreeBuilder
    {
        #region Instance Variables

        /// <summary>
        /// NamespaceDictionary of all test suites we have created to represent 
        /// namespaces. Used to locate namespace parent suites for fixtures.
        /// </summary>
        readonly Dictionary<string, TestSuite> _namespaceIndex  = new Dictionary<string, TestSuite>();

        /// <summary>
        /// Point in the tree where items in the global namespace are added
        /// </summary>
        private TestSuite _globalInsertionPoint;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceTreeBuilder"/> class.
        /// </summary>
        /// <param name="rootSuite">The root suite.</param>
        public NamespaceTreeBuilder( TestSuite rootSuite )
        {
            Guard.ArgumentNotNull(rootSuite, nameof(rootSuite));
            RootSuite = _globalInsertionPoint = rootSuite;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the root entry in the tree created by the NamespaceTreeBuilder.
        /// </summary>
        /// <value>The root suite.</value>
        public TestSuite RootSuite { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified fixtures to the tree.
        /// </summary>
        /// <param name="fixtures">The fixtures to be added.</param>
        public void Add( IList<Test> fixtures )
        {
            foreach (TestSuite fixture in fixtures)
                Add( fixture );
        }

        /// <summary>
        /// Adds the specified fixture to the tree.
        /// </summary>
        /// <param name="fixture">The fixture to be added.</param>
        public void Add( TestSuite fixture )
        {
            string ns = GetNamespaceForFixture(fixture);
            TestSuite containingSuite = GetNamespaceSuite( ns );

            if (fixture is SetUpFixture)
                AddSetUpFixture(fixture, containingSuite, ns);
            else
                containingSuite.Add( fixture );
        }

        #endregion

        #region Helper Method

        private static string GetNamespaceForFixture(TestSuite fixture)
        {
            string ns = fixture.FullName;
            int index = ns.IndexOfAny(new char[] { '[', '(' });
            if (index >= 0) ns = ns.Substring(0, index);
            index = ns.LastIndexOf('.');
            ns = index > 0 ? ns.Substring(0, index) : string.Empty;
            return ns;
        }

        private TestSuite GetNamespaceSuite( string ns )
        {
            Guard.ArgumentNotNull(ns, nameof(ns));

            if( ns  == "" ) return _globalInsertionPoint;

            if (_namespaceIndex.TryGetValue(ns, out TestSuite suiteToReturn))
                return suiteToReturn;

            TestSuite suite;
            int index = ns.LastIndexOf('.');

            if( index == -1 )
            {
                suite = new TestSuite( ns );
                _globalInsertionPoint.Add(suite);
            }
            else
            {
                string parentNamespace = ns.Substring( 0,index );
                TestSuite parent = GetNamespaceSuite( parentNamespace );
                string suiteName = ns.Substring( index+1 );
                suite = new TestSuite( parentNamespace, suiteName );
                parent.Add( suite );
            }

            _namespaceIndex[ns] = suite;
            return suite;
        }

        private void AddSetUpFixture(TestSuite newSetupFixture, TestSuite containingSuite, string ns)
        {
            // The SetUpFixture must replace the namespace suite
            // in which it is "contained". 
            //
            // First, add the old suite's children to the new
            // SetUpFixture and clear them from the old suite.
            foreach (TestSuite child in containingSuite.Tests)
                newSetupFixture.Add(child);

            containingSuite.Tests.Clear();

            if (containingSuite is SetUpFixture || containingSuite is TestAssembly)
            {
                // If the parent suite is another SetUpFixture or a TestAssembly,
                // it must be retained, because it may have properties set, which
                // are needed for proper execution. In both cases, the new
                // SetupFixture is nested below the parent suite.
                containingSuite.Add(newSetupFixture);
            }
            else
            {
                // Make the parent of the containing suite point to this
                // fixture instead
                // TODO: Get rid of this somehow?
                TestSuite? parent = (TestSuite?)containingSuite.Parent;
                if (parent == null)
                {
                    newSetupFixture.Name = RootSuite.Name;
                    RootSuite = newSetupFixture;
                }
                else
                {
                    parent.Tests.Remove(containingSuite);
                    parent.Add(newSetupFixture);
                }
            }

            // Update the dictionary
            _namespaceIndex[ns] = newSetupFixture;

            // Update global insertion point for global setup fixtures
            if (ns == "")
                _globalInsertionPoint = newSetupFixture;
        }

        #endregion
    }
}
