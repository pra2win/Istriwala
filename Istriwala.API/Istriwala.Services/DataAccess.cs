using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace Istriwala.Services
{
    /// <summary>
    /// Standard database access method
    /// </summary>
    public class DataAccess
    {
        #region Enums

        /// <summary>
        /// Wrapper enum for app.config connection strings
        /// </summary>
        public enum ConnectionStrings
        {
            Istriwala,
        }

        #endregion Enums


        #region Constants

        private const string _resultSetName = "table";

        #endregion Constants


        #region Public Methods

        /// <summary>
        /// Executes a SQL statement and retrieves a DataSet
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet ExecuteSQLSelect(ConnectionStrings connectionString, string sql)
        {
            DataSet results = new DataSet();

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(results);
                }
            }

            return results;
        }

        /// <summary>
        /// Executes a stored procedure and retrieves a DataSet
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        public static DataSet ExecuteSPSelect(ConnectionStrings connectionString, string procName)
        {
            return ExecuteSPSelect(connectionString, procName, null);
        }

        /// <summary>
        /// Executes a stored procedure and retrieves a DataSet
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteSPSelect(ConnectionStrings connectionString, string procName, ListDictionary procParameters)
        {
            DataSet results = new DataSet();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(procName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
                    if (procParameters != null)
                    {
                        foreach (DictionaryEntry entry in procParameters)
                        {
                            command.Parameters.AddWithValue(entry.Key.ToString(), entry.Value);
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(results);
                }
            }
            return results;
        }

        /// <summary>
        /// Execute the provided sql insert/update/delete command
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns>Number of rows affected by the command</returns>
        public static int ExecuteSQLNonQuery(ConnectionStrings connectionString, string sql)
        {
            int rowCount;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
                    rowCount = command.ExecuteNonQuery();
                }
            }
            return rowCount;
        }

        /// <summary>
        /// Execute the provided sql insert/update/delete command
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns>Number of rows affected by the command</returns>
        public static void ExecuteSQLNonQueryWithConnectionString(string connectionString, string sql)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Use Dapper to execute the parameterized SQL statement with the poco
        /// parameter supplying the parameters.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="poco"></param>
        /// <returns>A single integer return value, if requested</returns>
        public static int? ExecuteSQLNonQuery(ConnectionStrings connectionString, string sql, object poco)
        {
            int? retVal = null;

            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString()].ConnectionString))
            {
                sqlConnection.Open();
                IEnumerable<dynamic> results = sqlConnection.Query(sql, poco);
                sqlConnection.Close();

                if (results.Count() > 0)
                {
                    retVal = results.First();
                }
            }

            return retVal;
        }

        /// <summary>
        /// Execute a SQL select statement using the connection string provided, rather than
        /// one of our standard connection strings wrapped by BPA.Services.DataAccess.ConnectionStrings.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteSQLSelectWithConnectionString(string connectionString, string sql)
        {
            DataSet results = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(results);
                }
            }

            return results;
        }


        /// <summary>
        /// Execute the provided sql insert command
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns>Identity (primary key) ID for inserted row</returns>
        public static int ExecuteSQLNonQueryGetIdentity(ConnectionStrings connectionString, string sql)
        {
            var connString = ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString;
            return ExecuteSQLNonQueryGetIdentity(connString, sql);
        }

        /// <summary>
        /// Execute the provided sql insert command
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns>Identity (primary key) ID for inserted row</returns>
        public static int ExecuteSQLNonQueryGetIdentity(string connectionString, string sql)
        {
            int identityId;
            if (sql.ToLower().IndexOf("select @@identity") == -1)
            {
                sql += "; select @@identity;";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
                    identityId = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return identityId;
        }

        /// <summary>
        /// Execute the provided stored proc insert/update/delete command
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <returns>Whatever value the proc is designed to return: row count, error code, etc.</returns>
        public static int ExecuteSPNonQuery(ConnectionStrings connectionString, string procName)
        {
            return ExecuteSPNonQuery(connectionString, procName, null);
        }

        /// <summary>
        /// Execute the provided stored proc insert/update/delete command, with parameters
        /// automatically generated from the BPA.Core.Domain POCO class provided.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        public static int ExecuteSPNonQuery(ConnectionStrings connectionString, string procName, object poco)
        {
            var parms = GetListDictionary(connectionString, procName, poco);
            return ExecuteSPNonQuery(connectionString, procName, parms);
        }

        /// <summary>
        /// Execute the provided stored proc insert/update/delete command, with parameters
        /// automatically generated from the BPA.Core.Domain POCO class provided.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="poco"></param>
        /// <param name="additionalParameters">Required parameters that fall outside the scope of the POCO</param>
        /// <returns></returns>
        public static int ExecuteSPNonQuery(ConnectionStrings connectionString, string procName, object poco, ListDictionary additionalParameters)
        {
            var parms = GetListDictionary(connectionString, procName, poco);
            foreach (DictionaryEntry entry in additionalParameters)
            {
                parms.Add(entry.Key, entry.Value);
            }
            return ExecuteSPNonQuery(connectionString, procName, parms);
        }

        /// <summary>
        /// Execute the provided stored proc insert/update/delete command
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns>Whatever value the proc is designed to return: row count, error code, etc.</returns>
        public static int ExecuteSPNonQuery(ConnectionStrings connectionString, string procName, ListDictionary procParameters)
        {
            string connString = ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString;
            return ExecuteSPNonQuery(connString, procName, procParameters);
        }

        /// <summary>
        /// Execute the provided stored proc insert/update/delete command
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        public static int ExecuteSPNonQuery(string connectionString, string procName, ListDictionary procParameters)
        {
            int retVal;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(procName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

                    if (procParameters != null)
                    {
                        foreach (DictionaryEntry entry in procParameters)
                        {
                            SqlParameter parm = new SqlParameter(entry.Key.ToString(), entry.Value);
                            parm.Direction = ParameterDirection.Input;
                            command.Parameters.Add(parm);
                        }
                    }

                    SqlParameter returnValue = new SqlParameter("@RETURN_VALUE", SqlDbType.Int);
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnValue);

                    command.ExecuteNonQuery();

                    retVal = (int)returnValue.Value;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Executes a stored procedure that takes a single, table-valued parameter
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="parameterTypeName">The type name of the table-valued parameter</param>
        /// <returns>Whatever value the proc is designed to return: row count, error code, etc.</returns>
        public static int ExecuteSPNonQueryTVP(ConnectionStrings connectionString,
            string procName,
            string parameterName,
            object parameterValue,
            string parameterTypeName)
        {
            return ExecuteSPNonQueryTVP(connectionString, procName, parameterName, parameterValue, parameterTypeName, null);
        }

        /// <summary>
        /// Executes a stored procedure that takes one table-valued parameter, and a list of other natively-typed parameters
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="parameterName">The name (including "@" sign) of the table-valued parameter</param>
        /// <param name="parameterValue">The value of the table-valued parameter</param>
        /// <param name="parameterTypeName">The table-valued parameter type name</param>
        /// <param name="otherParameters"></param>
        /// <returns></returns>
        public static int ExecuteSPNonQueryTVP(ConnectionStrings connectionString,
            string procName,
            string parameterName,
            object parameterValue,
            string parameterTypeName,
            ListDictionary otherParameters)
        {
            int retVal;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(procName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

                    SqlParameter parm = new SqlParameter(parameterName, parameterValue);
                    parm.Direction = ParameterDirection.Input;
                    parm.SqlDbType = SqlDbType.Structured;
                    parm.TypeName = parameterTypeName;
                    command.Parameters.Add(parm);

                    if (otherParameters != null)
                    {
                        foreach (DictionaryEntry entry in otherParameters)
                        {
                            SqlParameter otherParm = new SqlParameter(entry.Key.ToString(), entry.Value);
                            otherParm.Direction = ParameterDirection.Input;
                            command.Parameters.Add(otherParm);
                        }
                    }

                    SqlParameter returnValue = new SqlParameter("@RETURN_VALUE", SqlDbType.Int);
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnValue);

                    command.ExecuteNonQuery();

                    retVal = (int)returnValue.Value;
                }
            }

            return retVal;
        }

        public static DataSet ExecuteSPSelectTVP(ConnectionStrings connectionString,
            string procName,
            string parameterName,
            object parameterValue,
            string parameterTypeName,
            ListDictionary otherParameters)
        {
            DataSet results = new DataSet();

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(procName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

                    SqlParameter parm = new SqlParameter(parameterName, parameterValue);
                    parm.Direction = ParameterDirection.Input;
                    parm.SqlDbType = SqlDbType.Structured;
                    parm.TypeName = parameterTypeName;
                    command.Parameters.Add(parm);

                    if (otherParameters != null)
                    {
                        foreach (DictionaryEntry entry in otherParameters)
                        {
                            SqlParameter otherParm = new SqlParameter(entry.Key.ToString(), entry.Value);
                            otherParm.Direction = ParameterDirection.Input;
                            command.Parameters.Add(otherParm);
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(results);
                }
            }

            return results;
        }

        /// <summary>
        /// Executes the stored procedure and returns a single variant value
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public static object ExecuteSPGetValue(ConnectionStrings connectionString, string procName, ListDictionary procParameters)
        {
            object retVal = new object();

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(procName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

                    if (procParameters != null)
                    {
                        foreach (DictionaryEntry entry in procParameters)
                        {
                            SqlParameter parm = new SqlParameter(entry.Key.ToString(), entry.Value);
                            parm.Direction = ParameterDirection.Input;
                            command.Parameters.Add(parm);
                        }
                    }

                    retVal = command.ExecuteScalar();
                }
            }

            return retVal;
        }

        /// <summary>
        /// Inserts all the records in the DataTable provided into the destination table.
        /// Executed as a transaction, so if any of the inserts in any batch fail, the
        /// entire operation will be rolled back.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="data"></param>
        /// <param name="batchSize"></param>
        public static void ExecuteBulkInsert(ConnectionStrings connectionString, string destinationTableName, DataTable data, int batchSize)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (SqlBulkCopy copy = new SqlBulkCopy(connection, SqlBulkCopyOptions.FireTriggers, transaction))
                {
                    copy.DestinationTableName = destinationTableName;
                    copy.BatchSize = batchSize;

                    foreach (DataColumn sourceColumn in data.Columns)
                    {
                        copy.ColumnMappings.Add(sourceColumn.ColumnName, sourceColumn.ColumnName);
                    }

                    try
                    {
                        copy.WriteToServer(data);
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }



        /// <summary>
        /// A simple method for straight conversion from the result sets contained in the provided stored procedure to a JSON string.
        /// Each result set is returned as a JSON object with a collection of JSON objects representing the returned rows.
        /// Default format is:
        /// {table1:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}],table2:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}]}
        /// </summary>
        /// <param name="connectionString">DataAccess.ConnectionStrings enum value</param>
        /// <param name="procName">The stored procedure to execute</param>
        /// <returns>JSON string</returns>
        public static string ExecuteSPSelectGetJSON(ConnectionStrings connectionString, string procName)
        {
            return ExecuteSPSelectGetJSON(connectionString, procName, null, null, false);
        }

        /// <summary>
        /// A simple method for straight conversion from the result sets contained in the provided stored procedure to a JSON string.
        /// Each result set is returned as a JSON object with a collection of JSON objects representing the returned rows.
        /// Default format is:
        /// {table1:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}],table2:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}]}
        /// </summary>
        /// <param name="connectionString">DataAccess.ConnectionStrings enum value</param>
        /// <param name="procName">The stored procedure to execute</param>
        /// <param name="procParameters">A ListDictionary of the stored procedure's input parameters</param>
        /// <returns>JSON string</returns>
        public static string ExecuteSPSelectGetJSON(ConnectionStrings connectionString, string procName, ListDictionary procParameters)
        {
            return ExecuteSPSelectGetJSON(connectionString, procName, procParameters, null, false);
        }

        /// <summary>
        /// A simple method for straight conversion from the result sets contained in the provided stored procedure to a JSON string.
        /// Each result set is returned as a JSON object with a collection of JSON objects representing the returned rows.
        /// Default format is:
        /// {table1:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}],table2:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}]}
        /// </summary>
        /// <param name="connectionString">DataAccess.ConnectionStrings enum value</param>
        /// <param name="procName">The stored procedure to execute</param>
        /// <param name="resultName">Replaces the default result set name "table"</param>
        /// <returns>JSON string</returns>
        public static string ExecuteSPSelectGetJSON(ConnectionStrings connectionString, string procName, string resultName)
        {
            return ExecuteSPSelectGetJSON(connectionString, procName, null, resultName, false);
        }

        /// <summary>
        /// A simple method for straight conversion from the result sets contained in the provided stored procedure to a JSON string.
        /// Each result set is returned as a JSON object with a collection of JSON objects representing the returned rows.
        /// Default format is:
        /// {table1:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}],table2:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}]}
        /// </summary>
        /// <param name="connectionString">DataAccess.ConnectionStrings enum value</param>
        /// <param name="procName">The stored procedure to execute</param>
        /// <param name="compact">A boolean value indicating whether to use the column names returned from the stored procedure, or use single letter columns names 
        /// (i.e. "a", "b", "aa") that are automatically generated by the method to reduce JSON size</param>
        /// <returns>JSON string</returns>
        public static string ExecuteSPSelectGetJSON(ConnectionStrings connectionString, string procName, bool compact)
        {
            return ExecuteSPSelectGetJSON(connectionString, procName, null, null, compact);
        }

        /// <summary>
        /// A simple method for straight conversion from the result sets contained in the provided stored procedure to a JSON string.
        /// Each result set is returned as a JSON object with a collection of JSON objects representing the returned rows.
        /// Default format is:
        /// {table1:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}],table2:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}]}
        /// </summary>
        /// <param name="connectionString">DataAccess.ConnectionStrings enum value</param>
        /// <param name="procName">The stored procedure to execute</param>
        /// <param name="procParameters">A ListDictionary of the stored procedure's input parameters</param>
        /// <param name="resultName">Replaces the default result set name "table"</param>
        /// <returns>JSON string</returns>
        public static string ExecuteSPSelectGetJSON(ConnectionStrings connectionString, string procName, ListDictionary procParameters, string resultName)
        {
            return ExecuteSPSelectGetJSON(connectionString, procName, procParameters, resultName, false);
        }

        /// <summary>
        /// A simple method for straight conversion from the result sets contained in the provided stored procedure to a JSON string.
        /// Each result set is returned as a JSON object with a collection of JSON objects representing the returned rows.
        /// Default format is:
        /// {table1:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}],table2:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}]}
        /// </summary>
        /// <param name="connectionString">DataAccess.ConnectionStrings enum value</param>
        /// <param name="procName">The stored procedure to execute</param>
        /// <param name="procParameters">A ListDictionary of the stored procedure's input parameters</param>
        /// <param name="compact">A boolean value indicating whether to use the column names returned from the stored procedure, or use single letter columns names 
        /// (i.e. "a", "b", "aa") that are automatically generated by the method to reduce JSON size</param>
        /// <returns>JSON string</returns>
        public static string ExecuteSPSelectGetJSON(ConnectionStrings connectionString, string procName, ListDictionary procParameters, bool compact)
        {
            return ExecuteSPSelectGetJSON(connectionString, procName, procParameters, null, compact);
        }

        /// <summary>
        /// A simple method for straight conversion from the result sets contained in the provided stored procedure to a JSON string.
        /// Each result set is returned as a JSON object with a collection of JSON objects representing the returned rows.
        /// Default format is:
        /// {table1:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}],table2:[{\"columnName\":\"valueAsString\",\"columnName\":\"valueAsString\"}]}
        /// </summary>
        /// <param name="connectionString">DataAccess.ConnectionStrings enum value</param>
        /// <param name="procName">The stored procedure to execute</param>
        /// <param name="procParameters">A ListDictionary of the stored procedure's input parameters</param>
        /// <param name="resultName">Replaces the default result set name "table"</param>
        /// <param name="compact">A boolean value indicating whether to use the column names returned from the stored procedure, or use single letter columns names 
        /// (i.e. "a", "b", "aa") that are automatically generated by the method to reduce JSON size</param>
        /// <returns>JSON string</returns>
        public static string ExecuteSPSelectGetJSON(ConnectionStrings connectionString, string procName, ListDictionary procParameters, string resultName, bool compact)
        {
            StringBuilder jsonResult = new StringBuilder();
            jsonResult.Append("{");

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(procName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
                if (procParameters != null)
                {
                    foreach (DictionaryEntry entry in procParameters)
                    {
                        command.Parameters.AddWithValue(entry.Key.ToString(), entry.Value);
                    }
                }

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (resultName == null || resultName == string.Empty)
                        {
                            resultName = _resultSetName;
                        }
                        string columnName = string.Empty;

                        int resultCtr = 1;

                        do
                        {
                            jsonResult.Append(resultName + resultCtr.ToString() + ":[");
                            while (reader.Read())
                            {
                                // LC 11.4: Commented out line returns each row as an object,
                                // which is what jtemplate wanted, but it might make more sense
                                // for each row to be a simple array instead of an object.
                                //jsonResult.Append("{");
                                jsonResult.Append("[");
                                int columnCount = reader.FieldCount;
                                for (int i = 0; i < columnCount; i++)
                                {
                                    // LC 11.4: Simplifying method further, so that column name is NOT included
                                    // in collection returned.
                                    //if (compact)
                                    //{
                                    //    columnName = GetCompactColumnName(i);
                                    //}
                                    //else
                                    //{
                                    //    columnName = reader.GetName(i);
                                    //}
                                    //jsonResult.Append("\"" + columnName + "\":\"" + reader[i].ToString() + "\",");
                                    jsonResult.Append("\"" + reader[i].ToString() + "\",");
                                }
                                jsonResult.Remove(jsonResult.Length - 1, 1);
                                //jsonResult.Append("},");
                                jsonResult.Append("],");
                            }
                            jsonResult.Remove(jsonResult.Length - 1, 1);
                            jsonResult.Append("],");
                            resultCtr++;
                        } while (reader.NextResult());

                        jsonResult.Remove(jsonResult.Length - 1, 1);
                    }
                }
            }

            jsonResult.Append("}");
            return jsonResult.ToString();
        }

        /// <summary>
        /// A simple method for straight conversion from the result sets returned by the provided stored procedure to a complex generic Dictionary collection.
        /// Each result set is returned as a Dictionary entry with a Key (string) indicating the result set and a Value (jagged string array) containing each row
        /// as a single-dimensional string array. So, a stored procedure that returns two result sets would return the following Dictionary structure:
        /// Key: "table1", Value: string[n]{ string[3]{"row1 column1","row1 column2","row1 column3"}, string[3]{"row2 column1","row2 column2","row2 column3"}...n }
        /// Key: "table2", Value: string[n]{ string[2]{"row1 column1","row1 column2"}, string[2]{"row2 column1","row2 column2"}...n }
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        /// <remarks>
        /// The purpose of this procedure is to return a type that can be easily serialized to a JSON array for use in client-side programming.
        /// </remarks>
        public static Dictionary<string, string[][]> ExecuteSPSelectGetDictionary(ConnectionStrings connectionString, string procName)
        {
            return ExecuteSPSelectGetDictionary(connectionString, procName, null);
        }

        /// <summary>
        /// A simple method for straight conversion from the result sets returned by the provided stored procedure to a complex generic Dictionary collection.
        /// Each result set is returned as a Dictionary entry with a Key (string) indicating the result set and a Value (jagged string array) containing each row
        /// as a single-dimensional string array. So, a stored procedure that returns two result sets would return the following Dictionary structure:
        /// Key: "table1", Value: string[n]{ string[3]{"row1 column1","row1 column2","row1 column3"}, string[3]{"row2 column1","row2 column2","row2 column3"}...n }
        /// Key: "table2", Value: string[n]{ string[2]{"row1 column1","row1 column2"}, string[2]{"row2 column1","row2 column2"}...n }
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        /// <remarks>
        /// The purpose of this procedure is to return a type that can be easily serialized to a JSON array for use in client-side programming.
        /// </remarks>
        public static Dictionary<string, string[][]> ExecuteSPSelectGetDictionary(ConnectionStrings connectionString, string procName, ListDictionary procParameters)
        {
            Dictionary<string, string[][]> result = new Dictionary<string, string[][]>();

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString("g")].ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(procName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
                if (procParameters != null)
                {
                    foreach (DictionaryEntry entry in procParameters)
                    {
                        command.Parameters.AddWithValue(entry.Key.ToString(), entry.Value);
                    }
                }

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int resultCtr = 1;

                        // Loop through each result set returned from the proc
                        do
                        {
                            // Temporary container for the rows in the current result set
                            List<string[]> rowList = new List<string[]>();

                            // Loop through each row in the current result set
                            while (reader.Read())
                            {
                                int columnCount = reader.FieldCount;
                                string[] currentRow = new string[columnCount];

                                // Loop through each column in the current row
                                for (int i = 0; i < columnCount; i++)
                                {
                                    // Add all row values to the row string array
                                    currentRow[i] = reader[i].ToString();
                                }

                                // Add the row to our temporary container
                                rowList.Add(currentRow);
                            }

                            // Add all the rows from the temporary list to our jagged array
                            // Note: could probably do some kind of CopyTo or Join here, but looping because it's what I know
                            string[][] allRows = new string[rowList.Count][];
                            for (int i = 0; i < rowList.Count; i++)
                            {
                                allRows[i] = rowList[i];
                            }

                            // Add the complete set to the result collection
                            string resultName = _resultSetName + resultCtr.ToString();
                            result.Add(resultName, allRows);

                            resultCtr++;
                        } while (reader.NextResult());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Use Dapper to execute the proc specified, returning the first item as the type (T) specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public static T ExecuteSPGetItem<T>(ConnectionStrings connectionString, string procName, object procParameters = null)
        {
            dynamic r = null;

            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString()].ConnectionString))
                {
                    sqlConnection.Open();
                    var results = (IEnumerable<T>)sqlConnection.Query<T>(procName, param: procParameters, commandType: CommandType.StoredProcedure, commandTimeout: Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]));
                    sqlConnection.Close();

                    if (results.Count() > 0)
                    {
                        r = results.First();
                    }
                }
            }
            catch (System.ArgumentException argEx)
            {
                if (argEx.Message == "When using the multi-mapping APIs ensure you set the splitOn param if you have keys other than Id\r\nParameter name: splitOn")
                {
                    // LC 11.17.2015: Catching and ignoring this specific message, because it indicates that the proc returned no results,
                    // and there's no reason to blow up when that happens. All other exceptions will remain unhandled.
                }
            }

            return r;
        }

        /// <summary>
        /// Use Dapper to execute the proc specified, returning a list of type (T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public static List<T> ExecuteSPGetList<T>(ConnectionStrings connectionString, string procName, object procParameters = null)
        {
            List<T> results = new List<T>();

            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString()].ConnectionString))
                {
                    sqlConnection.Open();
                    results = (List<T>)sqlConnection.Query<T>(procName, param: procParameters, commandType: CommandType.StoredProcedure, commandTimeout: Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]));
                    sqlConnection.Close();
                }
            }
            catch (System.ArgumentException argEx)
            {
                if (argEx.Message == "When using the multi-mapping APIs ensure you set the splitOn param if you have keys other than Id\r\nParameter name: splitOn")
                {
                    // LC 11.17.2015: Catching and ignoring this specific message, because it indicates that the proc returned no results,
                    // and there's no reason to blow up when that happens. All other exceptions will remain unhandled.
                }
            }

            return results;
        }

        /// <summary>
        /// Use Dapper to execute command proc specified, returning the first item as the type (T) specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public static T ExecuteSQLGetItem<T>(ConnectionStrings connectionString, string sql)
        {
            dynamic r = null;

            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString()].ConnectionString))
                {
                    sqlConnection.Open();
                    var results = (IEnumerable<T>)sqlConnection.Query<T>(sql, commandType: CommandType.Text, commandTimeout: Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]));
                    sqlConnection.Close();

                    if (results.Count() > 0)
                    {
                        r = results.First();
                    }
                }
            }
            catch (System.ArgumentException argEx)
            {
                if (argEx.Message == "When using the multi-mapping APIs ensure you set the splitOn param if you have keys other than Id\r\nParameter name: splitOn")
                {
                    // LC 11.17.2015: Catching and ignoring this specific message, because it indicates that the proc returned no results,
                    // and there's no reason to blow up when that happens. All other exceptions will remain unhandled.
                }
            }

            return r;
        }

        /// <summary>
        /// Use Dapper to execute the command specified, returning a list of type (T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public static List<T> ExecuteSQLGetList<T>(ConnectionStrings connectionString, string sql)
        {
            List<T> results = new List<T>();

            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString()].ConnectionString))
                {
                    sqlConnection.Open();
                    results = (List<T>)sqlConnection.Query<T>(sql, commandType: CommandType.Text, commandTimeout: Int32.Parse(ConfigurationManager.AppSettings["SQLCommandTimeout"]));
                    sqlConnection.Close();
                }
            }
            catch (System.ArgumentException argEx)
            {
                if (argEx.Message == "When using the multi-mapping APIs ensure you set the splitOn param if you have keys other than Id\r\nParameter name: splitOn")
                {
                    // LC 11.17.2015: Catching and ignoring this specific message, because it indicates that the proc returned no results,
                    // and there's no reason to blow up when that happens. All other exceptions will remain unhandled.
                }
            }

            return results;
        }

        #endregion Public Methods


        #region Private Methods

        private static string GetCompactColumnName(int idx)
        {
            string colName = string.Empty;
            string colNames = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] nameArr = colNames.Split(new char[1] { ',' }, System.StringSplitOptions.None);
            if (idx <= 25)
            {
                colName = nameArr[idx];
            }
            else
            {
                int idx2 = idx - (26 * (idx / 26));
                int mult = (idx / 26) + 1;
                string colNameVal = nameArr[idx2];
                for (int i = 0; i < mult; i++)
                {
                    colName += colNameVal;
                }
            }
            return colName;
        }

        /// <summary>
        /// A helper method to faciliate method overloads that take a BPA.Core.Domain type
        /// rather than a ListDictionary. Queries the proc for all its parameters, and adds
        /// an entry to the dictionary returned for each parameter available in the POCO.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        private static ListDictionary GetListDictionary(ConnectionStrings connectionString, string procName, object poco)
        {
            ListDictionary parms = new ListDictionary();

            string getParms = "select PARAMETER_NAME from information_schema.parameters where specific_name='" + procName + "'";
            List<string> parmList = new List<string>();
            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString.ToString()].ConnectionString))
            {
                sqlConnection.Open();
                parmList = sqlConnection.Query<string>(getParms, commandType: CommandType.Text).ToList();
                sqlConnection.Close();
            }

            Type c = poco.GetType();

            foreach (var parm in parmList)
            {
                var pi = c.GetProperty(parm.Replace("@", String.Empty));

                if (pi != null)
                {
                    var isString = pi.PropertyType.Name == "String";

                    if (isString)
                    {
                        if (!String.IsNullOrEmpty((string)pi.GetValue(poco, null)) && !String.IsNullOrWhiteSpace((string)pi.GetValue(poco, null)))
                        {
                            parms.Add(parm, pi.GetValue(poco, null));
                        }
                        else
                        {
                            parms.Add(parm, DBNull.Value);
                        }
                    }
                    else
                    {
                        var val = pi.GetValue(poco, null);
                        if (val == null) { val = DBNull.Value; }
                        parms.Add(parm, val);
                    }
                }
            }

            return parms;
        }

        #endregion Private Methods
    }
}
