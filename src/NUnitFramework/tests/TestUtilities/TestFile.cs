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
        private FileInfo _fileInfo;
        private long _fileLength;

        public TestFile(string fileName, string resourceName)
        {
            if (Path.IsPathRooted(fileName))
                _fileInfo = new FileInfo(fileName);
            else
            {
                var tempPath = Path.GetTempPath();
                _fileInfo = new FileInfo(Path.Combine(tempPath, fileName));
            }

            _resourceName = "NUnit.Framework.Tests." + resourceName;
            _fileLength = 0L;

            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream s = a.GetManifestResourceStream(_resourceName))
            {
                if (s == null) throw new Exception("Manifest Resource Stream " + _resourceName + " was not found.");

                var buffer = new byte[1024];
                using (FileStream fs = _fileInfo.Create())
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

        /// <summary>
        /// Gets the test file.
        /// </summary>
        public FileInfo File { get { return _fileInfo; } }

        /// <summary>
        /// Returns the full path of the contained test directory
        /// </summary>
        public override string ToString()
        {
            return _fileInfo == null ? string.Empty : _fileInfo.FullName;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
                if (disposing)
                    if (_fileInfo.Exists)
                        _fileInfo.Delete();

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
