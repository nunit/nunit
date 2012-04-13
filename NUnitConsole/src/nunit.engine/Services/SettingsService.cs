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

using System;
using System.IO;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
	/// <summary>
	/// Summary description for UserSettingsService.
	/// </summary>
	public class SettingsService : SettingsGroup, IService
	{
		static readonly string settingsFileName = "NUnitSettings.xml";

        private string settingsFile;
        private bool writeable;

        public SettingsService() : this(true) { }

		public SettingsService(bool writeable)
		{
            this.writeable = writeable;

            this.settingsFile = Path.Combine(NUnitConfiguration.ApplicationDirectory, settingsFileName);

			if ( File.Exists( settingsFile ) )
				LoadSettings(settingsFile);
		}

        #region IService Implementation

        private ServiceContext services;
        public ServiceContext ServiceContext 
        {
            get { return services; }
            set { services = value; }
        }

		public void InitializeService()
		{
		}

		public void UnloadService()
		{
            if ( writeable )
			    SaveSettings(settingsFile);

			this.Dispose();
		}
		#endregion
	}
}
