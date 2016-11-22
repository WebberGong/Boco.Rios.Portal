#region Apache Notice
/*****************************************************************************
 * $Header: $
 * $Revision: 408164 $
 * $Date: 2006-05-21 20:27:09 +0800 (星期日, 21 五月 2006) $
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

#region Using

using System.Collections.Specialized;
using System.Xml;
using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;

#endregion 


namespace IBatisNet.DataMapper.Configuration.Serializers
{
	/// <summary>
	/// Summary description for IsPropertyAvailableDeSerializer.
	/// </summary>
	public sealed class IsPropertyAvailableDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="configScope"></param>
		public IsPropertyAvailableDeSerializer(ConfigurationScope configScope)
		{
			_configScope = configScope;
		}

		#region IDeSerializer Members

		/// <summary>
		/// Deserialize a IsNotEmpty object
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public SqlTag Deserialize(XmlNode node)
		{
			IsPropertyAvailable isPropertyAvailable = new IsPropertyAvailable(_configScope.DataExchangeFactory.AccessorFactory);

			NameValueCollection prop = NodeUtils.ParseAttributes(node, _configScope.Properties);
			isPropertyAvailable.Prepend = NodeUtils.GetStringAttribute(prop, "prepend");
			isPropertyAvailable.Property = NodeUtils.GetStringAttribute(prop, "property");

			return isPropertyAvailable;
		}

		#endregion
	}
}
