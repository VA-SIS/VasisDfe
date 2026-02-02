namespace Vasis.MDFe.Api.Models.Entities
{
    /// <summary>
    /// Dados do veículo para o MDFe
    /// </summary>
    public class DadosVeiculo
    {
        public string Placa { get; set; } = string.Empty;
        public string Renavam { get; set; } = string.Empty;
        public int TaraKg { get; set; }
        public int CapacidadeKg { get; set; }
        public string Proprietario { get; set; } = string.Empty;
    }
}