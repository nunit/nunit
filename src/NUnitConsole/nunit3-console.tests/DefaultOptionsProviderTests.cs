﻿// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using NUnit.Common;
using NUnit.Framework;

#if !SILVERLIGHT && !NETCF && !PORTABLE
namespace NUnit.ConsoleRunner.Tests
{
    [TestFixture]
    public sealed class DefaultOptionsProviderTests
    {
        private const string EnvironmentVariableTeamcityProjectName = "TEAMCITY_PROJECT_NAME";

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableTeamcityProjectName, null);
        }

        [Test]
        public void ShouldRetTeamCityAsTrueWhenHasEnvironmentVariable_TEAMCITY_PROJECT_NAME()
        {
            // Given
            var provider = CreateInstance();

            // When
            Environment.SetEnvironmentVariable(EnvironmentVariableTeamcityProjectName, "Abc");

            // Then
            Assert.True(provider.TeamCity);
        }

        [Test]
        public void ShouldRetTeamCityAsFalseWhenHasNotEnvironmentVariable_TEAMCITY_PROJECT_NAME()
        {
            // Given
            var provider = CreateInstance();

            // When
            Environment.SetEnvironmentVariable(EnvironmentVariableTeamcityProjectName, string.Empty);

            // Then
            Assert.False(provider.TeamCity);
        }

        private static DefaultOptionsProvider CreateInstance()
        {
            return new DefaultOptionsProvider();
        }
    }
}
#endif