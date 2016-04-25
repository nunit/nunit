//
// Actions.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2007, 2010 Novell, Inc (http://www.novell.com)
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
//

using System;
using System.Runtime.CompilerServices;

#if NET_4_0
[assembly:TypeForwardedTo (typeof(Action<>))]
[assembly:TypeForwardedTo (typeof(Action<,>))]
[assembly:TypeForwardedTo (typeof(Action<,,>))]
[assembly:TypeForwardedTo (typeof(Action<,,,>))]
[assembly:TypeForwardedTo (typeof (Action))]
#endif

namespace System
{
#if NET_4_0
	public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9> (
		T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10> (
		T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11> (
		T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12> (
		T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13> (
		T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14> (
		T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15> (
		T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16> (
		T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
#else

	/// <summary>
    /// Encapsulates a method that takes a no parameters and does not return a value
    /// </summary>
	public delegate void Action ();

	/// <summary>
    /// Encapsulates a method that takes a two parameters and does not return a value
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <param name="arg1">The first parameter of the method that this delegate encapsulates</param>
    /// <param name="arg2">The second parameter of the method that this delegate encapsulates</param>
    public delegate void Action<T1, T2> (T1 arg1, T2 arg2);

	/// <summary>
    /// Encapsulates a method that takes three parameters and does not return a value
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <param name="arg1">The first parameter of the method that this delegate encapsulates</param>
    /// <param name="arg2">The second parameter of the method that this delegate encapsulates</param>
    /// <param name="arg3">The third parameter of the method that this delegate encapsulates</param>
	public delegate void Action<T1, T2, T3> (T1 arg1, T2 arg2, T3 arg3);

	/// <summary>
    /// Encapsulates a method that takes four parameters and does not return a value
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.This type parameter is contravariant.That is, you can use either the type you specified or any type that is less derived</typeparam>
    /// <param name="arg1">The first parameter of the method that this delegate encapsulates</param>
    /// <param name="arg2">The second parameter of the method that this delegate encapsulates</param>
    /// <param name="arg3">The third parameter of the method that this delegate encapsulates</param>
    /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates</param>
	public delegate void Action<T1, T2, T3, T4> (T1 arg1, T2 arg2, T3 arg3, T4 arg4);
#endif	
}
