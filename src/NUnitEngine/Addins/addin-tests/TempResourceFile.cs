// ***********************************************************************
// Copyright (c) 2007-2014 Charlie Poole
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

namespace NUnit.Engine.Tests
{
    using System;
    using System.IO;

    public class TempResourceFile : IDisposable
    {
        static readonly string TEMP_PATH = System.IO.Path.GetTempPath();

        public string Path { get; private set; }

        public bool IsRelativeToTempPath  { get; private set; }

        public TempResourceFile(Type type, string name) : this(type, name, null) {}

        public TempResourceFile(Type type, string name, string filePath)
        {
            if (filePath == null)
                filePath = name;

            IsRelativeToTempPath = !System.IO.Path.IsPathRooted(filePath);
            if (IsRelativeToTempPath)
                filePath = System.IO.Path.Combine(TEMP_PATH, filePath);

            Path = filePath;

            Stream stream = type.Assembly.GetManifestResourceStream(type, name);
            byte[] buffer = new byte[(int)stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            string dir = System.IO.Path.GetDirectoryName(Path);
            if(dir != null && dir.Length != 0)
            {
                Directory.CreateDirectory(dir);
            }

            using(FileStream fileStream = new FileStream(Path, FileMode.Create))
            {
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }

        public void Dispose()
        {
            File.Delete(Path);
            
            string path = Path;
            while(true)
            {
                path = System.IO.Path.GetDirectoryName(path);
                if(path == null || path.Length == 0)
                    break;
                if(IsRelativeToTempPath && path.Length <= TEMP_PATH.Length)
                    break;
                if(Directory.GetFiles(path).Length > 0)
                    break;

                Directory.Delete(path);
            }
        }
    }
}
