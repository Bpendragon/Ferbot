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
		private readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
		};


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

			// deserialise model
			return JsonConvert.DeserializeObject<Model>(json, this.JsonSettings);
		}

		public void WriteJsonFile<Model>(string fullPath, Model model) where Model : class
		{
			// create directory if needed
			string dir = Path.GetDirectoryName(fullPath);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			// write file
			string json = JsonConvert.SerializeObject(model, this.JsonSettings);
			File.WriteAllText(fullPath, json);
		}
	}
}

