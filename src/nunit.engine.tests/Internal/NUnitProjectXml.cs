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
using System.IO;

namespace NUnit.Engine.Internal.Tests
{
	/// <summary>
	/// Summary description for NUnitProjectXml.
	/// </summary>
	public class NUnitProjectXml
	{
		public static readonly string EmptyProject = "<NUnitProject />";
		
		public static readonly string EmptyConfigs = 
			"<NUnitProject>" + System.Environment.NewLine +
			"  <Settings activeconfig=\"Debug\" />" + System.Environment.NewLine +
			"  <Config name=\"Debug\" binpathtype=\"Auto\" />" + System.Environment.NewLine +
			"  <Config name=\"Release\" binpathtype=\"Auto\" />" + System.Environment.NewLine +
			"</NUnitProject>";
		
		public static readonly string NormalProject =
			"<NUnitProject>" + System.Environment.NewLine +
			"  <Settings activeconfig=\"Debug\" />" + System.Environment.NewLine +
			"  <Config name=\"Debug\" appbase=\"bin" + Path.DirectorySeparatorChar + "debug\" binpathtype=\"Auto\">" + System.Environment.NewLine +
			"    <assembly path=\"assembly1.dll\" />" + System.Environment.NewLine +
			"    <assembly path=\"assembly2.dll\" />" + System.Environment.NewLine +
			"  </Config>" + System.Environment.NewLine +
			"  <Config name=\"Release\" appbase=\"bin" + Path.DirectorySeparatorChar + "release\" binpathtype=\"Auto\">" + System.Environment.NewLine +
			"    <assembly path=\"assembly1.dll\" />" + System.Environment.NewLine +
			"    <assembly path=\"assembly2.dll\" />" + System.Environment.NewLine +
			"  </Config>" + System.Environment.NewLine +
			"</NUnitProject>";
		
		public static readonly string ManualBinPathProject =
			"<NUnitProject>" + System.Environment.NewLine +
			"  <Settings activeconfig=\"Debug\" />" + System.Environment.NewLine +
			"  <Config name=\"Debug\" binpath=\"bin_path_value\"  /> " + System.Environment.NewLine +
			"</NUnitProject>";

        public static readonly string ComplexSettingsProject =
        "<NUnitProject>" + System.Environment.NewLine +
        "  <Settings activeconfig=\"Release\" appbase=\"bin\" processModel=\"Separate\" domainUsage=\"Multiple\" />" + System.Environment.NewLine +
        "  <Config name=\"Debug\" appbase=\"debug\" binpathtype=\"Auto\" runtimeFramework=\"v2.0\">" + System.Environment.NewLine +
        "    <assembly path=\"assembly1.dll\" />" + System.Environment.NewLine +
        "    <assembly path=\"assembly2.dll\" />" + System.Environment.NewLine +
        "  </Config>" + System.Environment.NewLine +
        "  <Config name=\"Release\" appbase=\"release\" binpathtype=\"Auto\" runtimeFramework=\"v4.0\">" + System.Environment.NewLine +
        "    <assembly path=\"assembly1.dll\" />" + System.Environment.NewLine +
        "    <assembly path=\"assembly2.dll\" />" + System.Environment.NewLine +
        "  </Config>" + System.Environment.NewLine +
        "</NUnitProject>";
    }
}
