using Vasis.MDFe.Api.Models.Requests;
using Vasis.MDFe.Api.Models.Responses;

namespace Vasis.MDFe.Api.Services.Interfaces
{
    /// <summary>
    /// Interface para gerenciar o ciclo de vida completo do MDFe
    /// COMPLEMENTA o IMDFeValidationService (não substitui)
    /// </summary>
    public interface IMDFeLifecycleService
    {
        /// <summary>
        /// ETAPA 1: Criar XML do MDFe a partir dos dados informados
        /// </summary>
        Task<CriarMDFeResponse> CriarMDFeAsync(CriarMDFeRequest request);

        /// <summary>
        /// ETAPA 3: Assinar digitalmente o XML do MDFe
        /// </summary>
        Task<AssinarMDFeResponse> AssinarMDFeAsync(AssinarMDFeRequest request);

        /// <summary>
        /// ETAPA 4: Enviar MDFe assinado para a SEFAZ
        /// </summary>
        Task<EnviarMDFeResponse> EnviarMDFeAsync(EnviarMDFeRequest request);

        /// <summary>
        /// ETAPA 5: Consultar situação atual do MDFe na SEFAZ
        /// </summary>
        Task<ConsultarMDFeResponse> ConsultarMDFeAsync(ConsultarMDFeRequest request);

        /// <summary>
        /// ETAPA 6: Encerrar o MDFe (finalizar o transporte)
        /// </summary>
        Task<EncerrarMDFeResponse> EncerrarMDFeAsync(EncerrarMDFeRequest request);

        /// <summary>
        /// OPERAÇÃO AUXILIAR: Cancelar MDFe (quando necessário)
        /// </summary>
        Task<CancelarMDFeResponse> CancelarMDFeAsync(CancelarMDFeRequest request);

        /// <summary>
        /// OPERAÇÃO AUXILIAR: Consultar status do serviço da SEFAZ
        /// </summary>
        Task<StatusServicoResponse> ConsultarStatusServicoAsync(string uf, int tipoAmbiente = 2);

        // NOTA: Validação fica no IMDFeValidationService (não duplicar)
    }
}