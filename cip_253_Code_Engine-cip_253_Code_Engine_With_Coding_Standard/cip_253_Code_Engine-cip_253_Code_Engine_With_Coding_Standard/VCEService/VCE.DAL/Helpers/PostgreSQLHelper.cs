using Npgsql;
using System.Collections;
using System.Data;

namespace VCE.DAL
{
    public sealed class PostgreSQLHelper
    {
        public PostgreSQLHelper()
        {
            //
            // TODO: Add constructor logic here
            //

            #region private utility methods & constructors
        }

        /// <summary>
        /// This method is used to attach array of NpgsqlParameters to a NpgsqlCommand.
        ///
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.
        ///
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">an array of NpgsqlParameters tho be added to command</param>
        private static void AttachParameters(NpgsqlCommand command, NpgsqlParameter[] commandParameters)
        {
            foreach (NpgsqlParameter parameter in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
                {
                    parameter.Value = DBNull.Value;
                }
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of NpgsqlParameters.
        /// </summary>
        /// <param name="commandParameters">array of NpgsqlParameters to be assigned values</param>
        /// <param name="parameterValues">array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(NpgsqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }
            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }
            //iterate through the NpgsqlParameters, assigning the values from the corresponding position in the
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters
        /// to the provided command.
        /// </summary>
        /// <param name="command">the NpgsqlCommand to be prepared</param>
        /// <param name="connection">a valid NpgsqlConnection, on which to execute this command</param>
        /// <param name="transaction">a valid NpgsqlTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of NpgsqlParameters to be associated with the command or 'null' if no parameters are required</param>
        private static void PrepareCommand(NpgsqlCommand command, NpgsqlConnection connection, NpgsqlTransaction transaction, CommandType commandType, string commandText, NpgsqlParameter[] commandParameters)
        {   //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            //associate the connection with the command
            command.Connection = connection;
            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;
            //if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            //set the command type
            command.CommandType = commandType;
            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion private utility methods & constructors

        #region ExecuteNonQuery

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset and takes no parameters) against the database specified in
        /// the connection string.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteNonQuery(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset) against the database specified in the connection string
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {   //create & open a NpgsqlConnection, and dispose of it after we are done.
            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            {
                cn.Open();
                //call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns no resultset) against the database specified in
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="spName">the name of the stored prcedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(NpgsqlConnection connection, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteNonQuery(connection, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset) against the specified NpgsqlConnection
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters);

            //finally, execute the command.
            int retval = cmd.ExecuteNonQuery();

            // detach the NpgsqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns no resultset) against the specified NpgsqlConnection
        /// using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(NpgsqlConnection connection, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset and takes no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteNonQuery(transaction, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset) against the specified NpgsqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(NpgsqlTransaction transaction, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            //finally, execute the command.
            int retval = cmd.ExecuteNonQuery();

            // detach the NpgsqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns no resultset) against the specified
        /// NNpgsqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteNonQuery

        #region ExecuteDataSet

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the database specified in
        /// the connection string.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteDataset(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }

        public static DataSet ExecuteDataset_procedure(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteDataset_Procedure(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the database specified in the connection string
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create & open a NpgsqlConnection, and dispose of it after we are done.
            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteDataset(cn, commandType, commandText, commandParameters);
            }
        }

        public static DataSet ExecuteDataset_Procedure(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            DataSet dataSet = new DataSet();
            //create & open a NpgsqlConnection, and dispose of it after we are done.
            using (NpgsqlConnection connections = new NpgsqlConnection(connectionString))
            {
                connections.Open();
                NpgsqlTransaction tran = connections.BeginTransaction();

                NpgsqlCommand command = new NpgsqlCommand(commandText, connections);
                command.CommandType = CommandType.Text;
                command.Parameters.AddRange(commandParameters);
                command.ExecuteNonQuery();
                NpgsqlDataAdapter dataAdapter;
                int i = 0;
                foreach (NpgsqlParameter parm in commandParameters)
                {
                    if (parm.NpgsqlDbType == NpgsqlTypes.NpgsqlDbType.Refcursor)
                    {
                        string parm_val = string.Format("FETCH ALL IN \"{0}\"", parm.Value.ToString());
                        dataAdapter = new NpgsqlDataAdapter(parm_val.Trim().ToString(), connections);
                        dataSet.Tables.Add(parm_val);
                        dataAdapter.Fill(dataSet.Tables[i]);
                        i++;
                    }
                }
                tran.Commit();
                //call the overload that takes a connection in place of the connection string
                connections.Close();
                connections.Dispose();
            }
            return dataSet;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the database specified in
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(NpgsqlConnection connection, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteDataset(connection, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            NpgsqlDataAdapter dataAdaptar = new NpgsqlDataAdapter(cmd);
            DataSet dataSet = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            dataAdaptar.Fill(dataSet);

            // detach the NpgsqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();

            connection.Close();
            connection.Dispose();

            //return the dataset
            return dataSet;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection
        /// using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(NpgsqlConnection connection, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteDataset(transaction, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(NpgsqlTransaction transaction, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(cmd);
            DataSet dataSet = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            dataAdapter.Fill(dataSet);

            // detach the NpgsqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();

            //return the dataset
            return dataSet;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified
        /// NpgsqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteDataSet

        #region ExecuteReader

        /// <summary>
        /// this enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that
        /// we can set the appropriate CommandBehavior when calling ExecuteReader()
        /// </summary>
        private enum NpgsqlConnectionOwnership
        {
            /// <summary>Connection is owned and managed by SqlHelper</summary>
            Internal,

            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

        /// <summary>
        /// Create and prepare a NpgsqlCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        ///
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection, on which to execute this command</param>
        /// <param name="transaction">a valid NpgsqlTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of NpgsqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="connectionOwnership">indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
        /// <returns>NpgsqlDataReader containing the results of the command</returns>
        private static NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, NpgsqlTransaction transaction, CommandType commandType, string commandText, NpgsqlParameter[] commandParameters, NpgsqlConnectionOwnership connectionOwnership)
        {
            //create a command and prepare it for execution
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

            //create a reader
            NpgsqlDataReader dataReader;

            // call ExecuteReader with the appropriate CommandBehavior
            if (connectionOwnership == NpgsqlConnectionOwnership.External)
            {
                dataReader = cmd.ExecuteReader();
            }
            else
            {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }

            // detach the NpgsqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();

            return dataReader;
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the database specified in
        /// the connection string.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  NpgsqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteReader(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the database specified in the connection string
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  NpgsqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create & open a NpgsqlConnection
            NpgsqlConnection cn = new NpgsqlConnection(connectionString);
            cn.Open();

            try
            {
                //call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(cn, null, commandType, commandText, commandParameters, NpgsqlConnectionOwnership.Internal);
            }
            catch
            {
                //if we fail to return the SqlDatReader, we need to close the connection ourselves
                cn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the database specified in
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  NpgsqlDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  NpgsqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteReader(connection, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  NpgsqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters, NpgsqlConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection
        /// using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  NpgsqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  NpgsqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteReader(transaction, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///   NpgsqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(NpgsqlTransaction transaction, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //pass through to private overload, indicating that the connection is owned by the caller
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, NpgsqlConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified
        /// NpgsqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  NpgsqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a NpgsqlDataReader containing the resultset generated by the command</returns>
        public static NpgsqlDataReader ExecuteReader(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in
        /// the connection string.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteScalar(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset) against the database specified in the connection string
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create & open a NpgsqlConnection, and dispose of it after we are done.
            using (NpgsqlConnection connections = new NpgsqlConnection(connectionString))
            {
                connections.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteScalar(connections, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a 1x1 resultset) against the database specified in
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a NpgsqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(NpgsqlConnection connection, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteScalar(connection, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset) against the specified NpgsqlConnection
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters);

            //execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // detach the NpgsqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a 1x1 resultset) against the specified NpgsqlConnection
        /// using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(NpgsqlConnection connection, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of NpgsqlParameters
            return ExecuteScalar(transaction, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset) against the specified NpgsqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(NpgsqlTransaction transaction, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            //execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // detach the NpgsqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a 1x1 resultset) against the specified
        /// NpgsqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of NpgsqlParameters
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteScalar

        #region ExecuteXmlReader

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        //public static XmlReader ExecuteXmlReader(NpgsqlConnection connection, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of NpgsqlParameters
        //    return ExecuteXmlReader(connection, commandType, commandText, (NpgsqlParameter[])null);
        //}

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        //public static XmlReader ExecuteXmlReader(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        //{
        //    //create a command and prepare it for execution
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    PrepareCommand(cmd, connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters);

        //    //create the DataAdapter & DataSet
        //    XmlReader retval = cmd.ExecuteXmlReader();

        //    // detach the NpgsqlParameters from the command object, so they can be used again.
        //    cmd.Parameters.Clear();
        //    return retval;

        //}

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection
        /// using the provided parameter values.  This method will query the database to discover the parameters for the
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///
        /// e.g.:
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid NpgsqlConnection</param>
        /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        //public static XmlReader ExecuteXmlReader(NpgsqlConnection connection, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        NpgsqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of NpgsqlParameters
        //        return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
        //    }
        //}

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        //public static XmlReader ExecuteXmlReader(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of NpgsqlParameters
        //    return ExecuteXmlReader(transaction, commandType, commandText, (NpgsqlParameter[])null);
        //}

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new NpgsqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid NpgsqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        //public static XmlReader ExecuteXmlReader(NpgsqlTransaction transaction, string commandText, params NpgsqlParameter[] commandParameters)
        //{
        //    //create a command and prepare it for execution
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    PrepareCommand(cmd, transaction.Connection, transaction, commandText, commandParameters);

        //    //create the DataAdapter & DataSet
        //    XmlReader retval = cmd.ExecuteReader();

        //    // detach the NpgsqlParameters from the command object, so they can be used again.
        //    cmd.Parameters.Clear();
        //    return retval;
        //}

        ///// <summary>
        ///// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified
        ///// NpgsqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the
        ///// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        ///// </summary>
        ///// <remarks>
        ///// This method provides no access to output parameters or the stored procedure's return value parameter.
        /////
        ///// e.g.:
        /////  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);
        ///// </remarks>
        ///// <param name="transaction">a valid NpgsqlTransaction</param>
        ///// <param name="spName">the name of the stored procedure</param>
        ///// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        ///// <returns>a dataset containing the resultset generated by the command</returns>
        //public static XmlReader ExecuteXmlReader(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        NpgsqlParameter[] commandParameters = pgSqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of NpgsqlParameters
        //        return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
        //    }
        //}

        #endregion ExecuteXmlReader
    }

    public sealed class pgSqlHelperParameterCache
    {
        #region private methods, variables, and constructors

        //Since this class provides only static methods, make the default constructor private to prevent
        //instances from being created with "new SqlHelperParameterCache()".
        private pgSqlHelperParameterCache()
        { }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// resolve at run time the appropriate set of SqlParameters for a stored procedure
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">whether or not to include their return value parameter</param>
        /// <returns></returns>
        private static NpgsqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            using (NpgsqlConnection connections = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand(spName, connections))
            {
                connections.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                NpgsqlCommandBuilder.DeriveParameters(cmd);

                if (!includeReturnValueParameter)
                {
                    cmd.Parameters.RemoveAt(0);
                }

                NpgsqlParameter[] discoveredParameters = new NpgsqlParameter[cmd.Parameters.Count]; ;

                cmd.Parameters.CopyTo(discoveredParameters, 0);

                return discoveredParameters;
            }
        }

        //deep copy of cached SqlParameter array
        private static NpgsqlParameter[] CloneParameters(NpgsqlParameter[] originalParameters)
        {
            NpgsqlParameter[] clonedParameters = new NpgsqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (NpgsqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion private methods, variables, and constructors

        #region caching functions

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters to be cached</param>
        public static void CacheParameterSet(string connectionString, string commandText, params NpgsqlParameter[] commandParameters)
        {
            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an array of SqlParamters</returns>
        public static NpgsqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string hashKey = connectionString + ":" + commandText;

            NpgsqlParameter[] cachedParameters = (NpgsqlParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <returns>an array of SqlParameters</returns>
        public static NpgsqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">a bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>an array of SqlParameters</returns>
        public static NpgsqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            NpgsqlParameter[] cachedParameters;

            cachedParameters = (NpgsqlParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                cachedParameters = (NpgsqlParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
            }

            return CloneParameters(cachedParameters);
        }

        #endregion Parameter Discovery Functions
    }
}