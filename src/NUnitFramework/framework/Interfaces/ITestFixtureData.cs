// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITestCaseData interface is implemented by a class
    /// that is able to return the data required to create an
    /// instance of a parameterized test fixture.
    /// </summary>
    public interface ITestFixtureData : ITestData
    {
        /// <summary>
        /// Get the TypeArgs if separately set
        /// </summary>
        Type[]? TypeArgs { get;  }
    }
}
