using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;

namespace DocumentationReflector
{
	
	/// <summary>
	/// Primary class to reflect through assemblies and generate an <see cref="XDocument"/> from them
	/// </summary>
	public class Reflector
	{
		#region Constructors
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentationReflector.Reflector"/> class.
		/// </summary>
		/// <param name='assemblyDocumentationPairs'>
		/// Assembly-documentation pairs.
		/// </param>
		public Reflector(IDictionary<Assembly, XDocument> assemblyDocumentationPairs)
		{
			AssemblyDocumentationPairs = assemblyDocumentationPairs;
		}
		
		#endregion
		
		#region Public Properties
		
		/// <summary>
		/// Gets or sets the assembly documentation pairs.
		/// </summary>
		/// <value>
		/// The assembly documentation pairs.
		/// </value>
		public IDictionary<Assembly, XDocument> AssemblyDocumentationPairs { get; set; }
		
		#endregion
		
		#region Public Methods
		
		/// <summary>
		/// Generates the <see cref="XDocument"/> from the <see cref="AssemblyDocumentationPairs"/>
		/// </summary>
		/// <returns>
		/// <see cref="XDocument"/> representing a detailed reflection of the <see cref="AssemblyDocumentationPairs"/>
		/// </returns>
		public XDocument GenerateDocument(BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn)
		{
			XElement documentation = new XElement("documentation");
			
			foreach (KeyValuePair<Assembly, XDocument> pair in AssemblyDocumentationPairs)
			{
				Assembly currentAssembly = pair.Key;
				XDocument currentDocument = pair.Value;
				
				XElement assembly = new XElement("assembly");
				// Get assembly info
				
				XElement namespaces = new XElement("namespaces");
				
				foreach (string name in currentAssembly.GetTypes().Select(t => t.Namespace).Distinct())
				{
					XElement namespaceElement = new XElement("namespace",
					                                         new XElement("name", name),
					                                         GetDocumentation(currentDocument, name)
					                                         );
					
					namespaces.Add(namespaceElement);
				}

				assembly.Add(namespaces);

				XElement types = new XElement("types");
				
				foreach (Type currentType in currentAssembly.GetTypes())
				{
					XElement type = new XElement("type");
					
					string typeType;
					if (currentType.IsClass)
					{
						typeType = "class";
					}
					else
					if (currentType.IsInterface)
						{
							typeType = "interface";
						}
						else
						if (currentType.IsEnum)
							{
								typeType = "enum";
							}
							else
							if (currentType.IsValueType)
								{
									typeType = "struct";
								}
								else
								{
									typeType = "unknown";
								}
					
					type.Add(
						new XAttribute("id", currentType.FullName),
						new XAttribute("type", typeType),
						new XAttribute("abstract", currentType.IsAbstract),
						new XAttribute("sealed", currentType.IsSealed),
						new XAttribute("public", currentType.IsPublic),
						new XAttribute("nested", currentType.IsNested),
						new XElement("name", currentType.Name),
						new XElement("namespace", currentType.Namespace),
						new XElement("fullName", currentType.FullName),
						GetDocumentation(currentDocument, currentType)
						);
					
					// TODO: Identify internal and protected types
					
					foreach (Type currentGenericType in currentType.GetGenericArguments())
					{
						// Get generic argument info
						// Get generic argument documentation
						foreach (Type currentGenericTypeConstraint in currentGenericType.GetGenericParameterConstraints())
						{
							// Get generic argument constraint info
						}
					}
					
					foreach (Type currentInterface in currentType.GetInterfaces())
					{
						// Get interface info
					}
					
					foreach (Type currentNestedType in currentType.GetNestedTypes(bindingFlags))
					{
						// Get nested type info
					}
					
					XElement constructors = new XElement("constructors");
					
					foreach (ConstructorInfo currentConstructor in currentType.GetConstructors(bindingFlags))
					{
						XElement constructor = new XElement("constructor");
						
						constructor.Add(
							new XAttribute("id", GetMethodSignature(currentConstructor)),
							new XAttribute("abstract", currentConstructor.IsAbstract),
							new XAttribute("public", currentConstructor.IsPublic),
							new XAttribute("internal", currentConstructor.IsAssembly),
							new XAttribute("protected", currentConstructor.IsFamily),
							GetDocumentation(currentDocument, currentConstructor)
							);
						// TODO: solidify method/constructor info
						
						constructors.Add(constructor);
					}
					
					type.Add(constructors);
					
					foreach (PropertyInfo currentProperty in currentType.GetProperties(bindingFlags))
					{
						// Get property info
						// Get property documentation
					}
					
					foreach (FieldInfo currentField in currentType.GetFields(bindingFlags))
					{
						// Get field info
						// Get field documentation
					}
					
					foreach (MethodInfo currentMethod in currentType.GetMethods(bindingFlags))
					{
						// Get method info
						// Get method documentation
					}
					
					foreach (MethodInfo currentMethod in currentType.GetExtensionMethods())
					{
						Console.WriteLine("{0}.{1}", currentType.Name, currentMethod.Name);
						// Get method info
						// Get method documentation
					}
					
					foreach (EventInfo currentEvent in currentType.GetEvents(bindingFlags))
					{
						// Get event info
						// Get event documentation
					}
					
					types.Add(type);
				}

				assembly.Add(types);

				documentation.Add(assembly);
			}
			
			XDocument document = new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
				documentation
				);
			
			return document;
		}
		
		#endregion
		
		#region Private Methods
		
		/// <summary>
		/// Gets the documentation for a given namespace
		/// </summary>
		/// <returns>
		/// List of elements to append to the namespace's documentation element
		/// </returns>
		/// <param name='document'>
		/// Source XML documentation to search for namespace documentation from
		/// </param>
		/// <param name='name'>
		/// Namespace to search for
		/// </param>
		private IEnumerable<XElement> GetDocumentation(XDocument document, string name)
		{
			XElement member = GetMemberDocumentation(document, string.Format("N:{0}", name));
			IEnumerable<XElement> documentation;
			
			if (member == null)
			{
				documentation = new XElement[]{};
			}
			else
			{
				// TODO: Do some translation on the crefs
				documentation = member.Descendants();
			}
			
			return documentation;
		}
		
		private IEnumerable<XElement> GetDocumentation(XDocument document, Type type)
		{
			XElement member = GetMemberDocumentation(document, string.Format("T:{0}", type.FullName));
			IEnumerable<XElement> documentation;
			
			if (member == null)
			{
				documentation = new XElement[]{};
			}
			else
			{
				// TODO: Do some translation on the crefs
				documentation = member.Descendants();
			}
			
			return documentation;
		}
		
		private IEnumerable<XElement> GetDocumentation(XDocument document, MethodBase method)
		{
			XElement member = GetMemberDocumentation(document, string.Format("M:{0}", GetMethodSignature(method)));
			IEnumerable<XElement> documentation;
			
			if (member == null)
			{
				documentation = new XElement[]{};
			}
			else
			{
				// TODO: Do some translation on the crefs
				documentation = member.Descendants();
			}
			
			return documentation;
		}
		
		private XElement GetMemberDocumentation(XDocument document, string name)
		{
			return document.XPathSelectElement(string.Format("/doc/members/member[@name = '{0}']", name));
		}
		
		private string GetMethodSignature(MethodBase method)
		{
			string methodName = method.Name;
			
			if (method is ConstructorInfo)
			{
				methodName = ".#ctor";
			}
			
			string parameters = string.Join(",", method.GetParameters().Select(p => p.ParameterType.FullName).ToArray());
			
			return string.Format("{0}{1}{2}", method.DeclaringType.FullName, methodName, parameters.Any() ? string.Format("({0})", parameters) : null);
		}
		
		#endregion
	}
}

