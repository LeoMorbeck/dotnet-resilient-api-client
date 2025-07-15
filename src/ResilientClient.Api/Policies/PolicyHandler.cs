using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace ResilientClient.Api.Policies
{
    public static class PolicyHandler
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(PolicySettings settings, ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Lida com erros 5xx e 408
                .Or<TimeoutRejectedException>() // Lida com timeouts
                .WaitAndRetryAsync(
                    settings.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(settings.RetrySleepDurationSeconds, retryAttempt)), // Backoff exponencial
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning(
                            "Retentativa {RetryAttempt} para {Method} {Uri} devido a {StatusCode}. Aguardando {Timespan}s...",
                            retryAttempt,
                            outcome.Result?.RequestMessage?.Method,
                            outcome.Result?.RequestMessage?.RequestUri,
                            outcome.Result?.StatusCode,
                            timespan.TotalSeconds);
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(PolicySettings settings, ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    settings.CircuitBreakerFailureCount,
                    TimeSpan.FromSeconds(settings.CircuitBreakerDurationOfBreakSeconds),
                    onBreak: (outcome, timespan, context) =>
                    {
                        logger.LogError("CIRCUITO ABERTO! Falhas atingiram o limite. Próxima tentativa em {Timespan}s", timespan.TotalSeconds);
                    },
                    onReset: (context) =>
                    {
                        logger.LogInformation("CIRCUITO FECHADO! O serviço parece ter se recuperado.");
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogWarning("CIRCUITO EM MODO HALF-OPEN! Enviando a próxima requisição como teste.");
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(PolicySettings settings)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(settings.TimeoutSeconds);
        }
    }
}
