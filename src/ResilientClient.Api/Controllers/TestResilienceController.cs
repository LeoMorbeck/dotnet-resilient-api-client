using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResilientClient.Api.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestResilienceController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TestResilienceController> _logger;

        public TestResilienceController(IHttpClientFactory httpClientFactory, ILogger<TestResilienceController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private async Task<IActionResult> MakeRequest(string endpoint)
        {
            _logger.LogInformation("Disparando requisição para o endpoint: {Endpoint}", endpoint);
            var client = _httpClientFactory.CreateClient("ProviderMockClient");

            try
            {
                var response = await client.GetAsync(endpoint);
                // Mesmo se não for sucesso, retornamos o status code para análise
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uma exceção não tratada ocorreu ao chamar {Endpoint}", endpoint);
                return StatusCode(500, $"Exceção: {ex.GetType().Name} - {ex.Message}");
            }
        }

        [HttpGet("success")]
        public async Task<IActionResult> TestSuccess() => await MakeRequest("simulate/success");

        [HttpGet("unstable")]
        public async Task<IActionResult> TestUnstable() => await MakeRequest("simulate/unstable");

        [HttpGet("slow")]
        public async Task<IActionResult> TestSlow() => await MakeRequest("simulate/slow");

        [HttpGet("down")]
        public async Task<IActionResult> TestDown() => await MakeRequest("simulate/down");
    }
}
