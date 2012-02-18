using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Extensions to <see cref="System.Type"/>, thanks to Wolfgang Stelzhammer
	/// </summary>
	public static class TypeExtension
	{
		/// <summary>
		/// This Method extends the System.Type-type to get all extended methods. It searches hereby in all assemblies which are known by the current AppDomain.
		/// </summary>
		/// <remarks>
		/// Authored by Wolfgang Stelzhammer, Inspired by Jon Skeet from his answer on http://stackoverflow.com/questions/299515/c-sharp-reflection-to-identify-extension-methods
		/// </remarks>
		/// <returns>returns MethodInfo[] with the extended Method</returns>
		public static MethodInfo[] GetExtensionMethods(this Type t)
		{
			List<Type> AssTypes = new List<Type>();

			foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssTypes.AddRange(item.GetTypes());
			}

			var query = from type in AssTypes
                where type.IsSealed && !type.IsGenericType && !type.IsNested
                from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where method.IsDefined(typeof(ExtensionAttribute), false)
                where method.GetParameters()[0].ParameterType == t
                select method;
			return query.ToArray<MethodInfo>();
		}

		/// <summary>
		/// Extends the System.Type-type to search for a given extended MethodName.
		/// </summary>
		/// <param name="t">Type being extended</param>
		/// <param name="MethodName">Name of the Method</param>
		/// <returns>the found Method or null</returns>
		public static MethodInfo GetExtensionMethod(this Type t, string MethodName)
		{
			var mi = from method in t.GetExtensionMethods()
                where method.Name == MethodName
                select method;
			if (mi.Count<MethodInfo>() <= 0)
			{
				return null;
			}
			else
			{
				return mi.First<MethodInfo>();
			}
		}
	}
}
