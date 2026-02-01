namespace Vasis.MDFe.Api.Models.Responses
{
    /// <summary>
    /// Response da validação de MDFe
    /// </summary>
    public class ValidarMDFeResponse
    {
        /// <summary>
        /// Indica se a validação foi bem-sucedida
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Mensagem principal do resultado
        /// </summary>
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Lista de erros encontrados na validação
        /// </summary>
        public List<ErroValidacao> Erros { get; set; } = new List<ErroValidacao>();

        /// <summary>
        /// Lista de avisos encontrados na validação
        /// </summary>
        public List<AvisoValidacao> Avisos { get; set; } = new List<AvisoValidacao>();

        /// <summary>
        /// Dados extraídos do XML (quando validação é bem-sucedida)
        /// </summary>
        public DadosMDFe? DadosExtraidos { get; set; }

        /// <summary>
        /// Tempo de processamento da validação
        /// </summary>
        public TimeSpan TempoProcessamento { get; set; }
    }
}