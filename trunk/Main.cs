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
	/// Documentation Reflector console program to generate useful documentation of .NET assemblies
	/// </summary>
	public class ReflectorConsole
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
			Reflector reflector = new Reflector(assemblyPairs);
			XDocument document = reflector.GenerateDocument();

			using (XmlWriter writer = XmlWriter.Create(Console.Out, new XmlWriterSettings(){Indent = true}))
			{
				document.WriteTo(writer);
			}
		}
		
		/// <summary>
		/// Parses the command-line arguments
		/// </summary>
		/// <returns>
		/// Mapping of loaded assemblies and their associated XML documentation, if available
		/// </returns>
		/// <param name='arguments'>
		/// Command-line arguments that may contain filename wildcard patterns
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
