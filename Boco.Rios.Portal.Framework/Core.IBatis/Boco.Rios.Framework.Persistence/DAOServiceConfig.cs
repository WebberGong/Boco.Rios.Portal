using System;
using System.Collections.Generic;
using System.Text;

using IBatisNet.Common.Utilities;
using IBatisNet.DataAccess;
using IBatisNet.DataAccess.Configuration;

namespace Boco.Rios.Framework.Persistence
{
    /// <summary>
    /// DaoService的配置监控类（单件模式）
    /// </summary>
    public class DAOServiceConfig
    {
        static private object _synRoot = new Object();
        static private DAOServiceConfig _instance;

        //DaoManager
		//private IDaoManager _daoManager = null;

		/// <summary>
		/// 私有构造器
		/// </summary>
        private DAOServiceConfig() { }

        /// <summary>
        /// 获得实例
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
                                //加载配置文件，两种写法都可以
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
		/// 重置本单件对象
		/// </summary>
		/// <remarks>
		/// 需要验证配置签名
		/// </remarks>
		/// <param name="obj">
		/// </param>
		static public void Reset(object obj)
		{
			_instance =null;
		}

        /// <summary>
        /// 获得默认的DaoManager
        /// </summary>
		public IDaoManager DaoManager
		{
			get
			{
				return IBatisNet.DataAccess.DaoManager.GetInstance();
			}
		}

        /// <summary>
        /// 获得DaoManager
        /// </summary>
        /// <param name="contextName">上下文名称</param>
        /// <returns></returns>
        public IDaoManager GetDaoManager(string contextName)
        {
            return IBatisNet.DataAccess.DaoManager.GetInstance(contextName);
        }
    }
}
