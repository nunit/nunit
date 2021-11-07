// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if false
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NUnitLite
{
    /// <summary>
    /// Redirects output to a Tcp connection
    /// </summary>
    class TcpWriter : TextWriter
    {
        private string hostName;
        private int port;

        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;

        public TcpWriter(string hostName, int port)
        {
            this.hostName = hostName;
            this.port = port;
            this.client = new TcpClient(hostName, port);
            this.stream = client.GetStream();
            this.writer = new StreamWriter(stream);
        }

        public override void Write(char value)
        {
            writer.Write(value);
        }

        public override void Write(string value)
        {
            writer.Write(value);
        }

        public override void WriteLine(string value)
        {
            writer.WriteLine(value);
            writer.Flush();
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
    }
}
#endif
