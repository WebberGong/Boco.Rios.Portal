using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Pagination;
using IBatisNet.DataAccess;
using IBatisNet.DataAccess.Configuration;
using IBatisNet.DataAccess.DaoSessionHandlers;
using IBatisNet.DataAccess.Interfaces;
using IBatisNet.DataMapper;

namespace Boco.Rios.Framework.Persistence
{
    /// <summary>
    /// Dao的一个实现基类，本类的行为：
    /// 查询对象、对象集合、数据表、数据读取器
    /// 本实现使用了ibatis.net的O/R Mapper和语句
    /// 替换
    /// </summary>
    [Serializable]
    public class BaseSqlMapDao : IDao
    {
        //数据量很大时分批冒出数据，可以在继承类重置

        protected const int PAGE_SIZE = 4;

        // Magic number used to set the the maximum number of rows returned to 'all'. 
        internal const int NO_MAXIMUM_RESULTS = -1;
        // Magic number used to set the the number of rows skipped to 'none'. 
        internal const int NO_SKIPPED_RESULTS = -1;
        private static readonly Hashtable randomHash = new Hashtable();
        private static readonly Random random = new Random(ushort.MaxValue);

        protected bool ForceMaxResults =
            ConfigurationSettings.AppSettings["ForceMaxResults"] == null
                ? false
                : true;

        protected Int32 MAXRESULTS =
            ConfigurationSettings.AppSettings["MaxResults"] == null
                ? 100000
                : Int32.Parse(ConfigurationSettings.AppSettings["MaxResults"]);

        protected int TMPRESULT_SIZE = 99;

        private ISqlMapper _sqlMapper;

        /// <summary>
        /// 是否需要设置脏读标志
        /// 由每个具体的DAO根据业务场景，
        /// 在单个方法或者整体上进行设置
        /// </summary>
        private bool needDirtyRead = true;

        protected bool NeedDirtyRead
        {
            set { needDirtyRead = value; }
        }

        /// <summary>
        /// 获得本地SqlMap
        /// </summary>
        /// <returns>获得与DAO相关的SqlMap</returns>
        protected ISqlMapper GetLocalSqlMap()
        {
            if (_sqlMapper == null)
            {
                //DomDaoManagerBuilder builder = new DomDaoManagerBuilder();
                //builder.Configure();
                IDaoManager daoManager = DaoManager.GetInstance(this);
                if (daoManager == null)
                {
                    var builder = new DomDaoManagerBuilder();
                    builder.Configure();
                    daoManager = DaoManager.GetInstance(this);
                }

                var sqlMapDaoSession = (SqlMapDaoSession) daoManager.LocalDaoSession;

                _sqlMapper = sqlMapDaoSession.SqlMap;
                _sqlMapper.DataSource.ConnectionString = _sqlMapper.DataSource.ConnectionString;
                    //SymmetricEncryption.DecryptString(_sqlMapper.DataSource.ConnectionString);
            }

            return _sqlMapper;
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，查询对象列表
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数集合</param>
        /// <returns>返回获得的对象集</returns>
        protected IList ExecuteQueryForList(string statementName, object parameterObject)
        {
            //强制最大值返回(为了兼容以前的代码，后续要改)
            if (ForceMaxResults)
            {
                return ExecuteQueryForList(statementName, parameterObject, -1, MAXRESULTS);
            }
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                {
                    SetDirtyRead();
                }
                return sqlMap.QueryForList(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for list.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，查询对象列表
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数集合</param>
        /// <param name="skipResults">间隔记录数</param>
        /// <param name="maxResults">最大记录数</param>
        /// <returns>返回获得的对象集</returns>
        protected IList ExecuteQueryForList(string statementName, object parameterObject, int skipResults,
                                            int maxResults)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForList(statementName, parameterObject, skipResults, maxResults);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for list.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，查询对象列表（范型）
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数对象</param>
        /// <returns>返回获得的对象集</returns>
        protected IList<T> ExecuteQueryForList<T>(string statementName, object parameterObject)
        {
            //强制最大值返回(为了兼容以前的代码，后续要改)
            if (ForceMaxResults)
            {
                return ExecuteQueryForList<T>(statementName, parameterObject, -1, MAXRESULTS);
            }
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForList<T>(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for list.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// Execute SQL with batch result list return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="statementName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="batch"></param>
        /// <param name="listDelegate"></param>
        /// <returns></returns>
        protected IList<T> ExecuteQueryForList<T>(string statementName, object parameterObject, int batch,
                                                  ResultListDelegate<T> listDelegate)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForList(statementName, parameterObject, batch, listDelegate);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for list.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，查询对象列表（范型）
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数集合</param>
        /// <param name="skipResults">间隔记录数</param>
        /// <param name="maxResults">最大记录数</param>
        /// <returns>返回获得的对象集</returns>
        protected IList<T> ExecuteQueryForList<T>(string statementName, object parameterObject, int skipResults,
                                                  int maxResults)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForList<T>(statementName, parameterObject, skipResults, maxResults);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for list.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，查询分页对象列表[客户端分页]
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数对象</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>返回获得的对象集</returns>
        protected IPaginatedList ExecuteQueryForPaginatedList(string statementName, object parameterObject, int pageSize)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForPaginatedList(statementName, parameterObject, pageSize);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for paginated list.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，查询对象
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数名</param>
        /// <returns>返回获得的对象</returns>
        protected object ExecuteQueryForObject(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForObject(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for object.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，查询对象
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数名</param>
        /// <returns>返回获得的对象</returns>
        protected T ExecuteQueryForObject<T>(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForObject<T>(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for object.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，更新对象到数据库
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数对象</param>
        /// <returns>返回更新记录数</returns>
        protected int ExecuteUpdate(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                if (statementName == "UpdateUserLastLoginTime")
                {
                    if (sqlMap != null && sqlMap.LocalSession != null && sqlMap.LocalSession.IsTransactionStart)
                    {
                        System.Diagnostics.Debug.WriteLine("有事务启动UpdateUserLastLoginTime：" + sqlMap.LocalSession.IsTransactionStart);
                    }
                }
                return sqlMap.Update(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for update.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，插入新对象到数据库
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数对象</param>
        /// <returns>返回对象</returns>
        protected object ExecuteInsert(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                return sqlMap.Insert(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for insert.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 执行删除操作
        /// </summary>
        /// <param name="statementName"></param>
        /// <param name="parameterObject"></param>
        protected int ExecuteDelete(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                return sqlMap.Delete(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for delete.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，以事务的方式批量插入
        /// </summary>
        /// <param name="statementNames"></param>
        /// <param name="parameterObjects"></param>
        /// <returns></returns>
        protected object[] ExecuteBatchInsert(IList<string> statementNames,
                                              IList<object> parameterObjects)
        {
            Debug.Assert(statementNames.Count ==
                         parameterObjects.Count, "语句和参数的个数不匹配");

            ISqlMapper sqlMap = GetLocalSqlMap();
            var results = new object[statementNames.Count];

            if (sqlMap.IsSessionStarted) sqlMap.CloseConnection();

            using (ISqlMapSession sqlMapSession = sqlMap.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < statementNames.Count; i++)
                    {
                        results[i] = sqlMap.Insert(statementNames[i], parameterObjects[i]);
                    }
                    sqlMapSession.CommitTransaction();
                }
                catch (Exception e)
                {
                    Trace.Write(e.Message + "\n" + e.StackTrace);
                    sqlMapSession.RollBackTransaction();

                    throw new IBatisNetException(
                        "Error executing batch query '" + statementNames[0] + "' for insert.  Cause: " + e.Message, e);
                }
            }

            return results;
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，以事务的方式批量删除
        /// </summary>
        /// <param name="statementNames"></param>
        /// <param name="parameterObjects"></param>
        /// <returns></returns>
        protected object[] ExecuteBatchDelete(IList<string> statementNames,
                                              IList<object> parameterObjects)
        {
            Debug.Assert(statementNames.Count ==
                         parameterObjects.Count, "语句和参数的个数不匹配");

            ISqlMapper sqlMap = GetLocalSqlMap();
            var results = new object[statementNames.Count];

            if (sqlMap.IsSessionStarted) sqlMap.CloseConnection();

            using (ISqlMapSession sqlMapSession = sqlMap.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < statementNames.Count; i++)
                    {
                        results[i] = sqlMap.Delete(statementNames[i], parameterObjects[i]);
                    }
                    sqlMapSession.CommitTransaction();
                }
                catch (Exception e)
                {
                    Trace.Write(e.Message + "\n" + e.StackTrace);
                    sqlMapSession.RollBackTransaction();

                    throw new IBatisNetException(
                        "Error executing batch query '" + statementNames[0] + "' for delete.  Cause: " + e.Message, e);
                }
            }

            return results;
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，执行查询并返回数据库表
        /// </summary>
        /// <param name="statementName">语句名称</param>
        /// <param name="parameterObject">参数对象</param>
        /// <returns></returns>
        protected DataTable ExecuteQueryForDataTable(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForDataTable(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for object.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，执行查询并返回数据库表
        /// </summary>
        /// <param name="statementName">语句名称</param>
        /// <param name="parameterObject">参数对象</param>
        /// <param name="skipResults"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        protected DataTable ExecuteQueryForDataTable(string statementName, object parameterObject, int skipResults,
                                                     int maxResults)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return sqlMap.QueryForDataTable(statementName, parameterObject, skipResults, maxResults);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for object.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 执行无需返回结果的普通查询
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数</param>
        protected void ExecuteQuery(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                sqlMap.Update(statementName, parameterObject);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for object.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 执行查询并直接返回DataReader
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数</param>
        /// <returns>返回包装的SafeDataReader</returns>
        protected SafeDataReader ExecuteDataReader(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                return new SafeDataReader(sqlMap.ExecuteDataReader(statementName, parameterObject));
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for object.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，异步分批执行查询并返回数据库表
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数名</param>
        /// <param name="postProcessTempDataHandler"></param>
        /// <param name="tempDataArrivalHandler"></param>
        /// <returns></returns>
        protected void AsynExecuteQueryForDataTable(string statementName,
                                                    object parameterObject,
                                                    PostProcessTempDataHandler postProcessTempDataHandler,
                                                    TempDataArrivalHandler tempDataArrivalHandler)
        {
            //强制最大值返回(为了兼容以前的代码，后续要改)
            if (ForceMaxResults)
            {
                AsynExecuteQueryForDataTable(statementName, parameterObject,
                                             -1, MAXRESULTS,
                                             postProcessTempDataHandler, tempDataArrivalHandler);
            }
            else //正常执行
            {
                ISqlMapper sqlMap = GetLocalSqlMap();

                SafeDataReader dr = null;

                try
                {
                    if (needDirtyRead && ContainsDirtyReadStatement())
                        SetDirtyRead();

                    dr = ExecuteDataReader(statementName, parameterObject);

                    //using (SafeDataReader dr = ExecuteDataReader(statementName, parameterObject))
                    {
                        //输出表
                        DataTable resultTable = null;
                        int[,] indexMapping = null;

                        while (dr.Read())
                        {
                            if (resultTable == null)
                                resultTable = CreateSchemaTable(dr, out indexMapping);

                            #region 填充数据

                            DataRow dataRow = resultTable.NewRow();
                            int count = dr.FieldCount;
                            for (int i = 0; i < count; i++)
                            {
                                if (indexMapping[1, i] != -1)
                                    dataRow[indexMapping[1, i]] = dr[indexMapping[0, i]] ?? DBNull.Value;
                            }
                            resultTable.Rows.Add(dataRow);

                            #endregion

                            //临时数据送出
                            if (resultTable.Rows.Count > TMPRESULT_SIZE)
                            {
                                //数据后处理
                                if (postProcessTempDataHandler != null)
                                    postProcessTempDataHandler(resultTable);

                                tempDataArrivalHandler(resultTable);
                                resultTable = CreateSchemaTable(dr, out indexMapping);
                            }
                        }

                        //数据后处理
                        if (postProcessTempDataHandler != null)
                            postProcessTempDataHandler(resultTable);

                        //最后数据送出
                        tempDataArrivalHandler(resultTable);
                        //FireTempDataArrival(resultTable);
                    }
                }
                catch (Exception e)
                {
                    Trace.Write(e.Message + "\n" + e.StackTrace);
                    throw new IBatisNetException(
                        "Error executing query '" + statementName + "' for object.  Cause: " + e.Message, e);
                }
                finally
                {
                    if (dr != null) dr.Dispose();
                }
            }
        }

        /// <summary>
        /// 封装了对应的SqlMap的方法，异步分批执行查询并返回数据库表
        /// </summary>
        /// <param name="statementName">语句名</param>
        /// <param name="parameterObject">参数名</param>
        /// <param name="skipResults"></param>
        /// <param name="maxResults"></param>
        /// <param name="postProcessTempDataHandler"></param>
        /// <param name="tempDataArrivalHandler"></param>
        /// <returns></returns>
        protected void AsynExecuteQueryForDataTable(string statementName,
                                                    object parameterObject,
                                                    int skipResults,
                                                    int maxResults,
                                                    PostProcessTempDataHandler postProcessTempDataHandler,
                                                    TempDataArrivalHandler tempDataArrivalHandler)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            SafeDataReader dr = null;

            try
            {
                if (needDirtyRead && ContainsDirtyReadStatement())
                    SetDirtyRead();

                dr = ExecuteDataReader(statementName, parameterObject);

                //using (SafeDataReader dr = ExecuteDataReader(statementName, parameterObject))
                {
                    //输出表
                    DataTable resultTable = null;
                    int[,] indexMapping = null;

                    // skip results
                    for (int i = 0; i < skipResults; i++)
                    {
                        if (!dr.Read())
                        {
                            break;
                        }
                    }

                    int resultsFetched = 0;
                    while ((maxResults == NO_MAXIMUM_RESULTS || resultsFetched < maxResults)
                           && dr.Read())
                    {
                        //while (dr.Read())
                        if (resultTable == null)
                            resultTable = CreateSchemaTable(dr, out indexMapping);

                        #region 填充数据

                        DataRow dataRow = (resultTable).NewRow();
                        int count = dr.FieldCount;
                        for (int i = 0; i < count; i++)
                        {
                            if (indexMapping[1, i] != -1)
                                dataRow[indexMapping[1, i]] = dr[indexMapping[0, i]] ?? DBNull.Value;
                        }
                        (resultTable).Rows.Add(dataRow);

                        #endregion

                        //临时数据送出
                        if (resultTable.Rows.Count > TMPRESULT_SIZE)
                        {
                            //数据后处理
                            if (postProcessTempDataHandler != null)
                                postProcessTempDataHandler(resultTable);

                            tempDataArrivalHandler(resultTable);
                            resultTable = CreateSchemaTable(dr, out indexMapping);
                        }

                        resultsFetched++;
                    }

                    if (resultTable == null)
                    {
                        DataTable dataColumn = dr.GetSchemaTable();

                        if (dataColumn != null)
                        {
                            resultTable = new DataTable("statementName");

                            for (int i = 0; i < dataColumn.Rows.Count; i++)
                            {
                                resultTable.Columns.Add(dataColumn.Rows[i]["ColumnName"].ToString(),
                                                        (Type) dataColumn.Rows[i]["DataType"]);
                            }
                        }
                    }
                    //数据后处理
                    if (postProcessTempDataHandler != null)
                        postProcessTempDataHandler(resultTable);

                    //最后数据送出
                    tempDataArrivalHandler(resultTable);
                    //FireTempDataArrival(resultTable);
                }
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                throw new IBatisNetException(
                    "Error executing query '" + statementName + "' for object.  Cause: " + e.Message, e);
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }
        }

        /// <summary>
        /// 根据数据读取器创建数据表
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private DataTable CreateSchemaTable(SafeDataReader dataReader, out int[,] indexMapping)
        {
            var resultTable = new DataTable();

            //数据结构表
            DataTable dataColumn = dataReader.GetSchemaTable();
            indexMapping = new int[2,dataColumn.Rows.Count];
            for (int i = 0, j = 0; i < dataColumn.Rows.Count; i++)
            {
                indexMapping[0, i] = i;
                indexMapping[1, i] = -1;

                if (!resultTable.Columns.Contains(dataColumn.Rows[i]["ColumnName"].ToString()))
                {
                    resultTable.Columns.Add(dataColumn.Rows[i]["ColumnName"].ToString(),
                                            (Type) dataColumn.Rows[i]["DataType"]);

                    indexMapping[1, i] = j++;
                }
            }

            return resultTable;
        }

        protected DataTable CreateSchemaTable(string statementName, object parameterObject, out int[,] indexMapping)
        {
            return CreateSchemaTable(ExecuteDataReader(statementName, parameterObject), out indexMapping);
        }

        /// <summary>
        /// 批量数据写入
        /// </summary>
        /// <param name="targetTable">目标表</param>
        /// <param name="sourceData">源数据</param>
        public int BulkCopyAutoTransaction(string targetTable, DataTable sourceData)
        {
            Trace.WriteLine(DateTime.Now + ":" + "开始拷贝数据到数据库");
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                System.Threading.Thread.CurrentThread.IsBackground = true;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                int result = sqlMap.BulkCopyAutoTransaction(targetTable, sourceData);
                stopwatch.Stop();
                Trace.WriteLine(DateTime.Now + ":" + "完成拷贝数据到数据库");
                Trace.WriteLine("拷贝数据记录数:" + result);
                Trace.WriteLine(string.Format("耗时{0}", stopwatch.Elapsed.TotalSeconds));
                System.Threading.Thread.CurrentThread.IsBackground = false;
                return result;
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                System.Threading.Thread.CurrentThread.IsBackground = false;
                throw new IBatisNetException(
                    "Error executing bulkcopy '" + targetTable + "' for object.  Cause: " + e.Message, e);
            }
            finally
            {
                //sqlMap.CloseConnection();
            }
        }

        /// <summary>
        /// 批量数据写入
        /// </summary>
        /// <param name="targetTable">源表</param>
        /// <param name="sourceData">目标表</param>
        /// <param name="trans">事务</param>
        /// <returns>返回影响行数</returns>
        public int BulkCopyLocalTransaction(string targetTable, DataTable sourceData)
        {
            Trace.WriteLine(DateTime.Now + ":" + "开始拷贝数据到数据库\n");
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                System.Threading.Thread.CurrentThread.IsBackground = true;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                int result = sqlMap.BulkCopyLocalTransaction(targetTable, sourceData);
                stopwatch.Stop();
                Trace.WriteLine(DateTime.Now + ":" + "完成拷贝数据到数据库");
                Trace.WriteLine("拷贝数据记录数:" + result);
                Trace.WriteLine(string.Format("耗时{0}", stopwatch.Elapsed.TotalSeconds));
                System.Threading.Thread.CurrentThread.IsBackground = false;
                return result;
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                if (sqlMap != null && sqlMap.LocalSession != null && sqlMap.LocalSession.IsTransactionStart)
                {
                    System.Diagnostics.Debug.WriteLine("有事务启动：" + sqlMap.LocalSession.IsTransactionStart);
                }
                System.Threading.Thread.CurrentThread.IsBackground = false;
                //////try
                //////{
                //////    sqlMap.RollBackTransaction(true);
                //////}
                //////catch (Exception ex) { Trace.WriteLine(ex.Message + "\n" + ex.StackTrace); }
                throw new IBatisNetException(
                    "Error executing bulkcopy '" + targetTable + "' for object.  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// 直接将数据文件覆盖或追加到目标表中
        /// </summary>
        /// <param name="serverLocalFilePath">数据库服务端的数据文件</param>
        /// <param name="targetTableName">远程数据表</param>
        protected bool LoadFrom(string serverLocalFilePath, string targetTableName)
        {
            return false;
        }

        /// <summary>
        /// 直接将数据文件覆盖或追加到目标表中
        /// </summary>
        /// <param name="localFilePath">本地数据文件</param>
        /// <param name="serverLocalFilePath">数据库服务端的数据文件</param>
        /// <param name="targetTableName">远程数据表</param>
        protected bool LoadFrom(string localFilePath, string serverLocalFilePath,
                                string targetTableName)
        {
            return false;
        }

        /// <summary>
        /// 直接将数据文件覆盖或追加到目标表中
        /// </summary>
        /// <param name="localFilePath">本地数据文件</param>
        /// <param name="serverLocalFilePath">数据库服务端的数据文件</param>
        /// <param name="targetTableName">远程数据表</param>
        /// <param name="fileTransProtocol">文件传输方式</param>
        protected bool LoadFrom(string localFilePath, string serverLocalFilePath,
                                string targetTableName, FileTransProtocol fileTransProtocol)
        {
            //IUploader uploader = Uploader.NewUploader(localFilePath,
            //    serverLocalFilePath);
            //uploader.

            return false;
        }

        /// <summary>
        /// 直接将数据文件覆盖或追加到目标表中
        /// </summary>
        /// <param name="ftpServer">数据库服务器</param>
        /// <param name="ftpPort">数据库端口</param>
        /// <param name="remoteDir">服务器目录</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="localFilePath">本地文件</param>
        /// <param name="targetTableName">数据库表</param>
        /// <returns>返回是否成功</returns>
        protected bool LoadFrom(string ftpServer,
                                int ftpPort,
                                string userName,
                                string password,
                                string remoteDir,
                                string localFilePath,
                                string targetTableName)
        {
            try
            {
                //////////上传文件
                ////////using (FTP ftp = FTP.CreateFTP(ftpServer, ftpPort, 
                ////////    userName, password, remoteDir))
                ////////{
                ////////    ftp.OpenUpload(localFilePath);

                ////////    while (ftp.DoUpload() > 0)
                ////////    {
                ////////        int i = (int)(((ftp.BytesTotal) * 100) / ftp.FileSize);
                ////////    }
                ////////}

                //////////执行导入
                ////////Hashtable param = new Hashtable(2);
                ////////param.Add("FileName", localFilePath);
                ////////param.Add("TargetTableName", targetTableName);
                ////////ExecuteQuery("LoadFrom", param);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + ex.StackTrace);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 创建临时表并且返回临时表名称
        /// </summary>
        /// <returns>返回创建的临时表名称</returns>
        public string CreateTempNEIDTable(string name)
        {
            string tempTableName = GetTempTableName(name);
            Debug.WriteLine("CreateTempIDTable" + tempTableName + name);
            try
            {
                ExecuteQuery("CreateTempIDTable", tempTableName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return tempTableName;
        }

        /// <summary>
        /// 创建临时表并且返回临时表名称
        /// </summary>
        /// <returns>返回创建的临时表名称</returns>
        public string CreateTempNENameTable(string name)
        {
            //构造临时表
            string tempTableName = GetTempTableName(name);
            ExecuteQuery("CreateTempNameTable", tempTableName);

            return tempTableName;
        }

        /// <summary>
        /// 创建临时表并且返回临时表名称
        /// </summary>
        /// <returns>返回创建的临时表名称</returns>
        public string CreateTempTimeIDTable(string name)
        {
            //构造临时表
            string tempTableName = GetTempTableName(name);
            try
            {
                ExecuteQuery("CreateTempTimeTable", tempTableName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return tempTableName;
        }

        public string GetTempTableName(string name)
        {
            return name ?? "tmp" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + GetRandomShortInt();
        }

        /// <summary>
        /// 删除临时表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>返回删除是否成功</returns>
        public bool DropTempTable(string tableName)
        {
            Debug.WriteLine("DropTempTable" + tableName);
            ExecuteQuery("DropTempTable", tableName);

            return true;
        }

        /// <summary>
        /// 是否设置脏读，当配置文件设置了
        /// "NeedDirtyRead"脏读语句时启用，否则不启用
        /// </summary>
        /// <returns></returns>
        private bool ContainsDirtyReadStatement()
        {
            return _sqlMapper.MappedStatements.Contains("NeedDirtyRead");
        }

        /// <summary>
        /// 是否需要使用永久临时ID表
        /// </summary>
        /// <returns></returns>
        private bool UsingFixedTmpTable()
        {
            return _sqlMapper.MappedStatements.Contains("UsingFixedTmpTable");
        }

        /// <summary>
        /// 为只读查询设置脏读
        /// </summary>
        private void SetDirtyRead()
        {
            SetIsoLevel(IsolationLevel.ReadUncommitted);
        }

        /// <summary>
        /// 复原脏读
        /// </summary>
        private void ResetDirtyRead()
        {
            SetIsoLevel(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 设置隔离级别
        /// </summary>
        /// <param name="level">隔离级别</param>
        private void SetIsoLevel(IsolationLevel level)
        {
            switch (level)
            {
                case IsolationLevel.ReadCommitted:
                    _sqlMapper.ExecuteSetConfig("SetReadCommited", null);
                    break;
                case IsolationLevel.ReadUncommitted:
                    _sqlMapper.ExecuteSetConfig("NeedDirtyRead", null);
                    break;
            }
        }

        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <returns></returns>
        private ushort GetRandomShortInt()
        {
            var randomNum = (ushort) random.Next(ushort.MinValue, ushort.MaxValue);

            while (randomHash.ContainsKey(randomNum))
            {
                randomNum = (ushort) random.Next(ushort.MinValue, ushort.MaxValue);
            }
            if (randomHash.Count >= 10000)
                randomHash.Clear();
            randomHash.Add(randomNum, randomNum);

            return randomNum;
        }

        #region Nested type: FileTransProtocol

        protected enum FileTransProtocol
        {
            ftp = 0,
            http = 1,
            tcp = 2
        } ;

        #endregion

        /// <summary>
        /// 临时数据到达
        /// </summary>
        /// <param name="dt">数据表</param>
        //private void FireTempDataArrival(object dt)
        //{
        //    if (TempDataArrival != null)
        //        TempDataArrival(dt);
        //}

        //临时数据到达代理
        //public delegate void TempDataArrivalHandler(DataTable dt);
        //public event TempDataArrivalHandler TempDataArrival;
    }

    /// <summary>
    /// 临时数据返回代理
    /// </summary>
    /// <param name="dt"></param>
    public delegate void TempDataArrivalHandler(object dt);

    /// <summary>
    /// 临时数据返回后处理
    /// </summary>
    /// <param name="dt"></param>
    public delegate void PostProcessTempDataHandler(object dt);
}