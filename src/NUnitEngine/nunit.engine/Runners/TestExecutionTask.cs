// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

namespace NUnit.Engine.Runners
{
    public class TestExecutionTask : ITestExecutionTask
    {
        private readonly ITestEngineRunner _runner;
        private readonly ITestEventListener _listener;
        private readonly TestFilter _filter;
        private volatile TestEngineResult _result;
        private readonly bool _disposeRunner;

        public TestExecutionTask(ITestEngineRunner runner, ITestEventListener listener, TestFilter filter, bool disposeRunner)
        {
            _disposeRunner = disposeRunner;
            _filter = filter;
            _listener = listener;
            _runner = runner;
        }

        public void Execute()
        {
            _result = _runner.Run(_listener, _filter);
            if (_disposeRunner)
                _runner.Dispose();
        }

        public TestEngineResult Result()
        {
            return _result;
        }
    }
}
