// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// OSPlatform represents a particular operating system platform
    /// </summary>
    // This class invokes security critical P/Invoke and 'System.Runtime.InteropServices.Marshal' methods.
    // Callers of this method have no influence on how these methods are used so we define a 'SecuritySafeCriticalAttribute'
    // rather than a 'SecurityCriticalAttribute' to enable use by security transparent callers.
    [SecuritySafeCritical]
    public class OSPlatform
    {
        #region Static Members
        private static readonly Lazy<OSPlatform> currentPlatform = new Lazy<OSPlatform> (() =>
        {
            OSPlatform currentPlatform;

            OperatingSystem os = Environment.OSVersion;

            if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 5)
            {
                if (os.Version.Major == 6 && os.Version.Minor >= 2)
                    os = new OperatingSystem(os.Platform, GetWindows81PlusVersion(os.Version));
#if NETSTANDARD2_0
                ProductType productType = GetProductType();
                currentPlatform = new OSPlatform(os.Platform, os.Version, productType);
#else
                OSVERSIONINFOEX osvi = new OSVERSIONINFOEX();
                osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
                GetVersionEx(ref osvi);
                currentPlatform = new OSPlatform(os.Platform, os.Version, (ProductType)osvi.ProductType);
#endif
            }
            else if (CheckIfIsMacOSX(os.Platform))
            {
                // Mono returns PlatformID.Unix for OSX (see https://www.mono-project.com/docs/faq/technical/#how-to-detect-the-execution-platform)
                // The above check uses uname to confirm it is MacOSX and we change the PlatformId here.
                currentPlatform = new OSPlatform(PlatformID.MacOSX, os.Version);
            }
            else
                currentPlatform = new OSPlatform(os.Platform, os.Version);

            return currentPlatform;
        });


        /// <summary>
        /// Platform ID for Unix as defined by .NET
        /// </summary>
        public static readonly PlatformID UnixPlatformID_Microsoft = (PlatformID)4;

        /// <summary>
        /// Platform ID for Unix as defined by Mono
        /// </summary>
        public static readonly PlatformID UnixPlatformID_Mono = (PlatformID)128;

        /// <summary>
        /// Platform ID for XBox as defined by .NET and Mono
        /// </summary>
        public static readonly PlatformID XBoxPlatformID = (PlatformID)5;

        /// <summary>
        /// Platform ID for MacOSX as defined by .NET and Mono
        /// </summary>
        public static readonly PlatformID MacOSXPlatformID = (PlatformID)6;

        /// <summary>
        /// Get the OSPlatform under which we are currently running
        /// </summary>
        public static OSPlatform CurrentPlatform
        {
            get
            {
                return currentPlatform.Value;
            }
        }
        #endregion

        #region Members used for Win32NT platform only

        /// <summary>
        /// Gets the actual OS Version, not the incorrect value that might be
        /// returned for Win 8.1 and Win 10
        /// </summary>
        /// <remarks>
        /// If an application is not manifested as Windows 8.1 or Windows 10,
        /// the version returned from Environment.OSVersion will not be 6.3 and 10.0
        /// respectively, but will be 6.2 and 6.3. The correct value can be found in
        /// the registry.
        /// </remarks>
        /// <param name="version">The original version</param>
        /// <returns>The correct OS version</returns>
        private static Version GetWindows81PlusVersion(Version version)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        var buildStr = key.GetValue("CurrentBuildNumber") as string;
                        int.TryParse(buildStr, out var build);

                        // These two keys are in Windows 10 only and are DWORDS
                        var major = key.GetValue("CurrentMajorVersionNumber") as int?;
                        var minor = key.GetValue("CurrentMinorVersionNumber") as int?;
                        if (major.HasValue && minor.HasValue)
                        {
                            return new Version(major.Value, minor.Value, build);
                        }

                        // If we get here, we are not Windows 10, so we are Windows 8
                        // or 8.1. 8.1 might report itself as 6.2, but will have 6.3
                        // in the registry. We can't do this earlier because for backwards
                        // compatibility, Windows 10 also has 6.3 for this key.
                        var currentVersion = key.GetValue("CurrentVersion") as string;
                        if(currentVersion == "6.3")
                        {
                            return new Version(6, 3, build);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return version;
        }

        /// <summary>
        /// Product Type Enumeration used for Windows
        /// </summary>
        public enum ProductType
        {
            /// <summary>
            /// Product type is unknown or unspecified
            /// </summary>
            Unknown,

            /// <summary>
            /// Product type is Workstation
            /// </summary>
            WorkStation,

            /// <summary>
            /// Product type is Domain Controller
            /// </summary>
            DomainController,

            /// <summary>
            /// Product type is Server
            /// </summary>
            Server,
        }

#if NETSTANDARD2_0
        private static ProductType GetProductType()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        var installationType = key.GetValue("InstallationType") as string;
                        switch(installationType)
                        {
                            case "Client":
                                return ProductType.WorkStation;
                            case "Server":
                            case "Server Core":
                                return ProductType.Server;
                            default:
                                return ProductType.Unknown;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return ProductType.Unknown;
        }
#else
        [StructLayout(LayoutKind.Sequential)]
        struct OSVERSIONINFOEX
        {
#pragma warning disable IDE1006 // P/invoke doesn’t need to follow naming convention
            public uint dwOSVersionInfoSize;
            public readonly uint dwMajorVersion;
            public readonly uint dwMinorVersion;
            public readonly uint dwBuildNumber;
            public readonly uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public readonly string szCSDVersion;
            public readonly Int16 wServicePackMajor;
            public readonly Int16 wServicePackMinor;
            public readonly Int16 wSuiteMask;
#pragma warning restore IDE1006
            public readonly Byte ProductType;
            public readonly Byte Reserved;
        }

        [DllImport("Kernel32.dll")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX osvi);
#endif
#endregion

        /// <summary>
        /// Construct from a platform ID and version
        /// </summary>
        public OSPlatform(PlatformID platform, Version version)
        {
            Platform = platform;
            Version = version;
        }

        /// <summary>
        /// Construct from a platform ID, version and product type
        /// </summary>
        public OSPlatform(PlatformID platform, Version version, ProductType product)
            : this( platform, version )
        {
            Product = product;
        }

        /// <summary>
        /// Get the platform ID of this instance
        /// </summary>
        public PlatformID Platform { get; }

        /// <summary>
        /// Implemented to use in place of Environment.OSVersion.ToString()
        /// </summary>
        /// <returns>A representation of the platform ID and version in an approximation of the format used by Environment.OSVersion.ToString()</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            switch (Platform)
            {
                case PlatformID.Win32NT:
                    sb.Append("Microsoft Windows NT");
                    break;
                case PlatformID.Win32Windows:
                    sb.Append("Microsoft Windows 95/98");
                    break;
                case PlatformID.Win32S:
                    sb.Append("Microsoft Windows Win32s");
                    break;
                case PlatformID.WinCE:
                    sb.Append("Microsoft Windows CE");
                    break;
                default:
                    sb.Append(Platform);
                    break;
            }

            sb.Append(" ").Append(Version);
            return sb.ToString();
        }

        /// <summary>
        /// Get the Version of this instance
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Get the Product Type of this instance
        /// </summary>
        public ProductType Product { get; }

        /// <summary>
        /// Return true if this is a windows platform
        /// </summary>
        public bool IsWindows
        {
            get
            {
                return Platform is PlatformID.Win32NT or PlatformID.Win32Windows or PlatformID.Win32S or PlatformID.WinCE;
            }
        }

        /// <summary>
        /// Return true if this is a Unix or Linux platform
        /// </summary>
        public bool IsUnix
        {
            get
            {
                return Platform == UnixPlatformID_Microsoft
                    || Platform == UnixPlatformID_Mono;
            }
        }

        /// <summary>
        /// Return true if the platform is Win32S
        /// </summary>
        public bool IsWin32S
        {
            get { return Platform == PlatformID.Win32S; }
        }

        /// <summary>
        /// Return true if the platform is Win32Windows
        /// </summary>
        public bool IsWin32Windows
        {
            get { return Platform == PlatformID.Win32Windows; }
        }

        /// <summary>
        ///  Return true if the platform is Win32NT
        /// </summary>
        public bool IsWin32NT
        {
            get { return Platform == PlatformID.Win32NT; }
        }

        /// <summary>
        /// Return true if the platform is Windows CE
        /// </summary>
        public bool IsWinCE
        {
            get { return Platform == PlatformID.WinCE; }
        }

        /// <summary>
        /// Return true if the platform is Xbox
        /// </summary>
        public bool IsXbox
        {
            get { return Platform == XBoxPlatformID; }
        }

        /// <summary>
        /// Return true if the platform is MacOSX
        /// </summary>
        public bool IsMacOSX
        {
            get { return Platform == MacOSXPlatformID; }
        }

        static int UnameSafe(IntPtr buf)
        {
            try
            {
                return uname(buf);
            }
            catch (DllNotFoundException)
            {
                // https://github.com/nunit/nunit/issues/3608
                return -1;
            }
        }

        [DllImport("libc")]
#pragma warning disable IDE1006 // P/invoke doesn’t need to follow naming convention
        static extern int uname(IntPtr buf);
#pragma warning restore IDE1006

        static bool CheckIfIsMacOSX(PlatformID platform)
        {
            if (platform == PlatformID.MacOSX)
                return true;

            if (platform != PlatformID.Unix)
                return false;

            IntPtr buf = Marshal.AllocHGlobal(8192);
            bool isMacOSX = false;
            if (UnameSafe(buf) == 0)
            {
                string os = Marshal.PtrToStringAnsi(buf);
                isMacOSX = os.Equals("Darwin");
            }
            Marshal.FreeHGlobal(buf);
            return isMacOSX;
        }

        /// <summary>
        /// Return true if the platform is Windows 95
        /// </summary>
        public bool IsWin95
        {
            get { return Platform == PlatformID.Win32Windows && Version.Major == 4 && Version.Minor == 0; }
        }

        /// <summary>
        /// Return true if the platform is Windows 98
        /// </summary>
        public bool IsWin98
        {
            get { return Platform == PlatformID.Win32Windows && Version.Major == 4 && Version.Minor == 10; }
        }

        /// <summary>
        /// Return true if the platform is Windows ME
        /// </summary>
        public bool IsWinME
        {
            get { return Platform == PlatformID.Win32Windows && Version.Major == 4 && Version.Minor == 90; }
        }

        /// <summary>
        /// Return true if the platform is NT 3
        /// </summary>
        public bool IsNT3
        {
            get { return Platform == PlatformID.Win32NT && Version.Major == 3; }
        }

        /// <summary>
        /// Return true if the platform is NT 4
        /// </summary>
        public bool IsNT4
        {
            get { return Platform == PlatformID.Win32NT && Version.Major == 4; }
        }

        /// <summary>
        /// Return true if the platform is NT 5
        /// </summary>
        public bool IsNT5
        {
            get { return Platform == PlatformID.Win32NT && Version.Major == 5; }
        }

        /// <summary>
        /// Return true if the platform is Windows 2000
        /// </summary>
        public bool IsWin2K
        {
            get { return IsNT5 && Version.Minor == 0; }
        }

        /// <summary>
        /// Return true if the platform is Windows XP
        /// </summary>
        public bool IsWinXP
        {
            get { return IsNT5 && (Version.Minor == 1  || Version.Minor == 2 && Product == ProductType.WorkStation); }
        }

        /// <summary>
        /// Return true if the platform is Windows 2003 Server
        /// </summary>
        public bool IsWin2003Server
        {
            get { return IsNT5 && Version.Minor == 2 && Product == ProductType.Server; }
        }

        /// <summary>
        /// Return true if the platform is NT 6
        /// </summary>
        public bool IsNT6
        {
            get { return Platform == PlatformID.Win32NT && Version.Major == 6; }
        }

        /// <summary>
        /// Return true if the platform is NT 6.0
        /// </summary>
        public bool IsNT60
        {
            get { return IsNT6 && Version.Minor == 0; }
        }

        /// <summary>
        /// Return true if the platform is NT 6.1
        /// </summary>
        public bool IsNT61
        {
            get { return IsNT6 && Version.Minor == 1; }
        }

        /// <summary>
        /// Return true if the platform is NT 6.2
        /// </summary>
        public bool IsNT62
        {
            get { return IsNT6 && Version.Minor == 2; }
        }

        /// <summary>
        /// Return true if the platform is NT 6.3
        /// </summary>
        public bool IsNT63
        {
            get { return IsNT6 && Version.Minor == 3; }
        }

        /// <summary>
        /// Return true if the platform is Vista
        /// </summary>
        public bool IsVista
        {
            get { return IsNT60 && Product == ProductType.WorkStation; }
        }

        /// <summary>
        /// Return true if the platform is Windows 2008 Server (original or R2)
        /// </summary>
        public bool IsWin2008Server
        {
            get { return IsWin2008ServerR1 || IsWin2008ServerR2; }
        }

        /// <summary>
        /// Return true if the platform is Windows 2008 Server (original)
        /// </summary>
        public bool IsWin2008ServerR1
        {
            get { return IsNT60 && Product == ProductType.Server; }
        }

        /// <summary>
        /// Return true if the platform is Windows 2008 Server R2
        /// </summary>
        public bool IsWin2008ServerR2
        {
            get { return IsNT61 && Product == ProductType.Server; }
        }

        /// <summary>
        /// Return true if the platform is Windows 2012 Server (original or R2)
        /// </summary>
        public bool IsWin2012Server
        {
            get { return IsWin2012ServerR1 || IsWin2012ServerR2; }
        }

        /// <summary>
        /// Return true if the platform is Windows 2012 Server (original)
        /// </summary>
        public bool IsWin2012ServerR1
        {
            get { return IsNT62 && Product == ProductType.Server; }
        }

        /// <summary>
        /// Return true if the platform is Windows 2012 Server R2
        /// </summary>
        public bool IsWin2012ServerR2
        {
            get { return IsNT63 && Product == ProductType.Server; }
        }

        /// <summary>
        /// Return true if the platform is Windows 7
        /// </summary>
        public bool IsWindows7
        {
            get { return IsNT61 && Product == ProductType.WorkStation; }
        }

        /// <summary>
        /// Return true if the platform is Windows 8
        /// </summary>
        public bool IsWindows8
        {
            get { return IsNT62 && Product == ProductType.WorkStation; }
        }

        /// <summary>
        /// Return true if the platform is Windows 8.1
        /// </summary>
        public bool IsWindows81
        {
            get { return IsNT63 && Product == ProductType.WorkStation; }
        }

        /// <summary>
        /// Return true if the platform is Windows 10
        /// </summary>
        public bool IsWindows10
        {
            get { return Platform == PlatformID.Win32NT && Version.Major == 10 && Product == ProductType.WorkStation; }
        }

        /// <summary>
        /// Return true if the platform is Windows Server. This is named Windows
        /// Server 10 to distinguish it from previous versions of Windows Server.
        /// </summary>
        public bool IsWindowsServer10
        {
            get { return Platform == PlatformID.Win32NT && Version.Major == 10 && Product == ProductType.Server; }
        }
    }
}
