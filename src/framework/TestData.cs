// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Threading;
using System.Text;

namespace NUnit.Framework
{
  /// <summary>
  /// Generators of random data which can be used in testing methods
  /// </summary>
  public static class TestData
  {
    
    /// <summary>
    /// Default characters for random functions.
    /// </summary>
    /// <remarks>Default characters are the English alphabet (uppercase &amp; lowercase), arabic numerals, and underscore</remarks>
    public const string DefaultStringChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789_";

    //private static Random randNum = new Random(unchecked((int)(DateTime.Now.Ticks)));
    
    /// <summary>
    /// Create a test folder for results/artifacts.
    /// </summary>
    /// <param name="directoryName"></param>
    /// <returns><see cref="DirectoryInfo">DirectoryInfo</see> object containing folder information</returns>
    /// <remarks>If no name is specified then a random GUID is created in the system's temp folder</remarks>
    public static DirectoryInfo CreateTestDirectory(string directoryName = null)
    {
      return Directory.CreateDirectory(directoryName ?? Path.Combine(Path.GetTempPath(),CreateRandomGuid()));
    }
    
    /// <summary>
    /// Generate a random Guid
    /// </summary>
    /// <returns>string GUID</returns>
    public static string CreateRandomGuid()
    {
      return Guid.NewGuid().ToString();
    }
    
    /// <summary>
    /// Generate a random string based on the characters from the input string.
    /// </summary>
    /// <param name="outputLength">desired length of output string.</param>
    /// <param name="allowedChars">string representing the set of characters from which to construct the resulting string</param>
    /// <returns>A random string of arbitrary length</returns>
    public static string CreateRandomString(int outputLength, string allowedChars)
    {

      StringBuilder sb = new StringBuilder(outputLength);

      for (int i = 0; i < outputLength ; i++)
      {
        sb.Append(allowedChars[(int)((allowedChars.Length) * TestContext.CurrentContext.Random.GetDouble())]);
      }
      
      return sb.ToString();
    }
    
    /// <summary>
    /// Generate a random string based on the characters from the input string.
    /// </summary>
    /// <param name="outputLength">desired length of output string. The default is 20 characters.</param>
    /// <returns>A random string of arbitrary length</returns>
    /// <remarks>Uses <see cref="DefaultStringChars">DefaultStringChars</see> as the input character set </remarks>
    public static string CreateRandomString(int outputLength=20)
    {
      return CreateRandomString(outputLength, DefaultStringChars);
    }
    
    /// <summary>
    /// Creates a text file with an arbirary number of characters
    /// </summary>
    /// <param name="charLength">length of characters to place in the file</param>
    /// <param name="fileLocation">the location to file</param>
    /// <param name="enc">Encoding to use when generating file</param>
    /// <returns>File information object</returns>
    public static FileInfo CreateRandomTextFile(int charLength, string fileLocation, Encoding enc)
    {
      
      File.WriteAllText(fileLocation, CreateRandomString(charLength), enc);
      
      return new FileInfo(fileLocation);
      
    }
    
    /// <summary>
    /// Creates a text file with an arbirary number of characters
    /// </summary>
    /// <param name="charLength">length of characters to place in the file</param>
    /// <param name="fileLocation">the location to file</param>
    /// <returns>File information object</returns>
    /// <remarks>Files are written with UTF-8 encoding</remarks>
    public static FileInfo CreateRandomTextFile(int charLength, string fileLocation)
    {
      return CreateRandomTextFile(charLength, fileLocation, Encoding.UTF8);
    }
    
    /// <summary>
    /// Create a random array of byte data of arbitrary length
    /// </summary>
    /// <param name="dataLength">the length of the desired data in bytes</param>
    /// <returns>array of bytes</returns>
    public static byte[] CreateRandomBinaryData(int dataLength)
    {
      
      Byte[] buffer = new Byte[dataLength];
      
      for (int i=0; i< buffer.Length; i++)
      {
        buffer[i]=TestContext.CurrentContext.Random.GetByte();
      }
      return buffer;
      
    }

    /// <summary>
    /// Generate a binary file filled with random byte data
    /// </summary>
    /// <param name="dataLength">the length of the desired data in bytes</param>
    /// <param name="fileLocation">path and filename of the desired output file</param>
    /// <returns><see cref="FileInfo">FileInfo</see>object</returns>
    public static FileInfo CreateRandomBinaryFile(int dataLength, string fileLocation)
    {
      byte[] buffer = CreateRandomBinaryData(dataLength);
      
      using (var f = File.Create(fileLocation))
      {
        f.Write(buffer, 0, buffer.Length);
      }
      
      return new FileInfo(fileLocation);
    }
    
    /// <summary>
    /// Determines is the content of two files are the same. The functions goes through each
    /// byte when making the comparison
    /// </summary>
    /// <param name="path1">location of reference file</param>
    /// <param name="path2">location of test file</param>
    /// <returns>TRUE if file contents are equal, FALSE otherwise</returns>
    public static bool FileContentsEqual(string path1, string path2)
    {
      byte[] file1 = File.ReadAllBytes(path1);
      byte[] file2 = File.ReadAllBytes(path2);
      if (file1.Length == file2.Length)
      {
        for (int i = 0; i < file1.Length; i++)
        {
          if (file1[i] != file2[i])
          {
            return false;
          }
        }
        return true;
      }
      return false;
    }
    
  }
}
