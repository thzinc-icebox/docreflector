using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace DocumentationReflector
{
	/// <summary>
	/// Extensions to <see cref="MemberInfo"/>
	/// </summary>
	public static class MemberInfoExtension
	{
		/// <summary>
		/// Gets the documentation identifier string, formatted according to http://msdn.microsoft.com/en-us/library/fsbx0t7x%28v=vs.71%29.aspx
		/// </summary>
		/// <returns>
		/// The identifier string representing the extended <see cref="MemberInfo"/>
		/// </returns>
		/// <param name='memberInfo'>
		/// <see cref="MemberInfo"/> being extended
		/// </param>
		public static string GetIDString(this MemberInfo memberInfo)
		{
			StringBuilder id = new StringBuilder();
			
			string ns;
			if (memberInfo is Type){
				ns = (memberInfo as Type).Namespace;
			}
			else {
				ns = memberInfo.DeclaringType.FullName;
			}
			string name = memberInfo.Name.Replace(".", "#");
			
			switch (memberInfo.MemberType)
			{
				case MemberTypes.Event:
					id.Append("E");
					break;
				case MemberTypes.Field:
					id.Append("F");
					break;
				case MemberTypes.Constructor:
				case MemberTypes.Method:
					id.Append("M");
					break;
				case MemberTypes.Property:
					id.Append("P");
					break;
				case MemberTypes.NestedType:
				case MemberTypes.TypeInfo:
					id.Append("T");
					break;
				default:
					id.Append("!");
					break;
			}

			id.AppendFormat(":{0}.{1}", ns, name);
			
			switch (memberInfo.MemberType)
			{
				case MemberTypes.Constructor:
				case MemberTypes.Method:
					MethodBase method = memberInfo as MethodBase;
					IEnumerable<ParameterInfo> parameters;
					
					if (method != null && (parameters = method.GetParameters()).Any()) {
						id.Append("(");
						id.Append(string.Join(",", parameters.Select(p => p.ParameterType.FullName).ToArray()));
						id.Append(")");
					}
					break;
			}

			return id.ToString();
		}
	}
}

