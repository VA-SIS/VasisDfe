namespace Vasis.MDFe.Api.Models.Requests
{
    /// <summary>
    /// Request para encerramento de MDFe - ETAPA 6 DO CICLO
    /// </summary>
    public class EncerrarMDFeRequest
    {
        /// <summary>
        /// Chave de acesso do MDFe a ser encerrado
        /// </summary>
        public string ChaveAcesso { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora do encerramento
        /// </summary>
        public DateTime DataHoraEncerramento { get; set; }

        /// <summary>
        /// UF onde ocorreu o encerramento
        /// </summary>
        public string UfEncerramento { get; set; } = string.Empty;

        /// <summary>
        /// Município onde ocorreu o encerramento
        /// </summary>
        public string MunicipioEncerramento { get; set; } = string.Empty;

        /// <summary>
        /// Código do município de encerramento
        /// </summary>
        public string CodigoMunicipioEncerramento { get; set; } = string.Empty;
    }
}