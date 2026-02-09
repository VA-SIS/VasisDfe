// Vasis.MDFe.Api/Controllers/MdfeConfigController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vasis.MDFe.Configuration;   // Seu projeto de configuração
using Vasis.MDFe.Common.Models;    // Referência aos DTOs compartilhados
using Microsoft.Extensions.Logging; // Necessário para ILogger
using System; // Necessário para Exception

namespace Vasis.MDFe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MdfeConfigController : ControllerBase
    {
        private readonly ConfiguracaoMDFe _mdfeConfig;
        private readonly ILogger<MdfeConfigController> _logger;

        public MdfeConfigController(IOptions<ConfiguracaoMDFe> mdfeConfigAccessor, ILogger<MdfeConfigController> logger)
        {
            _mdfeConfig = mdfeConfigAccessor.Value;
            _logger = logger;
        }

        /// <summary>
        /// Retorna o status atual das configurações do MDF-e.
        /// </summary>
        /// <returns>O DTO com o status das configurações do MDF-e.</returns>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<MdfeConfigStatusDto> GetMdfeConfigurationStatus()
        {
            try
            {
                // Mapeia os dados do modelo interno (Vasis.MDFe.Configuration.ConfiguracaoMDFe)
                // para o DTO compartilhado (Vasis.MDFe.Common.Models.MdfeConfigStatusDto).
                var configStatus = new MdfeConfigStatusDto
                {
                    MdfeConfigIsValid = _mdfeConfig.IsValid(),
                    CertificadoDigital = new CertificadoDigitalConfigDto
                    {
                        UsaWindowsStore = _mdfeConfig.CertificadoDigital.UsaWindowsStore,
                        NomeArquivoCertificado = _mdfeConfig.CertificadoDigital.NomeArquivoCertificado,
                        CaminhoCompletoArquivo = _mdfeConfig.CertificadoDigital.CaminhoCompletoArquivo,
                        Thumbprint = _mdfeConfig.CertificadoDigital.Thumbprint,
                        // *** LINHAS AJUSTADAS AQUI ***
                        // Como StoreLocation e StoreName já são strings na sua classe de configuração,
                        // não precisamos de .HasValue ou .Value.
                        StoreLocation = _mdfeConfig.CertificadoDigital.StoreLocation,
                        StoreName = _mdfeConfig.CertificadoDigital.StoreName,
                        // *** FIM DAS LINHAS AJUSTADAS ***
                        SenhaPresente = !string.IsNullOrWhiteSpace(_mdfeConfig.CertificadoDigital.Senha)
                    },
                    EmpresaEmitente = new EmpresaEmitenteConfigDto
                    {
                        CNPJ = _mdfeConfig.EmpresaEmitente.CNPJ,
                        InscricaoEstadual = _mdfeConfig.EmpresaEmitente.InscricaoEstadual,
                        RazaoSocial = _mdfeConfig.EmpresaEmitente.RazaoSocial,
                        NomeFantasia = _mdfeConfig.EmpresaEmitente.NomeFantasia,
                        EnderecoLogradouro = _mdfeConfig.EmpresaEmitente.EnderecoLogradouro,
                        EnderecoNumero = _mdfeConfig.EmpresaEmitente.EnderecoNumero,
                        EnderecoComplemento = _mdfeConfig.EmpresaEmitente.EnderecoComplemento,
                        EnderecoBairro = _mdfeConfig.EmpresaEmitente.EnderecoBairro,
                        EnderecoCodigoMunicipio = _mdfeConfig.EmpresaEmitente.EnderecoCodigoMunicipio,
                        EnderecoNomeMunicipio = _mdfeConfig.EmpresaEmitente.EnderecoNomeMunicipio,
                        EnderecoUF = _mdfeConfig.EmpresaEmitente.EnderecoUF.ToString(), // Convertendo enum Estado para string
                        EnderecoCEP = _mdfeConfig.EmpresaEmitente.EnderecoCEP,
                        Telefone = _mdfeConfig.EmpresaEmitente.Telefone,
                        Email = _mdfeConfig.EmpresaEmitente.Email,
                        RNTRC = _mdfeConfig.EmpresaEmitente.RNTRC
                    },
                    SistemaDFe = new SistemaDFeConfigDto
                    {
                        TipoAmbiente = _mdfeConfig.SistemaDFe.TipoAmbiente.ToString(), // Convertendo enum TipoAmbiente para string
                        VersaoLayoutMDFe = _mdfeConfig.SistemaDFe.VersaoLayoutMDFe.ToString(), // Convertendo enum VersaoServico para string
                        PastaSchemas = _mdfeConfig.SistemaDFe.PastaSchemas,
                        CaminhoCompletoSchemas = _mdfeConfig.SistemaDFe.CaminhoCompletoSchemas,
                        PastaSalvarXml = _mdfeConfig.SistemaDFe.PastaSalvarXml,
                        CaminhoCompletoSalvarXml = _mdfeConfig.SistemaDFe.CaminhoCompletoSalvarXml,
                        IsSalvarXml = _mdfeConfig.SistemaDFe.IsSalvarXml,
                        TimeOutServicoMs = _mdfeConfig.SistemaDFe.TimeOutServicoMs,
                        UFEmitente = _mdfeConfig.SistemaDFe.UFEmitente.ToString() // Convertendo enum Estado para string
                    }
                };

                _logger.LogInformation("Status das configurações do MDF-e solicitado. Validade: {IsValid}", configStatus.MdfeConfigIsValid);
                return Ok(configStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o status das configurações do MDF-e.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao obter o status das configurações do MDF-e.", Error = ex.Message });
            }
        }
    }
}