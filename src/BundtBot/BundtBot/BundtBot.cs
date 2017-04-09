﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BundtCord.Discord;

namespace BundtBot
{
    public class BundtBot
    {
        DiscordClient _client;

        static readonly MyLogger _logger = new MyLogger(nameof(BundtBot));

        //internal static Dictionary<Guild, TextChannel> TextChannelOverrides = new Dictionary<Guild, TextChannel>();

        public async Task StartAsync()
        {
            _client = new DiscordClient(File.ReadAllText("bottoken"));

            RegisterEventHandlers();

            await _client.ConnectAsync();
        }

        void RegisterEventHandlers()
        {
            _client.MessageCreated += async (message) =>
            {
                try
                {
                    if (message.Author.Id == _client.Me.Id) return;

                    var messageContent = message.Content;

                    if (messageContent.StartsWith("!echo "))
                    {
                        await message.TextChannel.SendMessageAsync(messageContent.Substring(5));
                    }
                    else if (messageContent == "!hello")
                    {
                        // Reject if user is not in a voice channel
                        if (message.Author.VoiceChannel == null)
                        {
                            await message.TextChannel.SendMessageAsync("You're going to want to be in a voice channel for this...");
                            return;
                        }

                        await message.TextChannel.SendMessageAsync("I see you in channel " + message.Author.VoiceChannel.Name);

                        // Join voice channel
                        await message.Author.VoiceChannel.JoinAsync();

                        await Task.Delay(2345);

                        // Leave voice channel
                        await message.Author.VoiceChannel.LeaveAsync();

                        // Play helloworld.opus
                        // Leave channel
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception thrown while handling event " + nameof(_client.MessageCreated));
                    _logger.LogError(ex);
                }
            };

            _client.ServerCreated += async (server) => {
                try
                {
                    await server.TextChannels.First().SendMessageAsync("yo");

                    var voiceChannel = server.VoiceChannels.First();

                    // Join voice channel
                    await voiceChannel.JoinAsync();

                    await Task.Delay(2000);

                    // Leave voice channel
                    //await voiceChannel.LeaveAsync();

                    // Play helloworld.opus
                    // Leave channel
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception thrown while handling event " + nameof(_client.ServerCreated));
                    _logger.LogError(ex);
                }
			};

            /*_client.Ready += (ready) => {
				_logger.LogInfo("Client is Ready/Connected! ໒( ͡ᵔ ▾ ͡ᵔ )७", ConsoleColor.Green);
				_logger.LogInfo("Setting game...");
				_client.SetGame(Assembly.GetEntryAssembly().GetName().Version.ToString());
			};
			
			_client.TextChannelCreated += async (textChannel) => {
				try {
					await textChannel.SendMessage("less is more");
					if (!textChannel.Name.ToLower().Contains("bundtbot")) return;
					TextChannelOverrides[textChannel.Guild] = textChannel;
				} catch (Exception ex) {
					_logger.LogError(ex);
				}
			};*/
        }
    }
}
