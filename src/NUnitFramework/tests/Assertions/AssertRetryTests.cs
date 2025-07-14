// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using System;

namespace Ullink.desk.uitests.test;

public class RetryAssertTests
{
    [SetUp]
    public void SetUp()
    {
        Assert.RetryDefaultInterval = TimeSpan.FromMilliseconds(10);
    }

    [Test]
    public void GivenATrueConditionAfterRetry_Passes()
    {
        int count = 0;
        Assert.Retry(() => count++, Is.EqualTo(2));
    }

    [Test]
    public void GivenAConditionAndMessage_Passes()
    {
        int count = 0;
        Assert.Retry(() => count++, Is.EqualTo(2), "message");
    }

    [Test]
    public void GivenAConditionAndAndTimeoutAndMessage_Passes()
    {
        int count = 0;
        Assert.Retry(() => count++, Is.EqualTo(2), TimeSpan.FromSeconds(2), "message");
    }

    [Test]
    public void GivenAConditionAndAndTimeoutAndIntervalAndMessage_Passes()
    {
        int count = 0;
        Assert.Retry(() => count++, Is.EqualTo(2), TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(10), "message");
    }

    [Test]
    public void GivenAFalseCondition_Fails()
    {
        var originalTimeout = Assert.RetryDefaultTimeout;
        try
        {
            Assert.RetryDefaultTimeout = TimeSpan.FromSeconds(1);
            var ex = Assert.Throws<AssertionException>(() => Assert.Retry(() => 40 + 1, Is.EqualTo(2)));

            Assert.That(ex.Message, Does.Contain("Assert.Retry(() => 40 + 1, Is.EqualTo(2))"));
        }
        finally
        {
            Assert.RetryDefaultTimeout = originalTimeout;
        }
    }
}
