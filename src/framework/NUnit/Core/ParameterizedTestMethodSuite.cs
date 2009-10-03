// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************
using System.Reflection;
using System.Text;

namespace NUnit.Core
{
    /// <summary>
    /// ParameterizedMethodSuite holds a collection of individual
    /// TestMethods with their arguments applied.
    /// </summary>
    public class ParameterizedMethodSuite : TestSuite
    {
        /// <summary>
        /// Construct from a MethodInfo
        /// </summary>
        /// <param name="method"></param>
        public ParameterizedMethodSuite(MethodInfo method)
            : base(method.ReflectedType.FullName, method.Name)
        {
            this.maintainTestOrder = true;
        }

        /// <summary>
        /// Override Run, setting Fixture to that of the Parent.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override TestResult Run(ITestListener listener, TestFilter filter)
        {
            if (this.Parent != null)
            {
                this.Fixture = this.Parent.Fixture;
                TestSuite suite = this.Parent as TestSuite;
                if (suite != null)
                {
                    this.setUpMethods = suite.GetSetUpMethods();
                    this.tearDownMethods = suite.GetTearDownMethods();
                }
            }

            // DYNAMIC: Get the parameters, and add the methods here.
            
            return base.Run(listener, filter);
        }

        /// <summary>
        /// Override DoOneTimeSetUp to avoid executing any
        /// TestFixtureSetUp method for this suite
        /// </summary>
        /// <param name="suiteResult"></param>
        protected override void DoOneTimeSetUp(TestResult suiteResult)
        {
        }

        /// <summary>
        /// Override DoOneTimeTearDown to avoid executing any
        /// TestFixtureTearDown method for this suite.
        /// </summary>
        /// <param name="suiteResult"></param>
        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
        }
    }
}
