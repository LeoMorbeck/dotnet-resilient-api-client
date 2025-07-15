using System.Diagnostics;

namespace ResilientClient.Api.Handlers
{
    public class LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> _logger) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation(
                "Iniciando requisição: {Method} {Uri}",
                request.Method,
                request.RequestUri);

            // Continua o pipeline e obtém a resposta
            var response = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Requisição concluída com sucesso em {ElapsedMilliseconds}ms: {StatusCode} {Method} {Uri}",
                    stopwatch.ElapsedMilliseconds,
                    (int)response.StatusCode,
                    request.Method,
                    request.RequestUri);
            }
            else
            {
                _logger.LogWarning(
                    "Requisição falhou em {ElapsedMilliseconds}ms: {StatusCode} {Method} {Uri}",
                    stopwatch.ElapsedMilliseconds,
                    (int)response.StatusCode,
                    request.Method,
                    request.RequestUri);
            }

            return response;
        }
    }
}
