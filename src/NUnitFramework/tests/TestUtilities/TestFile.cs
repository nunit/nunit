using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NUnit.TestUtilities
{
    public class TestFile : IDisposable
    {
        private bool _disposedValue = false;
        private string _resourceName;
        private string _fileName;
        private long _fileLength;

        public TestFile(string fileName, string resourceName)
        {
            _resourceName = "NUnit.Framework.Tests." + resourceName;
            _fileName = fileName;
            _fileLength = 0L;

            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream s = a.GetManifestResourceStream(_resourceName))
            {
                if (s == null) throw new Exception("Manifest Resource Stream " + _resourceName + " was not found.");

                byte[] buffer = new byte[1024];
                using (FileStream fs = File.Create(_fileName))
                {
                    while (true)
                    {
                        int count = s.Read(buffer, 0, buffer.Length);
                        if (count == 0) break;
                        fs.Write(buffer, 0, count);
                        _fileLength += count;
                    }
                }
            }
        }

        public long FileLength
        {
            get { return _fileLength; }
        }

        public long OffsetOf(char target)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream s = a.GetManifestResourceStream(_resourceName))
            {
                if (s == null) throw new Exception("Manifest Resource Stream " + _resourceName + " was not found.");

                byte[] buffer = new byte[1024];
                long offset = 0L;

                while (true)
                {
                    int count = s.Read(buffer, 0, buffer.Length);
                    if (count == 0) break;
                    foreach (char c in buffer)
                        if (c == target)
                            return offset;
                        else
                            offset++;
                }

                return -1L;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (File.Exists(_fileName))
                    {
                        File.Delete(_fileName);
                    }
                }
            }
            _disposedValue = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
