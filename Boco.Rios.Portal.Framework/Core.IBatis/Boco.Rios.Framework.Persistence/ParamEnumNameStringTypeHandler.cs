using System;

using IBatisNet.DataMapper.TypeHandlers;

namespace Boco.Rios.Framework.Persistence
{
    /// <summary>
    /// �ַ����͵�ö�ٴ�����(ֻ������ö���͵ĸ�ֵ������)
    /// </summary>
    public class ParamEnumNameStringTypeHandler:ITypeHandlerCallback
    {
        #region ITypeHandlerCallback ��Ա
        /// <summary>
        /// ����DataReader����ֵ
        /// </summary>
        /// <param name="setter">������</param>
        /// <param name="parameter">C#����ֵ</param>
        public void SetParameter(IParameterSetter setter, object parameter)
        {
            setter.DataParameter.Value = parameter.ToString();
        }

        /// <summary>
        /// ��DataReader��ȡ����ֵ��ת��
        /// </summary>
        /// <param name="getter">DataReader��ȡ��</param>
        /// <returns>����ת�����C#����</returns>
        [Obsolete("���������׼ʹ��")]
        public object GetResult(IResultGetter getter)
        {
            throw new Exception("���������׼ʹ��");
        }

        /// <summary>
        /// ��ö�����ƽ����ɶ�Ӧ��Enum
        /// </summary>
        /// <param name="type">ö������</param>
        /// <param name="s">�ִ�ֵ</param>
        /// <returns>���ش���Ľ��</returns>
        [Obsolete("���������׼ʹ��")]
        public object ValueOf(string s)
        {
            throw new Exception("���������׼ʹ��");
        }

        /// <summary>
        /// ����Nullֵ--������Ϣ
        /// </summary>
        [Obsolete("������Բ�׼ʹ��")]
        public object NullValue
        {
            get { throw new Exception("���������׼ʹ��"); }
        }

        #endregion
    }
}
