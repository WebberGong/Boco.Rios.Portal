using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    /// <summary>
    /// <see cref="IResultStrategy"/> implementation when 
    /// a 'resultClass' attribute is specified and
    /// the type of the result object is <see cref="IDictionary"/>.
    /// </summary>
    public sealed class DataTableStrategy : IResultStrategy
    {
        #region IResultStrategy Members

        private int[,] indexMapping;

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/> 
        /// when a 'resultClass' attribute is specified on the statement and
        /// the 'resultClass' attribute is a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="resultObject">The result object.</param>
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object outObject = resultObject;

            if (outObject == null)
            {
                outObject = new DataTable(request.Statement.Id);

                DataTable dataColumn = reader.GetSchemaTable();
                indexMapping = new int[2,dataColumn.Rows.Count];
                for (int i = 0,j=0; i < dataColumn.Rows.Count; i++)
                {
                    indexMapping[0,i] = i;
                    indexMapping[1,i] = -1;

                    if(!((DataTable)outObject).Columns.Contains(dataColumn.Rows[i]["ColumnName"].ToString()))
                    {
                        ((DataTable)outObject).Columns.Add(dataColumn.Rows[i]["ColumnName"].ToString(),
                            (Type)dataColumn.Rows[i]["DataType"]);

                        indexMapping[1,i] = j++;
                    }
                }
            
            }

            DataRow dataRow = ((DataTable)outObject).NewRow();
            int count = reader.FieldCount;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    //ResultProperty property = new ResultProperty();
                    //property.PropertyName = "value";
                    //property.ColumnIndex = i;
                    //property.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(reader.GetFieldType(i));
                    //property.GetDataBaseValue(reader);
                    if (indexMapping[1, i] != -1)
                        dataRow[indexMapping[1, i]] = reader[indexMapping[0, i]] ?? DBNull.Value;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message + "\n" + e.StackTrace);
            }

            ((DataTable)outObject).Rows.Add(dataRow);

            return outObject;
        }

        #endregion
    }
}
