// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

#if !NUNITLITE
using System;
using System.Reflection;
using System.Text;
using System.IO;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// AssemblyReader knows how to find various things in an assembly header
    /// </summary>
    public class AssemblyReader : IDisposable
    {
        private string assemblyPath;
        private BinaryReader rdr;
        private FileStream fs;

        UInt16 dos_magic = 0xffff;
        uint pe_signature = 0xffffffff;
        UInt16 numDataSections;
        UInt16 optionalHeaderSize;

        private uint peHeader = 0;
        private uint fileHeader = 0;
        private uint optionalHeader = 0;
        private uint dataDirectory = 0;
        private uint dataSections = 0;

        private struct Section
        {
            public uint virtualAddress;
            public uint virtualSize;
            public uint fileOffset;
        };

        private Section[] sections;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyReader"/> class.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public AssemblyReader( string assemblyPath )
        {
            this.assemblyPath = assemblyPath;
            CalcHeaderOffsets();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyReader"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AssemblyReader( Assembly assembly )
        {
            this.assemblyPath = AssemblyHelper.GetAssemblyPath( assembly );
            CalcHeaderOffsets();
        }

        private void CalcHeaderOffsets()
        {
            this.fs = new FileStream( assemblyPath, FileMode.Open, FileAccess.Read );
            this.rdr = new BinaryReader( fs );
            dos_magic = rdr.ReadUInt16();
            if ( dos_magic == 0x5a4d )
            {
                fs.Position = 0x3c;
                peHeader = rdr.ReadUInt32();
                fileHeader = peHeader + 4;
                optionalHeader = fileHeader + 20;
                dataDirectory = optionalHeader + 96;
                // dotNetDirectoryEntry = dataDirectory + 14 * 8;

                fs.Position = peHeader;
                pe_signature = rdr.ReadUInt32();
                rdr.ReadUInt16(); // machine
                numDataSections = rdr.ReadUInt16();
                fs.Position += 12;
                optionalHeaderSize = rdr.ReadUInt16();
                dataSections = optionalHeader + optionalHeaderSize;

                sections = new Section[numDataSections];
                fs.Position = dataSections;
                for( int i = 0; i < numDataSections; i++ )
                {
                    fs.Position += 8;
                    sections[i].virtualSize = rdr.ReadUInt32();
                    sections[i].virtualAddress = rdr.ReadUInt32();
                    uint rawDataSize = rdr.ReadUInt32();
                    sections[i].fileOffset = rdr.ReadUInt32();
                    if ( sections[i].virtualSize == 0 )
                        sections[i].virtualSize = rawDataSize;

                    fs.Position += 16;
                }
            }
        }

        private uint DataDirectoryRva( int n )
        {
            fs.Position = dataDirectory + n * 8;
            return rdr.ReadUInt32();
        }

        private uint RvaToLfa( uint rva )
        {
            for( int i = 0; i < numDataSections; i++ )
                if ( rva >= sections[i].virtualAddress && rva < sections[i].virtualAddress + sections[i].virtualSize )
                    return rva - sections[i].virtualAddress + sections[i].fileOffset;

            return 0;
        }

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        /// <value>The assembly path.</value>
        public string AssemblyPath
        {
            get { return assemblyPath; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a valid PE file.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is a valid PE file; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidPeFile
        {
            get { return dos_magic == 0x5a4d && pe_signature == 0x00004550; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a .NET executable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is a .NET executable; otherwise, <c>false</c>.
        /// </value>
        public bool IsDotNetFile
        {
            get { return IsValidPeFile && DataDirectoryRva(14) != 0; }
        }

        /// <summary>
        /// Gets the image runtime version for the assembly.
        /// </summary>
        /// <value>The image runtime version.</value>
        public string ImageRuntimeVersion
        {
            get 
            {
                string runtimeVersion = string.Empty;

                uint rva = DataDirectoryRva(14);
                if ( rva != 0 )
                {
                    fs.Position = RvaToLfa( rva ) + 8;
                    uint metadata = rdr.ReadUInt32();
                    fs.Position = RvaToLfa( metadata );
                    if ( rdr.ReadUInt32() == 0x424a5342 )
                    {
                        // Copy string representing runtime version
                        fs.Position += 12;
                        StringBuilder sb = new StringBuilder();
                        char c;
                        while ((c = rdr.ReadChar())!= '\0')
                            sb.Append(c);

                        if (sb[0] == 'v') // Last sanity check
                            runtimeVersion = sb.ToString();

                        // Could do fixups here for bad _values in older files
                        // like 1.x86, 1.build, etc. But we are only using
                        // the major version anyway
                    }
                }

                return runtimeVersion; 
            }
        }

        /// <summary>
        /// Closes the file stream and reader and sets references to null.
        /// </summary>
        public void Dispose()
        {
            if ( fs != null )
                fs.Close();
            if ( rdr != null )
                rdr.Close();

            fs = null;
            rdr = null;
        }
    }
}
#endif
