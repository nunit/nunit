// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

using System.Collections;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    public partial class Assert
    {
        #region True

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void True(bool? condition, string message, params object[] args)
        {
            Assert.That(condition, Is.True ,message, args);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void True(bool condition, string message, params object[] args)
        {
           Assert.That(condition, Is.True, message, args);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void True(bool? condition)
        {
            Assert.That(condition, Is.True ,null, null);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void True(bool condition)
        {
            Assert.That(condition, Is.True ,null, null);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsTrue(bool? condition, string message, params object[] args)
        {
            Assert.That(condition, Is.True ,message, args);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsTrue(bool condition, string message, params object[] args)
        {
            Assert.That(condition, Is.True ,message, args);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsTrue(bool? condition)
        {
            Assert.That(condition, Is.True ,null, null);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsTrue(bool condition)
        {
            Assert.That(condition, Is.True ,null, null);
        }

        #endregion

        #region False

        /// <summary>
        /// Asserts that a condition is false. If the condition is true the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void False(bool? condition, string message, params object[] args)
        {
            Assert.That(condition, Is.False ,message, args);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void False(bool condition, string message, params object[] args)
        {
            Assert.That(condition, Is.False ,message, args);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        public static void False(bool? condition)
        {
            Assert.That(condition, Is.False ,null, null);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        public static void False(bool condition)
        {
            Assert.That(condition, Is.False ,null, null);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsFalse(bool? condition, string message, params object[] args)
        {
            Assert.That(condition, Is.False ,message, args);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsFalse(bool condition, string message, params object[] args)
        {
            Assert.That(condition, Is.False ,message, args);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        public static void IsFalse(bool? condition)
        {
            Assert.That(condition, Is.False ,null, null);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true the method throws
        /// an <see cref="AssertionException"/>.
        /// </summary> 
        /// <param name="condition">The evaluated condition</param>
        public static void IsFalse(bool condition)
        {
            Assert.That(condition, Is.False ,null, null);
        }

        #endregion

        #region NotNull

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <code>null</code>
        /// If the object is <code>null</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotNull(object anObject, string message, params object[] args)
        {
            Assert.That(anObject, Is.Not.Null ,message, args);
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <code>null</code>
        /// If the object is <code>null</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void NotNull(object anObject)
        {
            Assert.That(anObject, Is.Not.Null ,null, null);
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <code>null</code>
        /// If the object is <code>null</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotNull(object anObject, string message, params object[] args)
        {
            Assert.That(anObject, Is.Not.Null ,message, args);
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <code>null</code>
        /// If the object is <code>null</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void IsNotNull(object anObject)
        {
            Assert.That(anObject, Is.Not.Null ,null, null);
        }

        #endregion

        #region Null

        /// <summary>
        /// Verifies that the object that is passed in is equal to <code>null</code>
        /// If the object is not <code>null</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Null(object anObject, string message, params object[] args)
        {
            Assert.That(anObject, Is.Null ,message, args);
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <code>null</code>
        /// If the object is not <code>null</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void Null(object anObject)
        {
            Assert.That(anObject, Is.Null ,null, null);
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <code>null</code>
        /// If the object is not <code>null</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNull(object anObject, string message, params object[] args)
        {
            Assert.That(anObject, Is.Null ,message, args);
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <code>null</code>
        /// If the object is not <code>null</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void IsNull(object anObject)
        {
            Assert.That(anObject, Is.Null ,null, null);
        }

        #endregion

        #region IsNaN

        /// <summary>
        /// Verifies that the double that is passed in is an <code>NaN</code> value.
        /// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNaN(double aDouble, string message, params object[] args)
        {
            Assert.That(aDouble, Is.NaN ,message, args);
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <code>NaN</code> value.
        /// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        public static void IsNaN(double aDouble)
        {
            Assert.That(aDouble, Is.NaN ,null, null);
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <code>NaN</code> value.
        /// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNaN(double? aDouble, string message, params object[] args)
        {
            Assert.That(aDouble, Is.NaN ,message, args);
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <code>NaN</code> value.
        /// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        public static void IsNaN(double? aDouble)
        {
            Assert.That(aDouble, Is.NaN ,null, null);
        }

        #endregion

        #region IsEmpty

        #region String

        /// <summary>
        /// Assert that a string is empty - that is equal to string.Empty
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsEmpty(string aString, string message, params object[] args)
        {
            Assert.That(aString, new EmptyStringConstraint() ,message, args);
        }

        /// <summary>
        /// Assert that a string is empty - that is equal to string.Empty
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        public static void IsEmpty(string aString)
        {
            Assert.That(aString, new EmptyStringConstraint() ,null, null);
        }

        #endregion

        #region Collection

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsEmpty(IEnumerable collection, string message, params object[] args)
        {
            Assert.That(collection, new EmptyCollectionConstraint() ,message, args);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsEmpty(IEnumerable collection)
        {
            Assert.That(collection, new EmptyCollectionConstraint() ,null, null);
        }

        #endregion

        #endregion

        #region IsNotEmpty

        #region String

        /// <summary>
        /// Assert that a string is not empty - that is not equal to string.Empty
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotEmpty(string aString, string message, params object[] args)
        {
            Assert.That(aString, Is.Not.Empty ,message, args);
        }

        /// <summary>
        /// Assert that a string is not empty - that is not equal to string.Empty
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        public static void IsNotEmpty(string aString)
        {
            Assert.That(aString, Is.Not.Empty ,null, null);
        }

        #endregion

        #region Collection

        /// <summary>
        /// Assert that an array, list or other collection is not empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotEmpty(IEnumerable collection, string message, params object[] args)
        {
            Assert.That(collection, Is.Not.Empty ,message, args);
        }

        /// <summary>
        /// Assert that an array, list or other collection is not empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsNotEmpty(IEnumerable collection)
        {
            Assert.That(collection, Is.Not.Empty ,null, null);
        }

        #endregion

        #endregion
    }
}
