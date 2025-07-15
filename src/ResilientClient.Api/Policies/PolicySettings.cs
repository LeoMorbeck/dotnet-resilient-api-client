namespace ResilientClient.Api.Policies
{
    public class PolicySettings
    {
        public int RetryCount { get; set; }
        public int RetrySleepDurationSeconds { get; set; }
        public int CircuitBreakerFailureCount { get; set; }
        public int CircuitBreakerDurationOfBreakSeconds { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
