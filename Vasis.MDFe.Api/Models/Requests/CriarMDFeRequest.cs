using Vasis.MDFe.Api.Models.Entities;

namespace Vasis.MDFe.Api.Models.Requests
{
    /// <summary>
    /// Request para criação de MDFe - ETAPA 1 DO CICLO
    /// </summary>
    public class CriarMDFeRequest
    {
        // Dados do emitente
        public string CnpjEmitente { get; set; } = string.Empty;
        public string RazaoSocialEmitente { get; set; } = string.Empty;

        // Dados do transporte
        public string UfInicio { get; set; } = string.Empty;
        public string UfFim { get; set; } = string.Empty;
        public DateTime DataInicioViagem { get; set; }

        // Dados da carga
        public decimal ValorTotalCarga { get; set; }
        public decimal QuantidadeCarga { get; set; }
        public string UnidadeMedida { get; set; } = string.Empty;

        // Documentos fiscais
        public List<DocumentoFiscal> DocumentosFiscais { get; set; } = new();

        // Veículo e condutor
        public DadosVeiculo Veiculo { get; set; } = new();
        public List<DadosCondutor> Condutores { get; set; } = new();
    }
}