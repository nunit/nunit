// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
	using System;

	/// <summary>
	/// Identifies a method to be called immediately after each test is run.
	/// The method is guaranteed to be called, even if an exception is thrown.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
	public class TearDownAttribute : NUnitAttribute
	{}
}
