using ResilientClient.Api.Handlers;
using ResilientClient.Api.Policies;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();
builder.Services.AddTransient<LoggingDelegatingHandler>();

var policySettings = builder.Configuration.GetSection("PolicySettings").Get<PolicySettings>()!;

builder.Services.AddHttpClient("ProviderMockClient", client =>
{
    var providerUrl = builder.Configuration["ProviderMockUrl"];
    client.BaseAddress = new Uri(providerUrl!);
})
.AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
.AddHttpMessageHandler<LoggingDelegatingHandler>()
.AddPolicyHandler((services, request) => PolicyHandler.GetRetryPolicy(policySettings, services.GetRequiredService<ILogger<Program>>()))
.AddPolicyHandler((services, request) => PolicyHandler.GetCircuitBreakerPolicy(policySettings, services.GetRequiredService<ILogger<Program>>()))
.AddPolicyHandler(PolicyHandler.GetTimeoutPolicy(policySettings));

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();