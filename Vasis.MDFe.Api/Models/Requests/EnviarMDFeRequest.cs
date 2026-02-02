namespace Vasis.MDFe.Api.Models.Requests
{
    /// <summary>
    /// Request para envio de MDFe - ETAPA 4 DO CICLO
    /// </summary>
    public class EnviarMDFeRequest
    {
        /// <summary>
        /// XML do MDFe assinado para envio
        /// </summary>
        public string XmlAssinado { get; set; } = string.Empty;

        /// <summary>
        /// Ambiente de envio (1-Produção, 2-Homologação)
        /// </summary>
        public int TipoAmbiente { get; set; } = 2;

        /// <summary>
        /// Indica se deve aguardar o processamento síncrono
        /// </summary>
        public bool ProcessamentoSincrono { get; set; } = true;
    }
}