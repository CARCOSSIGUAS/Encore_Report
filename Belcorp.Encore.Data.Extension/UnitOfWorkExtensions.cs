namespace BelcoBelcorp.Encore.Data.Extension
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class UnitOfWorkExtensions
    {
        public static IEnumerable<T> ExecuteSqlQuery<T>(this IUnitOfWork unitOfWork, string procedureName, Object parameters)
        {
            dynamic context = unitOfWork.GetType().GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitOfWork);

            DatabaseFacade database = context.Database;

            IDbCommand cmd = database.GetDbConnection().CreateCommand();
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            object[] sqlParameters = GetParametersSqlQuery(parameters);

            if (sqlParameters.Any())
            {
                foreach (object sqlParameter in sqlParameters)
                {
                    cmd.Parameters.Add(sqlParameter);
                }
            }

            IEnumerable<T> queryResult;
            DataTable dataTable = new DataTable();
            if (cmd.Connection.State.HasFlag(ConnectionState.Closed))
                cmd.Connection.Open();

            try
            {
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                    queryResult = ConvertTo<T>(dataTable);
                }
            }
            finally
            {
                cmd.Connection.Close();
            }


            return queryResult;
        }
    }
}
