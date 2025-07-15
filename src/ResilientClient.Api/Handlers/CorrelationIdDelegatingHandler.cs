using System.Diagnostics;

namespace ResilientClient.Api.Handlers
{
    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

            if (!request.Headers.Contains(CorrelationIdHeaderName))
            {
                request.Headers.Add(CorrelationIdHeaderName, correlationId);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
