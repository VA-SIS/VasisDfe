using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vasis.MDFe.Api.Models.Requests;
using Vasis.MDFe.Api.Models.Responses;
using Vasis.MDFe.Api.Services.Interfaces;

namespace Vasis.MDFe.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciar o ciclo de vida completo do MDFe
    /// Implementa todas as etapas: CRIAR → VALIDAR → ASSINAR → ENVIAR → CONSULTAR → ENCERRAR
    /// </summary>
    [ApiController]
    [Route("api/mdfe")]
    [Authorize]
    public class MDFeController : ControllerBase
    {
        private readonly IMDFeValidationService _validationService;
        private readonly IMDFeLifecycleService _lifecycleService;
        private readonly ILogger<MDFeController> _logger;

        public MDFeController(
            IMDFeValidationService validationService,
            IMDFeLifecycleService lifecycleService,
            ILogger<MDFeController> logger)
        {
            _validationService = validationService;
            _lifecycleService = lifecycleService;
            _logger = logger;
        }

        /// <summary>
        /// ETAPA 1: Criar XML do MDFe a partir dos dados informados
        /// </summary>
        /// <param name="request">Dados para criação do MDFe</param>
        /// <returns>XML gerado e chave de acesso</returns>
        [HttpPost("criar")]
        [ProducesResponseType(typeof(CriarMDFeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CriarMDFe([FromBody] CriarMDFeRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando criação de MDFe");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Dados inválidos para criação do MDFe");
                }

                var resultado = await _lifecycleService.CriarMDFeAsync(request);

                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"MDFe criado com sucesso. Chave: {resultado.ChaveAcesso}");
                }
                else
                {
                    _logger.LogWarning($"Falha na criação do MDFe: {resultado.Mensagem}");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante criação do MDFe");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// ETAPA 2: Validar XML do MDFe contra schemas e regras de negócio
        /// </summary>
        /// <param name="request">Request com XML para validação</param>
        /// <returns>Resultado da validação com erros e avisos</returns>
        [HttpPost("validar")]
        [ProducesResponseType(typeof(ValidarMDFeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ValidarMDFe([FromBody] ValidarMDFeRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando validação de MDFe");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Dados inválidos para validação");
                }

                var resultado = await _validationService.ValidarMDFeAsync(request);

                if (resultado.Sucesso)
                {
                    _logger.LogInformation("MDFe validado com sucesso");
                }
                else
                {
                    _logger.LogWarning($"Falha na validação do MDFe: {resultado.Erros.Count} erro(s) encontrado(s)");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante validação do MDFe");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// ETAPA 3: Assinar digitalmente o XML do MDFe
        /// </summary>
        /// <param name="request">Request com XML para assinatura</param>
        /// <returns>XML assinado digitalmente</returns>
        [HttpPost("assinar")]
        [ProducesResponseType(typeof(AssinarMDFeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AssinarMDFe([FromBody] AssinarMDFeRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando assinatura de MDFe");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Dados inválidos para assinatura");
                }

                var resultado = await _lifecycleService.AssinarMDFeAsync(request);

                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"MDFe assinado com sucesso. Chave: {resultado.ChaveAcesso}");
                }
                else
                {
                    _logger.LogWarning($"Falha na assinatura do MDFe: {resultado.Mensagem}");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante assinatura do MDFe");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// ETAPA 4: Enviar MDFe assinado para a SEFAZ
        /// </summary>
        /// <param name="request">Request com XML assinado para envio</param>
        /// <returns>Protocolo de autorização da SEFAZ</returns>
        [HttpPost("enviar")]
        [ProducesResponseType(typeof(EnviarMDFeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> EnviarMDFe([FromBody] EnviarMDFeRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando envio de MDFe para SEFAZ");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Dados inválidos para envio");
                }

                var resultado = await _lifecycleService.EnviarMDFeAsync(request);

                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"MDFe enviado com sucesso. Protocolo: {resultado.ProtocoloAutorizacao}");
                }
                else
                {
                    _logger.LogWarning($"Falha no envio do MDFe: {resultado.Mensagem}");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante envio do MDFe");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// ETAPA 5: Consultar situação atual do MDFe na SEFAZ
        /// </summary>
        /// <param name="chaveAcesso">Chave de acesso do MDFe</param>
        /// <param name="tipoAmbiente">Ambiente (1-Produção, 2-Homologação)</param>
        /// <returns>Status e dados atuais do MDFe</returns>
        [HttpGet("consultar/{chaveAcesso}")]
        [ProducesResponseType(typeof(ConsultarMDFeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ConsultarMDFe(string chaveAcesso, [FromQuery] int tipoAmbiente = 2)
        {
            try
            {
                _logger.LogInformation($"Iniciando consulta de MDFe. Chave: {chaveAcesso}");

                if (string.IsNullOrWhiteSpace(chaveAcesso) || chaveAcesso.Length != 44)
                {
                    return BadRequest("Chave de acesso inválida");
                }

                var request = new ConsultarMDFeRequest
                {
                    ChaveAcesso = chaveAcesso,
                    TipoAmbiente = tipoAmbiente
                };

                var resultado = await _lifecycleService.ConsultarMDFeAsync(request);

                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"Consulta realizada com sucesso. Status: {resultado.StatusMDFe}");
                }
                else
                {
                    _logger.LogWarning($"Falha na consulta do MDFe: {resultado.Mensagem}");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante consulta do MDFe");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// ETAPA 6: Encerrar o MDFe (finalizar o transporte)
        /// </summary>
        /// <param name="request">Request com dados do encerramento</param>
        /// <returns>Protocolo de encerramento</returns>
        [HttpPost("encerrar")]
        [ProducesResponseType(typeof(EncerrarMDFeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> EncerrarMDFe([FromBody] EncerrarMDFeRequest request)
        {
            try
            {
                _logger.LogInformation($"Iniciando encerramento de MDFe. Chave: {request.ChaveAcesso}");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Dados inválidos para encerramento");
                }

                var resultado = await _lifecycleService.EncerrarMDFeAsync(request);

                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"MDFe encerrado com sucesso. Protocolo: {resultado.ProtocoloEncerramento}");
                }
                else
                {
                    _logger.LogWarning($"Falha no encerramento do MDFe: {resultado.Mensagem}");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante encerramento do MDFe");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// OPERAÇÃO AUXILIAR: Cancelar MDFe (quando necessário)
        /// </summary>
        /// <param name="request">Request com dados do cancelamento</param>
        /// <returns>Protocolo de cancelamento</returns>
        [HttpPost("cancelar")]
        [ProducesResponseType(typeof(CancelarMDFeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CancelarMDFe([FromBody] CancelarMDFeRequest request)
        {
            try
            {
                _logger.LogInformation($"Iniciando cancelamento de MDFe. Chave: {request.ChaveAcesso}");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Dados inválidos para cancelamento");
                }

                var resultado = await _lifecycleService.CancelarMDFeAsync(request);

                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"MDFe cancelado com sucesso. Protocolo: {resultado.ProtocoloCancelamento}");
                }
                else
                {
                    _logger.LogWarning($"Falha no cancelamento do MDFe: {resultado.Mensagem}");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante cancelamento do MDFe");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// OPERAÇÃO AUXILIAR: Consultar status do serviço da SEFAZ
        /// </summary>
        /// <param name="uf">UF para consulta</param>
        /// <param name="tipoAmbiente">Ambiente (1-Produção, 2-Homologação)</param>
        /// <returns>Status do serviço SEFAZ</returns>
        [HttpGet("status-servico/{uf}")]
        [ProducesResponseType(typeof(StatusServicoResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ConsultarStatusServico(string uf, [FromQuery] int tipoAmbiente = 2)
        {
            try
            {
                _logger.LogInformation($"Consultando status do serviço SEFAZ. UF: {uf}");

                if (string.IsNullOrWhiteSpace(uf) || uf.Length != 2)
                {
                    return BadRequest("UF inválida");
                }

                var resultado = await _lifecycleService.ConsultarStatusServicoAsync(uf.ToUpper(), tipoAmbiente);

                _logger.LogInformation($"Status do serviço consultado. Status: {resultado.StatusServico}");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante consulta de status do serviço");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// OPERAÇÃO AUXILIAR: Extrair dados básicos de um XML de MDFe
        /// </summary>
        /// <param name="xmlContent">Conteúdo XML do MDFe</param>
        /// <returns>Dados extraídos do MDFe</returns>
        [HttpPost("extrair-dados")]
        [ProducesResponseType(typeof(DadosMDFe), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ExtrairDados([FromBody] string xmlContent)
        {
            try
            {
                _logger.LogInformation("Extraindo dados básicos do MDFe");

                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    return BadRequest("Conteúdo XML não pode ser vazio");
                }

                var dados = await _validationService.ExtrairDadosBasicosAsync(xmlContent);

                if (dados != null)
                {
                    _logger.LogInformation($"Dados extraídos com sucesso. Chave: {dados.ChaveAcesso}");
                }
                else
                {
                    _logger.LogWarning("Não foi possível extrair dados do XML");
                }

                return Ok(dados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair dados do MDFe");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// OPERAÇÃO AUXILIAR: Validar apenas estrutura XML (sem regras de negócio)
        /// </summary>
        /// <param name="xmlContent">Conteúdo XML para validação estrutural</param>
        /// <returns>Resultado da validação estrutural</returns>
        [HttpPost("validar-estrutura")]
        [ProducesResponseType(typeof(ValidarMDFeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ValidarEstrutura([FromBody] string xmlContent)
        {
            try
            {
                _logger.LogInformation("Validando estrutura XML do MDFe");

                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    return BadRequest("Conteúdo XML não pode ser vazio");
                }

                var resultado = await _validationService.ValidarEstruturaXmlAsync(xmlContent);

                if (resultado.Sucesso)
                {
                    _logger.LogInformation("Estrutura XML validada com sucesso");
                }
                else
                {
                    _logger.LogWarning($"Falha na validação estrutural: {resultado.Erros.Count} erro(s)");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante validação estrutural");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}