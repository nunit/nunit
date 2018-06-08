using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Inverts the flow of control so that multiple asserts can be driven by code with varying execution models.
    /// </summary>
    internal struct AssertMultipleHelper
    {
        private readonly TestExecutionContext _context;

        private AssertMultipleHelper(TestExecutionContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Each call to <see cref="Start"/> must have a balancing call to <see cref="Finally"/>.
        /// </summary>
        public static AssertMultipleHelper Start()
        {
            var helper = new AssertMultipleHelper(TestExecutionContext.CurrentContext);
            Guard.OperationValid(helper._context != null, "Assert.Multiple called outside of a valid TestExecutionContext");

            helper._context.MultipleAssertLevel++;

            return helper;
        }

        /// <summary>
        /// Each call to <see cref="Start"/> must have a balancing call to <see cref="Finally"/>.
        /// </summary>
        public void Finally()
        {
            _context.MultipleAssertLevel--;
        }

        /// <summary>
        /// To be called after <see cref="Finally"/>, but only if there was no non-assertion exception before completing the multiple assert block.
        /// </summary>
        public void OnSuccess()
        {
            if (_context.MultipleAssertLevel == 0 && _context.CurrentResult.PendingFailures > 0)
            {
                _context.CurrentResult.RecordTestCompletion();
                throw new MultipleAssertException(_context.CurrentResult);
            }
        }
    }
}
