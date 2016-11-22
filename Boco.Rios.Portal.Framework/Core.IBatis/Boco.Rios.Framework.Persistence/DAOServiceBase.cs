using IBatisNet.DataAccess;

namespace Boco.Rios.Framework.Persistence
{
    /// <summary>
    /// Dao�������
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    public abstract class DAOServiceBase<T>
        where T : class
    {
        #region ˽�б���

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
        /// ������
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
        /// ������
        /// </summary>
        /// <param name="contextName">����������</param>
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
        /// ��ʱ���ݵ���
        /// </summary>
        /// <param name="dt">���ݱ�</param>
        protected void FireTempDataArrival(object data)
        {
            if (TempDataArrival != null)
            {
                //��������
                PostRunTempData(data);

                TempDataArrival(data);
            }
        }

        ///// <summary>
        ///// ��ʱ���ݱ�ĺ���
        ///// </summary>
        ///// <param name="dt"></param>
        protected virtual void PostRunTempData(object data)
        {

        }

        //��ʱ���ݵ������
        public delegate void TempDataArrivalHandler(object data);
        public event TempDataArrivalHandler TempDataArrival;
         * */
    }
}