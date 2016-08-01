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
using System.Collections.Generic;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// ServiceManager handles access to all services - global
    /// facilities shared by all instances of TestEngine.
    /// </summary>
    public class ServiceManager : IDisposable
    {
        private List<IService> _services = new List<IService>();
        private Dictionary<Type, IService> _serviceIndex = new Dictionary<Type, IService>();

        static Logger log = InternalTrace.GetLogger(typeof(ServiceManager));

        public bool ServicesInitialized { get; private set; }

        #region Public Methods

        public IService GetService( Type serviceType )
        {
            IService theService = null;

            if (_serviceIndex.ContainsKey(serviceType))
                theService = _serviceIndex[serviceType];
            else
                foreach( IService service in _services )
                {
                    if( serviceType.IsInstanceOfType( service ) )
                    {
                        _serviceIndex[serviceType] = service;
                        theService = service;
                        break;
                    }
                }

            if (theService == null)
                log.Error(string.Format("Requested service {0} was not found", serviceType.FullName));
            else
                log.Debug(string.Format("Request for service {0} satisfied by {1}", serviceType.Name, theService.GetType().Name));
            
            return theService;
        }

        public void AddService(IService service)
        {
            _services.Add(service);
            log.Debug("Added " + service.GetType().Name);
        }

        public void StartServices()
        {
            foreach( IService service in _services )
            {
                if (service.Status == ServiceStatus.Stopped)
                {
                    string name = service.GetType().Name;
                    log.Info("Initializing " + name);
                    try
                    {
                        service.StartService();
                        if (service.Status == ServiceStatus.Error)
                            throw new InvalidOperationException("Service failed to initialize " + name);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Should we pass this exception through?
                        log.Error("Failed to initialize " + name );
                        log.Error(ex.ToString());
                        throw;
                    }
                }
            }

            this.ServicesInitialized = true;
        }

        public void StopServices()
        {
            // Stop services in reverse of initialization order
            int index = _services.Count;
            while (--index >= 0)
            {
                IService service = _services[index];
                if (service.Status == ServiceStatus.Started)
                {
                    string name = service.GetType().Name;
                    log.Info("Stopping " + name);
                    try
                    {
                        service.StopService();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failure stopping service " + name);
                        log.Error(ex.ToString());
                    }
                }
            }
        }

        public void ClearServices()
        {
            log.Info("Clearing Service list");
            _services.Clear();
        }

        #endregion

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                StopServices();

                if (disposing)
                    foreach (IService service in _services)
                    {
                        IDisposable disposable = service as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                    }

                _disposed = true;
            }
        }

        #endregion
    }
}
