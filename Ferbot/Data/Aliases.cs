using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Ferbot.Data
{
	class Aliases
	{

		public Dictionary<ulong, List<String>> UserAliases { get; set; }

		public Aliases() { }
		
		public Aliases(ulong ID, string alias)
		{
			UserAliases[ID] = new List<string>();
			UserAliases[ID].Add(alias);
		}

	}
}
