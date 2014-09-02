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
using System.Collections.Generic;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// Summary description for RecentFilesService.
    /// </summary>
    public class RecentFilesService : IRecentFiles, IService
    {
        private IList<string> _fileEntries = new List<string>();

        private const int MinSize = 0;
        private const int MaxSize = 24;
        private const int DefaultSize = 5;

        #region Properties

        public ServiceContext ServiceContext { get; set; }

        public int MaxFiles
        {
            get 
            { 
                int size = ServiceContext.UserSettings.GetSetting("Gui.RecentProjects.MaxFiles", DefaultSize );
                
                if ( size < MinSize ) size = MinSize;
                if ( size > MaxSize ) size = MaxSize;
                
                return size;
            }
            set 
            { 
                int oldSize = MaxFiles;
                int newSize = value;
                
                if ( newSize < MinSize ) newSize = MinSize;
                if ( newSize > MaxSize ) newSize = MaxSize;

                ServiceContext.UserSettings.SaveSetting( "Gui.RecentProjects.MaxFiles", newSize );
                if ( newSize < oldSize ) SaveEntriesToSettings();
            }
        }
        #endregion

        #region Public Methods

        public IList<string> Entries { get { return _fileEntries; } }
        
        public void Remove( string fileName )
        {
            _fileEntries.Remove(fileName);
        }

        public void SetMostRecent( string filePath )
        {
            _fileEntries.Remove(filePath);

            _fileEntries.Insert( 0, filePath );
            if( _fileEntries.Count > MaxFiles )
                _fileEntries.RemoveAt( MaxFiles );
        }
        #endregion

        #region Helper Methods for saving and restoring the settings

        private void LoadEntriesFromSettings()
        {
            _fileEntries.Clear();

            // TODO: Prefix should be provided by caller
            AddEntriesForPrefix("Gui.RecentProjects");
        }

        private void AddEntriesForPrefix(string prefix)
        {
            for (int index = 1; index < MaxFiles; index++)
            {
                if (_fileEntries.Count >= MaxFiles) break;

                string fileSpec = ServiceContext.UserSettings.GetSetting(GetRecentFileKey(prefix, index)) as string;
                if (fileSpec != null) _fileEntries.Add(fileSpec);
            }
        }

        private void SaveEntriesToSettings()
        {
            string prefix = "Gui.RecentProjects";
            ISettings settings = ServiceContext.UserSettings;

            while( _fileEntries.Count > MaxFiles )
                _fileEntries.RemoveAt( _fileEntries.Count - 1 );

            for( int index = 0; index < MaxSize; index++ ) 
            {
                string keyName = GetRecentFileKey( prefix, index + 1 );
                if ( index < _fileEntries.Count )
                    settings.SaveSetting( keyName, _fileEntries[index] );
                else
                    settings.RemoveSetting( keyName );
            }

            // Remove legacy entries here
            settings.RemoveGroup("RecentProjects");
        }

        private string GetRecentFileKey( string prefix, int index )
        {
            return string.Format( "{0}.File{1}", prefix, index );
        }
        #endregion

        #region IService Members

        public void UnloadService()
        {
            SaveEntriesToSettings();
        }

        public void InitializeService()
        {
            LoadEntriesFromSettings();
        }

        #endregion
    }
}
