// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Collections.Generic;
using System.IO;

namespace NUnit.ConsoleRunner.Utilities
{
    internal class TeamCityAssemblyNameProvider
    {
        private static readonly char[] Separator = { '-' };
        private readonly object lockObject = new object();
        private readonly Dictionary<string, string> _assemblyNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public void RegisterSuite(string suiteId, string fullName)
        {
            string flowId;
            string assemblyName;
            if (!TryGetFlowId(suiteId, out flowId) || !TryGetAssemblyNameFromFullname(fullName, out assemblyName))
            {
                return;
            }

            lock (lockObject)
            {
                this._assemblyNames[flowId] = assemblyName;
            }
        }

        public void UnregisterSuite(string suiteId, string fullName)
        {
            string flowId;
            string assemblyName;
            if (!TryGetFlowId(suiteId, out flowId) || !TryGetAssemblyNameFromFullname(fullName, out assemblyName))
            {
                return;
            }

            lock (lockObject)
            {
                this._assemblyNames.Remove(flowId);
            }
        }

        public bool TryGetAssemblyName(string testId, out string assemblyName)
        {
            string flowId;
            if (!TryGetFlowId(testId, out flowId))
            {
                assemblyName = default(string);
                return false;
            }

            lock (lockObject)
            {
                return this._assemblyNames.TryGetValue(flowId, out assemblyName);
            }
        }

        internal static bool TryGetFlowId(string id, out string flowId)
        {
            if (string.IsNullOrEmpty(id))
            {
                flowId = default(string);
                return false;
            }

            var idItems = id.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            if (idItems.Length != 2 || string.IsNullOrEmpty(idItems[0].Trim()))
            {
                flowId = default(string);
                return false;
            }

            flowId = idItems[0];
            return true;
        }

        internal static bool TryGetAssemblyNameFromFullname(string fullName, out string assemblyName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                assemblyName = default(string);
                return false;
            }

            var curAssemblyName = Path.GetFileName(fullName);
            var res = !StringComparer.InvariantCultureIgnoreCase.Equals(fullName, curAssemblyName);

            if (!res)
            {
                assemblyName = default(string);
                return false;
            }

            assemblyName = curAssemblyName;
            return true;
        }
    }
}
