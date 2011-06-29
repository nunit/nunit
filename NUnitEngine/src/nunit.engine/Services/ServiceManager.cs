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
#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif

namespace NUnit.Engine.Services
{
	/// <summary>
	/// ServiceManager handles access to all services - global
    /// facilities shared by all instances of TestEngine.
	/// </summary>
	public class ServiceManager : IServiceManager
	{
#if CLR_2_0 || CLR_4_0
        private List<IService> services = new List<IService>();
        private Dictionary<Type, IService> serviceIndex = new Dictionary<Type, IService>();
#else
		private ArrayList services = new ArrayList();
		private Hashtable serviceIndex = new Hashtable();
#endif

        //static Logger log = InternalTrace.GetLogger(typeof(ServiceManager));

		public void AddService( IService service )
		{
			services.Add( service );
            //log.Debug( "Added " + service.GetType().Name );
		}

		public IService GetService( Type serviceType )
		{
            IService theService = null;

#if CLR_2_0 || CLR_4_0
            if (serviceIndex.ContainsKey(serviceType))
#else
            if (serviceIndex.Contains(serviceType))
#endif
			    theService = (IService)serviceIndex[serviceType];
			else
				foreach( IService service in services )
				{
					// TODO: Does this work on Mono?
					if( serviceType.IsInstanceOfType( service ) )
					{
						serviceIndex[serviceType] = service;
						theService = service;
						break;
					}
				}

            //if ( theService == null )
            //    log.Error( string.Format( "Requested service {0} was not found", serviceType.FullName ) );
            //else
            //    log.Debug( string.Format( "Request for service {0} satisfied by {1}", serviceType.Name, theService.GetType().Name ) );
			
			return theService;
		}

		public void InitializeServices()
		{
			foreach( IService service in services )
			{
                //log.Info( "Initializing " + service.GetType().Name );
                //try
                //{
                    service.InitializeService();
                //}
                //catch (Exception ex)
                //{
                //    log.Error("Failed to initialize service", ex);
                //}
			}
		}

		public void StopAllServices()
		{
			// Stop services in reverse of initialization order
			// TODO: Deal with dependencies explicitly
			int index = services.Count;
            while (--index >= 0)
            {
                IService service = services[index] as IService;
                //log.Info( "Stopping " + service.GetType().Name );
                //try
                //{
                    service.UnloadService();
                //}
                //catch (Exception ex)
                //{
                //    log.Error("Failure stopping service", ex);
                //}
            }
		}

		public void ClearServices()
		{
            //log.Info("Clearing Service list");
			services.Clear();
		}
	}
}
