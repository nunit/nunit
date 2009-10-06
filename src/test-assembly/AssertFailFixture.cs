// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using NUnit.Framework;

namespace NUnit.TestData
{
	[TestFixture]
	public class AssertFailFixture
	{
        [Test]
        public void CallAssertFail()
        {
            Assert.Fail();
        }

        [Test]
        public void CallAssertFailWithMessage()
        {
            Assert.Fail("MESSAGE");
        }

        [Test]
        public void CallAssertFailWithMessageAndArgs()
        {
            Assert.Fail("MESSAGE: {0}+{1}={2}", 2, 2, 4);
        }
    }
}
