using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Xml.XPath;

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
		public XDocument GenerateDocument()
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
					// Get type info
					// Get type documentation
					
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
					
					foreach (Type currentNestedType in currentType.GetNestedTypes())
					{
						// Get nested type info
					}
					
					foreach (ConstructorInfo currentConstructor in currentType.GetConstructors())
					{
						// Get constructor info
						// Get constructor documentation
					}
					
					foreach (MemberInfo currentMember in currentType.GetMembers())
					{
						// Get member info
						// Get member documentation
					}
					
					
					foreach (MethodInfo currentMethod in currentType.GetMethods())
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
					
					foreach (EventInfo currentEvent in currentType.GetEvents())
					{
						// Get event info
						// Get event documentation
					}
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
			XElement member = document.XPathSelectElement(string.Format("/doc/members/member[@name = 'N:{0}']", name));
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
		
		#endregion
	}
}

