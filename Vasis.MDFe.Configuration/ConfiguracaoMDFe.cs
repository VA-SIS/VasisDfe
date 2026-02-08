namespace Vasis.MDFe.Configuration
{
    /// <summary>
    /// Classe mestre que agrega todas as configurações necessárias para a operação do MDF-e.
    /// Esta é a estrutura que será lida do arquivo de configuração (ex: appsettings.json)
    /// e injetada como dependência na aplicação.
    /// </summary>
    public class ConfiguracaoMDFe
    {
        /// <summary>
        /// Obtém ou define as configurações do certificado digital para assinatura e autenticação.
        /// </summary>
        public ConfiguracaoCertificadoDigital CertificadoDigital { get; set; } = new ConfiguracaoCertificadoDigital();

        /// <summary>
        /// Obtém ou define as informações da empresa emitente do MDF-e.
        /// </summary>
        public ConfiguracaoEmpresa EmpresaEmitente { get; set; } = new ConfiguracaoEmpresa();

        /// <summary>
        /// Obtém ou define as configurações gerais do sistema DFe, como ambiente e versões.
        /// </summary>
        public ConfiguracaoSistemaDFe SistemaDFe { get; set; } = new ConfiguracaoSistemaDFe();

        /// <summary>
        /// Valida a consistência de todas as configurações agrupadas.
        /// </summary>
        /// <returns><c>true</c> se todas as configurações internas forem válidas; caso contrário, <c>false</c>.</returns>
        public bool IsValid()
        {
            // Valida cada sub-configuração. Se alguma for inválida, a configuração geral é inválida.
            return CertificadoDigital.IsValid() &&
                   EmpresaEmitente.IsValid() &&
                   SistemaDFe.IsValid();
        }
    }
}