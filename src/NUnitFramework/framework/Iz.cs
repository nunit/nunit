// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// The Iz class is a synonym for Is intended for use in VB,
    /// which regards Is as a keyword.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract class Iz : Is
    {
    }
}
