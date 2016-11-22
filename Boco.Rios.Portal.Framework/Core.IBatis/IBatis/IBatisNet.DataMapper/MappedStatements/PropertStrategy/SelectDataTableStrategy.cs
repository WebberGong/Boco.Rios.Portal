using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;


namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    /// <summary>
    /// <see cref="IPropertyStrategy"/> implementation when a 'select' attribute exists
    /// on an <see cref="ResultProperty"/> and the object property is 
    /// different from an <see cref="Array"/> or an <see cref="IList"/>.
    /// </summary>
    public sealed class SelectDataTableStrategy : IPropertyStrategy
    {

        #region IPropertyStrategy members

        ///<summary>
        /// Sets value of the specified <see cref="ResultProperty"/> on the target object
        /// when a 'select' attribute exists and fills an object property.
        /// on the <see cref="ResultProperty"/> are empties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="mapping">The ResultProperty.</param>
        /// <param name="target">The target.</param>
        /// <param name="reader">The current <see cref="IDataReader"/></param>
        /// <param name="keys">The keys</param>
        public void Set(RequestScope request, IResultMap resultMap,
            ResultProperty mapping, ref object target, IDataReader reader, object keys)
        {
            // Get the select statement
            IMappedStatement selectStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);

            PostBindind postSelect = new PostBindind();
            postSelect.Statement = selectStatement;
            postSelect.Keys = keys;
            postSelect.Target = target;
            postSelect.ResultProperty = mapping;

            postSelect.Method = PostBindind.ExecuteMethod.ExecuteQueryForDataTable;
            request.QueueSelect.Enqueue(postSelect);
        }
        
        /// <summary>
        /// Gets the value of the specified <see cref="ResultProperty"/> that must be set on the target object.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="target">The target object</param>
        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            throw new NotSupportedException("Get method on ResultMapStrategy is not supported");
        }	    
        #endregion
    }
}
