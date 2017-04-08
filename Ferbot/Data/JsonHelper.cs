using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Ferbot.Data
{
	internal class JsonHelper
	{
		/// <summary>
		/// The Handler that reads to and from disk the appropriate pieces
		/// </summary>
		private readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
		};

		/// <summary>
		/// Reads from JSON on Disk to objects in memory
		/// </summary>
		/// <typeparam name="Model">The Class of the object being deserialized</typeparam>
		/// <param name="fullPath">Where to read the data from</param>
		/// <returns></returns>
		public Model ReadJsonFile<Model>(string fullPath) where Model : class
		{
			string json;
			try
			{
				json = File.ReadAllText(fullPath);
			}
			catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
			{
				return null;
			}

			
			return JsonConvert.DeserializeObject<Model>(json, this.JsonSettings);
		}

		/// <summary>
		/// Writes Objects to disk as JSON
		/// </summary>
		/// <typeparam name="Model">The Class type being written</typeparam>
		/// <param name="fullPath">The Path to write too</param>
		/// <param name="model">The Object to be written to disk</param>
		public void WriteJsonFile<Model>(string fullPath, Model model) where Model : class
		{
			
			string dir = Path.GetDirectoryName(fullPath);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			
			string json = JsonConvert.SerializeObject(model, this.JsonSettings);
			File.WriteAllText(fullPath, json);
		}
	}
}

