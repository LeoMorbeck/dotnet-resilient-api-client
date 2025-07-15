using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ResilientClient.Provider.Mock.Controllers
{
    [Route("simulate")]
    [ApiController]
    public class SimulatedFailureController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, int> _requestCounts = new();
        private readonly ILogger<SimulatedFailureController> _logger;

        public SimulatedFailureController(ILogger<SimulatedFailureController> logger)
        {
            _logger = logger;
        }

        [HttpGet("success")]
        public IActionResult GetSuccess()
        {
            _logger.LogInformation("Requisição de sucesso recebida.");
            return Ok(new { message = "Esta requisição foi um sucesso!" });
        }

        [HttpGet("slow")]
        public async Task<IActionResult> GetSlowResponse()
        {
            _logger.LogInformation("Requisição lenta recebida. Aguardando 15 segundos...");
            await Task.Delay(TimeSpan.FromSeconds(15));
            _logger.LogInformation("Requisição lenta concluída.");
            return Ok(new { message = "Esta requisição foi lenta, mas funcionou." });
        }

        [HttpGet("unstable")]
        public IActionResult GetUnstableResponse()
        {
            var key = "unstable";
            var count = _requestCounts.AddOrUpdate(key, 1, (k, v) => v + 1);

            if (count % 3 != 0)
            {
                _logger.LogWarning("Requisição instável FALHOU. Tentativa #{Count}.", count);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Serviço indisponível. Tente novamente.");
            }

            _logger.LogInformation("Requisição instável teve SUCESSO na tentativa #{Count}.", count);
            _requestCounts.TryRemove(key, out _);
            return Ok(new { message = $"Funcionou na tentativa número {count}!" });
        }

        [HttpGet("down")]
        public IActionResult GetDownResponse()
        {
            _logger.LogError("Requisição para serviço 'down' recebida. Retornando erro.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Este serviço está permanentemente fora do ar.");
        }
    }
}