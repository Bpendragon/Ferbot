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
		/// <summary>
		/// This is actually just a nice way of viewing the UserAliases of the underlying _aliases object
		/// </summary>
		public Dictionary<ulong, List<string>> UserAliases
		{
			get
			{
				return _aliases.UserAliases;
			}
		}
		private Aliases _aliases;
		public BotConfig BotConfig { get; }

		/// <summary>
		/// This is the Controller which modifies the aliases and then writes changes to disk
		/// </summary>
		/// <param name="jh">The JSonHelper that does the actual writing</param>
		public DataController(JsonHelper jh)
		{
			this.jh = jh;
			BotConfig = jh.ReadJsonFile<BotConfig>(@"..\..\BotConfig.json");
			_aliases = jh.ReadJsonFile<Aliases>(@"..\..\UserAliases.json");
		}

		/// <summary>
		/// Adds an Alias to the User
		/// </summary>
		/// <param name="UserID">ulong UserID of the SocketUser we're saving information for</param>
		/// <param name="Alias">The Alias the User wants to add and keep track of</param>
		/// <returns></returns>
		public AddSuccess AddAlias(ulong UserID, string Alias)
		{
			var SuccessLevel = AddSuccess.AlreadyExists;
			bool SuccessfulWrite = false;
			//See if the user has Saved any Aliases previously
			if (UserAliases.ContainsKey(UserID))
			{
				//If they have make sure they aren't trying to save the same alias again
				if (UserAliases[UserID].Contains(Alias, StringComparer.InvariantCultureIgnoreCase))
				{
					UserAliases[UserID].Add(Alias);
					UserAliases[UserID].Sort(StringComparer.InvariantCultureIgnoreCase);
					SuccessLevel = AddSuccess.Success;
				}
			}
			else  //If this is their first Alias, get the List for them set up.
			{
				var tempList = new List<string>();
				tempList.Add(Alias);
				UserAliases[UserID] = tempList;
				SuccessLevel = AddSuccess.Success;
			}

			//If we had to make any changes, write them to disk
			if (SuccessLevel != AddSuccess.AlreadyExists)
			{
				SuccessfulWrite = WriteToDisk();
			}
			//Make sure the write was succesful, if not tell the user as such
			if (SuccessfulWrite)
			{
				return SuccessLevel;
			}
			else
			{
				return AddSuccess.WriteFailure;
			}
		}

		/// <summary>
		/// Removes the appropriate alias for the User in question
		/// </summary>
		/// <param name="UserID">UserID of the user who wants to remove an alias</param>
		/// <param name="Alias">The alias we want to remove.</param>
		/// <returns></returns>
		public RemoveSuccess RemoveAlias(ulong UserID, string Alias)
		{
			var SuccessLevel = RemoveSuccess.NoSuchAlias;
			bool SuccessfulWrite = false;

			//Make sure the Alias we want actually exists. We can also check to make sure the user has even added any aliases previously
			if (UserAliases[UserID] != null && UserAliases[UserID].Contains(Alias, StringComparer.InvariantCultureIgnoreCase))
			{
				UserAliases[UserID].RemoveAt(UserAliases[UserID].FindIndex(n => n.Equals(Alias, StringComparison.InvariantCultureIgnoreCase)));
				SuccessLevel = RemoveSuccess.Success;
			}


			//Attempt to write changes to disk
			if (SuccessLevel != RemoveSuccess.NoSuchAlias)
			{
				SuccessfulWrite = WriteToDisk();
			}


			//Return the outcome
			if (SuccessfulWrite)
			{
				return SuccessLevel;
			}
			else
			{
				return RemoveSuccess.WriteFailure;
			}

		}

		private bool WriteToDisk()
		{
			throw new NotImplementedException();
		}
	}
}
