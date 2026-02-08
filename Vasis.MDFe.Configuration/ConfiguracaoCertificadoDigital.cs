using System.Text.Json.Serialization; // Adicionar este using

namespace Vasis.MDFe.Configuration
{
    /// <summary>
    /// Representa as configurações do certificado digital.
    /// Suporta carregamento de arquivo .pfx ou da Windows Store.
    /// </summary>
    public class ConfiguracaoCertificadoDigital
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o certificado deve ser carregado
        /// da Windows Store do sistema operacional.
        /// Se <c>true</c>, <see cref="Thumbprint"/> é obrigatório.
        /// Se <c>false</c>, <see cref="NomeArquivoCertificado"/> e <see cref="Senha"/> são obrigatórios.
        /// </summary>
        public bool UsaWindowsStore { get; set; } = false;

        /// <summary>
        /// Obtém ou define o Thumbprint do certificado digital na Windows Store.
        /// Obrigatório se <see cref="UsaWindowsStore"/> for <c>true</c>.
        /// </summary>
        public string? Thumbprint { get; set; }

        /// <summary>
        /// Obtém ou define a localização da store do Windows para o certificado.
        /// Default: CurrentUser. (Ex: "CurrentUser", "LocalMachine")
        /// </summary>
        public string StoreLocation { get; set; } = "CurrentUser";

        /// <summary>
        /// Obtém ou define o nome da store do Windows para o certificado.
        /// Default: My. (Ex: "My", "Root")
        /// </summary>
        public string StoreName { get; set; } = "My";

        /// <summary>
        /// Obtém ou define o NOME do arquivo do certificado digital (.pfx), sem o caminho completo.
        /// O caminho completo será construído em tempo de execução pela aplicação.
        /// Obrigatório se <see cref="UsaWindowsStore"/> for <c>false</c>.
        /// </summary>
        public string? NomeArquivoCertificado { get; set; }

        /// <summary>
        /// [IGNORADO PELO JSON] Obtém ou define o caminho absoluto completo para o arquivo do certificado digital.
        /// Definido programaticamente no startup da aplicação.
        /// </summary>
        [JsonIgnore] // Esta propriedade não será lida/escrita do/para o JSON
        public string? CaminhoCompletoArquivo { get; set; }

        /// <summary>
        /// Obtém ou define a senha do certificado digital.
        /// Obrigatório se o certificado for carregado de um arquivo (.pfx) e <see cref="UsaWindowsStore"/> for <c>false</c>.
        /// </summary>
        public string? Senha { get; set; }

        /// <summary>
        /// Valida se as configurações do certificado digital estão preenchidas corretamente
        /// com base no método de carregamento escolhido.
        /// </summary>
        /// <returns><c>true</c> se as configurações do certificado são válidas; caso contrário, <c>false</c>.</returns>
        public bool IsValid()
        {
            if (UsaWindowsStore)
            {
                // Para Windows Store, Thumbprint é obrigatório
                return !string.IsNullOrWhiteSpace(Thumbprint);
            }
            else
            {
                // Para arquivo, NomeArquivoCertificado, Senha E o CaminhoCompletoArquivo resolvido são obrigatórios
                return !string.IsNullOrWhiteSpace(NomeArquivoCertificado) &&
                       !string.IsNullOrWhiteSpace(CaminhoCompletoArquivo) &&
                       !string.IsNullOrWhiteSpace(Senha);
            }
        }
    }
}