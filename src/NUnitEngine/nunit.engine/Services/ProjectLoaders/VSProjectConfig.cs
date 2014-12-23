// ***********************************************************************
// Copyright (c) 2002-2014 Charlie Poole
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
using System.Collections.Specialized;

namespace NUnit.Engine.Services.ProjectLoaders
{
	/// <summary>
	/// Originally, we used the same ProjectConfig class for both
	/// NUnit and Visual Studio projects. Since we really do very
	/// little with VS Projects, this class has been created to 
	/// hold the name and the collection of assembly paths.
	/// </summary>
	public class VSProjectConfig
	{
		private string name;
		
		private StringCollection assemblies = new StringCollection();

		public VSProjectConfig( string name )
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public StringCollection Assemblies
		{
			get { return assemblies; }
		}
	}
}
