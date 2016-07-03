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
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace NUnit.Framework
{

    /// <summary>
    /// Database Connection
    /// </summary>
    public interface INUnitDatabaseConnection
    {

        /// <summary>
        /// Performs query and returns set of results
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        IEnumerable Query(string qry);


        /// <summary>
        /// The name of the driver as specified in the connection string
        /// </summary>
        string DatabaseDriverName { get; }
    }

    /// <summary>
    /// Maps the various database vendors to their appropriate .NET driver
    /// </summary>
    public class DatabaseConnectionType
    {

        /// <summary>
        /// The mapping of the various database types to the local database driver in .NET
        /// </summary>
        IList<string> _drivers = null;

        /// <summary>
        /// The numeric ID of the driver type
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The database driver type
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Identification of symbol</param>
        /// <param name="driverName">Name of the driver</param>
        /// <param name="dbNames">List of string drivers used for mapping different driver types.</param>
        private DatabaseConnectionType(int id, string driverName, IList<string> dbNames)
        {
            Id = id;
            Name = driverName;
            _drivers = dbNames;
        }

        /// <summary>
        /// No type specified 
        /// </summary>
        public static DatabaseConnectionType None = new DatabaseConnectionType(-1, "None", null);

        /// <summary>
        /// Generic .NET DataProvider (SqlClient)
        /// </summary>
        public static DatabaseConnectionType Generic = new DatabaseConnectionType(0, "Generic", new List<string>() { "SQLExpress" });

        /// <summary>
        /// ODBC connection
        /// </summary>
        public static DatabaseConnectionType ODBC = new DatabaseConnectionType(1, "ODBC", new List<string>() { "SQL Server", "Microsoft Access Driver", "Microsoft ODBC Driver for Oracle", "Microsoft ODBC for Oracle", "Oracle ODBC Driver", "IBM DB2 ODBC DRIVER", "MySql", "MySql ODBC 3.51 Driver", "Microsoft Excel Driver", "Microsoft Text Driver" });

        /// <summary>
        /// OLE connection
        /// </summary>
        public static DatabaseConnectionType OLE = new DatabaseConnectionType(2, "OLE", new List<string>() { "SQLOLEDB", "MSDAORA", "DB2OLEDB", "MySqlProv" });

        /// <summary>
        /// Checks a connection string for the appropriate driver type
        /// </summary>
        /// <param name="connString"></param>
        /// <returns>Tuple of [bool,string], one verifying the driver type, the second the name of the specific driver</returns>
        public Tuple<bool, string> Verify(string connString)
        {
            if (_drivers == null)
                return new Tuple<bool, string>(false, string.Empty);

            foreach (var dvr in _drivers)
            {
                if (connString.Contains(dvr))
                {
                    return new Tuple<bool, string>(true, dvr);

                }
            }
            return new Tuple<bool, string>(false, string.Empty);
        }

    }


    /// <summary>
    /// Database Connection
    /// </summary>
    public class NUnitDatabaseConnection : INUnitDatabaseConnection
    {

        DatabaseConnectionType _connType = DatabaseConnectionType.None;
        string _connStr = string.Empty;

        /// <summary>
        /// The name of the driver as specified in the connection string
        /// </summary>
        [DefaultValue("")]
        public string DatabaseDriverName { get; private set; }

        /// <summary>
        /// The driver connection type
        /// </summary>
        public DatabaseConnectionType ConnectionType
        {
            get
            {
                return _connType;
            }
            private set
            {
                _connType = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connStr">connection string</param>
        private NUnitDatabaseConnection(string connStr)
        {
            this._connStr = connStr;

            var result = DatabaseConnectionType.OLE.Verify(connStr);
            if (result.Item1)
            {
                _connType = DatabaseConnectionType.OLE;
                DatabaseDriverName = result.Item2;
                return;
            }

            result = DatabaseConnectionType.ODBC.Verify(connStr);
            if (result.Item1)
            {
                _connType = DatabaseConnectionType.ODBC;
                DatabaseDriverName = result.Item2;
                return;
            }

            _connType = DatabaseConnectionType.Generic;
            DatabaseDriverName = string.Empty;
        }

        /// <summary>
        /// Factory to create Database connection based on the entered string
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns>implementation of INUnitDatabaseConnection</returns>
        public static INUnitDatabaseConnection Create(string connStr)
        {
            return new NUnitDatabaseConnection(connStr);
        }

        /// <summary>
        /// Executes the query on the database
        /// </summary>
        /// <param name="query"></param>
        /// <returns>List of row data</returns>
        public IEnumerable Query(string query)
        {

            if (_connType.Id == DatabaseConnectionType.Generic.Id)
            {
                return ClientQuery<SqlCommand, SqlConnection>(_connStr, query);
            }

            if (_connType.Id == DatabaseConnectionType.ODBC.Id)
            {
                return ClientQuery<OdbcCommand, OdbcConnection>(_connStr, query);
            }

            if (_connType.Id == DatabaseConnectionType.OLE.Id)
            {
                return ClientQuery<OleDbCommand, OleDbConnection>(_connStr, query);
            }

            return null;
        }


        /// <summary>
        /// Executes the query 
        /// </summary>
        /// <typeparam name="T">IDbCommand</typeparam>
        /// <typeparam name="W">IDbConnection</typeparam>
        /// <param name="connStr">connection string</param>
        /// <param name="query">SQL query</param>
        /// <returns>list of rows</returns>
        private IEnumerable ClientQuery<T, W>(string connStr, string query)
            where T : IDbCommand, new()
            where W : IDbConnection, new()
        {

            using (var conn = new W())
            {
                conn.ConnectionString = connStr;
                conn.Open();

                var cmd = new T();
                cmd.CommandText = query;
                cmd.Connection = conn;

                var reader = cmd.ExecuteReader();
                var rowData = new object[reader.FieldCount];

                while (reader.Read() && !reader.IsClosed)
                {
                    reader.GetValues(rowData);
                    yield return rowData;
                }

            }
        }


    }


    /// <summary>
    /// Import Data from a Database resource
    /// </summary>
    public class DatabaseData : DataSource
    {

        /// <summary>
        /// List of column names to fetch from the database
        /// </summary>
        /// <remarks>List order matters as this list is used in the database query to isolate specific columns in the table.</remarks>
        [DefaultValue(null)]
        public string[] Columns { get; private set; }

        /// <summary>
        /// Table from which to fetch data
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// The database connection
        /// </summary>
        [DefaultValue(null)]
        public INUnitDatabaseConnection Connection { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The connection string</param>
        /// <param name="tableName">Table to retrieve data</param>
        /// <param name="columns">list of columns to fetch from table. It must equal the number of parameters in the method</param>
        public DatabaseData(string source, string tableName, string columns)
        {
            if (source == string.Empty)
                throw new ArgumentException("Connection sting cannot be empty", "Name");

            if (tableName == string.Empty || tableName == null)
                throw new ArgumentException("Table name must be specified must be specified.", "TableName");

            if (columns == null || columns == string.Empty)
                throw new ArgumentException("Columns for tables must be specified.", "Columns");

            Name = source;
            TableName = tableName;
            Columns = columns.Split(',');

        }


        /// <summary>
        /// Obtains data from source
        /// </summary>
        /// <returns>list of vectors by which various test case scenarios will be created</returns>
        public override IEnumerable GetData()
        {

            // open file and check that dimensions match that of parameters
            IList<object[]> testData = new List<object[]>();

            var conn = Connection ?? NUnitDatabaseConnection.Create(Name);


            /*
                The generic query following the SQL standard. Other queries for databases that don't 
                follow the standard are overridden once the particular DB is identified.

                The query is constructed based on the columns names specified. If a specific row count is
                specified, we apply the SQL standard function FETCH FIRST.

                Unfortunately, many popular databases do not implement this standard and thus some arbitration 
                is required. The DatabaseConnectionType contains a set of database vendors for a given driver. 
            */

            string targetColumns = string.Join(",", Columns);
            string standardQuery = string.Format("SELECT {0} From [{1}]", targetColumns, TableName);


            string vendor = conn.DatabaseDriverName;
            if (vendor.Contains("Microsoft Excel Driver"))
            {
                standardQuery = standardQuery.Replace("]", "$]");
            }


            if (RowsToRead > 0)
            {
                if (vendor.Contains("MySql"))
                {
                    standardQuery += string.Format(" LIMIT {0}", RowsToRead);
                }
                else if (vendor.Contains("SQL Server") || vendor.Contains("Microsoft Excel Driver") || vendor.Contains("Microsoft Access Database"))
                {
                    standardQuery.Insert(standardQuery.IndexOf(" "), string.Format(" TOP {0} ", RowsToRead));
                }
                else if (vendor.Contains("Microsoft ODBC Driver for Oracle") || vendor.Contains("Oracle ODBC Driver"))
                {
                    standardQuery = string.Format("SELECT * FROM ( {0} ) WHERE rownum <= {1}", standardQuery, RowsToRead);
                }
                else
                {
                    standardQuery += string.Format(" FETCH FIRST {0} ROWS ONLY", RowsToRead);
                }
            }

            foreach (object[] row in conn.Query(standardQuery))
            {
                testData.Add(row);
            }

            return testData;
        }

    }
}
