namespace Vasis.MDFe.Configuration
{
    /// <summary>
    /// Representa as configurações para o certificado digital utilizado nas operações do MDF-e.
    /// Esta classe é utilizada para carregar informações de certificado de arquivos de configuração
    /// ou de variáveis de ambiente na API.
    /// </summary>
    public class ConfiguracaoCertificadoDigital
    {
        /// <summary>
        /// Obtém ou define o caminho completo para o arquivo do certificado digital (.pfx).
        /// Obrigatório se <see cref="UsaWindowsStore"/> for <c>false</c>.
        /// </summary>
        public string CaminhoArquivo { get; set; }

        /// <summary>
        /// Obtém ou define a senha do certificado digital.
        /// Obrigatório se o certificado for carregado de um arquivo (.pfx) e <see cref="UsaWindowsStore"/> for <c>false</c>.
        /// </summary>
        public string Senha { get; set; }

        /// <summary>
        /// Obtém ou define o Thumbprint (ou Hash SHA1) do certificado digital.
        /// Obrigatório se <see cref="UsaWindowsStore"/> for <c>true</c> para localizar o certificado na Windows Certificate Store.
        /// </summary>
        public string Thumbprint { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o certificado digital deve ser carregado da Windows Certificate Store.
        /// Se <c>true</c>, o certificado é buscado pelo <see cref="Thumbprint"/>.
        /// Se <c>false</c>, o certificado é carregado do <see cref="CaminhoArquivo"/> com a <see cref="Senha"/>.
        /// O valor padrão é <c>false</c>.
        /// </summary>
        public bool UsaWindowsStore { get; set; } = false;

        /// <summary>
        /// Obtém ou define o local de armazenamento da Windows Certificate Store (CurrentUser ou LocalMachine).
        /// Relevante apenas se <see cref="UsaWindowsStore"/> for <c>true</c>.
        /// O valor padrão é "CurrentUser".
        /// </summary>
        public string StoreLocation { get; set; } = "CurrentUser"; // Ex: "CurrentUser", "LocalMachine"

        /// <summary>
        /// Obtém ou define o nome da loja de certificados na Windows Certificate Store (My, Root, etc.).
        /// Relevante apenas se <see cref="UsaWindowsStore"/> for <c>true</c>.
        /// O valor padrão é "My".
        /// </summary>
        public string StoreName { get; set; } = "My"; // Ex: "My", "Root", "AddressBook", "TrustedPeople"

        /// <summary>
        /// Valida se as configurações do certificado digital estão completas e consistentes
        /// com base no método de carregamento escolhido (<see cref="UsaWindowsStore"/>).
        /// </summary>
        /// <returns><c>true</c> se as configurações são consideradas válidas para uso; caso contrário, <c>false</c>.</returns>
        public bool IsValid()
        {
            if (UsaWindowsStore)
            {
                // Se o certificado está na Windows Store, o Thumbprint é essencial.
                // StoreLocation e StoreName têm padrões e são geralmente válidos.
                return !string.IsNullOrWhiteSpace(Thumbprint);
            }
            else
            {
                // Se o certificado é de arquivo, o caminho e a senha são cruciais.
                return !string.IsNullOrWhiteSpace(CaminhoArquivo) && !string.IsNullOrWhiteSpace(Senha);
            }
        }
    }
}