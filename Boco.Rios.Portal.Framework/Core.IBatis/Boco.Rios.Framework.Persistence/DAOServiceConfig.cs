using System;
using System.Collections.Generic;
using System.Text;

using IBatisNet.Common.Utilities;
using IBatisNet.DataAccess;
using IBatisNet.DataAccess.Configuration;

namespace Boco.Rios.Framework.Persistence
{
    /// <summary>
    /// DaoService�����ü���ࣨ����ģʽ��
    /// </summary>
    public class DAOServiceConfig
    {
        static private object _synRoot = new Object();
        static private DAOServiceConfig _instance;

        //DaoManager
		//private IDaoManager _daoManager = null;

		/// <summary>
		/// ˽�й�����
		/// </summary>
        private DAOServiceConfig() { }

        /// <summary>
        /// ���ʵ��
        /// </summary>
        static public DAOServiceConfig Default
		{
            get
            {
                if (_instance == null)
                {
                    lock (_synRoot)
                    {
                        if (_instance == null)
                        {
                            ConfigureHandler handler = Reset;
                            try
                            {
                                //���������ļ�������д��������
                                new DomDaoManagerBuilder().ConfigureAndWatch(handler);
                                //IBatisNet.DataAccess.DaoManager.ConfigureAndWatch(handler);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                throw;
                            }

                            _instance = new DAOServiceConfig();
                        }
                    }
                }
                return _instance;
            }
		}
        
		/// <summary>
		/// ���ñ���������
		/// </summary>
		/// <remarks>
		/// ��Ҫ��֤����ǩ��
		/// </remarks>
		/// <param name="obj">
		/// </param>
		static public void Reset(object obj)
		{
			_instance =null;
		}

        /// <summary>
        /// ���Ĭ�ϵ�DaoManager
        /// </summary>
		public IDaoManager DaoManager
		{
			get
			{
				return IBatisNet.DataAccess.DaoManager.GetInstance();
			}
		}

        /// <summary>
        /// ���DaoManager
        /// </summary>
        /// <param name="contextName">����������</param>
        /// <returns></returns>
        public IDaoManager GetDaoManager(string contextName)
        {
            return IBatisNet.DataAccess.DaoManager.GetInstance(contextName);
        }
    }
}
