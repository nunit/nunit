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
using System.Reflection;
using System.Text;
using System.IO;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// AssemblyReader knows how to find various things in an assembly header
    /// </summary>
    public class AssemblyReader : IDisposable
    {
        private readonly string assemblyPath;
        private BinaryReader rdr;
        private FileStream fs;

        private UInt16 dos_magic = 0xffff;
        private UInt32 pe_signature = 0xffffffff;
        private UInt16 numberOfSections;
        private UInt16 optionalHeaderSize;
        private PEType peType;
        private UInt32 numDataDirectoryEntries;
        private CorFlags corFlags;

        private uint peHeader;
        private uint fileHeader;
        private uint optionalHeader;
        private uint dataDirectory;
        private uint dataSections;

        private struct DataSection
        {
            public uint VirtualAddress;
            public uint VirtualSize;
            public uint FileOffset;
        };

        private enum PEType : ushort
        {
            PE32 = 0x10b,
            PE32Plus = 0x20b
        }

        [Flags]
        private enum CorFlags : uint
        {
            // CorHdr.h
            COMIMAGE_FLAGS_ILONLY            = 0x00000001,
            COMIMAGE_FLAGS_32BITREQUIRED     = 0x00000002,
            COMIMAGE_FLAGS_IL_LIBRARY        = 0x00000004,
            COMIMAGE_FLAGS_STRONGNAMESIGNED  = 0x00000008,
            COMIMAGE_FLAGS_NATIVE_ENTRYPOINT = 0x00000010,
            COMIMAGE_FLAGS_TRACKDEBUGDATA    = 0x00010000,
            COMIMAGE_FLAGS_32BITPREFERRED    = 0x00020000,
        }

        private DataSection[] sections;

        public AssemblyReader( string assemblyPath )
        {
            this.assemblyPath = assemblyPath;
            CalcHeaderOffsets();
        }

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

                fs.Position = optionalHeader;
                peType = (PEType)rdr.ReadUInt16();

                dataDirectory = peType == PEType.PE32Plus
                    ? optionalHeader + 112
                    : optionalHeader + 96;

                fs.Position = dataDirectory - 4;
                numDataDirectoryEntries = rdr.ReadUInt32();

                fs.Position = peHeader;
                pe_signature = rdr.ReadUInt32();
                rdr.ReadUInt16(); // machine
                numberOfSections = rdr.ReadUInt16();
                fs.Position += 12;
                optionalHeaderSize = rdr.ReadUInt16();
                dataSections = optionalHeader + optionalHeaderSize;

                sections = new DataSection[numberOfSections];
                fs.Position = dataSections;
                for( int i = 0; i < numberOfSections; i++ )
                {
                    fs.Position += 8;
                    sections[i].VirtualSize = rdr.ReadUInt32();
                    sections[i].VirtualAddress = rdr.ReadUInt32();
                    uint rawDataSize = rdr.ReadUInt32();
                    sections[i].FileOffset = rdr.ReadUInt32();
                    if ( sections[i].VirtualSize == 0 )
                        sections[i].VirtualSize = rawDataSize;

                    fs.Position += 16;
                }

                if (IsDotNetFile)
                {
                    uint rva = DataDirectoryRva(14);
                    if (rva != 0)
                    {
                        fs.Position = RvaToLfa(rva) + 8;
                        uint metadata = rdr.ReadUInt32();
                        fs.Position = RvaToLfa(metadata);
                        if (rdr.ReadUInt32() == 0x424a5342)
                        {
                            // Copy string representing runtime version
                            fs.Position += 12;
                            StringBuilder sb = new StringBuilder();
                            char c;
                            while ((c = rdr.ReadChar()) != '\0')
                                sb.Append(c);

                            if (sb[0] == 'v') // Last sanity check
                                ImageRuntimeVersion = sb.ToString();

                            // Could do fixups here for bad values in older files
                            // like 1.x86, 1.build, etc. But we are only using
                            // the major version anyway

                            // Jump back and find the CorFlags
                            fs.Position = RvaToLfa(rva) + 16;
                            corFlags = (CorFlags)rdr.ReadUInt32();
                        }
                    }
                }
            }
        }

        private uint DataDirectoryRva( int n )
        {
            fs.Position = dataDirectory + n * 8;
            return rdr.ReadUInt32();
        }

        private uint RvaToLfa(uint rva)
        {
            foreach (var section in sections)
                if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.VirtualSize)
                    return rva - section.VirtualAddress + section.FileOffset;

            return 0;
        }

        public string AssemblyPath
        {
            get { return assemblyPath; }
        }

        public bool IsValidPeFile
        {
            get { return dos_magic == 0x5a4d && pe_signature == 0x00004550; }
        }

        public bool IsDotNetFile
        {
            get { return IsValidPeFile && numDataDirectoryEntries > 14 && DataDirectoryRva(14) != 0; }
        }

        /// <summary>
        /// Will return true if the assembly is 32 bit, or if it prefers to run 32-bit
        /// </summary>
        public bool ShouldRun32Bit
        {
            get
            {
                // C++/CLI
                if ((corFlags & CorFlags.COMIMAGE_FLAGS_NATIVE_ENTRYPOINT) != 0)
                    return peType != PEType.PE32Plus;

                return peType != PEType.PE32Plus && (corFlags & CorFlags.COMIMAGE_FLAGS_32BITREQUIRED) != 0;
            }
        }

        public string ImageRuntimeVersion { get; private set; }

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
