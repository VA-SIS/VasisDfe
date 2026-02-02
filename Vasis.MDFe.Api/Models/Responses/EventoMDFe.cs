namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Representa um evento do MDFe (cancelamento, encerramento, etc.)
    /// </summary>
    public class EventoMDFe
    {
        public string TipoEvento { get; set; } = string.Empty;
        public string DescricaoEvento { get; set; } = string.Empty;
        public DateTime DataHoraEvento { get; set; }
        public string? ProtocoloEvento { get; set; }
        public string? Justificativa { get; set; }
        public int NumeroSequencial { get; set; }
    }
}