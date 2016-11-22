
#region Apache Notice
/*****************************************************************************
 * $Header: $
 * $Revision: 383115 $
 * $Date: 2006-03-04 22:21:51 +0800 (星期�? 04 三月 2006) $
 * 
 * iBATIS.NET Data Mapper
 * Copyright (C) 2004 - Gilles Bayon
 *  
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

namespace IBatisNet.DataAccess.Interfaces
{
	/// <summary>
	/// Mark a class as a Dao object.
	/// </summary>
	public interface IDao
	{
        /// <summary>
        /// 
        /// </summary>
        //event TempDataArrivalHandler TempDataArrival;
	}

    /// <summary>
    /// 
    /// </summary>
    public interface ICRUDDao
    {
        /// <summary>
        /// ִ�в�ѯ��ֱ�ӷ���DataReader
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">����</param>
        /// <returns>���ذ�װ��SafeDataReader</returns>
        SafeDataReader ExecuteSafeDataReader(string statementName, object parameterObject);

        /// <summary>
        /// ִ�в���
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">����</param>
        void ExecuteDaoInsert(string statementName, object parameterObject);

        /// <summary>
        /// ִ�и���
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">����</param>
        void ExecuteDaoUpdate(string statementName, object parameterObject);

        /// <summary>
        /// ִ��ɾ��
        /// </summary>
        /// <param name="statementName">�����</param>
        /// <param name="parameterObject">����</param>
        void ExecuteDaoDelete(string statementName, object parameterObject);
    }
}
