using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Ferbot.Data;

namespace Ferbot.Messages
{
	class MessageController
	{
		DataController dc;
		public MessageController(DataController dc)
		{
			this.dc = dc;
		}

		internal Task Message(SocketMessage message)
		{
			throw new NotImplementedException();
		}

	}
}
