/*****************************************************************************
 * Added by Qingfa
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
    /// <summary>
    /// <see cref="IPostSelectStrategy"/> implementation to exceute a query for datatable.
    /// </summary>
    public class DataTableStrategy:IPostSelectStrategy
    {
        #region IPostSelectStrategy ≥…‘±

        /// <summary>
        /// Executes the specified <see cref="PostBindind"/>.
        /// </summary>
        /// <param name="postSelect">The <see cref="PostBindind"/>.</param>
        /// <param name="request">The <see cref="RequestScope"/></param>
        public void Execute(PostBindind postSelect, IBatisNet.DataMapper.Scope.RequestScope request)
        {
            DataTable value = postSelect.Statement.ExecuteQueryForDataTable(request.Session, postSelect.Keys);
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, value);
        }

        #endregion
    }
}
