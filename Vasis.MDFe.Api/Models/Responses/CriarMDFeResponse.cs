namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Response da criação de MDFe
    /// </summary>
    public class CriarMDFeResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? XmlGerado { get; set; }
        public string? ChaveAcesso { get; set; }
        public string? NumeroMDFe { get; set; }
        public List<string> Erros { get; set; } = new();
        public TimeSpan TempoProcessamento { get; set; }
    }
}