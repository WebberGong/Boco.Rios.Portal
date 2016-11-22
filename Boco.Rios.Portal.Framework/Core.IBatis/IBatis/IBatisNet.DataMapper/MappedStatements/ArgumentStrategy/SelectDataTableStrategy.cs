using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using IBatisNet.DataMapper.Commands;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;

namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    /// <summary>
    /// 数据库表的构造器策略
    /// </summary>
    public sealed class SelectDataTableStrategy : IArgumentStrategy
    {
        /// <summary>
        /// Gets the value of an argument constructor.
        /// </summary>
        /// <param name="request">The current <see cref="RequestScope"/>.</param>
        /// <param name="mapping">The <see cref="ResultProperty"/> with the argument infos.</param>
        /// <param name="reader">The current <see cref="IDataReader"/>.</param>
        /// <param name="keys">The keys</param>
        /// <returns>The paremeter value.</returns>
        public object GetValue(RequestScope request, ResultProperty mapping,
                               ref IDataReader reader, object keys)
        {
            // Get the select statement
            IMappedStatement selectStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);

            reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
            return selectStatement.ExecuteQueryForDataTable(request.Session, keys);
        }
    }

}
