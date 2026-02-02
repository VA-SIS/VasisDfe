namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Response da consulta de status do serviço SEFAZ
    /// </summary>
    public class StatusServicoResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? StatusServico { get; set; }
        public string? DescricaoStatus { get; set; }
        public DateTime? DataHoraConsulta { get; set; }
        public string? VersaoAplicativo { get; set; }
        public string? TempoMedioResposta { get; set; }
        public List<string> Erros { get; set; } = new();
        public TimeSpan TempoProcessamento { get; set; }
    }
}