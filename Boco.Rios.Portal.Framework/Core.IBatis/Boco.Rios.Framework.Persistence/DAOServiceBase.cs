using IBatisNet.DataAccess;

namespace Boco.Rios.Framework.Persistence
{
    /// <summary>
    /// Dao服务基础
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public abstract class DAOServiceBase<T>
        where T : class
    {
        #region 私有变量

        /// <summary>
        /// DaoManager
        /// </summary>
        private IDaoManager daoManager;

        protected IDaoManager DaoManager
        {
            get { return daoManager; }
            set { daoManager = value; }
        }

        protected T Dao { get; set; }

        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        protected DAOServiceBase()
        {
            //daoManager = DAOServiceConfig.Default.DaoManager;

            //--Modified by luanju, the DaoManager couldn't return the instance of IDaoMananger
            daoManager = DAOServiceConfig.Default.GetDaoManager("SqlMapDao");
            Dao = (T) daoManager.GetDao(typeof (T));

            //((IDao)_Dao).TempDataArrival += new IBatisNet.DataAccess.Interfaces.TempDataArrivalHandler(FireTempDataArrival);
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="contextName">上下文名称</param>
        protected DAOServiceBase(string contextName)
        {
            if (contextName == "")
            {
                daoManager = DAOServiceConfig.Default.DaoManager;
                Dao = (T) daoManager.GetDao(typeof (T));
            }
            else
            {
                daoManager = DAOServiceConfig.Default.GetDaoManager(contextName);
                Dao = (T) daoManager.GetDao(typeof (T));
            }
        }
        /*
        /// <summary>
        /// 临时数据到达
        /// </summary>
        /// <param name="dt">数据表</param>
        protected void FireTempDataArrival(object data)
        {
            if (TempDataArrival != null)
            {
                //后处理数据
                PostRunTempData(data);

                TempDataArrival(data);
            }
        }

        ///// <summary>
        ///// 临时数据表的后处理
        ///// </summary>
        ///// <param name="dt"></param>
        protected virtual void PostRunTempData(object data)
        {

        }

        //临时数据到达代理
        public delegate void TempDataArrivalHandler(object data);
        public event TempDataArrivalHandler TempDataArrival;
         * */
    }
}