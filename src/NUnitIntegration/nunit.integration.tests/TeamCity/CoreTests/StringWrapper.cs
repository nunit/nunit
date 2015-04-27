namespace JetBrains.TeamCityCert.Tools.Tests
{
    using System.IO;

    public class StringWrapper : TextReader
    {
        private readonly string _string;
        private int _offset = 0;

        public StringWrapper(string str)
        {
            _string = str;
        }

        public override int Peek()
        {
            throw new System.NotImplementedException();
        }

        public override int Read()
        {
            if (_offset == _string.Length)
            {
                return -1;
            }
            
            if (_offset > _string.Length)
            {
                throw new IOException("Beyond end");
            }

            return _string[_offset++];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public override string ReadToEnd()
        {
            throw new System.NotImplementedException();
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public override string ReadLine()
        {
            throw new System.NotImplementedException();
        }
    }
}