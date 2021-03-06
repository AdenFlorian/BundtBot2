﻿using BundtBot.Discord.Models.Embed;
using Newtonsoft.Json;

namespace DiscordApiWrapper.RestApi.RestApiRequests
{
    public class NewMessageRequest : RestApiRequest
    {
        [JsonIgnore]
        internal override RestRequestType RequestType => RestRequestType.Post;

        [JsonIgnore]
        internal override string RequestUri => $"channels/{_channelId}/messages";
        
        [JsonIgnore]
		ulong _channelId;

        /// <summary>
        /// The message contents (up to 2000 characters).
        /// </summary>
        [JsonProperty("content")]
        [JsonRequired]
        public string Content;

        /// <summary>
        /// A nonce that can be used for optimistic message sending.
        /// Optional.
        /// </summary>
        [JsonProperty("nonce")]
        public ulong? Nonce;

        /// <summary>Optional.</summary>
        [JsonProperty("tts")]
        public bool IsTextToSpeech;

        // TODO
        /// <summary>
        /// The contents of the file being sent.
        /// One of content, file, embeds (multipart form data only).
        /// </summary>
        //[JsonProperty("file")]
        //public ??? file;

        /// <summary>Optional.</summary>
        [JsonProperty("embed")]
        public Embed EmbeddedContent;

		public NewMessageRequest(ulong channelId)
		{
            _channelId = channelId;
		}
    }
}
