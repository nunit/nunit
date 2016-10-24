// ***********************************************************************
// Copyright (c) 2009-2015 Charlie Poole
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
using System.Collections;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Diagnostics;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class DataSourceTests
    {
        [OneTimeTearDown]
        public void CleanUp()
        {
            ArrayList testFiles = new ArrayList();

            testFiles.Add("TestCsvFile1.csv");
            testFiles.Add("TestCsvFile2.csv");
            testFiles.Add("TestCsvFile3.csv");
            testFiles.Add("TestCsvFile4.csv");
            testFiles.Add("TestCsvFile5.csv");
            testFiles.Add("TestCsvFile6.csv");
            testFiles.Add("TestCsvFile7.csv");
            testFiles.Add("TestXlsFile.xls");
            testFiles.Add("TestXmlFile1.xml");
            testFiles.Add("TestXmlFile2.xml");
            testFiles.Add("TestXmlFile4.xml");
            testFiles.Add("TestXmlFile5.xml");

            foreach (string f in testFiles)
            {
                File.Delete(f);
            }
        }

        #region General

        [Description("Verify that non-existant filename source providers are gracefully captured.")]
        [TestCase(typeof(CsvDataAttribute), "blah")]
        [TestCase(typeof(XmlDataAttribute), "blah")]
        public void SourceMayNotExist(Type dataSourceType, string filename)
        {
            try
            {
                var instance = Activator.CreateInstance(dataSourceType, new object[] { filename });
                Debug.WriteLine(instance.ToString());
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf(typeof(FileNotFoundException), e);
            }
        }

        [Description("Cannot set number of records to read as negative number")]
        [TestCase(@"TestCsvFile1.csv")]
        public void CannotSetRowsToReadLessThanZero(string source)
        {
            var csvData = new CsvData(source);
            Assert.Throws(typeof(ArgumentException), () => { csvData.RowsToRead = -1; });
        }

        [Test]
        [TestCase(typeof(CsvDataAttribute), "TestCsvFile4.csv")]
        [TestCase(typeof(XmlDataAttribute), "TestXmlFile4.xml")]
        public void CastingError(Type dataSourceType, string source)
        {
            try
            {
                var instance = Activator.CreateInstance(dataSourceType, new string[] { source });
                Debug.WriteLine(instance.ToString());
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf(typeof(InvalidCastException), e);
            }
        }

        #endregion

        #region CSV

        [Description("I can read all rows in a file")]
        [TestCase(@"TestCsvFile1.csv")]
        public void CsvOpenSourceReadAllRows(string source)
        {
            var csvData = new CsvData(source);
            IEnumerable data = csvData.GetData();
            Assert.AreEqual(3, ((IList<object[]>)data).Count);
        }

        [Description("I can read all rows in a file despite there being an empty row in the file")]
        [TestCase(@"TestCsvFile6.csv")]
        public void CsvOpenSourceReadAllRowsSkipEmptyLines(string source)
        {
            var csvData = new CsvData(source);
            IEnumerable data = csvData.GetData();
            Assert.AreEqual(3, ((IList<object[]>)data).Count);
        }

        [Description("Only read the selected row count")]
        [TestCase(@"TestCsvFile1.csv", 2)]
        public void CsvOpenSourceReadSelectedRows(string source, int rows)
        {
            var csvData = new CsvData(source);
            csvData.RowsToRead = rows;
            IEnumerable data = csvData.GetData();
            Assert.AreEqual(rows, ((IList<object[]>)data).Count);
        }

        [Description("Parse a CSV with a different delimiter than default (comma).")]
        [CsvData(@"TestCsvFile7.csv", Delimiter = ":")]
        public void CsvOpenSourceParseWithNonDefaultDelimiter(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        [Test]
        [Description("Read a file with quoted and non-quoted data.")]
        [CsvData(@"TestCsvFile3.csv")]
        public void CsvQuotedAndNonQuotedContent(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        #endregion

        #region XML

        [Description("I can read all rows in a file")]
        [TestCase(@"TestXmlFile1.xml")]
        public void XmlOpenSourceReadAllRows(string source)
        {
            var xmlData = new XmlData(source);
            IEnumerable data = xmlData.GetData();
            Assert.AreEqual(3, ((IList<object[]>)data).Count);
        }

        [Description("Only read the selected row count")]
        [TestCase(@"TestXmlFile1.xml", 2)]
        public void XmlOpenSourceReadSelectedRows(string source, int rows)
        {
            var xmlData = new XmlData(source);
            xmlData.RowsToRead = rows;
            IEnumerable data = xmlData.GetData();
            Assert.AreEqual(rows, ((IList<object[]>)data).Count);
        }

        [Test]
        [XmlData(@"TestXmlFile1.xml")]
        public void XmlReadAllRows(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        #endregion

        #region Database

        [Description("I can read all rows in a file")]
        [TestCase("source", null, null, typeof(ArgumentException))]
        [TestCase("source", "table", "", typeof(ArgumentException))]
        [TestCase("", "table", "", typeof(ArgumentException))]
        [TestCase(null, "table", "", typeof(ArgumentException))]
        [TestCase("source", "table", null, typeof(ArgumentException))]
        public void DatabaseThrowExceptionForInvalidConstructorParams(string source, string table, string columns, Type expectedException)
        {
            try
            {
                var dbData = new DatabaseData(source, table, columns);
                Debug.WriteLine(dbData);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf(expectedException, e);
            }
        }

        private class TestDatabaseConnectionStub1 : INUnitDatabaseConnection
        {
            public TestDatabaseConnectionStub1(int expectedRowsCount)
            {
                ReturnRowCount = expectedRowsCount;
            }

            public string DatabaseDriverName
            {
                get
                {
                    return "TestDatabase Driver";
                }
            }

            public int ReturnRowCount { get; private set; }

            public IEnumerable Query(string s)
            {

                if (ReturnRowCount == 3)
                {
                    return new List<object[]>()
                 {
                     new object[] { 1, 2, 3 },
                     new object[] { 4, 5, 6 },
                     new object[] { 7, 8, 9 }
                 };
                }
                else if (ReturnRowCount == 1)
                {
                    return new List<object[]>()
                 {
                     new object[] { 1, 2, 3 }
                 };
                }

                return new List<object[]>() { };
            }
        }

        [Description("Should return the correct number of rows after executing a query against a known database")]
        [TestCase(3)]
        [TestCase(1)]
        [TestCase(0)]
        public void DatabaseConstructRowCollectionAfterExecutingQuery(int expectedRowsCount)
        {
            var dbData = new DatabaseData("source", "table", "c1,c2");
            dbData.Connection = new TestDatabaseConnectionStub1(expectedRowsCount);
            IEnumerable data = dbData.GetData();
            Assert.AreEqual(expectedRowsCount, ((IList<object[]>)data).Count);
        }

        [Test]
        [Ignore("Test is a bit unstable on different windows platforms. Needs more investigation.")]
        [Platform(Include = "Win", Reason = "Driver exists only on windows machines")]
        [DatabaseData("Driver={Microsoft Excel Driver (*.xls)};Driverid=790;Dbq=TestXlsFile.xls;DefaultDir=.;", "Test1", "c1,c2,c3")]
        public void DatabaseSourceMayReturnArgumentsAsObjectArray(int n, int d, int q)
        {
            Assert.AreEqual(q, n / d);
        }

        private class TestDatabaseConnectionStub2 : INUnitDatabaseConnection {

            INUnitDatabaseConnection conn;

            public TestDatabaseConnectionStub2(string connStr)
            {
                conn = NUnitDatabaseConnection.Create(connStr);
            }

            public string DatabaseDriverName
            {
                get { return conn.DatabaseDriverName;  }
            }

            public string DatabaseDriverType
            {
                get
                {
                    return (conn as NUnitDatabaseConnection).ConnectionType.Name;
                }
            }

            public IEnumerable Query(string qry)
            {
                return new List<object[]>(){ new object[] { 1, 2, 3 } };
            }
        }

        [Test]
        [Description("Assure that the proper driver is instantiated based on the connection string")]
        [TestCase("Driver={SQL Server};Server=DataBaseNamex;DataBase=DataBaseName;Uid=UserName;Pwd=Secret;", "ODBC")]
        [TestCase("Driver={SQL Server};Server=ServerName;DataBase=DataBaseName;Trusted_Connection=Yes;","ODBC")]
        [TestCase("Driver=SQLOLEDB;Data Source=ServerName;Initial Catalog=DataBaseName;User id=UserName;Password=Secret;","OLE")]
        [TestCase("Data Source = ServerName;Initial Catalog = DataBaseName;User id = UserName;Password = Secret;", "Generic")]
        [TestCase("Network Library=DBMSSOCN;Data Source=xxx.xxx.xxx.xxx,1433;Initial Catalog=DataBaseName;User Id=UserName;Password=Secret;", "Generic")]
        [TestCase("Data Source=.\\SQLExpress;User Instance=true;User Id=UserName;Password=Secret;AttachDbFilename=|DataDirectory|DataBaseName.mdf;", "Generic")]
        [TestCase(@"Driver={Microsoft Access Driver (*.mdb)};Dbq=c:\myPath\myDb.mdb;Exclusive=1;Uid=Admin;Pwd=;","ODBC")]
        [TestCase("Driver={Microsoft ODBC for Oracle};Server=OracleServer.world;Uid=UserName;Pwd=Secret;", "ODBC")]
        [TestCase(@"Driver={Microsoft Excel Driver (*.xls)};Driverid=790;Dbq=C:\MyPath\SpreadSheet.xls;DefaultDir=C:\MyPath;", "ODBC")]
        [TestCase("Driver={IBM DB2 ODBC DRIVER};DataBase=DataBaseName;HostName=ServerName;Protocol=TCPIP;Port=PortNumber;Uid=UserName;Pwd=Secret;", "ODBC")]
        public void InstantiateProperDriverFromConnectionString(string connString, string dbDriver)
        {
            var dbData = new DatabaseData(connString, "table", "c1,c2,c3");
            dbData.Connection = new TestDatabaseConnectionStub2(connString);
            var data = dbData.GetData();
            Debug.WriteLine(data);
            Assert.AreEqual(dbDriver, (dbData.Connection as TestDatabaseConnectionStub2).DatabaseDriverType);
        }
    
        #endregion
    }
}
