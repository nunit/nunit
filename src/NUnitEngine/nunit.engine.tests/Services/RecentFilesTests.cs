// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    using Fakes;
    
    /// <summary>
    /// This fixture is used to test both RecentProjects and
    /// its base class RecentFiles.  If we add any other derived
    /// classes, the tests should be refactored.
    /// </summary>
    [TestFixture]
    public class RecentFilesTests
    {
        private const int MAX = 24;
        private const int MIN = 0;
        private const int DEFAULT = 5;

        RecentFilesService _recentFiles;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceContext();
            services.Add(new FakeSettingsService());
            _recentFiles = new RecentFilesService();
            services.Add(_recentFiles);
            services.ServiceManager.InitializeServices();
        }

        [Test]
        public void ServiceIsStarted()
        {
            Assert.That(_recentFiles.Status, Is.EqualTo(ServiceStatus.Started));
        }

        [Test]
        public void CountDefault()
        {
            Assert.AreEqual( DEFAULT, _recentFiles.MaxFiles );
        }

        [Test]
        public void CountOverMax()
        {
            _recentFiles.MaxFiles = MAX + 1;
            Assert.AreEqual( MAX, _recentFiles.MaxFiles );
        }

        [Test]
        public void CountUnderMin()
        {
            _recentFiles.MaxFiles = MIN - 1;
            Assert.AreEqual( MIN, _recentFiles.MaxFiles );
        }

        [Test]
        public void CountAtMax()
        {
            _recentFiles.MaxFiles = MAX;
            Assert.AreEqual( MAX, _recentFiles.MaxFiles );
        }

        [Test]
        public void CountAtMin()
        {
            _recentFiles.MaxFiles = MIN;
            Assert.AreEqual( MIN, _recentFiles.MaxFiles );
        }

        [Test]
        public void EmptyList()
        {
            Assert.IsNotNull(  _recentFiles.Entries, "Entries should never be null" );
            Assert.AreEqual( 0, _recentFiles.Entries.Count );
        }

        [Test]
        public void AddSingleItem()
        {
            CheckAddItems( 1 );
        }

        [Test]
        public void AddMaxItems()
        {
            CheckAddItems( 5 );
        }

        [Test]
        public void AddTooManyItems()
        {
            CheckAddItems( 10 );
        }

        [Test]
        public void IncreaseSize()
        {
            _recentFiles.MaxFiles = 10;
            CheckAddItems( 10 );
        }

        [Test]
        public void ReduceSize()
        {
            _recentFiles.MaxFiles = 3;
            CheckAddItems( 10 );
        }

        [Test]
        public void IncreaseSizeAfterAdd()
        {
            SetMockValues(5);
            _recentFiles.MaxFiles = 7;
            _recentFiles.SetMostRecent( "30" );
            _recentFiles.SetMostRecent( "20" );
            _recentFiles.SetMostRecent( "10" );
            CheckListContains( 10, 20, 30, 1, 2, 3, 4 );
        }

        [Test]
        public void ReduceSizeAfterAdd()
        {
            SetMockValues( 5 );
            _recentFiles.MaxFiles = 3;
            CheckMockValues( 3 );
        }

        [Test]
        public void ReorderLastProject()
        {
            SetMockValues( 5 );
            _recentFiles.SetMostRecent( "5" );
            CheckListContains( 5, 1, 2, 3, 4 );
        }

        [Test]
        public void ReorderSingleProject()
        {
            SetMockValues( 5 );
            _recentFiles.SetMostRecent( "3" );
            CheckListContains( 3, 1, 2, 4, 5 );
        }

        [Test]
        public void ReorderMultipleProjects()
        {
            SetMockValues( 5 );
            _recentFiles.SetMostRecent( "3" );
            _recentFiles.SetMostRecent( "5" );
            _recentFiles.SetMostRecent( "2" );
            CheckListContains( 2, 5, 3, 1, 4 );
        }

        [Test]
        public void ReorderSameProject()
        {
            SetMockValues( 5 );
            _recentFiles.SetMostRecent( "1" );
            CheckListContains( 1, 2, 3, 4, 5 );
        }

        [Test]
        public void ReorderWithListNotFull()
        {
            SetMockValues( 3 );
            _recentFiles.SetMostRecent( "3" );
            CheckListContains( 3, 1, 2 );
        }

        [Test]
        public void RemoveFirstProject()
        {
            SetMockValues( 3 );
            _recentFiles.Remove("1");
            CheckListContains( 2, 3 );
        }

        [Test]
        public void RemoveOneProject()
        {
            SetMockValues( 4 );
            _recentFiles.Remove("2");
            CheckListContains( 1, 3, 4 );
        }

        [Test]
        public void RemoveMultipleProjects()
        {
            SetMockValues( 5 );
            _recentFiles.Remove( "3" );
            _recentFiles.Remove( "1" );
            _recentFiles.Remove( "4" );
            CheckListContains( 2, 5 );
        }
        
        [Test]
        public void RemoveLastProject()
        {
            SetMockValues( 5 );
            _recentFiles.Remove("5");
            CheckListContains( 1, 2, 3, 4 );
        }

        #region Helper Methods

        // Set RecentFiles to a list of known values up
        // to a maximum. Most recent will be "1", next 
        // "2", and so on...
        private void SetMockValues(int count)
        {
            for (int num = count; num > 0; --num)
                _recentFiles.SetMostRecent(num.ToString());
        }

        // Check that the list is set right: 1, 2, ...
        private void CheckMockValues(int count)
        {
            var files = _recentFiles.Entries;
            Assert.AreEqual(count, files.Count, "Count");

            for (int index = 0; index < count; index++)
                Assert.AreEqual((index + 1).ToString(), files[index], "Item");
        }

        // Check that we can add count items correctly
        private void CheckAddItems(int count)
        {
            SetMockValues(count);
            Assert.AreEqual("1", _recentFiles.Entries[0], "RecentFile");

            CheckMockValues(Math.Min(count, _recentFiles.MaxFiles));
        }

        // Check that the list contains a set of entries
        // in the order given and nothing else.
        private void CheckListContains(params int[] item)
        {
            var files = _recentFiles.Entries;
            Assert.AreEqual(item.Length, files.Count, "Count");

            for (int index = 0; index < files.Count; index++)
                Assert.AreEqual(item[index].ToString(), files[index], "Item");
        }

        #endregion
    }
}
