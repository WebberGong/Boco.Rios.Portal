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
    /// Dao��һ��ʵ�ֻ��࣬�������Ϊ��
    /// ��ѯ���󡢶��󼯺ϡ����ݱ����ݶ�ȡ��
    /// ��ʵ��ʹ����ibatis.net��O/R Mapper�����
    /// �滻
    /// </summary>
    [Serializable]
    public class BaseSqlMapDao : IDao
    {
        //�������ܴ�ʱ����ð�����ݣ������ڼ̳�������

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
        /// �Ƿ���Ҫ���������־
        /// ��ÿ�������DAO����ҵ�񳡾���
        /// �ڵ����������������Ͻ�������
        /// </summary>
        private bool needDirtyRead = true;

        protected bool NeedDirtyRead
        {
            set { needDirtyRead = value; }
        }

        /// <summary>
        /// ��ñ���SqlMap
        /// </summary>
        /// <returns>�����DAO��ص�SqlMap</returns>
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�������ѯ�����б�
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">��������</param>
        /// <returns>���ػ�õĶ���</returns>
        protected IList ExecuteQueryForList(string statementName, object parameterObject)
        {
            //ǿ�����ֵ����(Ϊ�˼�����ǰ�Ĵ��룬����Ҫ��)
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�������ѯ�����б�
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">��������</param>
        /// <param name="skipResults">�����¼��</param>
        /// <param name="maxResults">����¼��</param>
        /// <returns>���ػ�õĶ���</returns>
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�������ѯ�����б����ͣ�
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">��������</param>
        /// <returns>���ػ�õĶ���</returns>
        protected IList<T> ExecuteQueryForList<T>(string statementName, object parameterObject)
        {
            //ǿ�����ֵ����(Ϊ�˼�����ǰ�Ĵ��룬����Ҫ��)
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�������ѯ�����б����ͣ�
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">��������</param>
        /// <param name="skipResults">�����¼��</param>
        /// <param name="maxResults">����¼��</param>
        /// <returns>���ػ�õĶ���</returns>
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�������ѯ��ҳ�����б�[�ͻ��˷�ҳ]
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">��������</param>
        /// <param name="pageSize">ҳ��С</param>
        /// <returns>���ػ�õĶ���</returns>
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�������ѯ����
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">������</param>
        /// <returns>���ػ�õĶ���</returns>
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�������ѯ����
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">������</param>
        /// <returns>���ػ�õĶ���</returns>
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
        /// ��װ�˶�Ӧ��SqlMap�ķ��������¶������ݿ�
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">��������</param>
        /// <returns>���ظ��¼�¼��</returns>
        protected int ExecuteUpdate(string statementName, object parameterObject)
        {
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                if (statementName == "UpdateUserLastLoginTime")
                {
                    if (sqlMap != null && sqlMap.LocalSession != null && sqlMap.LocalSession.IsTransactionStart)
                    {
                        System.Diagnostics.Debug.WriteLine("����������UpdateUserLastLoginTime��" + sqlMap.LocalSession.IsTransactionStart);
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
        /// ��װ�˶�Ӧ��SqlMap�ķ����������¶������ݿ�
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">��������</param>
        /// <returns>���ض���</returns>
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
        /// ִ��ɾ������
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�����������ķ�ʽ��������
        /// </summary>
        /// <param name="statementNames"></param>
        /// <param name="parameterObjects"></param>
        /// <returns></returns>
        protected object[] ExecuteBatchInsert(IList<string> statementNames,
                                              IList<object> parameterObjects)
        {
            Debug.Assert(statementNames.Count ==
                         parameterObjects.Count, "���Ͳ����ĸ�����ƥ��");

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
        /// ��װ�˶�Ӧ��SqlMap�ķ�����������ķ�ʽ����ɾ��
        /// </summary>
        /// <param name="statementNames"></param>
        /// <param name="parameterObjects"></param>
        /// <returns></returns>
        protected object[] ExecuteBatchDelete(IList<string> statementNames,
                                              IList<object> parameterObjects)
        {
            Debug.Assert(statementNames.Count ==
                         parameterObjects.Count, "���Ͳ����ĸ�����ƥ��");

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
        /// ��װ�˶�Ӧ��SqlMap�ķ�����ִ�в�ѯ���������ݿ��
        /// </summary>
        /// <param name="statementName">�������</param>
        /// <param name="parameterObject">��������</param>
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
        /// ��װ�˶�Ӧ��SqlMap�ķ�����ִ�в�ѯ���������ݿ��
        /// </summary>
        /// <param name="statementName">�������</param>
        /// <param name="parameterObject">��������</param>
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
        /// ִ�����践�ؽ������ͨ��ѯ
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">����</param>
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
        /// ִ�в�ѯ��ֱ�ӷ���DataReader
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">����</param>
        /// <returns>���ذ�װ��SafeDataReader</returns>
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
        /// ��װ�˶�Ӧ��SqlMap�ķ������첽����ִ�в�ѯ���������ݿ��
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">������</param>
        /// <param name="postProcessTempDataHandler"></param>
        /// <param name="tempDataArrivalHandler"></param>
        /// <returns></returns>
        protected void AsynExecuteQueryForDataTable(string statementName,
                                                    object parameterObject,
                                                    PostProcessTempDataHandler postProcessTempDataHandler,
                                                    TempDataArrivalHandler tempDataArrivalHandler)
        {
            //ǿ�����ֵ����(Ϊ�˼�����ǰ�Ĵ��룬����Ҫ��)
            if (ForceMaxResults)
            {
                AsynExecuteQueryForDataTable(statementName, parameterObject,
                                             -1, MAXRESULTS,
                                             postProcessTempDataHandler, tempDataArrivalHandler);
            }
            else //����ִ��
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
                        //�����
                        DataTable resultTable = null;
                        int[,] indexMapping = null;

                        while (dr.Read())
                        {
                            if (resultTable == null)
                                resultTable = CreateSchemaTable(dr, out indexMapping);

                            #region �������

                            DataRow dataRow = resultTable.NewRow();
                            int count = dr.FieldCount;
                            for (int i = 0; i < count; i++)
                            {
                                if (indexMapping[1, i] != -1)
                                    dataRow[indexMapping[1, i]] = dr[indexMapping[0, i]] ?? DBNull.Value;
                            }
                            resultTable.Rows.Add(dataRow);

                            #endregion

                            //��ʱ�����ͳ�
                            if (resultTable.Rows.Count > TMPRESULT_SIZE)
                            {
                                //���ݺ���
                                if (postProcessTempDataHandler != null)
                                    postProcessTempDataHandler(resultTable);

                                tempDataArrivalHandler(resultTable);
                                resultTable = CreateSchemaTable(dr, out indexMapping);
                            }
                        }

                        //���ݺ���
                        if (postProcessTempDataHandler != null)
                            postProcessTempDataHandler(resultTable);

                        //��������ͳ�
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
        /// ��װ�˶�Ӧ��SqlMap�ķ������첽����ִ�в�ѯ���������ݿ��
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">������</param>
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
                    //�����
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

                        #region �������

                        DataRow dataRow = (resultTable).NewRow();
                        int count = dr.FieldCount;
                        for (int i = 0; i < count; i++)
                        {
                            if (indexMapping[1, i] != -1)
                                dataRow[indexMapping[1, i]] = dr[indexMapping[0, i]] ?? DBNull.Value;
                        }
                        (resultTable).Rows.Add(dataRow);

                        #endregion

                        //��ʱ�����ͳ�
                        if (resultTable.Rows.Count > TMPRESULT_SIZE)
                        {
                            //���ݺ���
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
                    //���ݺ���
                    if (postProcessTempDataHandler != null)
                        postProcessTempDataHandler(resultTable);

                    //��������ͳ�
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
        /// �������ݶ�ȡ���������ݱ�
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private DataTable CreateSchemaTable(SafeDataReader dataReader, out int[,] indexMapping)
        {
            var resultTable = new DataTable();

            //���ݽṹ��
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
        /// ��������д��
        /// </summary>
        /// <param name="targetTable">Ŀ���</param>
        /// <param name="sourceData">Դ����</param>
        public int BulkCopyAutoTransaction(string targetTable, DataTable sourceData)
        {
            Trace.WriteLine(DateTime.Now + ":" + "��ʼ�������ݵ����ݿ�");
            ISqlMapper sqlMap = GetLocalSqlMap();
            try
            {
                System.Threading.Thread.CurrentThread.IsBackground = true;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                int result = sqlMap.BulkCopyAutoTransaction(targetTable, sourceData);
                stopwatch.Stop();
                Trace.WriteLine(DateTime.Now + ":" + "��ɿ������ݵ����ݿ�");
                Trace.WriteLine("�������ݼ�¼��:" + result);
                Trace.WriteLine(string.Format("��ʱ{0}", stopwatch.Elapsed.TotalSeconds));
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
        /// ��������д��
        /// </summary>
        /// <param name="targetTable">Դ��</param>
        /// <param name="sourceData">Ŀ���</param>
        /// <param name="trans">����</param>
        /// <returns>����Ӱ������</returns>
        public int BulkCopyLocalTransaction(string targetTable, DataTable sourceData)
        {
            Trace.WriteLine(DateTime.Now + ":" + "��ʼ�������ݵ����ݿ�\n");
            ISqlMapper sqlMap = GetLocalSqlMap();

            try
            {
                System.Threading.Thread.CurrentThread.IsBackground = true;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                int result = sqlMap.BulkCopyLocalTransaction(targetTable, sourceData);
                stopwatch.Stop();
                Trace.WriteLine(DateTime.Now + ":" + "��ɿ������ݵ����ݿ�");
                Trace.WriteLine("�������ݼ�¼��:" + result);
                Trace.WriteLine(string.Format("��ʱ{0}", stopwatch.Elapsed.TotalSeconds));
                System.Threading.Thread.CurrentThread.IsBackground = false;
                return result;
            }
            catch (Exception e)
            {
                Trace.Write(e.Message + "\n" + e.StackTrace);
                if (sqlMap != null && sqlMap.LocalSession != null && sqlMap.LocalSession.IsTransactionStart)
                {
                    System.Diagnostics.Debug.WriteLine("������������" + sqlMap.LocalSession.IsTransactionStart);
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
        /// ֱ�ӽ������ļ����ǻ�׷�ӵ�Ŀ�����
        /// </summary>
        /// <param name="serverLocalFilePath">���ݿ����˵������ļ�</param>
        /// <param name="targetTableName">Զ�����ݱ�</param>
        protected bool LoadFrom(string serverLocalFilePath, string targetTableName)
        {
            return false;
        }

        /// <summary>
        /// ֱ�ӽ������ļ����ǻ�׷�ӵ�Ŀ�����
        /// </summary>
        /// <param name="localFilePath">���������ļ�</param>
        /// <param name="serverLocalFilePath">���ݿ����˵������ļ�</param>
        /// <param name="targetTableName">Զ�����ݱ�</param>
        protected bool LoadFrom(string localFilePath, string serverLocalFilePath,
                                string targetTableName)
        {
            return false;
        }

        /// <summary>
        /// ֱ�ӽ������ļ����ǻ�׷�ӵ�Ŀ�����
        /// </summary>
        /// <param name="localFilePath">���������ļ�</param>
        /// <param name="serverLocalFilePath">���ݿ����˵������ļ�</param>
        /// <param name="targetTableName">Զ�����ݱ�</param>
        /// <param name="fileTransProtocol">�ļ����䷽ʽ</param>
        protected bool LoadFrom(string localFilePath, string serverLocalFilePath,
                                string targetTableName, FileTransProtocol fileTransProtocol)
        {
            //IUploader uploader = Uploader.NewUploader(localFilePath,
            //    serverLocalFilePath);
            //uploader.

            return false;
        }

        /// <summary>
        /// ֱ�ӽ������ļ����ǻ�׷�ӵ�Ŀ�����
        /// </summary>
        /// <param name="ftpServer">���ݿ������</param>
        /// <param name="ftpPort">���ݿ�˿�</param>
        /// <param name="remoteDir">������Ŀ¼</param>
        /// <param name="userName">�û���</param>
        /// <param name="password">����</param>
        /// <param name="localFilePath">�����ļ�</param>
        /// <param name="targetTableName">���ݿ��</param>
        /// <returns>�����Ƿ�ɹ�</returns>
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
                //////////�ϴ��ļ�
                ////////using (FTP ftp = FTP.CreateFTP(ftpServer, ftpPort, 
                ////////    userName, password, remoteDir))
                ////////{
                ////////    ftp.OpenUpload(localFilePath);

                ////////    while (ftp.DoUpload() > 0)
                ////////    {
                ////////        int i = (int)(((ftp.BytesTotal) * 100) / ftp.FileSize);
                ////////    }
                ////////}

                //////////ִ�е���
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
        /// ������ʱ���ҷ�����ʱ������
        /// </summary>
        /// <returns>���ش�������ʱ������</returns>
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
        /// ������ʱ���ҷ�����ʱ������
        /// </summary>
        /// <returns>���ش�������ʱ������</returns>
        public string CreateTempNENameTable(string name)
        {
            //������ʱ��
            string tempTableName = GetTempTableName(name);
            ExecuteQuery("CreateTempNameTable", tempTableName);

            return tempTableName;
        }

        /// <summary>
        /// ������ʱ���ҷ�����ʱ������
        /// </summary>
        /// <returns>���ش�������ʱ������</returns>
        public string CreateTempTimeIDTable(string name)
        {
            //������ʱ��
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
        /// ɾ����ʱ��
        /// </summary>
        /// <param name="tableName">����</param>
        /// <returns>����ɾ���Ƿ�ɹ�</returns>
        public bool DropTempTable(string tableName)
        {
            Debug.WriteLine("DropTempTable" + tableName);
            ExecuteQuery("DropTempTable", tableName);

            return true;
        }

        /// <summary>
        /// �Ƿ�����������������ļ�������
        /// "NeedDirtyRead"������ʱ���ã���������
        /// </summary>
        /// <returns></returns>
        private bool ContainsDirtyReadStatement()
        {
            return _sqlMapper.MappedStatements.Contains("NeedDirtyRead");
        }

        /// <summary>
        /// �Ƿ���Ҫʹ��������ʱID��
        /// </summary>
        /// <returns></returns>
        private bool UsingFixedTmpTable()
        {
            return _sqlMapper.MappedStatements.Contains("UsingFixedTmpTable");
        }

        /// <summary>
        /// Ϊֻ����ѯ�������
        /// </summary>
        private void SetDirtyRead()
        {
            SetIsoLevel(IsolationLevel.ReadUncommitted);
        }

        /// <summary>
        /// ��ԭ���
        /// </summary>
        private void ResetDirtyRead()
        {
            SetIsoLevel(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// ���ø��뼶��
        /// </summary>
        /// <param name="level">���뼶��</param>
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
        /// ��ȡһ�������
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
        /// ��ʱ���ݵ���
        /// </summary>
        /// <param name="dt">���ݱ�</param>
        //private void FireTempDataArrival(object dt)
        //{
        //    if (TempDataArrival != null)
        //        TempDataArrival(dt);
        //}

        //��ʱ���ݵ������
        //public delegate void TempDataArrivalHandler(DataTable dt);
        //public event TempDataArrivalHandler TempDataArrival;
    }

    /// <summary>
    /// ��ʱ���ݷ��ش���
    /// </summary>
    /// <param name="dt"></param>
    public delegate void TempDataArrivalHandler(object dt);

    /// <summary>
    /// ��ʱ���ݷ��غ���
    /// </summary>
    /// <param name="dt"></param>
    public delegate void PostProcessTempDataHandler(object dt);
}