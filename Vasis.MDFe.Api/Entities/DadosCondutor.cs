namespace Vasis.MDFe.Api.Models.Entities
{
    /// <summary>
    /// Dados do condutor para o MDFe
    /// </summary>
    public class DadosCondutor
    {
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Cnh { get; set; } = string.Empty;
    }
}