namespace Vasis.MDFe.Api.Models.Entities
{
    /// <summary>
    /// Representa um documento fiscal no MDFe
    /// </summary>
    public class DocumentoFiscal
    {
        public string ChaveNFe { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Serie { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
    }
}