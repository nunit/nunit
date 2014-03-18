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
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// Summary description for ServerBase.
    /// </summary>
    public abstract class ServerBase : MarshalByRefObject, IDisposable
    {
        protected string uri;
        protected int port;

        private TcpChannel channel;
        private bool isMarshalled;

        private object theLock = new object();

        protected ServerBase()
        {
        }

        /// <summary>
        /// Constructor used to provide
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="port"></param>
        protected ServerBase(string uri, int port)
        {
            this.uri = uri;
            this.port = port;
        }

        public string ServerUrl
        {
            get { return string.Format("tcp://127.0.0.1:{0}/{1}", port, uri); }
        }

        public virtual void Start()
        {
            if (uri != null && uri != string.Empty)
            {
                lock (theLock)
                {
                    this.channel = ServerUtilities.GetTcpChannel(uri + "Channel", port, 100);

                    RemotingServices.Marshal(this, uri);
                    this.isMarshalled = true;
                }

                if (this.port == 0)
                {
                    ChannelDataStore store = this.channel.ChannelData as ChannelDataStore;
                    if (store != null)
                    {
                        string channelUri = store.ChannelUris[0];
                        this.port = int.Parse(channelUri.Substring(channelUri.LastIndexOf(':') + 1));
                    }
                }
            }
        }

        [System.Runtime.Remoting.Messaging.OneWay]
        public virtual void Stop()
        {
            lock( theLock )
            {
                if ( this.isMarshalled )
                {
                    RemotingServices.Disconnect( this );
                    this.isMarshalled = false;
                }

                if ( this.channel != null )
                {
                    ChannelServices.UnregisterChannel( this.channel );
                    this.channel = null;
                }

                Monitor.PulseAll( theLock );
            }
        }

        public void WaitForStop()
        {
            lock( theLock )
            {
                Monitor.Wait( theLock );
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Stop();
        }

        #endregion

        #region InitializeLifetimeService
        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion
    }
}
