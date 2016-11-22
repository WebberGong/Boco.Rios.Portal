using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IBatisNet.DataMapper.TypeHandlers;

namespace Boco.Rios.Framework.Persistence
{
    public class DoubleDecimalTypeHandler : ITypeHandlerCallback
    {
        #region ITypeHandlerCallback 成员

        /// <summary>
        /// 设置DataReader参数值
        /// </summary>
        /// <param name="setter">设置器</param>
        /// <param name="parameter">C#参数值</param>
        public void SetParameter(IParameterSetter setter, object parameter)
        {
            setter.Value = Convert.ToDecimal(parameter);
        }

        /// <summary>
        /// 从DataReader读取的数值的转换
        /// </summary>
        /// <param name="getter">DataReader获取器</param>
        /// <returns>返回转化后的C#类型</returns>
        public object GetResult(IResultGetter getter)
        {
            return getter.Value is System.DBNull ? 0.0d : Convert.ToDouble(getter.Value);
        }

        /// <summary>
        /// 将枚举名称解析成对应的Enum
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="s">字串值</param>
        /// <returns>返回处理的结果</returns>
        public object ValueOf(string s)
        {
            // 这个函数用于将nullvalue值翻译成要比较的null值
            // 假如没有，则推荐返回字符串s
            return s;
        }

        /// <summary>
        /// 返回Null值--基本信息
        /// </summary>
        public object NullValue
        {
            get { return double.NaN; }
        }

        #endregion
    }
}
