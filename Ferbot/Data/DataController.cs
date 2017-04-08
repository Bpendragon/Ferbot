using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using System.IO;


namespace Ferbot.Data
{

	/// <summary>
	/// This is the Object that reads and writes the Aliases to disk so that they can be used after bot shut-down.
	/// </summary>
	class DataController
	{
		JsonHelper jh;
		public Dictionary<ulong, List<string>> UserAliases
		{
			get
			{
				return _aliases.UserAliases;
			}
		}
		private Aliases _aliases;
		public BotConfig BotConfig { get; }

		public DataController(JsonHelper jh)
		{
			this.jh = jh;
			BotConfig = jh.ReadJsonFile<BotConfig>(@"..\..\BotConfig.json");
			_aliases = jh.ReadJsonFile<Aliases>(@"..\..\UserAliases.json");
		}

		public AddSuccess AddAlias(ulong UserID, string Alias)
		{
			var SuccessLevel = AddSuccess.AlreadyExists;
			bool SuccessfulWrite = false;
			if (_aliases.UserAliases.ContainsKey(UserID))
			{
				if (_aliases.UserAliases[UserID].Contains(Alias, StringComparer.InvariantCultureIgnoreCase))
				{
					_aliases.UserAliases[UserID].Add(Alias);
					SuccessLevel = AddSuccess.Success;
				}
			}
			else
			{
				var tempList = new List<string>();
				tempList.Add(Alias);
				_aliases.UserAliases[UserID] = tempList;
				SuccessLevel = AddSuccess.Success;
			}

			if (SuccessLevel != AddSuccess.AlreadyExists)
			{
				SuccessfulWrite = WriteToDisk();
			}

			if (SuccessfulWrite)
			{
				return SuccessLevel;
			}
			else
			{
				return AddSuccess.WriteFailure;
			}
		}

		public void RemoveAlias(ulong UserID, string Alias)
		{
			var SuccessLevel = RemoveSuccess.NoSuchAlias;
			if (_aliases.UserAliases[UserID].Contains(Alias, StringComparer.InvariantCultureIgnoreCase))
			{

			}
		}

		private bool WriteToDisk()
		{
			throw new NotImplementedException();
		}
	}
}
