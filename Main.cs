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
	/// Reflector.
	/// </summary>
	public class Reflector
	{
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main(string[] args)
		{
			IDictionary<Assembly, XDocument> assemblyPairs = ParseArguments(args);
			
			XElement documentation = new XElement("documentation");
			
			foreach (KeyValuePair<Assembly, XDocument> pair in assemblyPairs)
			{
				Assembly currentAssembly = pair.Key;
				XDocument currentDocument = pair.Value;
				
				// Get basic assembly information
				XElement assembly = new XElement("assembly",
					new XElement("name", currentAssembly.GetName().Name),
					new XElement("fullName", currentAssembly.FullName),
					new XElement("manifestModuleName", currentAssembly.ManifestModule.Name)
					);
				
				// Add list of namespaces
				assembly.Add(
					new XElement("namespaces",
						currentAssembly.GetTypes()
							.Select(t => t.Namespace)
							.Distinct()
							.OrderBy(n => n)
							.Select(n => new XElement("namespace", n))
					));
				
				XElement types = new XElement("types");
				
				foreach (Type currentType in currentAssembly.GetTypes())
				{
					XElement type = new XElement("type",
						new XElement("name", currentType.Name),
						new XElement("fullName", currentType.FullName),
						new XElement("baseType", 
							new XElement("fullName", currentType.BaseType.FullName),
							new XElement("assemblyFullName", currentType.BaseType.Assembly.FullName)
							)
						);
					
					types.Add(type);
				}
				
				assembly.Add(types);
				
				documentation.Add(assembly);
			}
			
			XDocument document = new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
				documentation
				);

			using (XmlWriter writer = XmlWriter.Create(Console.Out, new XmlWriterSettings(){Indent = true}))
			{
				document.WriteTo(writer);
			}
		}
		
		/// <summary>
		/// Parses the arguments.
		/// </summary>
		/// <returns>
		/// The arguments.
		/// </returns>
		/// <param name='arguments'>
		/// Arguments.
		/// </param>
		private static IDictionary<Assembly, XDocument> ParseArguments(IEnumerable<string> arguments)
		{
			HashSet<string> filenames = new HashSet<string>();
			foreach (var argument in arguments)
			{
				string pattern = Path.GetFileName(argument);
				string directory = string.IsNullOrEmpty(directory = Path.GetDirectoryName(argument)) ? Directory.GetCurrentDirectory() : directory;
				
				foreach (string filename in Directory.GetFiles(directory, pattern))
				{
					if (!filenames.Contains(filename))
					{
						filenames.Add(filename);
					}
				}
			}
			
			IEnumerable<Assembly> assemblies = filenames.Where(f => Path.GetExtension(f).Equals(".exe", StringComparison.CurrentCultureIgnoreCase) || Path.GetExtension(f).Equals(".dll", StringComparison.CurrentCultureIgnoreCase)).Select(f => Assembly.LoadFile(f));
			IEnumerable<XDocument> documents = filenames.Where(f => Path.GetExtension(f).Equals(".xml", StringComparison.CurrentCultureIgnoreCase)).Select(f => XDocument.Load(f));
			
			return assemblies.ToDictionary(a => a, a => documents.FirstOrDefault(d => {
				string documentAssemblyName = d.XPathSelectElement("/doc/assembly/name").Value;
				AssemblyName assemblyName = a.GetName();
				
				// Mono puts out a path to the assembly in the /doc/assembly/name element, whereas MS's document generator puts out just the assembly name
				return assemblyName.Name.Equals(documentAssemblyName) || assemblyName.Name.Equals(Path.GetFileName(documentAssemblyName));
			}));
		}
	}
}
