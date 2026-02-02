namespace Vasis.MDFe.Api.Models.Requests
{
    /// <summary>
    /// Request para assinatura de MDFe - ETAPA 3 DO CICLO
    /// </summary>
    public class AssinarMDFeRequest
    {
        /// <summary>
        /// XML do MDFe a ser assinado
        /// </summary>
        public string XmlContent { get; set; } = string.Empty;

        /// <summary>
        /// Certificado a ser usado (opcional - usa o configurado se não informado)
        /// </summary>
        public string? Caminhocertificado { get; set; }

        /// <summary>
        /// Senha do certificado (opcional - usa a configurada se não informado)
        /// </summary>
        public string? SenhaCertificado { get; set; }
    }
}