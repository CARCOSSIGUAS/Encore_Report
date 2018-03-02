// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IUnitOfWork"/> and <see cref="IUnitOfWork{TContext}"/> interface.
    /// </summary>
    /// <typeparam name="TContext">The type of the db context.</typeparam>
    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext>, IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private bool disposed = false;
        private Dictionary<Type, object> repositories;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
        public TContext DbContext => _context;

        /// <summary>
        /// Changes the database name. This require the databases in the same machine. NOTE: This only work for MySQL right now.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <remarks>
        /// This only been used for supporting multiple databases in the same model. This require the databases in the same machine.
        /// </remarks>
        public void ChangeDatabase(string database)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State.HasFlag(ConnectionState.Open))
            {
                connection.ChangeDatabase(database);
            }
            else
            {
                var connectionString = Regex.Replace(connection.ConnectionString.Replace(" ", ""), @"(?<=[Dd]atabase=)\w+(?=;)", database, RegexOptions.Singleline);
                connection.ConnectionString = connectionString;
            }

            // Following code only working for mysql.
            var items = _context.Model.GetEntityTypes();
            foreach (var item in items)
            {
                if (item.Relational() is RelationalEntityTypeAnnotations extensions)
                {
                    extensions.Schema = database;
                }
            }
        }

        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new Repository<TEntity>(_context);
            }

            return (IRepository<TEntity>)repositories[type];
        }

        /// <summary>
        /// Executes the specified raw SQL command.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The number of state entities written to database.</returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters) => _context.Database.ExecuteSqlCommand(sql, parameters);

        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IQueryable{T}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class => _context.Set<TEntity>().FromSql(sql, parameters);

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges(bool ensureAutoHistory = false)
        {
            if (ensureAutoHistory)
            {
                _context.EnsureAutoHistory();
            }

            return _context.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false)
        {
            if (ensureAutoHistory)
            {
                _context.EnsureAutoHistory();
            }

            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Saves all changes made in this context to the database with distributed transaction.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <param name="unitOfWorks">An optional <see cref="IUnitOfWork"/> array.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks)
        {
            // TransactionScope will be included in .NET Core v2.0
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var count = 0;
                    foreach (var unitOfWork in unitOfWorks)
                    {
                        var uow = unitOfWork as UnitOfWork<DbContext>;
                        uow.DbContext.Database.UseTransaction(transaction.GetDbTransaction());
                        count += await uow.SaveChangesAsync(ensureAutoHistory);
                    }

                    count += await SaveChangesAsync(ensureAutoHistory);

                    transaction.Commit();

                    return count;
                }
                catch (Exception ex)
                {

                    transaction.Rollback();

                    throw ex;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // clear repositories
                    if (repositories != null)
                    {
                        repositories.Clear();
                    }

                    // dispose the db context.
                    _context.Dispose();
                }
            }

            disposed = true;
        }

        public IEnumerable<T> ExecuteSqlQuery<T>(string procedureName, object parameters)
        {
            IDbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            object[] sqlParameters = this.GetParametersSqlQuery(parameters);

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

        private object[] GetParametersSqlQuery(Object parameters)
        {
            IList<SqlParameter> listaParametro = new List<SqlParameter>();

            foreach (PropertyInfo prop in parameters.GetType().GetProperties())
            {
                dynamic valor = prop.GetValue(parameters, null);

                Type type = Nullable.GetUnderlyingType(prop.PropertyType);
                type = (type == null) ? prop.PropertyType : type;

                if (type == typeof(System.Data.DataTable))
                {
                    listaParametro.Add(new SqlParameter() { ParameterName = "@" + prop.Name, Value = valor, SqlDbType = SqlDbType.Structured });
                }
                else
                {
                    listaParametro.Add(new SqlParameter() { ParameterName = "@" + prop.Name, Value = valor });
                }
            }

            return listaParametro.ToArray();
        }
        public T CreateItem<T>(DataRow row)
        {
            string columnName;
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in row.Table.Columns)
                {
                    columnName = column.ColumnName;
                    //Get property with same columnName
                    PropertyInfo prop = obj.GetType().GetProperty(columnName,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    try
                    {
                        //Get value for the column
                        object value = (row[columnName].GetType() == typeof(DBNull))
                        ? null : row[columnName];
                        //Set property value
                        prop.SetValue(obj, value, null);
                    }
                    catch
                    {
                        throw;
                        //Catch whatever here
                    }
                }
            }
            return obj;
        }
        public IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;
            if (rows != null)
            {
                list = new List<T>();
                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }
            return list;
        }
        public IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
                return null;

            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in table.Rows)
                rows.Add(row);

            return ConvertTo<T>(rows);
        }
    }
}
