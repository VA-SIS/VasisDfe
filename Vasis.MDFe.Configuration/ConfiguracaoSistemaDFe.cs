using System;
using System.Text.Json.Serialization; // Adicionar este using

namespace Vasis.MDFe.Configuration
{
    /// <summary>
    /// Representa as configurações gerais do sistema DFe, como ambiente de trabalho, versões de layout
    /// e diretórios para schemas e XMLs.
    /// </summary>
    public class ConfiguracaoSistemaDFe
    {
        /// <summary>
        /// Obtém ou define o tipo de ambiente (Produção ou Homologação).
        /// Ex: "Producao", "Homologacao".
        /// </summary>
        public string TipoAmbiente { get; set; } = "Homologacao";

        /// <summary>
        /// Obtém ou define a versão do layout do MDF-e a ser utilizada.
        /// Ex: "3.00", "1.00".
        /// </summary>
        public string VersaoLayoutMDFe { get; set; } = "3.00";

        /// <summary>
        /// Obtém ou define o NOME da pasta onde os arquivos de schemas XSD estão localizados.
        /// O caminho completo será construído em tempo de execução.
        /// </summary>
        public string PastaSchemas { get; set; } = "Schemas";

        /// <summary>
        /// [IGNORADO PELO JSON] Obtém ou define o caminho absoluto completo para a pasta de schemas XSD.
        /// Definido programaticamente no startup da aplicação.
        /// </summary>
        [JsonIgnore] // Esta propriedade não será lida/escrita do/para o JSON
        public string? CaminhoCompletoSchemas { get; set; }

        /// <summary>
        /// Obtém ou define o NOME da pasta onde os XMLs de envio e retorno devem ser salvos (opcional).
        /// O caminho completo será construído em tempo de execução.
        /// </summary>
        public string PastaSalvarXml { get; set; } = "XMLsGerados";

        /// <summary>
        /// [IGNORADO PELO JSON] Obtém ou define o caminho absoluto completo para a pasta de XMLs de envio e retorno.
        /// Definido programaticamente no startup da aplicação.
        /// </summary>
        [JsonIgnore] // Esta propriedade não será lida/escrita do/para o JSON
        public string? CaminhoCompletoSalvarXml { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os XMLs de envio e retorno devem ser salvos em disco.
        /// </summary>
        public bool IsSalvarXml { get; set; } = true;

        /// <summary>
        /// Obtém ou define o tempo limite (em milissegundos) para as chamadas aos serviços web da SEFAZ.
        /// </summary>
        public int TimeOutServicoMs { get; set; } = 30000; // 30 segundos

        /// <summary>
        /// Obtém ou define a sigla da Unidade da Federação (UF) do emitente para as requisições de serviço.
        /// </summary>
        public string? UFEmitente { get; set; }

        /// <summary>
        /// Valida se as configurações essenciais do sistema DFe estão preenchidas.
        /// </summary>
        /// <returns><c>true</c> se as configurações essenciais são válidas; caso contrário, <c>false</c>.</returns>
        public bool IsValid()
        {
            // Valida as propriedades que vêm do JSON e os caminhos completos resolvidos
            var baseValid = !string.IsNullOrWhiteSpace(TipoAmbiente) &&
                            !string.IsNullOrWhiteSpace(VersaoLayoutMDFe) &&
                            !string.IsNullOrWhiteSpace(PastaSchemas) && // Verifica o nome da pasta
                            !string.IsNullOrWhiteSpace(CaminhoCompletoSchemas) && // E o caminho resolvido
                            !string.IsNullOrWhiteSpace(UFEmitente) &&
                            TimeOutServicoMs > 0;

            // Se for para salvar XMLs, o nome da pasta e o caminho resolvido também são obrigatórios
            if (IsSalvarXml)
            {
                return baseValid &&
                       !string.IsNullOrWhiteSpace(PastaSalvarXml) &&
                       !string.IsNullOrWhiteSpace(CaminhoCompletoSalvarXml);
            }

            return baseValid;
        }
    }
}