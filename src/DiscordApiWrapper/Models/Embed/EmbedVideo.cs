﻿using Newtonsoft.Json;

namespace BundtBot.Discord.Models.Embed
{
    public class EmbedVideo
	{
		[JsonProperty("url")]
		public string SourceUrl;

		[JsonProperty("height")]
		public int Height;

		[JsonProperty("width")]
		public int Width;
	}
}
