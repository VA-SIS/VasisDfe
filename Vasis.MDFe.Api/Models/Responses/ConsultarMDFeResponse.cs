namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Response da consulta de MDFe
    /// </summary>
    public class ConsultarMDFeResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? ChaveAcesso { get; set; }
        public string? StatusMDFe { get; set; }
        public string? DescricaoStatus { get; set; }
        public string? ProtocoloAutorizacao { get; set; }
        public DateTime? DataHoraAutorizacao { get; set; }
        public string? XmlCompleto { get; set; }
        public DadosMDFe? DadosMDFe { get; set; }
        public List<EventoMDFe> Eventos { get; set; } = new();
        public List<string> Erros { get; set; } = new();
        public TimeSpan TempoProcessamento { get; set; }
    }
}