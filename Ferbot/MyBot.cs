using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Ferbot.Messages;
using Ferbot.Data;

namespace Ferbot
{
	public class Program
	{
		
		DataController dc;
		DiscordSocketClient client;
		MessageController mc;
		JsonHelper jh;

		public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			client = new DiscordSocketClient();
			jh = new JsonHelper();
			dc = new DataController(jh);
			mc = new MessageController(dc, client);


			

			client.Log += Log;

			await client.LoginAsync(TokenType.Bot, dc.BotConfig.token);
			await client.StartAsync();

			client.MessageReceived += MessageRecieved;

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private async Task MessageRecieved(SocketMessage message)
		{
			await mc.Message(message);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}