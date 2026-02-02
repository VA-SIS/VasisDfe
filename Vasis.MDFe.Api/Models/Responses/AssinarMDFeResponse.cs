namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Response da assinatura de MDFe
    /// </summary>
    public class AssinarMDFeResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? XmlAssinado { get; set; }
        public string? ChaveAcesso { get; set; }
        public List<string> Erros { get; set; } = new();
        public TimeSpan TempoProcessamento { get; set; }
    }
}