using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    internal static class AssertThatHelper
    {
        public static void Start()
        {
            TestExecutionContext.CurrentContext.IncrementAssertCount();
        }

        public static void End(ConstraintResult result, string message, params object[] args)
        {
            if (result.IsSuccess) return;

            MessageWriter writer = new TextMessageWriter(message, args);
            result.WriteMessageTo(writer);

            Assert.ReportFailure(writer.ToString());
        }

#if !NET20
        public static void End(ConstraintResult result, Func<string> getExceptionMessage)
        {
            if (result.IsSuccess) return;

            End(result, getExceptionMessage.Invoke());
        }
#endif
    }
}
