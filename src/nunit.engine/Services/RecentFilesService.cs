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
        private IList<RecentFileEntry> fileEntries = new List<RecentFileEntry>();

		private const int MinSize = 0;
		private const int MaxSize = 24;
		private const int DefaultSize = 5;

		#region Properties

        public ServiceContext ServiceContext { get; set; }

		public int Count
		{
			get { return fileEntries.Count; }
		}

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

		public IList<RecentFileEntry> Entries { get { return fileEntries; } }
		
		public void Remove( string fileName )
		{
            int index = IndexOf(fileName);
            if (index != -1)
                fileEntries.RemoveAt(index);
        }

		public void SetMostRecent( string fileName )
		{
			SetMostRecent( new RecentFileEntry( fileName ) );
		}

		public void SetMostRecent( RecentFileEntry entry )
		{
			int index = IndexOf(entry.Path);

			if(index != -1)
				fileEntries.RemoveAt(index);

			fileEntries.Insert( 0, entry );
			if( fileEntries.Count > MaxFiles )
				fileEntries.RemoveAt( MaxFiles );
		}
		#endregion

		#region Helper Methods for saving and restoring the settings

   		private int IndexOf( string fileName )
		{
			for( int index = 0; index < Count; index++ )
				if ( fileEntries[index].Path == fileName )
					return index;
			return -1;
		}

        private void LoadEntriesFromSettings()
		{
			fileEntries.Clear();

            AddEntriesForPrefix("Gui.RecentProjects");

            // Try legacy entries if nothing was found
            if (fileEntries.Count == 0)
            {
                AddEntriesForPrefix("RecentProjects.V2");
                AddEntriesForPrefix("RecentProjects.V1");
            }

            // Try even older legacy format
            if (fileEntries.Count == 0)
                AddEntriesForPrefix("RecentProjects");
		}

        private void AddEntriesForPrefix(string prefix)
        {
            for (int index = 1; index < MaxFiles; index++)
            {
                if (fileEntries.Count >= MaxFiles) break;

                string fileSpec = ServiceContext.UserSettings.GetSetting(GetRecentFileKey(prefix, index)) as string;
                if (fileSpec != null) fileEntries.Add(RecentFileEntry.Parse(fileSpec));
            }
        }

		private void SaveEntriesToSettings()
		{
			string prefix = "Gui.RecentProjects";
            ISettings settings = ServiceContext.UserSettings;

			while( fileEntries.Count > MaxFiles )
				fileEntries.RemoveAt( fileEntries.Count - 1 );

			for( int index = 0; index < MaxSize; index++ ) 
			{
				string keyName = GetRecentFileKey( prefix, index + 1 );
				if ( index < fileEntries.Count )
					settings.SaveSetting( keyName, fileEntries[index].Path );
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
