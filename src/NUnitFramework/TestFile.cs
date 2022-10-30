// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestUtilities
{
    public class TestFile : IDisposable
    {
        private bool _disposedValue = false;
        private readonly string _resourceName;
        private readonly FileInfo _fileInfo;
        private readonly long _fileLength;

        public TestFile(string resourceName)
            : this(Path.GetTempFileName(), resourceName, false)
        {
        }

        public TestFile(string fileName, string contentSource, bool isContent)
        {
            if (Path.IsPathRooted(fileName))
                _fileInfo = new FileInfo(fileName);
            else
            {
                var tempPath = Path.GetTempPath();
                _fileInfo = new FileInfo(Path.Combine(tempPath, fileName));
            }

            if (isContent)
            {
                using (var fs = _fileInfo.CreateText())
                {
                    fs.Write(contentSource);
                }

                _fileLength = contentSource.Length;
                return;
            }

            // HACK! Only way I can figure out to avoid having two copies of TestFile
            _resourceName = GetType().Assembly.GetName().Name.Contains("nunitlite")
                ? "NUnitLite.Tests." + contentSource
                : "NUnit.Framework." + contentSource;
            _fileLength = 0L;

            Assembly a = typeof(TestFile).Assembly;
            using (Stream s = a.GetManifestResourceStream(_resourceName))
            {
                if (s == null)
                    throw new Exception("Manifest Resource Stream " + _resourceName + " was not found.");

                var buffer = new byte[1024];
                using (FileStream fs = _fileInfo.Create())
                {
                    while (true)
                    {
                        int count = s.Read(buffer, 0, buffer.Length);
                        if (count == 0)
                            break;
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
            if (_resourceName == null)
                return -1L;

            Assembly a = typeof(TestFile).Assembly;
            using (Stream s = a.GetManifestResourceStream(_resourceName))
            {
                if (s == null)
                    throw new Exception("Manifest Resource Stream " + _resourceName + " was not found.");

                byte[] buffer = new byte[1024];
                long offset = 0L;

                while (true)
                {
                    int count = s.Read(buffer, 0, buffer.Length);
                    if (count == 0)
                        break;
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
