namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Response do encerramento de MDFe
    /// </summary>
    public class EncerrarMDFeResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? ChaveAcesso { get; set; }
        public string? ProtocoloEncerramento { get; set; }
        public DateTime? DataHoraEncerramento { get; set; }
        public string? XmlEventoEncerramento { get; set; }
        public List<string> Erros { get; set; } = new();
        public TimeSpan TempoProcessamento { get; set; }
    }
}