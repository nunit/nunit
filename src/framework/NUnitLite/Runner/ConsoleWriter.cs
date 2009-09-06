using System;
using System.IO;

namespace NUnitLite.Runner
{
    /// <summary>
    /// Provide an alternative to Console.Out for 
    /// version 1.0 of the compact framework.
    /// </summary>
    public class ConsoleWriter : TextWriter
    {
        private static TextWriter writer;

        public static TextWriter Out
        {
            get
            {
                if ( writer == null )
                    writer = new ConsoleWriter();

                return writer; 
            }
        }

        public override void Write(char value)
        {
            Console.Write(value);
        }

        public override void Write(string value)
        {
            Console.Write(value);
        }

        public override void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
    }
}
