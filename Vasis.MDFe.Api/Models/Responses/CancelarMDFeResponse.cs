namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Response do cancelamento de MDFe
    /// </summary>
    public class CancelarMDFeResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? ChaveAcesso { get; set; }
        public string? ProtocoloCancelamento { get; set; }
        public DateTime? DataHoraCancelamento { get; set; }
        public string? XmlEventoCancelamento { get; set; }
        public string? Justificativa { get; set; }
        public List<string> Erros { get; set; } = new();
        public TimeSpan TempoProcessamento { get; set; }
    }
}