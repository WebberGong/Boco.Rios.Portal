using System;
using System.Collections.Generic;
using System.Text;

namespace IBatisNet.DataAccess
{
    public enum DAOMethodType
    {
        NoDaoSession,
        AutoDaoSession,
        CustomizedDaoSession
    }

    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Property)]
    public class DAOMethodAttribute:Attribute
    {
        private DAOMethodType daoMethodType;
        /// <summary>
        /// ��������
        /// </summary>
        public DAOMethodType DAOMethodType
        {
            get { return this.daoMethodType; }
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="daoMethodType"></param>
        public DAOMethodAttribute(DAOMethodType daoMethodType)
        {
            this.daoMethodType = daoMethodType;
        }
    }
}
