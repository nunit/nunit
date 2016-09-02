// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
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
using System.Collections;
using System.ComponentModel;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// A set of Assert methods operating on one or more collections
    /// </summary>
    public class CollectionAssert
    {
        #region Equals and ReferenceEquals

        /// <summary>
        /// The Equals method throws an InvalidOperationException. This is done 
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a">The Left object in the comparison</param>
        /// <param name="b">The Right object in the comparison</param>
        /// <returns>Not applicable</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            throw new InvalidOperationException("CollectionAssert.Equals should not be used for Assertions");
        }

        /// <summary>
        /// override the default ReferenceEquals to throw an InvalidOperationException. This 
        /// implementation makes sure there is no mistake in calling this function 
        /// as part of Assert. 
        /// </summary>
        /// <param name="a">The Left object in the comparison</param>
        /// <param name="b">The Right object in the comparison</param>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("CollectionAssert.ReferenceEquals should not be used for Assertions");
        }

        #endregion
                
        #region AllItemsAreInstancesOfType

        /// <summary>
        /// Asserts that all items contained in collection are of the type specified by expectedType.
        /// </summary>
        /// <param name="collection">IEnumerable containing objects to be considered</param>
        /// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
        public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType)
        {
            AllItemsAreInstancesOfType(collection, expectedType, string.Empty, null);
        }

        /// <summary>
        /// Asserts that all items contained in collection are of the type specified by expectedType.
        /// </summary>
        /// <param name="collection">IEnumerable containing objects to be considered</param>
        /// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType, string message, params object[] args)
        {
            Assert.That(collection, Is.All.InstanceOf(expectedType), message, args);
        }
        
        /// <summary>
        /// Asserts that all items contained in collection are of the type specified by expectedType.
        /// </summary>
        /// <param name="collection">IEnumerable containing objects to be considered</param>
        /// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType, Func<string> getExceptionMessage)
        {
            Assert.That(collection, Is.All.InstanceOf(expectedType), getExceptionMessage);
        }
        
        /// <summary>
        /// Asserts that all items contained in collection are of the type specified by expectedType.
        /// </summary>
        /// <param name="collection">IEnumerable containing objects to be considered</param>
        /// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, Is.All.InstanceOf(expectedType), getExceptionMessage);
        }
        
        #endregion

        #region AllItemsAreNotNull

        /// <summary>
        /// Asserts that all items contained in collection are not equal to null.
        /// </summary>
        /// <param name="collection">IEnumerable containing objects to be considered</param>
        public static void AllItemsAreNotNull(IEnumerable collection)
        {
            AllItemsAreNotNull(collection, string.Empty, null);
        }

        /// <summary>
        /// Asserts that all items contained in collection are not equal to null.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AllItemsAreNotNull(IEnumerable collection, string message, params object[] args)
        {
            Assert.That(collection, Is.All.Not.Null, message, args);
        }
        
        /// <summary>
        /// Asserts that all items contained in collection are not equal to null.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AllItemsAreNotNull(IEnumerable collection, Func<string> getExceptionMessage)
        {
            Assert.That(collection, Is.All.Not.Null, getExceptionMessage);
        }
        
        /// <summary>
        /// Asserts that all items contained in collection are not equal to null.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AllItemsAreNotNull(IEnumerable collection, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, Is.All.Not.Null, getExceptionMessage);
        }
        
        #endregion

        #region AllItemsAreUnique

        /// <summary>
        /// Ensures that every object contained in collection exists within the collection
        /// once and only once.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        public static void AllItemsAreUnique(IEnumerable collection)
        {
            AllItemsAreUnique(collection, string.Empty, null);
        }
        
        /// <summary>
        /// Ensures that every object contained in collection exists within the collection
        /// once and only once.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AllItemsAreUnique(IEnumerable collection, string message, params object[] args)
        {
            Assert.That(collection, Is.Unique, message, args);
        }
        
        /// <summary>
        /// Ensures that every object contained in collection exists within the collection
        /// once and only once.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AllItemsAreUnique(IEnumerable collection, Func<string> getExceptionMessage)
        {
            Assert.That(collection, Is.Unique, getExceptionMessage);
        }
        
        /// <summary>
        /// Ensures that every object contained in collection exists within the collection
        /// once and only once.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AllItemsAreUnique(IEnumerable collection, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, Is.Unique, getExceptionMessage);
        }
        
        #endregion

        #region AreEqual

        /// <summary>
        /// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
        /// and contain the exact same objects in the same order.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        public static void AreEqual(IEnumerable expected, IEnumerable actual)
        {
            AreEqual(expected, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
        /// and contain the exact same objects in the same order.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each IEnumerable</param>
        public static void AreEqual(IEnumerable expected, IEnumerable actual, IComparer comparer)
        {
            AreEqual(expected, actual, comparer, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
        /// and contain the exact same objects in the same order.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreEqual(IEnumerable expected, IEnumerable actual, string message, params object[] args)
        {
            Assert.That(actual, Is.EqualTo(expected), message, args);
        }

        /// <summary>
        /// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
        /// and contain the exact same objects in the same order.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreEqual(IEnumerable expected, IEnumerable actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Is.EqualTo(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
        /// and contain the exact same objects in the same order.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreEqual(IEnumerable expected, IEnumerable actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Is.EqualTo(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
        /// and contain the exact same objects in the same order.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each IEnumerable</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreEqual(
            IEnumerable expected,
            IEnumerable actual,
            IComparer comparer,
            string message,
            params object[] args)
        {
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message, args);
        }

        /// <summary>
        /// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
        /// and contain the exact same objects in the same order.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreEqual(
            IEnumerable expected,
            IEnumerable actual,
            IComparer comparer,
            Func<string> getExceptionMessage)
        {
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
        /// and contain the exact same objects in the same order.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreEqual(
            IEnumerable expected,
            IEnumerable actual,
            IComparer comparer,
            Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), getExceptionMessage);
        }
        
        #endregion

        #region AreEquivalent

        /// <summary>
        /// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        public static void AreEquivalent(IEnumerable expected, IEnumerable actual)
        {
            AreEquivalent(expected, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreEquivalent(IEnumerable expected, IEnumerable actual, string message, params object[] args)
        {
            Assert.That(actual, Is.EquivalentTo(expected), message, args);
        }

        /// <summary>
        /// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreEquivalent(IEnumerable expected, IEnumerable actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Is.EquivalentTo(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreEquivalent(IEnumerable expected, IEnumerable actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Is.EquivalentTo(expected), getExceptionMessage);
        }

        #endregion

        #region AreNotEqual

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        public static void AreNotEqual(IEnumerable expected, IEnumerable actual)
        {
            AreNotEqual(expected, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each IEnumerable</param>
        public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer)
        {
            AreNotEqual(expected, actual, comparer, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEqual(IEnumerable expected, IEnumerable actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.EqualTo(expected), message, args);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreNotEqual(IEnumerable expected, IEnumerable actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Is.Not.EqualTo(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreNotEqual(IEnumerable expected, IEnumerable actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Is.Not.EqualTo(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each IEnumerable</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.EqualTo(expected).Using(comparer), message, args);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Is.Not.EqualTo(expected).Using(comparer), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Is.Not.EqualTo(expected).Using(comparer), getExceptionMessage);
        }

        #endregion

        #region AreNotEquivalent

        /// <summary>
        /// Asserts that expected and actual are not equivalent.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual)
        {
            AreNotEquivalent(expected, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not equivalent.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.EquivalentTo(expected), message, args);
        }

        /// <summary>
        /// Asserts that expected and actual are not equivalent.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Is.Not.EquivalentTo(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that expected and actual are not equivalent.
        /// </summary>
        /// <param name="expected">The first IEnumerable of objects to be considered</param>
        /// <param name="actual">The second IEnumerable of objects to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Is.Not.EquivalentTo(expected), getExceptionMessage);
        }

        #endregion

        #region Contains

        /// <summary>
        /// Asserts that collection contains actual as an item.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="actual">Object to be found within collection</param>
        public static void Contains(IEnumerable collection, object actual)
        {
            Contains(collection, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that collection contains actual as an item.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="actual">Object to be found within collection</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Contains(IEnumerable collection, object actual, string message, params object[] args)
        {
            Assert.That(collection, Has.Member(actual), message, args);
        }

        /// <summary>
        /// Asserts that collection contains actual as an item.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="actual">Object to be found within collection</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Contains(IEnumerable collection, object actual, Func<string> getExceptionMessage)
        {
            Assert.That(collection, Has.Member(actual), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that collection contains actual as an item.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="actual">Object to be found within collection</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Contains(IEnumerable collection, object actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, Has.Member(actual), getExceptionMessage);
        }

        #endregion

        #region DoesNotContain

        /// <summary>
        /// Asserts that collection does not contain actual as an item.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="actual">Object that cannot exist within collection</param>
        public static void DoesNotContain(IEnumerable collection, object actual)
        {
            DoesNotContain(collection, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that collection does not contain actual as an item.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="actual">Object that cannot exist within collection</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotContain(IEnumerable collection, object actual, string message, params object[] args)
        {
            Assert.That(collection, Has.No.Member(actual), message, args);
        }

        /// <summary>
        /// Asserts that collection does not contain actual as an item.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="actual">Object that cannot exist within collection</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotContain(IEnumerable collection, object actual, Func<string> getExceptionMessage)
        {
            Assert.That(collection, Has.No.Member(actual), getExceptionMessage);
        }
        
        /// <summary>
        /// Asserts that collection does not contain actual as an item.
        /// </summary>
        /// <param name="collection">IEnumerable of objects to be considered</param>
        /// <param name="actual">Object that cannot exist within collection</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotContain(IEnumerable collection, object actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, Has.No.Member(actual), getExceptionMessage);
        }
        
        #endregion

        #region IsNotSubsetOf

        /// <summary>
        /// Asserts that the superset does not contain the subset
        /// </summary>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset)
        {
            IsNotSubsetOf(subset, superset, string.Empty, null);
        }

        /// <summary>
        /// Asserts that the superset does not contain the subset
        /// </summary>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object[] args)
        {
            Assert.That(subset, Is.Not.SubsetOf(superset), message, args);
        }

        /// <summary>
        /// Asserts that the superset does not contain the subset
        /// </summary>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset, Func<string> getExceptionMessage)
        {
            Assert.That(subset, Is.Not.SubsetOf(superset), getExceptionMessage);
        }
        
        /// <summary>
        /// Asserts that the superset does not contain the subset
        /// </summary>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(subset, Is.Not.SubsetOf(superset), getExceptionMessage);
        }
        
        #endregion

        #region IsSubsetOf

        /// <summary>
        /// Asserts that the superset contains the subset.
        /// </summary>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        public static void IsSubsetOf(IEnumerable subset, IEnumerable superset)
        {
            IsSubsetOf(subset, superset, string.Empty, null);
        }

        /// <summary>
        /// Asserts that the superset contains the subset.
        /// </summary>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object[] args)
        {
            Assert.That(subset, Is.SubsetOf(superset), message, args);
        }

        /// <summary>
        /// Asserts that the superset contains the subset.
        /// </summary>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsSubsetOf(IEnumerable subset, IEnumerable superset, Func<string> getExceptionMessage)
        {
            Assert.That(subset, Is.SubsetOf(superset), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that the superset contains the subset.
        /// </summary>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsSubsetOf(IEnumerable subset, IEnumerable superset, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(subset, Is.SubsetOf(superset), getExceptionMessage);
        }

        #endregion

        #region IsNotSupersetOf

        /// <summary>
        /// Asserts that the subset does not contain the superset
        /// </summary>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset)
        {
            IsNotSupersetOf(superset, subset, string.Empty, null);
        }

        /// <summary>
        /// Asserts that the subset does not contain the superset
        /// </summary>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset, string message, params object[] args)
        {
            Assert.That(superset, Is.Not.SupersetOf(subset), message, args);
        }
        
        /// <summary>
        /// Asserts that the subset does not contain the superset
        /// </summary>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset, Func<string> getExceptionMessage)
        {
            Assert.That(superset, Is.Not.SupersetOf(subset), getExceptionMessage);
        }
        
        /// <summary>
        /// Asserts that the subset does not contain the superset
        /// </summary>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(superset, Is.Not.SupersetOf(subset), getExceptionMessage);
        }
        
        #endregion

        #region IsSupersetOf

        /// <summary>
        /// Asserts that the subset contains the superset.
        /// </summary>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        public static void IsSupersetOf(IEnumerable superset, IEnumerable subset)
        {
            IsSupersetOf(superset, subset, string.Empty, null);
        }

        /// <summary>
        /// Asserts that the subset contains the superset.
        /// </summary>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsSupersetOf(IEnumerable superset, IEnumerable subset, string message, params object[] args)
        {
            Assert.That(superset, Is.SupersetOf(subset), message, args);
        }

        /// <summary>
        /// Asserts that the subset contains the superset.
        /// </summary>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsSupersetOf(IEnumerable superset, IEnumerable subset, Func<string> getExceptionMessage)
        {
            Assert.That(superset, Is.SupersetOf(subset), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that the subset contains the superset.
        /// </summary>
        /// <param name="superset">The IEnumerable superset to be considered</param>
        /// <param name="subset">The IEnumerable subset to be considered</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsSupersetOf(IEnumerable superset, IEnumerable subset, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(superset, Is.SupersetOf(subset), getExceptionMessage);
        }

        #endregion

        #region IsEmpty

        /// <summary>
        /// Assert that an array,list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        public static void IsEmpty(IEnumerable collection)
        {
            IsEmpty(collection, string.Empty, null);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsEmpty(IEnumerable collection, string message, params object[] args)
        {
            Assert.That(collection, new EmptyCollectionConstraint(), message, args);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsEmpty(IEnumerable collection, Func<string> getExceptionMessage)
        {
            Assert.That(collection, new EmptyCollectionConstraint(), getExceptionMessage);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsEmpty(IEnumerable collection, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, new EmptyCollectionConstraint(), getExceptionMessage);
        }

        #endregion

        #region IsNotEmpty

        /// <summary>
        /// Assert that an array,list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        public static void IsNotEmpty(IEnumerable collection)
        {
            IsNotEmpty(collection, string.Empty, null);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsNotEmpty(IEnumerable collection, string message, params object[] args)
        {
            Assert.That(collection, new NotConstraint(new EmptyCollectionConstraint()), message, args);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsNotEmpty(IEnumerable collection, Func<string> getExceptionMessage)
        {
            Assert.That(collection, new NotConstraint(new EmptyCollectionConstraint()), getExceptionMessage);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsNotEmpty(IEnumerable collection, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, new NotConstraint(new EmptyCollectionConstraint()), getExceptionMessage);
        }

        #endregion
 
        #region IsOrdered

        /// <summary>
        /// Assert that an array, list or other collection is ordered
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        public static void IsOrdered(IEnumerable collection)
        {
            IsOrdered(collection, string.Empty, null);
        }

        /// <summary>
        /// Assert that an array, list or other collection is ordered
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsOrdered(IEnumerable collection, string message, params object[] args)
        {
            Assert.That(collection, Is.Ordered, message, args);
        }

        /// <summary>
        /// Assert that an array, list or other collection is ordered
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsOrdered(IEnumerable collection, Func<string> getExceptionMessage)
        {
            Assert.That(collection, Is.Ordered, getExceptionMessage);
        }

        /// <summary>
        /// Assert that an array, list or other collection is ordered
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsOrdered(IEnumerable collection, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, Is.Ordered, getExceptionMessage);
        }

        /// <summary>
        /// Assert that an array, list or other collection is ordered
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="comparer">A custom comparer to perform the comparisons</param>
        public static void IsOrdered(IEnumerable collection, IComparer comparer)
        {
            IsOrdered(collection, comparer, string.Empty, null);
        }

        /// <summary>
        /// Assert that an array, list or other collection is ordered
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="comparer">A custom comparer to perform the comparisons</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsOrdered(IEnumerable collection, IComparer comparer, string message, params object[] args)
        {
            Assert.That(collection, Is.Ordered.Using(comparer), message, args);
        }

        /// <summary>
        /// Assert that an array, list or other collection is ordered
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="comparer">A custom comparer to perform the comparisons</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsOrdered(IEnumerable collection, IComparer comparer, Func<string> getExceptionMessage)
        {
            Assert.That(collection, Is.Ordered.Using(comparer), getExceptionMessage);
        }

        /// <summary>
        /// Assert that an array, list or other collection is ordered
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing IEnumerable</param>
        /// <param name="comparer">A custom comparer to perform the comparisons</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsOrdered(IEnumerable collection, IComparer comparer, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(collection, Is.Ordered.Using(comparer), getExceptionMessage);
        }

        #endregion
    }
}
