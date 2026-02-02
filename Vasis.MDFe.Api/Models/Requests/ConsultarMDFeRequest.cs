namespace Vasis.MDFe.Api.Models.Requests
{
    /// <summary>
    /// Request para consulta de MDFe - ETAPA 5 DO CICLO
    /// </summary>
    public class ConsultarMDFeRequest
    {
        /// <summary>
        /// Chave de acesso do MDFe a ser consultado
        /// </summary>
        public string ChaveAcesso { get; set; } = string.Empty;

        /// <summary>
        /// Ambiente de consulta (1-Produção, 2-Homologação)
        /// </summary>
        public int TipoAmbiente { get; set; } = 2;
    }
}