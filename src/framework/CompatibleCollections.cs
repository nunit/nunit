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

using System;

namespace NUnit
{
    /// <summary>
    /// ObjectList represents a collection of objects. It is implemented 
    /// as a List&lt;object&gt; in .NET 2.0 or higher and as an ArrayList otherwise.
    /// ObjectList does not attempt to be a general replacement for either of
    /// these classes but only implements what is needed within the framework.
    /// </summary>
#if CLR_2_0
    public class ObjectList : System.Collections.Generic.List<object>
    {
        public void AddRange(System.Collections.ICollection collection)
        {
            foreach (object item in collection)
                Add(item);
        }
    }
#else
    public class ObjectList : System.Collections.ArrayList { }
#endif

    /// <summary>
    /// ArgumentsCollection represents a collection of object arrrays. It is implemented 
    /// as a List&lt;object[]&gt; in .NET 2.0 or higher and as an ArrayList otherwise.
    /// ObjectArrayList does not attempt to be a general replacement for either of
    /// these classes but only implements what is needed within the framework. It
    /// is primarily used to hold argument lists within the framework.
    /// </summary>
#if CLR_2_0
    class ArgumentsCollection : System.Collections.Generic.List<object[]> { }
#else
    class ArgumentsCollection : System.Collections.ArrayList { }
#endif

    /// <summary>
    /// StringCollection represents a collection of strings. It is implemented as a
    /// List&lt;string&gt; in .NET 2.0 or higher. In .NET 1.x, we base it on a
    /// System.Collections.Specialized.StringCollection.
    /// StringCollection does not attempt to be a general replacement for either of
    /// these classes but only implements what is needed within the framework.
    /// </summary>
#if CLR_2_0x
    public class StringCollection : System.Collections.Generic.List<String> { }
#else
    public class StringCollection : System.Collections.Specialized.StringCollection
    {
        public string[] ToArray()
        {
            string[] result = new string[this.Count];

            int index = 0;
            foreach(string s in this)
                result[index++] = s;

            return result;
        }
    }
#endif
}
