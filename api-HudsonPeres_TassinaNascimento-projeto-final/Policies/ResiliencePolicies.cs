using Polly;
using Polly.Extensions.Http;

namespace api_HudsonPeres_TassinaNascimento_projeto_final.Policies;

public static class ResiliencePolicies
{
    //retry 3 tentativas com backoff exponencial
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    //circuit breaker após 5 falhas consecutivas, abre o circuito por 30 segundos
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }

    //combina as duas políticas (retry + circuit breaker)
    public static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
    {
        return Policy.WrapAsync(GetRetryPolicy(), GetCircuitBreakerPolicy());
    }
}