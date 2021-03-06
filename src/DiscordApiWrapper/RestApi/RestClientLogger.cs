using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BundtBot;

namespace DiscordApiWrapper.RestApi
{
    class RestClientLogger : DelegatingHandler
    {
        static readonly MyLogger _logger = new MyLogger(nameof(RestClientLogger), ConsoleColor.DarkMagenta);

        public RestClientLogger(HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _logger.SetLogLevel(BundtFig.GetValue("loglevel-restclientlogger"));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _logger.LogInfo(
                new LogMessage($"Requested "),
                new LogMessage($"{request.Method} ", ConsoleColor.Magenta),
                new LogMessage(request.RequestUri.PathAndQuery, ConsoleColor.DarkMagenta));
            _logger.LogTrace(request);
            if (request.Content != null)
            {
                _logger.LogTrace(await request.Content.ReadAsStringAsync());
            }

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogInfo(
                new LogMessage($"Received "),
                new LogMessage($"{(int)response.StatusCode} {response.StatusCode}", ConsoleColor.Magenta),
                new LogMessage($" in response to "),
                new LogMessage($"{response.RequestMessage.Method} ", ConsoleColor.Magenta),
                new LogMessage($"{response.RequestMessage.RequestUri.PathAndQuery}", ConsoleColor.DarkMagenta));
            _logger.LogTrace(response);
            _logger.LogTrace("The current system time is " + DateTime.UtcNow);
            if (response.Content != null)
            {
                _logger.LogTrace(await response.Content.ReadAsStringAsync());
            }

            return response;
        }
    }
}