// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Classic
{
    /// <summary>
    /// The Assert class contains a collection of static methods that
    /// implement the most common assertions used in NUnit.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract partial class Assert : Framework.Assert
    {
    }
}
