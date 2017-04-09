using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Ferbot.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ferbot.Messages
{
	partial class MessageController
	{
		/// <summary>
		/// Executes a message that has been determined to be a command
		/// </summary>
		/// <param name="message">The incoming message.</param>
		/// <returns></returns>
		internal async Task ExecuteCommand(SocketMessage message)
		{
			var tokens = message.Content.Split();
			if (tokens.Count() < 2)
			{
				await UnknownCommand(message, tokens);
				return;
			}
			switch (tokens[1].ToLower())
			{
				case "ping": await Pong(message, tokens); break;
				case "add":
				case "addalias": await AddAlias(message, tokens); break;
				case "remove":
				case "removealias": await RemoveAlias(message, tokens); break;
				case "list":
				case "listaliases": await ListAliases(message, tokens); break;
				case "help":
				case "h":
				case "man": await FerbotHelp(message); break;

				default: await UnknownCommand(message, tokens); break;
			}
		}

		#region commands
		/// <summary>
		/// Retruns the help message in the channel the command was used.
		/// </summary>
		/// <param name="message">The original message, which contains channel information.</param>
		/// <returns></returns>
		private async Task FerbotHelp(SocketMessage message)
		{
			await message.Channel.SendMessageAsync(
				 "```"
				+ $"Ferbot Version {dc.BotConfig.version}\n"
				+ "Copyright (c) 2017 Bpendragon\n"
				+ "====================\n"
				+ "All Commands are prefaced with !ferbot\n"
				+ "All Commands are case insensitive\n"
				+ "====================\n"
				+ "Ping                                  | Bot replies \"Pong!\"\n"
				+ "AddAlias [ALIAS]                      | Adds [ALIAS] to your list of aliases.\n"
				+ "Add [ALIAS]                           |\n"
				+ "--------------------------------------|\n"
				+ "RemoveAlias [ALIAS]                   | Removes [ALIAS] from your list of aliases.\n"
				+ "Remove [ALIAS]                        |\n"
				+ "--------------------------------------|\n"
				+ "ListAliases (opt) [User1, User2, ...] | Lists registered aliases of mentioned users.\n"
				+ "--------------------------------------| If no users mentioned, returns the sender's aliases.\n"
				+ "List (opt) [User1, User2, ...]        |\n"
				+ "--------------------------------------|\n"
				+ "help                                  | Displays this help.\n"
				+ "h                                     |\n"
				+ "man                                   |\n"
				+ "```"
				);
		}

		/// <summary>
		/// Lists the Alias(es) of either the sending User, or users mentioned in the message.
		/// </summary>
		/// <param name="message">The original message</param>
		/// <param name="tokens">The message Split into words</param>
		/// <returns></returns>
		private async Task ListAliases(SocketMessage message, string[] tokens)
		{
			StringBuilder sb = new StringBuilder("Alias(es) for selected user(s)\n");
			if (message.MentionedUsers.Count > 0)
			{
				foreach (var user in message.MentionedUsers)
				{
					sb.Append($"{user.Mention}: ");
					if (!dc.UserAliases.ContainsKey(user.Id))
					{
						sb.Append("\n");
						continue;
					}
					foreach (var alias in dc.UserAliases[user.Id])
					{
						sb.Append($"{alias}, ");
					}
					sb.Remove(sb.Length - 2, 2);
					sb.Append("\n");
				}
			}
			else
			{
				sb.Append($"{message.Author.Mention}: ");
				foreach (var alias in dc.UserAliases[message.Author.Id])
				{
					sb.Append($"{alias}, ");
				}
				sb.Remove(sb.Length - 2, 2);
				sb.Append("\n");
			}

			await message.Channel.SendMessageAsync(sb.ToString());
		}

		/// <summary>
		/// Deregisters an alias for the sending user.
		/// </summary>
		/// <param name="message">The original message</param>
		/// <param name="tokens">The message Split into words</param>
		/// <returns></returns>
		private async Task RemoveAlias(SocketMessage message, string[] tokens)
		{
			try
			{
				await message.Channel.SendMessageAsync($"Attempting to remove alias \"{tokens[2]}\"");
				var success = dc.RemoveAlias(message.Author.Id, tokens[2]);
				switch (success)
				{
					case RemoveSuccess.Success:
						await message.Channel.SendMessageAsync($"{message.Author.Mention} Alias removed.");
						break;
					case RemoveSuccess.NoSuchAlias:
						await message.Channel.SendMessageAsync($"{message.Author.Mention} No Such Alias exists");
						break;
					case RemoveSuccess.WriteFailure:
						await message.Channel.SendMessageAsync($"Could not write to disk, no aliases deregistered, please contact my handler.");
						break;
					case RemoveSuccess.Unknown:
						await message.Channel.SendMessageAsync($"Sorry, {message.Author.Mention} but something's gone wrong "
											+ $"while attempting to remove your alias of \"{tokens[2]}\", "
											+ "and I'm not sure exactly what it is. Please contact my handler for further assistance");

						break;
				}
			}
			catch (Exception)
			{

				await message.Channel.SendMessageAsync("Malformed command please use `!Ferbot RemoveAlias [Alias To Remove]`");
			}
		}

		/// <summary>
		/// Registers an alias for teh sending user
		/// </summary>
		/// <param name="message">The original message</param>
		/// <param name="tokens">The message Split into words</param>
		/// <returns></returns>
		private async Task AddAlias(SocketMessage message, string[] tokens)
		{
			try
			{
				await message.Channel.SendMessageAsync($"{message.Author.Mention} I'm attempting to Add Alias \"{tokens[2]}\" for you");
				var success = dc.AddAlias(message.Author.Id, tokens[2]);
				switch (success)
				{
					case AddSuccess.Success:
						await message.Channel.SendMessageAsync($"I have now registered \"{tokens[2]}\" "
																	+ $"for you as an alias {message.Author.Mention}");
						break;
					case AddSuccess.AlreadyExists:
						await message.Channel.SendMessageAsync($"You already have \"{tokens[2]}\" "
										 + $" registered for you as an alias {message.Author.Mention}");
						break;
					case AddSuccess.WriteFailure:
						await message.Channel.SendMessageAsync($"Could not write to disk, no aliases registered, please contact my handler.");
						break;
					case AddSuccess.Unknown:
						await message.Channel.SendMessageAsync($"Sorry, {message.Author.Mention} but something's gone wrong "
																	+ $"while attempting to add your alias of \"{tokens[2]}\", "
																	+ "and I'm not sure exactly what it is. Please contact my handler for further assistance");
						break;
				}
			}
			catch (Exception)
			{
				await message.Channel.SendMessageAsync("Malformed command please use `!Ferbot AddAlias [Alias To Add]`");
			}
		}

		/// <summary>
		/// Returned if command is unknown.
		/// </summary>
		/// <param name="message">The original message</param>
		/// <param name="tokens">The message Split into words</param>
		/// <returns></returns>
		private async Task UnknownCommand(SocketMessage message, string[] tokens)
		{
			await message.Channel.SendMessageAsync(
				$"I'm sorry {message.Author.Mention} but that is not a recognized command\n"
				+"Please use `!ferbot help` to see current commands"
				);
		}

		/// <summary>
		/// Replies to !ferbot ping" with "!pong" as a heartbeat check.
		/// </summary>
		/// <param name="message">The original message</param>
		/// <param name="tokens">The message Split into words</param>
		/// <returns></returns>
		private async Task Pong(SocketMessage message, string[] tokens)
		{
			await message.Channel.SendMessageAsync("Pong!");
		}

		#endregion commands

		/// <summary>
		/// Checks all non-command and non-bot messages for alias matches.
		/// Ignores matches that have already been mentioned.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private async Task CheckMessage(SocketMessage message)
		{
			var usersToNotify = new List<SocketUser>();

			foreach (var user in dc.UserAliases)
			{
				var results = from alias in user.Value
							  where (message.Content.IndexOf(alias, StringComparison.InvariantCultureIgnoreCase) >= 0)
							  select alias;
				if (results.Count() > 0)
					usersToNotify.Add(client.GetUser(user.Key));
			}

			usersToNotify.RemoveAll(n => message.MentionedUsers.Contains(n));
			StringBuilder sb = new StringBuilder();
			foreach (var u in usersToNotify)
			{
				sb.Append($"{u.Mention}, ");
			}
			sb.Remove(sb.Length - 2, 2);

			await message.Channel.SendMessageAsync($"{sb.ToString()} ^");

		}


	}
}