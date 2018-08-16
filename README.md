# Ferbot
A Discord Bot for handling aliases and mentioning.

Currently broken. Uses an old version of the discord.net API.


The goal of this bot is to allow for notifications in Discord something akin to old IRC client "Aliases" that would notify you if your nickname was used. For instance "Bpen" instead of "Bpendragon"

It does this by listening to the chat, and comparing every message that does not start with `!ferbot` against it's alias list (which is actually saved to a JSON text file locally for persistence across sessions).

`!ferbot` commands change the behavior of the bot, adding or removing aliases, or listing out all known aliases for a user. Commands can be found in `Ferbot\Messages\FerbotCommands.cs`

This bot is named after the Phinedroids and Ferbots of Disney's "Phineas and Ferb" because it was designed for use by a group from the Phineas and Ferb Fan Wiki IRC group that has since migrated to Discord. 

