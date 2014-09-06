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
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using NUnit.Framework;

namespace NUnit.Engine.Internal.Tests
{
	/// <summary>
	/// Summary description for RemotingUtilitiesTests.
	/// </summary>
	[TestFixture]
	public class ServerUtilityTests
	{
		TcpChannel channel1;
		TcpChannel channel2;

		[TearDown]
		public void ReleaseChannels()
		{
			ServerUtilities.SafeReleaseChannel( channel1 );
			ServerUtilities.SafeReleaseChannel( channel2 );
		}

		[Test]
		public void CanGetTcpChannelOnSpecifiedPort()
		{
			channel1 = ServerUtilities.GetTcpChannel( "test", 1234 );
			Assert.AreEqual( "test", channel1.ChannelName );
			channel2 = ServerUtilities.GetTcpChannel( "test", 4321 );
			Assert.AreEqual( "test", channel2.ChannelName );
			Assert.AreEqual( channel1, channel2 );
			Assert.AreSame( channel1, channel2 );
			ChannelDataStore cds = (ChannelDataStore)channel1.ChannelData;
			Assert.AreEqual( "tcp://127.0.0.1:1234", cds.ChannelUris[0] );
		}

		[Test]
		public void CanGetTcpChannelOnUnpecifiedPort()
		{
			channel1 = ServerUtilities.GetTcpChannel( "test", 0 );
			Assert.AreEqual( "test", channel1.ChannelName );
			channel2 = ServerUtilities.GetTcpChannel( "test", 0 );
			Assert.AreEqual( "test", channel2.ChannelName );
			Assert.AreEqual( channel1, channel2 );
			Assert.AreSame( channel1, channel2 );
		}
	}
}
