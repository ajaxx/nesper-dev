///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace com.espertech.esper.common.@internal.db.drivers
{
    /// <summary>
    /// A database driver specific to the SQLServer
    /// </summary>
    [Serializable]
    public class DbDriverSqlServer : BaseDbDriver
    {
        public DbDriverSqlServer()
        {
        }

        protected DbDriverSqlServer(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Creates a connection string builder.
        /// </summary>
        /// <returns></returns>
        protected override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new SqlConnectionStringBuilder();
        }

        /// <summary>
        /// Factory method that is used to create instance of a connection.
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            DbConnection dbConnection = new SqlConnection(ConnectionString);
            dbConnection.Open();
            return dbConnection;
        }

        /// <summary>
        /// Gets a value indicating whether [use position parameters].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use position parameters]; otherwise, <c>false</c>.
        /// </value>
        protected override bool UsePositionalParameters {
            get { return false; }
        }

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <value>The param prefix.</value>
        protected override string ParamPrefix {
            get { return "@"; }
        }
    }
}