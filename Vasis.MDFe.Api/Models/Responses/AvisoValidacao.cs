namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Representa um aviso de validação
    /// </summary>
    public class AvisoValidacao
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Campo { get; set; }
        public string Severidade { get; set; } = "Aviso";
    }
}