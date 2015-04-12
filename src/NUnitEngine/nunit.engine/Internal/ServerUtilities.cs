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
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Reflection;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// Summary description for RemotingUtilities.
    /// </summary>
    public static class ServerUtilities
    {
        static readonly Logger log = InternalTrace.GetLogger(typeof(ServerUtilities));

        /// <summary>
        ///  Create a TcpChannel with a given name, on a given port.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="name"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private static TcpChannel CreateTcpChannel( string name, int port, int limit )
        {
            var props = new Hashtable {{"port", port}, {"name", name}, {"bindTo", "127.0.0.1"}};

            var serverProvider = new BinaryServerFormatterSinkProvider();

            // NOTE: TypeFilterLevel and "clientConnectionLimit" property don't exist in .NET 1.0.
            Type typeFilterLevelType = typeof(object).Assembly.GetType("System.Runtime.Serialization.Formatters.TypeFilterLevel");
            if (typeFilterLevelType != null)
            {
                PropertyInfo typeFilterLevelProperty = serverProvider.GetType().GetProperty("TypeFilterLevel");
                object typeFilterLevel = Enum.Parse(typeFilterLevelType, "Full");
                typeFilterLevelProperty.SetValue(serverProvider, typeFilterLevel, null);

//                props.Add("clientConnectionLimit", limit);
            }

            var clientProvider = new BinaryClientFormatterSinkProvider();

            return new TcpChannel( props, clientProvider, serverProvider );
        }

        public static TcpChannel GetTcpChannel()
        {
            return GetTcpChannel( "", 0, 2 );
        }

        /// <summary>
        /// Get a channel by name, casting it to a TcpChannel.
        /// Otherwise, create, register and return a TcpChannel with
        /// that name, on the port provided as the second argument.
        /// </summary>
        /// <param name="name">The channel name</param>
        /// <param name="port">The port to use if the channel must be created</param>
        /// <returns>A TcpChannel or null</returns>
        public static TcpChannel GetTcpChannel( string name, int port )
        {
            return GetTcpChannel( name, port, 2 );
        }
        
        /// <summary>
        /// Get a channel by name, casting it to a TcpChannel.
        /// Otherwise, create, register and return a TcpChannel with
        /// that name, on the port provided as the second argument.
        /// </summary>
        /// <param name="name">The channel name</param>
        /// <param name="port">The port to use if the channel must be created</param>
        /// <param name="limit">The client connection limit or negative for the default</param>
        /// <returns>A TcpChannel or null</returns>
        public static TcpChannel GetTcpChannel( string name, int port, int limit )
        {
            var channel = ChannelServices.GetChannel( name ) as TcpChannel;

            if ( channel == null )
            {
                // NOTE: Retries are normally only needed when rapidly creating
                // and destroying channels, as in running the NUnit tests.
                var retries = 10;
                while( --retries > 0 )
                    try
                    {
                        channel = CreateTcpChannel( name, port, limit );
                        ChannelServices.RegisterChannel( channel, false );
                        break;
                    }
                    catch( Exception e )
                    {
                        log.Error("Failed to create/register channel", e);
                        System.Threading.Thread.Sleep(300);
                    }
            }

            return channel;
        }

        public static void SafeReleaseChannel( IChannel channel )
        {
            if( channel != null )
                try
                {
                    ChannelServices.UnregisterChannel( channel );
                }
                catch( RemotingException )
                {
                    // Channel was not registered - ignore
                }
        }
    }
}
