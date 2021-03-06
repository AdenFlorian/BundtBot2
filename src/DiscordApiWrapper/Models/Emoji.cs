﻿using Newtonsoft.Json;

namespace BundtBot.Discord.Models
{
    public class Emoji
	{
		/// <summary>
		/// Id of emoji (if custom emoji).
		/// </summary>
		[JsonProperty("id")]
		public ulong? Id;
		
		[JsonProperty("name")]
		public string Name;
	}
}
