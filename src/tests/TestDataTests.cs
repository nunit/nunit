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
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using NUnit.Framework;
using System.Threading;

namespace NUnit.Framework.Tests
{
  [TestFixture]
  public class TestDataTests
  {
    
    Stopwatch sw = new Stopwatch();
    
    [Test]
    [Description("Create Random GUIDs and report performance")]
    [TestCase(20)]
    [TestCase(500)]
    [TestCase(10000)]
    [TestCase(500000)]
    public void CreateRandomGuidsTest(int trials)
    {
      double totalTime = 0f;
      
      for(int i=0; i < trials; i++)
      {
        sw.Restart();
        var guid = TestData.CreateRandomGuid();
        sw.Stop();
        totalTime += sw.Elapsed.TotalSeconds;
      }
      
      Console.WriteLine("Average time to generate single GUID after {0} trials:\t{1:####0.00000000000} s", trials, totalTime/(double)trials);
      
    }
    
    [Test]
    [Description("Generate a random string of variable length. Uses default characters")]
    [TestCase(20, 100)]
    [TestCase(100, 100)]
    [TestCase(5000, 100)]
    [TestCase(10000, 100)]
    [TestCase(200000, 100)]
    public void CreateRandomStringDefaultCharsTest(int charLength, int trials)
    {
      
      double totalTime = 0f;
      
      for(int i=0; i < trials; i++)
      {
        sw.Restart();
        var str = TestData.CreateRandomString(charLength);
        sw.Stop();
        totalTime += sw.Elapsed.TotalSeconds;
      }
      
      Console.WriteLine("Average time to generate {0} characters after {1} trials:\t{2:####0.0000000000} s", charLength, trials, totalTime/(double)trials);
      
    }
    
    
    [Test]
    [Description("Generate a random string of variable length. Uses non-default characters")]
    [TestCase(20, 100)]
    [TestCase(100, 100)]
    [TestCase(5000, 100)]
    [TestCase(10000, 100)]
    [TestCase(200000, 100)]
    public void CreateRandomStringUnicodeCharsTest(int charLength, int trials)
    {
      
      double totalTime = 0f;
      string nonDefaultChars="Tհè ɋüｉｃｋ ƃｒ߀ｗл ｆòх ｊùｍｐѕ ߀ѵｅｒ ｔɦë ɭâȥϒ Ꮷ߀ǥ";
      
      for(int i=0; i < trials; i++)
      {
        sw.Restart();
        var str = TestData.CreateRandomString(charLength,nonDefaultChars);
        sw.Stop();
        totalTime += sw.Elapsed.TotalSeconds;
      }
      
      Console.WriteLine("Average time to generate {0} characters after {1} trials:\t{2:####0.0000000000} s", charLength, trials, totalTime/(double)trials);
      
    }
    
    [Test]
    [Description("Create a test folder to hold test artifacts.")]
    public void CreateTestDirectoryTest()
    {
      // basic create/delete with verification
      var testDir = TestData.CreateTestDirectory();
      Console.WriteLine("Creating folder {0}", testDir.FullName);
      Assert.True(Directory.Exists(testDir.FullName), String.Format("Folder {0} with random name does not exist when it should.",testDir.Name));
      Console.WriteLine("Removing folder {0}", testDir.FullName);
      testDir.Delete();
      Assert.IsFalse(Directory.Exists(testDir.FullName), "The folder still exists when it should be deleted.");
      
      // create with a custom name
      testDir = TestData.CreateTestDirectory(TestData.CreateRandomString(20));
      Console.WriteLine("Creating folder {0}", testDir.FullName);
      Assert.True(Directory.Exists(testDir.FullName), String.Format("Folder {0} with random name does not exist when it should.",testDir.Name));
      Console.WriteLine("Removing folder {0}", testDir.FullName);
      testDir.Delete();
      Assert.IsFalse(Directory.Exists(testDir.FullName), "The folder still exists when it should be deleted.");
      
      //      // overwrite test folder
      //      string tempName = TestData.CreateRandomString(20);
      //      testDir = TestData.CreateTestDirectory(tempName);
      //      Assert.True(testDir.Exists, String.Format("Folder {0} with random name does not exist when it should.",testDir.Name));
      //      Assert.Throws<Exception>( () => TestData.CreateTestDirectory(tempName), "Attempt to create a folder with the same name should have failed.");
      
    }
    
    [Test]
    [Description("Generate random binary data of variable length.")]
    [TestCase(20, 100)]
    [TestCase(100, 100)]
    [TestCase(5000, 100)]
    [TestCase(10000, 100)]
    [TestCase(200000, 100)]
    public void CreateRandomBinaryDataTest(int dataLengthInBytes, int trials)
    {
      
      double totalTime = 0f;
      
      for(int i=0; i < trials; i++)
      {
        sw.Restart();
        var data = TestData.CreateRandomBinaryData(dataLengthInBytes);
        sw.Stop();
        totalTime += sw.Elapsed.TotalSeconds;
      }
      
      Console.WriteLine("Average time to generate {0} bytes of data after {1} trials:\t{2:####0.0000000000} s", dataLengthInBytes, trials, totalTime/(double)trials);
      
    }
    
    [Test]
    [Description("Generate random binary data files and compare for equality.")]
    [TestCase(128, 100)]
    [TestCase(1024, 100)]
    [TestCase(32768, 100)]
    public void CreateRandomBinaryFileTest(int dataLengthInBytes, int trials)
    {
            
      for(int i=0; i < trials; i++)
      {
        string fileLocation1 = Path.Combine(TestContext.CurrentContext.WorkDirectory, TestData.CreateRandomString());
        string fileLocation2 = Path.Combine(TestContext.CurrentContext.WorkDirectory, TestData.CreateRandomString());
        var data1 = TestData.CreateRandomBinaryFile(dataLengthInBytes, fileLocation1);
        var data2 = TestData.CreateRandomBinaryFile(dataLengthInBytes, fileLocation2);
        
        // For each trial ensure the data generated is always different.
        Assert.IsFalse(TestData.FileContentsEqual(data1.FullName, data2.FullName), "The data is equal when it should not be. Please check generator.");
      
      }
      
    }
    
    
  }
}
