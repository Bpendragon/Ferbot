using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Ferbot.Data;

namespace Ferbot.Messages
{
	partial class MessageController
	{
		DataController dc;
		DiscordSocketClient client;
	 
		
		public MessageController(DataController dc, DiscordSocketClient client)
		{
			this.dc = dc;
			this.client = client;
			
		}

		internal async Task Message(SocketMessage message)
		{
			if (!message.Author.IsBot)
			{
				if (message.Content.ToLower().StartsWith("!ferbot"))
				{
					await ExecuteCommand(message);
				}
				else
				{
					await CheckMessage(message);
				}
			}
		}

		
	}
}
