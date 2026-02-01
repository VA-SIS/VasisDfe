namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Representa um erro de validação
    /// </summary>
    public class ErroValidacao
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Campo { get; set; }
        public string? Linha { get; set; }
        public string Severidade { get; set; } = "Erro";
    }
}