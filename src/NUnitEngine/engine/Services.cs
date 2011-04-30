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

namespace NUnit.Engine
{
	/// <summary>
	/// Services is a utility class, which is used to simplify
    /// access to services within the test engine.
	/// </summary>
	public static class Services
	{
		#region AddinManager
        //private static AddinManager addinManager;
        //public static AddinManager AddinManager
        //{
        //    get 
        //    {
        //        if (addinManager == null )
        //            addinManager = (AddinManager)ServiceManager.Services.GetService( typeof( AddinManager ) );

        //        return addinManager; 
        //    }
        //}
		#endregion

		#region AddinRegistry
        //private static IAddinRegistry addinRegistry;
        //public static IAddinRegistry AddinRegistry
        //{
        //    get 
        //    {
        //        if (addinRegistry == null)
        //            addinRegistry = (IAddinRegistry)ServiceManager.Services.GetService( typeof( IAddinRegistry ) );
                
        //        return addinRegistry;
        //    }
        //}
		#endregion

		#region DomainManager
		private static DomainManager domainManager;
		public static DomainManager DomainManager
		{
			get
			{
				if ( domainManager == null )
					domainManager = (DomainManager)ServiceManager.Services.GetService( typeof( DomainManager ) );

				return domainManager;
			}
		}
		#endregion

		#region UserSettings
        //private static ISettings userSettings;
        //public static ISettings UserSettings
        //{
        //    get 
        //    { 
        //        if ( userSettings == null )
        //            userSettings = (ISettings)ServiceManager.Services.GetService( typeof( ISettings ) );

        //        // Temporary fix needed to run TestDomain tests in test AppDomain
        //        // TODO: Figure out how to set up the test domain correctly
        //        if ( userSettings == null )
        //            userSettings = new SettingsService();

        //        return userSettings; 
        //    }
        //}
		
		#endregion

		#region RecentFilesService
//        private static RecentFiles recentFiles;
//        public static RecentFiles RecentFiles
//        {
//            get
//            {
//                if ( recentFiles == null )
//                    recentFiles = (RecentFiles)ServiceManager.Services.GetService( typeof( RecentFiles ) );

//                return recentFiles;
//            }
//        }
		#endregion

		#region TestLoader
//        private static TestLoader loader;
//        public static TestLoader TestLoader
//        {
//            get
//            {
//                if ( loader == null )
//                    loader = (TestLoader)ServiceManager.Services.GetService( typeof( TestLoader ) );

//                return loader;
//            }
//        }
		#endregion

		#region TestAgency
//        private static TestAgency agency;
//        public static TestAgency TestAgency
//        {
//            get
//            {
//                if ( agency == null )
//                    agency = (TestAgency)ServiceManager.Services.GetService( typeof( TestAgency ) );

//                // Temporary fix needed to run ProcessRunner tests in test AppDomain
//                // TODO: Figure out how to set up the test domain correctly
////				if ( agency == null )
////				{
////					agency = new TestAgency();
////					agency.Start();
////				}

//                return agency;
//            }
//        }
		#endregion

		#region ProjectLoader
        //private static ProjectService projectService;
        //public static ProjectService ProjectService
        //{
        //    get
        //    {
        //        if ( projectService == null )
        //            projectService = (ProjectService)
        //                ServiceManager.Services.GetService( typeof( ProjectService ) );

        //        return projectService;
        //    }
        //}
		#endregion
	}
}
