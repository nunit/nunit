// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Common;
using NUnit.Framework;

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
            Assert.That(provider.TeamCity);
        }

        [Test]
        public void ShouldRetTeamCityAsFalseWhenHasNotEnvironmentVariable_TEAMCITY_PROJECT_NAME()
        {
            // Given
            var provider = CreateInstance();

            // When
            Environment.SetEnvironmentVariable(EnvironmentVariableTeamcityProjectName, string.Empty);

            // Then
            Assert.That(provider.TeamCity, Is.False);
        }

        private static DefaultOptionsProvider CreateInstance()
        {
            return new DefaultOptionsProvider();
        }
    }
}
