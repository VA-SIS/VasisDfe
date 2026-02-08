using System;

namespace Vasis.MDFe.Configuration
{
    /// <summary>
    /// Representa as configurações gerais do sistema DFe, como ambiente, versões de layout
    /// e diretórios para arquivos XML e schemas.
    /// </summary>
    public class ConfiguracaoSistemaDFe
    {
        /// <summary>
        /// Obtém ou define o tipo de ambiente (Produção ou Homologação).
        /// </summary>
        public string TipoAmbiente { get; set; } = "Homologacao"; // Ex: "Producao", "Homologacao"

        /// <summary>
        /// Obtém ou define a versão do layout do MDF-e a ser utilizada.
        /// Ex: "3.00", "1.00".
        /// </summary>
        public string VersaoLayoutMDFe { get; set; } = "3.00";

        /// <summary>
        /// Obtém ou define o diretório onde os arquivos de schemas XSD estão localizados.
        /// Estes schemas são usados para validação dos XMLs.
        /// </summary>
        public string DiretorioSchemas { get; set; }

        /// <summary>
        /// Obtém ou define o diretório onde os XMLs de envio e retorno devem ser salvos (opcional).
        /// Se nulo/vazio, os XMLs podem não ser salvos em disco, dependendo da implementação.
        /// </summary>
        public string DiretorioSalvarXml { get; set; }

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
        public string UFEmitente { get; set; }

        /// <summary>
        /// Valida se as configurações essenciais do sistema DFe estão preenchidas.
        /// </summary>
        /// <returns><c>true</c> se as configurações essenciais são válidas; caso contrário, <c>false</c>.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(TipoAmbiente) &&
                   !string.IsNullOrWhiteSpace(VersaoLayoutMDFe) &&
                   !string.IsNullOrWhiteSpace(DiretorioSchemas) &&
                   !string.IsNullOrWhiteSpace(UFEmitente) &&
                   TimeOutServicoMs > 0;
        }
    }
}