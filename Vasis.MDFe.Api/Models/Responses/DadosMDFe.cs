namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Dados principais extraídos do MDFe
    /// </summary>
    public class DadosMDFe
    {
        public string? ChaveAcesso { get; set; }
        public string? NumeroMDFe { get; set; }
        public string? SerieMDFe { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string? CnpjEmitente { get; set; }
        public string? RazaoSocialEmitente { get; set; }
        public string? UfInicio { get; set; }
        public string? UfFim { get; set; }
        public decimal? ValorTotalCarga { get; set; }
        public string? UnidadeMedida { get; set; }
        public decimal? QuantidadeCarga { get; set; }
    }
}