using Vasis.MDFe.Api.Models.Requests;
using Vasis.MDFe.Api.Models.Responses;

namespace Vasis.MDFe.Api.Services.Interfaces
{
    /// <summary>
    /// Interface para serviços de validação de MDFe
    /// </summary>
    public interface IMDFeValidationService
    {
        /// <summary>
        /// Valida um XML de MDFe usando as bibliotecas Zeus
        /// </summary>
        /// <param name="request">Request com dados para validação</param>
        /// <returns>Response com resultado da validação</returns>
        Task<ValidarMDFeResponse> ValidarMDFeAsync(ValidarMDFeRequest request);

        /// <summary>
        /// Valida apenas a estrutura XML contra o schema
        /// </summary>
        /// <param name="xmlContent">Conteúdo XML</param>
        /// <returns>Response com resultado da validação estrutural</returns>
        Task<ValidarMDFeResponse> ValidarEstruturaXmlAsync(string xmlContent);

        /// <summary>
        /// Extrai dados básicos do XML sem validação completa
        /// </summary>
        /// <param name="xmlContent">Conteúdo XML</param>
        /// <returns>Dados extraídos do MDFe</returns>
        Task<DadosMDFe?> ExtrairDadosBasicosAsync(string xmlContent);
    }
}