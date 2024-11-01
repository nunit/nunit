// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Common;
using NUnitLite;

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

public class TestsUnderTest
{
    public static TestResult Execute()
    {
        var type = new StackFrame(1, false).GetMethod()?.ReflectedType;
        var assembly = type?.Assembly;
        string testUnderTestClass = type?.GetNestedTypes().Single(x => x.GetCustomAttribute<TestSetupUnderTestAttribute>() is object).FullName!;
        TestLog.NewLogContext(testUnderTestClass);
        StringWriter consoleOutput = new StringWriter();
        var parameters = new List<string>
        {
            "--params", $"{TestLog.TestLogContextIdParameterName}={testUnderTestClass}",
            "--where", $"class == {testUnderTestClass} && cat == {TestSetupUnderTestAttribute.Category}"
        };

        int errorCode = new AutoRun(assembly).Execute(parameters.ToArray(),
            new ExtendedTextWrapper(consoleOutput), null);
        return new TestResult(errorCode, consoleOutput.ToString(), TestLog.GetLogsFor(testUnderTestClass));
    }
}
