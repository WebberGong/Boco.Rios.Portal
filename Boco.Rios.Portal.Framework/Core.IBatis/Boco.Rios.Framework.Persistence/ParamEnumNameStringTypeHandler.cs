using System;

using IBatisNet.DataMapper.TypeHandlers;

namespace Boco.Rios.Framework.Persistence
{
    /// <summary>
    /// 字符串型的枚举处理器(只能用在枚举型的赋值参数里)
    /// </summary>
    public class ParamEnumNameStringTypeHandler:ITypeHandlerCallback
    {
        #region ITypeHandlerCallback 成员
        /// <summary>
        /// 设置DataReader参数值
        /// </summary>
        /// <param name="setter">设置器</param>
        /// <param name="parameter">C#参数值</param>
        public void SetParameter(IParameterSetter setter, object parameter)
        {
            setter.DataParameter.Value = parameter.ToString();
        }

        /// <summary>
        /// 从DataReader读取的数值的转换
        /// </summary>
        /// <param name="getter">DataReader获取器</param>
        /// <returns>返回转化后的C#类型</returns>
        [Obsolete("这个方法不准使用")]
        public object GetResult(IResultGetter getter)
        {
            throw new Exception("这个方法不准使用");
        }

        /// <summary>
        /// 将枚举名称解析成对应的Enum
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="s">字串值</param>
        /// <returns>返回处理的结果</returns>
        [Obsolete("这个方法不准使用")]
        public object ValueOf(string s)
        {
            throw new Exception("这个方法不准使用");
        }

        /// <summary>
        /// 返回Null值--基本信息
        /// </summary>
        [Obsolete("这个属性不准使用")]
        public object NullValue
        {
            get { throw new Exception("这个方法不准使用"); }
        }

        #endregion
    }
}
