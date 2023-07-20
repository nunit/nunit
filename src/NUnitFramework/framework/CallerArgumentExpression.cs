// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if NET462
using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Dummy CallerArgumentExpression for Net 4.6.2 that does nothing.
    /// </summary>
    public class CallerArgumentExpression : Attribute
    {
        /// <summary>
        /// Dummy ctor to match the .net 6 CallerArgumentExpression
        /// </summary>
        /// <param name="s"></param>
        public CallerArgumentExpression(string s)
        {}
    }
}
#endif
