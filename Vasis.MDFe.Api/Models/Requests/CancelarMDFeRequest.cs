namespace Vasis.MDFe.Api.Models.Requests
{
    /// <summary>
    /// Request para cancelamento de MDFe
    /// </summary>
    public class CancelarMDFeRequest
    {
        /// <summary>
        /// Chave de acesso do MDFe a ser cancelado
        /// </summary>
        public string ChaveAcesso { get; set; } = string.Empty;

        /// <summary>
        /// Justificativa do cancelamento (mínimo 15 caracteres)
        /// </summary>
        public string Justificativa { get; set; } = string.Empty;

        /// <summary>
        /// Número sequencial do evento
        /// </summary>
        public int NumeroSequencial { get; set; } = 1;

        /// <summary>
        /// Protocolo de autorização do MDFe
        /// </summary>
        public string? ProtocoloAutorizacao { get; set; }
    }
}