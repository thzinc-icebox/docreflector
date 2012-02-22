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
				// TODO: Get assembly info
				
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
									typeType = "type"; // Unknown type
								}
					
					XElement type = new XElement(typeType);

					type.Add(
						new XAttribute("id", currentType.GetIDString()),
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
						XElement genericArgument = new XElement("genericArgument");
						genericArgument.Add(new XElement("id", currentGenericType.GetIDString()));
						// TODO: Get generic argument info
						// TODO: Get generic argument documentation
						
						foreach (Type currentGenericTypeConstraint in currentGenericType.GetGenericParameterConstraints())
						{
							XElement genericArgumentConstraint = new XElement("genericArgumentConstraint");
							genericArgumentConstraint.Add(new XElement("id", currentGenericTypeConstraint.GetIDString()));
							// TODO: Get generic argument constraint info
							genericArgument.Add(genericArgumentConstraint);
						}
						
						type.Add(genericArgument);
					}
					
					foreach (Type currentInterface in currentType.GetInterfaces())
					{
						XElement iface = new XElement("interface");
						iface.Add(new XAttribute("id", currentInterface.GetIDString()));
						// TODO: Get interface info
						
						type.Add(iface);
					}
					
					foreach (Type currentNestedType in currentType.GetNestedTypes(bindingFlags))
					{
						XElement nestedType = new XElement("nestedType");
						nestedType.Add(new XAttribute("id", currentNestedType.GetIDString()));
						// TODO: Get nested type info
						
						type.Add(nestedType);
					}
					
					foreach (ConstructorInfo currentConstructor in currentType.GetConstructors(bindingFlags))
					{
						XElement constructor = new XElement("constructor");
						
						constructor.Add(
							new XAttribute("id", currentConstructor.GetIDString()),
							new XAttribute("abstract", currentConstructor.IsAbstract),
							new XAttribute("public", currentConstructor.IsPublic),
							new XAttribute("internal", currentConstructor.IsAssembly),
							new XAttribute("protected", currentConstructor.IsFamily),
							GetDocumentation(currentDocument, currentConstructor)
							);
						// TODO: solidify method/constructor info
						
						type.Add(constructor);
					}
					
					foreach (PropertyInfo currentProperty in currentType.GetProperties(bindingFlags))
					{
						XElement property = new XElement("property");
						property.Add(new XAttribute("id", currentProperty.GetIDString()));
						// TODO: Get property info
						// TODO: Get property documentation
						
						type.Add(property);
					}
					
					foreach (FieldInfo currentField in currentType.GetFields(bindingFlags))
					{
						XElement field = new XElement("field");
						field.Add(new XAttribute("id", currentField.GetIDString()));
						// TODO: Get field info
						// TODO: Get field documentation
						
						type.Add(field);
					}
					
					foreach (MethodInfo currentMethod in currentType.GetMethods(bindingFlags))
					{
						XElement method = new XElement("method");
						method.Add(new XAttribute("id", currentMethod.GetIDString()));
						// TODO: Get method info
						// TODO: Get method documentation
						
						type.Add(method);
					}
					
					foreach (MethodInfo currentMethod in currentType.GetExtensionMethods())
					{
						XElement method = new XElement("method");
						method.Add(
							new XAttribute("id", currentMethod.GetIDString()),
							new XAttribute("extension", true)
							);
						// TODO: Get method info
						// TODO: Get method documentation
						
						type.Add(method);
					}
					
					foreach (EventInfo currentEvent in currentType.GetEvents(bindingFlags))
					{
						XElement ev = new XElement("event");
						ev.Add(new XAttribute("id", currentEvent.GetIDString()));
						// TODO: Get event info
						// TODO: Get event documentation
						
						type.Add(ev);
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
			return GetDocumentationById(document, string.Format("N:{0}", name));
		}
		
		/// <summary>
		/// Gets the documentation for a given <see cref="MemberInfo"/>
		/// </summary>
		/// <returns>
		/// List of elements to append to the member's documentation element
		/// </returns>
		/// <param name='document'>
		/// Source XML documentation to search for member documentation from
		/// </param>
		/// <param name='memberInfo'>
		/// Member info to search for
		/// </param>
		private IEnumerable<XElement> GetDocumentation(XDocument document, MemberInfo memberInfo)
		{
			return GetDocumentationById(document, memberInfo.GetIDString());
		}
		
		/// <summary>
		/// Gets the documentation by identifier
		/// </summary>
		/// <returns>
		/// List of elements to append to the member's documentation element
		/// </returns>
		/// <param name='document'>
		/// Source XML documentation to search for member documentation from
		/// </param>
		/// <param name='id'>
		/// Member identifier
		/// </param>
		private IEnumerable<XElement> GetDocumentationById(XDocument document, string id)
		{
			XElement member = document.XPathSelectElement(string.Format("/doc/members/member[@name = '{0}']", id));
			IEnumerable<XElement> documentation;
			
			if (member == null)
			{
				documentation = new XElement[]{};
			}
			else
			{
				documentation = member.Descendants();
			}
			
			return documentation;
		}
		
		#endregion
	}
}

